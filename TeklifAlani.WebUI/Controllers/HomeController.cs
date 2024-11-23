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
                return BadRequest("Arama deðeri boþ olamaz");
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
                return StatusCode(500, $"JSON deserialize hatasý: {ex.Message}");
            }
        }


    }
}
