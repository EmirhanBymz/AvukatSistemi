using System.ComponentModel.DataAnnotations;

namespace AvukatSistemi.ViewModels
{
    public class CreateClientViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string TCKimlikNo { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

        public string? Description { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
