using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;

        public HomeController(ILogger<HomeController> logger, LibraryContext db, IPasswordHasher<User> passwordHasher)
        {
            _logger = logger;
            _db = db;
            _passwordHasher = passwordHasher;
        }
         
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Register()
        {
            return View();
        }
        
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterDto dto)
        {
            var newUser = new User()
                {
                Email = dto.Email,
                FirstName = String.Empty,
                LastName = String.Empty,
                RoleId = 1,
                DateOfBirth = new DateOnly(1900, 01, 01),
                PersonalIdNumber = String.Empty,
                PhoneNumber = String.Empty,
                DateOfUserCreation = DateOnly.FromDateTime(DateTime.Now),
                };
                var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.PasswordHash = hashedPassword;
                _db.Users.Add(newUser);
                _db.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View("Login");
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginDto dto)
        {
            
            return RedirectToAction("Login");
        }



        public IActionResult Privacy()
        {
            return View();
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}