using AvukatSistemi.Data;  // DbContext için namespace
using AvukatSistemi.Models;
using AvukatSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AvukatSistemi.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager,
                                 UserManager<ApplicationUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (await _userManager.IsInRoleAsync(user, "Client")) // Rol adı kesinlikle "Client" olmalı
                {
                    return RedirectToAction("Dashboard", "Clients");
                }
                else if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                // Eğer kullanıcı herhangi bir rolü yoksa veya farklıysa buraya da yönlendirme eklenebilir.
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Geçersiz giriş.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Rol varsa ekle, yoksa oluştur ve ekle
                if (!await _roleManager.RoleExistsAsync("Client"))
                    await _roleManager.CreateAsync(new IdentityRole("Client"));

                await _userManager.AddToRoleAsync(user, "Client");

                // Client nesnesini oluştur ve kaydet
                var client = new Client
                {
                    Name = model.Username,
                    Email = model.Email,
                    Phone = model.Phone,
                    TCKimlikNo = model.TCKimlikNo
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToAction("Dashboard", "Clients");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
    }
}
