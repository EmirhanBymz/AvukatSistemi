using AvukatSistemi.Data;
using AvukatSistemi.Models;
using AvukatSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvukatSistemi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.ToListAsync();
            return View(clients);
        }
        // Müvekkil listesi
        public IActionResult ClientList()
        {
            List<Client> clients = _context.Clients.ToList();
            return View(clients);
        }

        // Müvekkil detayları + dosyalar
        public IActionResult ClientDetails(int id)
        {
            var client = _context.Clients
                .Include(c => c.CaseFiles)
                .FirstOrDefault(c => c.Id == id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        // Dosya indir
        public IActionResult DownloadFile(int id)
        {
            var file = _context.CaseFiles.FirstOrDefault(f => f.Id == id);
            if (file == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, file.FilePath);
            var mimeType = "application/octet-stream";
            return PhysicalFile(filePath, mimeType, file.FileName);
        }

        // Dosya sil
        [HttpPost]
        public IActionResult DeleteFile(int id)
        {
            var file = _context.CaseFiles.FirstOrDefault(f => f.Id == id);
            if (file == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, file.FilePath);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.CaseFiles.Remove(file);
            _context.SaveChanges();

            return RedirectToAction("ClientDetails", new { id = file.ClientId });
        }

        // Dosya ekleme sayfası
        [HttpGet]
        public IActionResult AddFile(int clientId)
        {
            ViewBag.ClientId = clientId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var file = await _context.CaseFiles.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            if (!Enum.TryParse<CaseStatus>(newStatus, out var parsedStatus))
            {
                TempData["Error"] = "Geçersiz durum değeri.";
                return RedirectToAction("ClientDetails", new { id = file.ClientId });
            }

            file.Status = parsedStatus;
            _context.CaseFiles.Update(file);
            await _context.SaveChangesAsync();

            return RedirectToAction("ClientDetails", new { id = file.ClientId });
        }


        // Dosya ekle
        [HttpPost]
        public IActionResult AddFile(int clientId, string description, string status, IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                ModelState.AddModelError("", "Lütfen bir dosya seçin.");
                ViewBag.ClientId = clientId;
                return View();
            }

            if (!Enum.TryParse<CaseStatus>(status, out var caseStatus))
            {
                ModelState.AddModelError("", "Geçersiz durum değeri.");
                ViewBag.ClientId = clientId;
                return View();
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Path.GetFileName(uploadedFile.FileName);
            var filePath = Path.Combine("uploads", fileName);
            var fullPath = Path.Combine(_env.WebRootPath, filePath);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                uploadedFile.CopyTo(stream);
            }

            var caseFile = new CaseFile
            {
                ClientId = clientId,
                Description = description,
                Status = caseStatus,
                FileName = fileName,
                FilePath = filePath
            };

            _context.CaseFiles.Add(caseFile);
            _context.SaveChanges();

            return RedirectToAction("ClientDetails", new { id = clientId });
        }

        // Müvekkil oluşturma sayfası
        [HttpGet]
        public IActionResult CreateClient()
        {
            return View();
        }


        // Müvekkil oluştur
        [HttpPost]
        public async Task<IActionResult> CreateClient(CreateClientViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = new Client
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                TCKimlikNo = model.TCKimlikNo

            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                ClientId = client.Id
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Client");

            return RedirectToAction("ClientList");
        }

        // Müvekkil sil (dosyalarla beraber)
        [HttpPost]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.CaseFiles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            // Dosyaları fiziksel sil
            if (client.CaseFiles != null)
            {
                foreach (var file in client.CaseFiles)
                {
                    var filePath = Path.Combine(_env.WebRootPath, file.FilePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }

            _context.CaseFiles.RemoveRange(client.CaseFiles);
            _context.Clients.Remove(client);

            await _context.SaveChangesAsync();

            return RedirectToAction("ClientList");
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
