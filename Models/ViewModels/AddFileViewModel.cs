using Microsoft.AspNetCore.Mvc;
using AvukatSistemi.ViewModels;
using AvukatSistemi.Data;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;



namespace AvukatSistemi.Models.ViewModels
{
    public class AddFileViewModel
    {
        public int ClientId { get; set; }
        public string Description { get; set; }

        // Dosya yüklemek için bir IFormFile property ekleyebilirsin
        public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
    }

}
