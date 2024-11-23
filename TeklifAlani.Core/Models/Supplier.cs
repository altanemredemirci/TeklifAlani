using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TeklifAlani.Core.Identity;

namespace TeklifAlani.Core.Models
{
    public class Supplier : IValidatableObject
    {
        public int Id { get; set; }
       
        public string CompanyName { get; set; }
      
        public string ContactName { get; set; }
        
        public string? Phone { get; set; }
     
        public string? Email { get; set; }

        // Supplier ile Brand arasında ilişki
        public int BrandId { get; set; }
               
        public Brand? Brand { get; set; }

        // ApplicationUser ile ilişki
        public string? ApplicationUserId { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Phone) && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult(
                    "Telefon veya Email bilgisinden en az birini girmelisiniz.",
                    new[] { nameof(Phone), nameof(Email) });
            }
        }
    }
}
