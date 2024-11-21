using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using TeklifAlani.Core.Models;

namespace TeklifAlani.WebUI.Models
{
    public class ProductModel
    {
        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("brand")]
        public Brand Brand { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("listPrice")]
        public decimal ListPrice { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("deliveryTime")]
        public string DeliveryTime { get; set; }

        [JsonPropertyName("totalPrice")]
        public double TotalPrice { get; set; }

        [JsonPropertyName("formattedPrice")]
        public string FormattedPrice => ListPrice.ToString("N", new CultureInfo("tr-TR"))+Currency;

        [JsonPropertyName("formattedTotalPrice")]
        public string FormattedTotalPrice => TotalPrice.ToString("N", new CultureInfo("tr-TR"))+Currency;
    }
}
