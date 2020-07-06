using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OneTimePassword.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        { }
        
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
    }
}
