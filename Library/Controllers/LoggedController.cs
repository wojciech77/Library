using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    public class LoggedController : Controller
    {
        

        public IActionResult Index()
        {
            return View();
        }

        

        
    }
}
