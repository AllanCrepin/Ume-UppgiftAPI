using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UmeåUppgiftAPI.Models;
using UmeåUppgiftAPI.Services;

namespace UmeåUppgiftAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CatApiService _catApiService;

        public HomeController(ILogger<HomeController> logger, CatApiService catApiService)
        {
            _logger = logger;
            _catApiService = catApiService;
        }

        public async Task<IActionResult> Index(int page = 0)
        {
            try
            {
                var images = await _catApiService.GetCatImagesAsync(20, page);
                
                var viewModel = new CatImagesViewModel
                {
                    Images = images,
                    CurrentPage = page,
                    TotalPages = 100 // Arbitrary large number since the API doesn't provide a total count
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cat images");
                return View(new CatImagesViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreCats(int page = 0)
        {
            try
            {
                var images = await _catApiService.GetCatImagesAsync(20, page);
                return Json(images);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching more cat images");
                return Json(new List<CatImage>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
