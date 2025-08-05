using Microsoft.AspNetCore.Mvc;

namespace AvukatSistemi.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; } // Örn: dosya.pdf
        public string FilePath { get; set; } // Eklenmeli

        public DateTime UploadDate { get; set; }

        public string Description { get; set; }

        public int ClientId { get; set; }         // ⬅️ EF bu alanı veritabanında arıyor
        public Client Client { get; set; }
    }
}
