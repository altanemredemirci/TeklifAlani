using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TeklifAlani.Core.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductCode { get; set; }

        public string Description { get; set; }

        public decimal ListPrice { get; set; }

        public string Currency { get; set; }

        public string Link { get; set; }

        public int BrandId { get; set; }

        [JsonPropertyName("brand")]
        public Brand Brand { get; set; }
    }

}
