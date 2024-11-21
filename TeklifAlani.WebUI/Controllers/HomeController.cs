using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using TeklifAlani.BLL.Services;
using TeklifAlani.WebUI.Models;

namespace TeklifAlani.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private UserService _userService;

        public HomeController(IConfiguration configuration, UserService userService)
        {
            _httpClient = new HttpClient();
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            //if (!User.Identity.IsAuthenticated)
            //{
            //    return Unauthorized(new { message = "Bu iþlemi yapmak için oturum açmanýz gerekiyor." });
            //}

            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Arama deðeri boþ olamaz");
            }

            // API'ye istek gönder
            var apiUrl = $"{_apiBaseUrl}/Values/Search?query={query}";
            var response = await _httpClient.GetStringAsync(apiUrl);

            // JSON'u deserialize ederken PascalCase property adlarýný korumak için options kullanýyoruz
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Property adlarýný küçük/büyük harf duyarsýz hale getirir
            };

            try
            {
                var results = JsonSerializer.Deserialize<List<ProductModel>>(response, options);

                var formattedResults = results.Select(x => new
                {
                    x.Brand,
                    x.Description,
                    x.Link,
                    x.ListPrice,
                    x.Currency,
                    x.ProductCode,
                    FormattedPrice = x.FormattedPrice
                });

                return Json(formattedResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"JSON deserialize hatasý: {ex.Message}");
            }
        }


    }
}
