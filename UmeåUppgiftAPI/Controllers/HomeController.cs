using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UmeåUppgiftAPI.Models;
using UmeåUppgiftAPI.Services;
using UmeåUppgiftAPI.Helpers;

namespace UmeåUppgiftAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CatApiService _catApiService;
        private const string SessionKey = "UserSession";

        public HomeController(ILogger<HomeController> logger, CatApiService catApiService)
        {
            _logger = logger;
            _catApiService = catApiService;
        }

        public async Task<IActionResult> Index(int page = 0)
        {
            // Get or initialize user session
            var userSession = HttpContext.Session.GetObject<UserSession>(SessionKey) ?? new UserSession();
            
            // Update user activity
            userSession.LastActivity = DateTime.UtcNow;
            userSession.PageVisits++;
            
            // Save session data
            HttpContext.Session.SetObject(SessionKey, userSession);
            
            try
            {
                var images = await _catApiService.GetCatImagesAsync(20, page);
                
                var viewModel = new CatImagesViewModel
                {
                    Images = images,
                    CurrentPage = page,
                    TotalPages = 100, // Arbitrary large number since the API doesn't provide a total count
                    UserSessionData = userSession // Pass session data to view
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cat images");
                return View(new CatImagesViewModel { UserSessionData = userSession });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreCats(int page = 0)
        {
            try
            {
                // Track page loading in session
                var userSession = HttpContext.Session.GetObject<UserSession>(SessionKey);
                if (userSession != null)
                {
                    userSession.LastActivity = DateTime.UtcNow;
                    if (!userSession.CustomData.ContainsKey("PagesLoaded"))
                    {
                        userSession.CustomData["PagesLoaded"] = new List<int>();
                    }
                    
                    var pagesLoaded = (List<int>)userSession.CustomData["PagesLoaded"];
                    pagesLoaded.Add(page);
                    userSession.CustomData["PagesLoaded"] = pagesLoaded;
                    
                    HttpContext.Session.SetObject(SessionKey, userSession);
                }
                
                var images = await _catApiService.GetCatImagesAsync(20, page);
                
                // Generate random cat names for each image
                string[] cats = { "Funny cat", "Cute cat", "Strange cat", "Weird Cat", "Scary cat" };
                var random = new Random();
                
                var imagesWithNames = images.Select(img => new 
                {
                    id = img.Id,
                    url = img.Url,
                    width = img.Width,
                    height = img.Height,
                    name = cats[random.Next(cats.Length)]
                }).ToList();
                
                return Json(imagesWithNames);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching more cat images");
                return Json(new List<CatImage>());
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFavoriteCat(string catId)
        {
            var userSession = HttpContext.Session.GetObject<UserSession>(SessionKey);
            if (userSession != null && !string.IsNullOrEmpty(catId))
            {
                if (!userSession.FavoriteCatIds.Contains(catId))
                {
                    userSession.FavoriteCatIds.Add(catId);
                    HttpContext.Session.SetObject(SessionKey, userSession);
                    return Json(new { success = true, message = "Added to favorites" });
                }
            }
            
            return Json(new { success = false, message = "Could not add to favorites" });
        }
        
        [HttpGet]
        public IActionResult SessionInfo()
        {
            var userSession = HttpContext.Session.GetObject<UserSession>(SessionKey);
            return View(userSession ?? new UserSession());
        }

        public IActionResult Privacy()
        {
            // Update session data on privacy page visit
            var userSession = HttpContext.Session.GetObject<UserSession>(SessionKey);
            if (userSession != null)
            {
                userSession.LastActivity = DateTime.UtcNow;
                userSession.PageVisits++;
                userSession.CustomData["VisitedPrivacyPage"] = true;
                HttpContext.Session.SetObject(SessionKey, userSession);
            }
            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
