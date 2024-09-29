using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class LoggedController : Controller
    {

        [Authorize(Roles = "User, Manager, Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return RedirectToAction("Index", "Home");
        }


    }
}
