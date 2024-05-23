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
            if (ModelState.IsValid)
            {
                var newUser = new User()
                {
                    Email = dto.Email,
                    FirstName = String.Empty,
                    LastName = String.Empty,
                    RoleId = 1,
                    DateOfBirth = new DateTime(1900, 01, 01),
                    PersonalIdNumber = String.Empty,
                    PhoneNumber = String.Empty,
                    DateOfUserCreation = DateTime.Now,
                };
                var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.PasswordHash = hashedPassword;
                _db.Users.Add(newUser);
                _db.SaveChanges();
                return RedirectToAction("Login");
            }
            else
            {
                
                if (ModelState.TryGetValue("Email", out var entry) && entry.Errors.Any(e => e.ErrorMessage == "That email is taken"))
                {
                    ModelState.AddModelError("Email", "That email is taken. Please choose a different email.");
                }

                return View("Register", dto);
            }
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
                if (result == PasswordVerificationResult.Success)
                {
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

                    return RedirectToAction("Index", "Logged");
                }
                else
                {
                    ModelState.AddModelError("Email", "Email or password doesn't match.");
                    return View("Login", dto);
                }

            }
            else
            {

                if (ModelState.TryGetValue("Email", out var entry) && entry.Errors.Any(e => e.ErrorMessage == "Email or password doesn't match."))
                {
                    ModelState.AddModelError("Email", "Email or password doesn't match.");
                }

                return View("Login", dto);
            }

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