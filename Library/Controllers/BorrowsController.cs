using Library.Data;
using Library.Models;
using Library.ViewModels;
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
        public IActionResult Borrow()
        {
            var borrowJson = HttpContext.Session.GetString("Borrow");
            var borrow = borrowJson != null ? JsonConvert.DeserializeObject<BorrowDto>(borrowJson) : null;

            if (borrow == null)
            {
                borrow = new BorrowDto
                {
                    Status = "Waiting to collect resources for borrow",
                    ReturnDay = DateTime.Now.Date.AddDays(7),
                    Resources = new List<Resource>()
                };
                HttpContext.Session.SetString("Borrow", JsonConvert.SerializeObject(borrow));
            }

            List<Resource> resourcesList = _db.Resources.ToList();

            var viewModel = new BorrowViewModel
            {
                Borrow = borrow,
                Resources = resourcesList
            };
            if (TempData.ContainsKey("ResourceBorrowed"))
            {
                ModelState.AddModelError("ResourceBorrowed", TempData["ResourceBorrowed"].ToString());
            }

            return View(viewModel);
        }

        [Authorize(Roles = "User")]
        [Route("Borrow/{id:int}")]
        public IActionResult Borrow(int id)
        {
            var resource = _db.Resources.Find(id);
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var borrowJson = HttpContext.Session.GetString("Borrow");
            var borrow = borrowJson != null ? JsonConvert.DeserializeObject<BorrowDto>(borrowJson) : null;

            if (resource != null && borrow != null)
            {
                
                var alreadyBorrowedInOther = _db.Borrows
                    .Any(b => b.UserId == userId &&  b.Resources.Any(r => r.Id == id));

                if (!alreadyBorrowedInOther)
                {
                    borrow.Resources.Add(resource);
                    resource.Quantity -= 1;
                    _db.Resources.Update(resource);
                    _db.SaveChanges();
                }
                else
                {
                    TempData["ResourceBorrowed"] = "You have already borrowed this resource in another borrow.";
                }
            }

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
            if(borrow.Resources.Any())
            {
                user.Borrows.Add(borrow);
                _db.Users.Update(user);
                _db.SaveChanges();
                HttpContext.Session.Remove("Borrow");
                return RedirectToAction("Borrowed");
            }
            else
            {
                return RedirectToAction(nameof(Borrow), new { id = string.Empty });
            }
        }

        public IActionResult CancelBorrow()
        {
            var borrowJson = HttpContext.Session.GetString("Borrow");
            var borrow = borrowJson != null ? JsonConvert.DeserializeObject<BorrowDto>(borrowJson) : null;

            if (borrow != null)
            {
                foreach (var resource in borrow.Resources)
                {
                    var dbResource = _db.Resources.Find(resource.Id);
                    if (dbResource != null)
                    {
                        dbResource.Quantity += 1; 
                        _db.Resources.Update(dbResource);
                    }
                }

                _db.SaveChanges(); 

                HttpContext.Session.Remove("Borrow"); 
            }

            return RedirectToAction(nameof(Borrow), new { id = string.Empty });
        }




        [Authorize(Roles = "User")]
        public IActionResult Borrowed()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _db.Users
                   .Include(u => u.Borrows)
                       .ThenInclude(b => b.Resources)  
                   .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            IEnumerable<BorrowDto> objBorrows = user.Borrows.ToList();
            return View(objBorrows);
        }

        [Authorize(Roles = "Admin, Manager")]
        public IActionResult UsersBorrows()
        {
            var usersWithBorrows = _db.Users
                .Include(u => u.Borrows)
                    .ThenInclude(b => b.Resources);

            var userBorrowsList = new List<(Guid Id, string UserName, IEnumerable<BorrowDto> Borrows)>();

            foreach (var user in usersWithBorrows)
            {
                var userBorrows = (Id: user.Id, UserName: $"{user.FirstName} {user.LastName}", Borrows: user.Borrows);
                userBorrowsList.Add(userBorrows);
            }

            return View(userBorrowsList);
        }


        [Authorize(Roles = "Admin, Manager")]
        public IActionResult DeleteBorrow(Guid userId, int borrowId)
        {
            var user = _db.Users.Include(u => u.Borrows).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(); // Jeśli użytkownik nie istnieje, zwracamy NotFound
            }

            var borrowToRemove = user.Borrows.FirstOrDefault(b => b.Id == borrowId);
            if (borrowToRemove == null)
            {
                return NotFound(); // Jeśli nie znaleziono BorrowDto o podanym ID w liście Borrows użytkownika, zwracamy NotFound
            }
            
            user.Borrows.Remove(borrowToRemove); // Usuwamy BorrowDto z listy Borrows

            _db.SaveChanges(); // Zapisujemy zmiany
            return RedirectToAction("UsersBorrows");
        }
    }

}
