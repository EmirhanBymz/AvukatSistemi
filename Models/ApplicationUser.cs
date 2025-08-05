using Microsoft.AspNetCore.Identity;


    namespace AvukatSistemi.Models
    {
        public class ApplicationUser : IdentityUser
        {
            public int? ClientId { get; set; }

         public Client? ClientProfile { get; set; }  


         }
    }
