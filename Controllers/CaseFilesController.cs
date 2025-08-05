using AvukatSistemi.Data;
using AvukatSistemi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AvukatSistemi.Controllers
{
    [Authorize(Roles = "Client")]
    public class CaseFilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CaseFilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var client = await _context.Clients.FirstOrDefaultAsync(c => c.TCKimlikNo == user.UserName);

            if (client == null)
                return NotFound();

            var caseFiles = await _context.CaseFiles
                .Where(cf => cf.ClientId == client.Id)
                .ToListAsync();

            return View(caseFiles);
        }

        public async Task<IActionResult> Download(int id)
        {
            var caseFile = await _context.CaseFiles.FindAsync(id);
            if (caseFile == null)
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", caseFile.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var contentType = "application/octet-stream";
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, contentType, caseFile.FileName);
        }
    }
}
