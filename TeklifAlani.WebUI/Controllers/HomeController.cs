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
            //    return Unauthorized(new { message = "Bu i�lemi yapmak i�in oturum a�man�z gerekiyor." });
            //}

            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Arama de�eri bo� olamaz");
            }

            // API'ye istek g�nder
            var apiUrl = $"{_apiBaseUrl}/Values/Search?query={query}";
            var response = await _httpClient.GetStringAsync(apiUrl);

            // JSON'u deserialize ederken PascalCase property adlar�n� korumak i�in options kullan�yoruz
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Property adlar�n� k���k/b�y�k harf duyars�z hale getirir
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
                return StatusCode(500, $"JSON deserialize hatas�: {ex.Message}");
            }
        }


    }
}
