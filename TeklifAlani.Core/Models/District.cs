using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TeklifAlani.Core.Identity;

namespace TeklifAlani.Core.Models
{
    public class District
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public List<ApplicationUser> ApplicationUsers { get; set; }
    }
}
