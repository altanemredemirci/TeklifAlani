using Microsoft.AspNetCore.Mvc;
using TeklifAlani.BLL.Abstract;

namespace TeklifAlani.WebUI.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var brands = _brandService.GetAll(); // Marka listesini alın
            return Json(brands); // JSON olarak döndür
        }
    }
}
