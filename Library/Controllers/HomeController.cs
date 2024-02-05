using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library.Controllers
{
    public class HomeController : Controller
    {
        

        private readonly LibraryContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;
        public readonly AuthenticationSettings _authenticationSettings;

        public HomeController(ILogger<HomeController> logger, LibraryContext db, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _logger = logger;
            _db = db;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
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
            
            if(ModelState.IsValid)
            {
                var user = _db.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == dto.Email);
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
                
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

                var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer,
                    _authenticationSettings.JwtIssuer,
                    claims,
                    expires: expires,
                    signingCredentials: cred);

                var tokenHandler = new JwtSecurityTokenHandler();
                var returnToken = tokenHandler.WriteToken(token);
                HttpContext.Response.Cookies.Append("token", returnToken,
                    new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.None
                    });
                if (user.RoleId == 2)
                {
                    return RedirectToAction("ManagerView", "Logged");

                }
                if (user.RoleId == 3)
                {
                    return RedirectToAction("AdminView", "Logged");

                }
                return RedirectToAction("Index", "Logged");

            }
            return View("Login");

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