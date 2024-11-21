using System.Text.Json.Serialization;

namespace TeklifAlani.Core.Models
{
    public class Brand
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public List<Product> Products { get; set; }  
    }
}
