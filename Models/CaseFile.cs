using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace AvukatSistemi.Models
{
    public class CaseFile
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string FilePath { get; set; } = string.Empty;

        public CaseStatus Status { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public int ClientId { get; set; } // <-- EF bu sütunu bekliyor

        public Client Client { get; set; }
    }
}
