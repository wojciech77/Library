using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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

            string? searchString = TempData["SearchString"] as string;
            List<Resource> resourcesList;

            if (string.IsNullOrWhiteSpace(searchString))
            {
                resourcesList = _db.Resources.ToList();
            }
            else
            {
                IQueryable<Resource> resourcesQuery = _db.Resources;

                
                resourcesQuery = resourcesQuery.Where(r =>
                     r.Title.Contains(searchString) ||
                     r.Author.Contains(searchString) ||
                     r.Type.Contains(searchString) ||
                     r.Category.Contains(searchString));

                resourcesList = resourcesQuery.ToList();

            }

            var viewModel = new BorrowDtoResources
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
        public IActionResult Search(string searchString)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                TempData["SearchString"] = searchString;

            }
            return RedirectToAction(nameof(Borrow), new { id = string.Empty });

        }



        [Authorize(Roles = "User")]
        public IActionResult BorrowResources()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _db.Users
                .Include(u => u.Borrows)
                .FirstOrDefault(u => u.Id == userId);

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
        [HttpGet]
        public IActionResult ChangeStatus(int borrowId, string status)
        {
            var borrow = _db.Borrows.Find(borrowId);
            if (borrow != null)
            {
                borrow.Status = status; // Zaktualizuj status
                _db.SaveChanges(); // Zapisz zmiany w bazie danych
            }
            return RedirectToAction("UsersBorrows"); // Przekieruj do odpowiedniego widoku
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
