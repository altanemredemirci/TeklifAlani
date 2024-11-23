using System.ComponentModel.DataAnnotations;

namespace TeklifAlani.Core.DTOs.ApplicationUserDTO
{
    public class CreateApplicationUserDTO
    {
        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        public string Logo { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(150, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [Display(Name = "Firma Ünvanı")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(30, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [Display(Name = "Ad")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(30, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [Display(Name = "Soyad")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(15, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [Display(Name = "Şehir")]
        public string City { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [Display(Name = "İlçe")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [Display(Name = "Adres")]
        public string Address { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(11, MinimumLength = 9, ErrorMessage = "{0} {2} ile {1} hane arasında olmalıdır.")]
        [RegularExpression(@"^\d{9,11}$", ErrorMessage = "{0} yalnızca 9 ile 11 arasında rakamlardan oluşmalıdır.")]
        [Display(Name = "Vergi No")]
        public string TaxNumber { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "T.C. Kimlik No {1} haneli olmalıdır.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "T.C. Kimlik No yalnızca 11 rakamdan oluşmalıdır.")]
        [Display(Name = "T.C. Kimlik No")]
        public string TC { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [Display(Name = "E-posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(200, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [Display(Name = "Sevk E-posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string ShipmentEmail { get; set; }


        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Telefon numarası tam olarak {1} haneli olmalıdır.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Telefon numarası yalnızca 11 rakamdan oluşmalıdır.")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifreniz en az 6 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]        
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Şifre Tekrarı")]
        public string RePassword { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(65, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [Display(Name = "Kullanıcı Adı")]
        public string? UserName { get; set; }

        public List<CreateSupplierDTO>? Suppliers { get; set; }
    }
}
