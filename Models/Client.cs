using System.ComponentModel.DataAnnotations;

namespace AvukatSistemi.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        

        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // !!! Bu alan eklenmeli: Giriş yapan kullanıcıyla eşleşme için
        [Required]
        public string TCKimlikNo { get; set; } = null!;

        public ICollection<Document> Documents { get; set; } = new List<Document>();

        public ICollection<CaseFile> CaseFiles { get; set; } = new List<CaseFile>();
    }
}
