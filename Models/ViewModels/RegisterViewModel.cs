using System.ComponentModel.DataAnnotations;

namespace AvukatSistemi.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "TC Kimlik No")]
        public string TCKimlikNo { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor")]
        public string ConfirmPassword { get; set; }
    }
}