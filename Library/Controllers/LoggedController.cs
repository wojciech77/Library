using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using NuGet.Common;

namespace Library.Controllers
{
    public class LoggedController : Controller
    {
        

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
