using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Reflection.Metadata;
using System.Security.Claims;


namespace Library.Controllers
{
    public class BorrowsController : Controller
    {
        private readonly LibraryContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public BorrowsController(LibraryContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        
        [Authorize(Roles = "User")]
        [Route("Borrow")]
        public IActionResult Borrow()
        {
            var borrowJson = HttpContext.Session.GetString("Borrow");
            var borrow = borrowJson != null ? JsonConvert.DeserializeObject<BorrowDto>(borrowJson) : null;

            if (borrow == null)
            {
                var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                borrow = new BorrowDto
                {
                    UserId = userId,
                    Status = "Waiting for confirm by user",
                    ReturnDay = DateTime.Now.Date.AddDays(7),
                    Resources = new List<Resource>()
                };
                HttpContext.Session.SetString("Borrow", JsonConvert.SerializeObject(borrow));
            }

            IEnumerable<Resource> objResourcesList = _db.Resources.ToList();
            return View(objResourcesList);
        }

        [Authorize(Roles = "User")]
        [Route("Borrow/{id:int}")]
        public IActionResult Borrow(int id)
        {
            var resource = _db.Resources.Find(id);

            // Retrieve the serialized BorrowDto from session
            var borrowJson = HttpContext.Session.GetString("Borrow");
            var borrow = borrowJson != null ? JsonConvert.DeserializeObject<BorrowDto>(borrowJson) : null;

            if (resource != null && borrow != null)
            {
                borrow.Resources.Add(resource);
                resource.Quantity -= 1;
            }

            // Serialize and store the updated BorrowDto back to session
            HttpContext.Session.SetString("Borrow", JsonConvert.SerializeObject(borrow));

            return RedirectToAction(nameof(Borrow), new { id = string.Empty });
        }

        [Authorize(Roles = "User")]
        public IActionResult BorrowResources()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _db.Users.Include(u => u.Borrows).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            var borrowJson = HttpContext.Session.GetString("Borrow");
            var borrow = borrowJson != null ? JsonConvert.DeserializeObject<BorrowDto>(borrowJson) : null;
            user.Borrows.Add(borrow);
            var userJson = JsonConvert.SerializeObject(user, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            HttpContext.Session.SetString("User", userJson);
            IEnumerable<BorrowDto> objBorrows = user.Borrows.ToList();
            return View(objBorrows);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToDatabase()
        {
            var userJson = HttpContext.Session.GetString("User");

            var user = JsonConvert.DeserializeObject<User>(userJson, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            if (user != null)
            {

                _db.Users.Update(user);
                HttpContext.Session.Remove("User");
                HttpContext.Session.Remove("Borrow");
                // Redirect to a success page or perform any other desired action
                return RedirectToAction("Borrowed");
            }

            HttpContext.Session.Remove("Borrow");
            HttpContext.Session.Remove("User");
            // Handle the case when the user is not found or other error occurs
            // ...

            return View("Error");
        }

        public IActionResult Borrowed()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _db.Users.Include(u => u.Borrows).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            IEnumerable<BorrowDto> objBorrows = user.Borrows.ToList();
            return View(objBorrows);
        }
    }
}
