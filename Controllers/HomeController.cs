using Microsoft.AspNetCore.Mvc;

namespace AvukatSistemi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
