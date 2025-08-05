using AvukatSistemi.Data;
using AvukatSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvukatSistemi.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env; // Burası artık çalışır
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var client = await _context.Clients
                .Include(c => c.CaseFiles)
                .FirstOrDefaultAsync(c => c.Email == user.UserName);

            if (client == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(client);
        }


        [HttpGet]
        public async Task<IActionResult> Files()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var client = await _context.Clients
                .Include(c => c.CaseFiles)
                .FirstOrDefaultAsync(c => c.Email == user.UserName);

            if (client == null)
                return NotFound();

            return View(client.CaseFiles.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var userName = User.Identity.Name;

            var client = await _context.Clients
                .Include(c => c.CaseFiles)
                .FirstOrDefaultAsync(c => c.Email == userName);

            if (client == null)
                return NotFound("Müvekkil bulunamadı");

            var file = client.CaseFiles.FirstOrDefault(d => d.Id == id);
            if (file == null)
                return NotFound("Dosya bulunamadı");

            var filePath = Path.Combine(_env.WebRootPath, "uploads", file.FileName); // ← burası değişti

            if (!System.IO.File.Exists(filePath))
                return NotFound("Sunucuda dosya bulunamadı");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, "application/octet-stream", file.FileName);
        }

    }
}
