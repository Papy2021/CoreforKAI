using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{currentCode}")]
        public IActionResult HttpStatusCodeHandler(int currentCode)
        {
            switch (currentCode)
            {
                case 404:
                    ViewBag.MessageEr = "The requested page couldnt be found";
                    break;
                case 500:
                    ViewBag.MessageEr = "The App Can't Resolve This Request";
                    break;

            }
            return View("ErrPage");
        }



        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View("Error");
        }
    }
}
