using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using NuGet.Common;
using Microsoft.AspNetCore.Authorization;
using System.Data;

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
