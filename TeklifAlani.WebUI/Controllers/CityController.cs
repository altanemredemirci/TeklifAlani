using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeklifAlani.BLL.Abstract;

namespace TeklifAlani.WebUI.Controllers
{
    public class CityController : Controller
    {
        private readonly ICityService _cityService;

        public CityController(ICityService cityService)
        {
            _cityService = cityService;
        }

        public IActionResult DistrictByCity(int id)
        {
            var districts = _cityService.GetDistrictsByCityId(id)
                                        .Select(d => new
                                        {
                                            d.Id,
                                            d.Name
                                        }).ToList();

            return Json(districts);
        }
    }
}
