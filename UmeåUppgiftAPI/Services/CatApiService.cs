using System.Text.Json;
using UmeåUppgiftAPI.Models;

namespace UmeåUppgiftAPI.Services
{
    public class CatApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.thecatapi.com/v1";
        
        public CatApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("x-api-key", "live_huPwPwH1lTUX9L21BWsjnumfYj2hG5XMftJegubGJziZAlJBfZJegDI6Zu6OBoTJ");
        }

        public async Task<List<CatImage>> GetCatImagesAsync(int limit = 20, int page = 0)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/images/search?limit={limit}&page={page}");
            
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var content = await response.Content.ReadAsStringAsync();
                var images = JsonSerializer.Deserialize<List<CatImage>>(content, options);
                return images ?? new List<CatImage>();
            }
            
            return new List<CatImage>();
        }
    }
} 