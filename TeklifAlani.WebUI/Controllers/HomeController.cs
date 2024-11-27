using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using TeklifAlani.BLL.Abstract;
using TeklifAlani.BLL.Services;
using TeklifAlani.Core.Models;
using TeklifAlani.WebUI.Models;

namespace TeklifAlani.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Arama de�eri bo� olamaz");
            }

            try
            {
                var results = await _productService.SearchProducts(query);

                var model = results.Select(x => new ProductModel()
                {
                    Brand = new Brand() { Id = x.Brand.Id, Name = x.Brand.Name },
                    Description = x.Description,
                    Link = x.Link,
                    ListPrice = x.ListPrice,
                    Currency = x.Currency,
                    ProductCode = x.ProductCode
                }).ToList();
             
                return Json(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"JSON deserialize hatas�: {ex.Message}");
            }
        }


        [HttpPost]
        public IActionResult Print([FromBody] List<ProductModel> model)
        {
            if (model == null || !model.Any())
            {
                return BadRequest("G�nderilen veri bo�.");
            }

            // Burada model �zerinden i�lemleri ger�ekle�tirin.
            // �rne�in, PDF dosyas� olu�turabilir ya da veritaban�na kaydedebilirsiniz.

            // �rnek i�lem:
            Debug.WriteLine($"Toplam �r�n Say�s�: {model.Count}");
            foreach (var item in model)
            {
                Debug.WriteLine($"Marka: {item.Brand.Name}, �r�n Kodu: {item.ProductCode}");
            }

            return Ok("Yazd�rma i�lemi ba�ar�l�.");
        }

    }
}
