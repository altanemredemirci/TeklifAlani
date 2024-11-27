using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using TeklifAlani.Core.Models;

namespace TeklifAlani.WebUI.Models
{
    public class ProductModel
    {
        public string ProductCode { get; set; }

        public Brand Brand { get; set; }

        public string Description { get; set; }

        public decimal ListPrice { get; set; }

        public string Currency { get; set; }

        public string Link { get; set; }

        public int Quantity { get; set; }

        public string DeliveryTime { get; set; }

        public double TotalPrice { get; set; }

        public string FormattedPrice => ListPrice.ToString()+Currency;

        public string FormattedTotalPrice => TotalPrice.ToString("N", new CultureInfo("tr-TR"))+Currency;
    }
}
