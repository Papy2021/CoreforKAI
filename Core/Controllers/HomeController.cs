using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IActionResult Welcome()
        {
            return View();
        }
    }
}
