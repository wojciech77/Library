using Library.Data;
using Library.Models;
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
        // Dependencies for the HomeController
        private readonly LibraryContext _db;                          // Entity Framework DbContext for accessing database
        private readonly IPasswordHasher<User> _passwordHasher;       // Service for hashing and verifying passwords
        public readonly AuthenticationSettings _authenticationSettings;  // App-specific authentication settings for JWT

        // Constructor to inject dependencies
        public HomeController(LibraryContext db, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        // Action to render the main landing page (Index view)
        public IActionResult Index()
        {
            return View();
        }

        // Action to render the registration form
        public IActionResult Register()
        {
            return View();
        }

        // POST: Handles user registration
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF attacks
        public IActionResult Register(RegisterDto dto)
        {
            // Check if the provided data is valid
            if (ModelState.IsValid)
            {
                // Create a new user with initial default values
                var newUser = new User()
                {
                    Email = dto.Email,
                    FirstName = String.Empty,
                    LastName = String.Empty,
                    RoleId = 1,  // Default role (e.g. User)
                    DateOfBirth = new DateTime(1900, 01, 01),  // Placeholder birthdate
                    PersonalIdNumber = String.Empty,           // Empty personal ID number
                    PhoneNumber = String.Empty,                // Empty phone number
                    DateOfUserCreation = DateTime.Now          // Set account creation date to now
                };

                // Hash the password before saving it
                var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
                newUser.PasswordHash = hashedPassword;

                // Save the new user to the database
                _db.Users.Add(newUser);
                _db.SaveChanges();

                // Redirect the user to the Login page after successful registration
                return RedirectToAction("Login");
            }
            else
            {
                // Handle case when the email is already taken
                if (ModelState.TryGetValue("Email", out var entry) && entry.Errors.Any(e => e.ErrorMessage == "That email is taken"))
                {
                    ModelState.AddModelError("Email", "That email is taken. Please choose a different email.");
                }

                // Redisplay the registration form with validation errors
                return View("Register", dto);
            }
        }

        // Action to render the login form
        public IActionResult Login()
        {
            return View("Login");
        }

        // POST: Handles user login
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF attacks
        public IActionResult Login(LoginDto dto)
        {
            // Validate the form inputs
            if (ModelState.IsValid)
            {
                // Find the user by email, including the role data
                var user = _db.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Email == dto.Email);

                // Verify the password using the stored hash
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    // Create claims for JWT token (includes user ID, full name, and role)
                    var claims = new List<Claim>()
                    {
                        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                        new(ClaimTypes.Role, $"{user.Role.Name}"),
                    };

                    // Generate the security key and credentials for signing the JWT
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
                    var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays); // Token expiration

                    // Create the JWT token
                    var token = new JwtSecurityToken(
                        issuer: _authenticationSettings.JwtIssuer,
                        audience: _authenticationSettings.JwtIssuer,
                        claims: claims,
                        expires: expires,
                        signingCredentials: cred
                    );

                    // Write the token to a cookie with specific options (e.g., HttpOnly, Secure)
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var returnToken = tokenHandler.WriteToken(token);
                    HttpContext.Response.Cookies.Append("token", returnToken, new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        HttpOnly = true,
                        Secure = true,
                        IsEssential = true,
                        SameSite = SameSiteMode.None
                    });

                    // Redirect to a different controller after successful login
                    return RedirectToAction("Index", "Logged");
                }
                else
                {
                    // Show error when email or password is incorrect
                    ModelState.AddModelError("Email", "Email or password doesn't match.");
                    return View("Login", dto);
                }
            }
            else
            {
                // Check for other validation errors and redisplay the login form
                if (ModelState.TryGetValue("Email", out var entry) && entry.Errors.Any(e => e.ErrorMessage == "Email or password doesn't match."))
                {
                    ModelState.AddModelError("Email", "Email or password doesn't match.");
                }

                return View("Login", dto);
            }
        }

        // Action to display privacy policy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Action to handle error pages, such as 404 or 500
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
