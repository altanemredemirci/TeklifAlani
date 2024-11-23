using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TeklifAlani.Core.Models
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<District> Districts { get; set; }
    }
}
