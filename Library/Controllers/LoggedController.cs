using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class LoggedController : Controller
    {
        // Action for logged-in users to access their dashboard
        // Only users with roles "User", "Manager", or "Admin" can access this view
        [Authorize(Roles = "User, Manager, Admin")]
        public IActionResult Index()
        {
            return View();
        }

        // Action to log the user out by removing the authentication token
        [HttpGet]
        public IActionResult Logout()
        {
            // Remove the authentication token stored in the cookies
            Response.Cookies.Delete("token");

            // Redirect the user to the home page after logout
            return RedirectToAction("Index", "Home");
        }
    }
}
