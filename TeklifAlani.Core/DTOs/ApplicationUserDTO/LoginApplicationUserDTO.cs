using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeklifAlani.Core.DTOs.ApplicationUserDTO
{
    public class LoginApplicationUserDTO
    {
        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Telefon numarası tam olarak {1} haneli olmalıdır.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Telefon numarası yalnızca 11 rakamdan oluşmalıdır.")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "{0} alanı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifreniz en az 6 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")] 
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
