using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TeklifAlani.BLL.Abstract;
using TeklifAlani.Core.Models;

namespace TeklifAlani.WebUI.ViewComponents.Account
{
    public class _SupplierBrandViewComponentPartial : ViewComponent
    {
        private readonly IBrandService _brandService;

        public _SupplierBrandViewComponentPartial(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int index, int? selectedBrandId = null)
        {
            var brands = _brandService.GetAll();
            ViewData["SelectedBrandId"] = selectedBrandId;
            ViewData["Index"] = index;
            return View(brands);
        }
    }
}
