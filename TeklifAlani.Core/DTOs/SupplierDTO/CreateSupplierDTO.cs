using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TeklifAlani.Core.DTOs
{
    public class CreateSupplierDTO : IValidatableObject
    {
        [Display(Name = "Tedarikçi ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Şirket Adı alanı zorunludur.")]
        [StringLength(150, ErrorMessage = "Şirket Adı en fazla {1} karakter olabilir.")]
        [Display(Name = "Şirket Adı")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "İletişim Kişisi alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "İletişim Kişisi en fazla {1} karakter olabilir.")]
        [Display(Name = "İletişim Kişisi")]
        public string ContactName { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "Telefon numarası tam olarak {1} haneli olmalıdır.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Telefon numarası yalnızca 11 rakamdan oluşmalıdır.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [StringLength(200, ErrorMessage = "E-posta en fazla {1} karakter olabilir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Marka alanı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir marka seçiniz.")]
        [Display(Name = "Marka")]
        public int BrandId { get; set; }

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
