using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Library.Controllers
{
    public class BorrowsController : Controller
    {
        private readonly LibraryContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession Session => _httpContextAccessor.HttpContext.Session;

        // Constructor for dependency injection
        public BorrowsController(LibraryContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        // Action to display the borrow page with available resources
        [Authorize(Roles = "User")]
        public IActionResult Borrow()
        {
            // Retrieve the borrow session data
            var borrowJson = HttpContext.Session.GetString("Borrow");
            var borrow = borrowJson != null ? JsonConvert.DeserializeObject<BorrowDto>(borrowJson) : null;

            // Initialize a new borrow session if none exists
            if (borrow == null)
            {
                borrow = new BorrowDto
                {
                    Status = "Waiting to collect resources for borrow",
                    Resources = new List<Resource>()
                };
                HttpContext.Session.SetString("Borrow", JsonConvert.SerializeObject(borrow));
            }

            // Handle search functionality via TempData
            string? searchString = TempData["SearchString"] as string;
            List<Resource> resourcesList;

            // Fetch the resources, filtered by the search string if present
            if (string.IsNullOrWhiteSpace(searchString))
            {
                resourcesList = _db.Resources.ToList();
            }
            else
            {
                resourcesList = _db.Resources
                                   .Where(r => r.Title.Contains(searchString) ||
                                               r.Author.Contains(searchString) ||
                                               r.Type.Contains(searchString) ||
                                               r.Category.Contains(searchString))
                                   .ToList();
            }

            // Prepare view model with borrow session and resources
            var viewModel = new BorrowDtoResources
            {
                Borrow = borrow,
                Resources = resourcesList
            };

            // Handle errors if a resource is already borrowed
            if (TempData.ContainsKey("ResourceBorrowed"))
            {
                ModelState.AddModelError("ResourceBorrowed", TempData["ResourceBorrowed"].ToString());
            }

            return View(viewModel);
        }

        // Action to borrow a specific resource by ID
        [Authorize(Roles = "User")]
        [Route("Borrow/{id:int}")]
        public IActionResult Borrow(int id)
        {
            var resource = _db.Resources.Find(id);
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var borrowJson = HttpContext.Session.GetString("Borrow");
            var borrow = borrowJson != null ? JsonConvert.DeserializeObject<BorrowDto>(borrowJson) : null;

            // Check if the resource exists and if the borrow session is active
            if (resource != null && borrow != null)
            {
                // Limit the number of resources a user can borrow to 3
                if (borrow.Resources.Count <= 2)
                {
                    // Ensure the resource hasn't already been borrowed by the user
                    var alreadyBorrowed = _db.Borrows.Any(b => b.UserId == userId && b.Resources.Any(r => r.Id == id));

                    if (!alreadyBorrowed)
                    {
                        borrow.Resources.Add(resource);
                        resource.Quantity -= 1; // Decrease the resource quantity
                        _db.Resources.Update(resource);
                        _db.SaveChanges();
                    }
                    else
                    {
                        TempData["ResourceBorrowed"] = "This resource has already been borrowed.";
                    }
                }
                else
                {
                    TempData["ResourcesMaximum"] = "You have reached the maximum number of resources in one borrow.";
                }
            }

            HttpContext.Session.SetString("Borrow", JsonConvert.SerializeObject(borrow));
            return RedirectToAction(nameof(Borrow), new { id = string.Empty });
        }

        // Action to handle search functionality and redirect to Borrow
        [Authorize(Roles = "User")]
        public IActionResult Search(string searchString)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                TempData["SearchString"] = searchString;
            }
            return RedirectToAction(nameof(Borrow), new { id = string.Empty });
        }

        // Action to finalize the borrowing process
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

            // If resources are in the borrow session, finalize the borrow
            if (borrow.Resources.Any())
            {
                user.Borrows.Add(borrow);
                _db.Users.Update(user);
                _db.SaveChanges();
                HttpContext.Session.Remove("Borrow"); // Clear the session
                return RedirectToAction("Borrowed");
            }
            else
            {
                return RedirectToAction(nameof(Borrow), new { id = string.Empty });
            }
        }

        // Action to cancel an active borrow and restore resource quantities
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
                        dbResource.Quantity += 1; // Restore resource quantity
                        _db.Resources.Update(dbResource);
                    }
                }

                _db.SaveChanges();
                HttpContext.Session.Remove("Borrow"); // Clear the session
            }

            return RedirectToAction(nameof(Borrow), new { id = string.Empty });
        }

        // Action to display borrowed resources for the current user
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

            return View(user.Borrows.ToList());
        }

        // Action for admin/managers to view all user borrows
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult UsersBorrows()
        {
            var borrows = _db.Borrows
                             .Include(b => b.Resources)
                             .Include(b => b.User)
                             .ToList();

            return View(borrows);
        }

        // Action to change the borrow status by admin/manager
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public IActionResult ChangeStatus(int borrowId, string status)
        {
            var borrow = _db.Borrows.Find(borrowId);

            if (borrow != null)
            {
                borrow.Status = status;

                if (status == "Borrowed")
                {
                    borrow.BorrowDay = DateTime.Now;
                    borrow.ReturnDay = DateTime.Now.AddDays(14); // Set return day
                }

                if (status == "Returned")
                {
                    borrow.ReturnDay = DateTime.Now;
                }

                _db.SaveChanges();
            }

            return RedirectToAction("UsersBorrows");
        }

        // Action to delete a borrow for a specific user
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult DeleteBorrow(Guid userId, int borrowId)
        {
            var user = _db.Users.Include(u => u.Borrows).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var borrowToRemove = user.Borrows.FirstOrDefault(b => b.Id == borrowId);

            if (borrowToRemove == null)
            {
                return NotFound();
            }

            user.Borrows.Remove(borrowToRemove);
            _db.SaveChanges();

            return RedirectToAction("UsersBorrows");
        }

        // Action to search through borrows based on criteria
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult SearchBorrows(string searchString)
        {
            IQueryable<BorrowDto> borrowsQuery = _db.Borrows
                .Include(b => b.User);

            if (!string.IsNullOrEmpty(searchString))
            {
                borrowsQuery = borrowsQuery.Where(b =>
                    b.Status.Contains(searchString) || 
                    b.User.FirstName.Contains(searchString) || 
                    b.User.LastName.Contains(searchString) || 
                    b.User.PersonalIdNumber.Contains(searchString) || 
                    b.ReturnDay.ToString().Contains(searchString)); 
            }

            IEnumerable<BorrowDto> objBorrowsList = borrowsQuery.ToList();
            return View("UsersBorrows", objBorrowsList); // 
        }
    }
}
