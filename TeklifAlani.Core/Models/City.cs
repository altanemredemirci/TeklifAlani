using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TeklifAlani.Core.Models
{
    public class City
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public List<District> Districts { get; set; }
    }
}
