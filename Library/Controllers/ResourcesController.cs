using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace Library.Controllers
{
    
    public class ResourcesController : Controller
    {
        private readonly LibraryContext _db;
        

        public ResourcesController(LibraryContext db)
        {
            _db = db;
        }
        [Authorize]
        public IActionResult Resources()
        {
            IEnumerable<Resource> objResourcesList = _db.Resources;
            return View(objResourcesList);
        }

        [Authorize(Roles = "Admin, Manager")]
        //GET
        public IActionResult AddResource()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Manager")]
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddResource(Resource obj)
        {
            _db.Resources.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Resources");

        }
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult EditResource(int id)
        {
            var resource = _db.Resources.Find(id);
            return View(resource);
        }
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult EditResource(Resource obj)
        {
            _db.Resources.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Resources");
        }
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult DeleteResource(int id)
        {
            var resource = _db.Resources.Find(id);
            _db.Resources.Remove(resource);
            _db.SaveChanges();
            return RedirectToAction("Resources");
        }
        [Authorize(Roles = "User")]
        [Route("Borrow")]
        public IActionResult Borrow()
        {
            IEnumerable<Resource> objResourcesList = _db.Resources.ToList();
            return View(objResourcesList);
        }
        [Authorize(Roles = "User")]
        [Route("Borrow/{id:int}")]
        public IActionResult Borrow(int id)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var resource = _db.Resources.Find(id);
            var user = _db.Users.Include(u => u.Resources).Single(x => x.Id == userId);

            if (resource != null && user != null)
            {
                user.Resources.Add(resource);
                resource.Quantity -= 1;
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Borrow), new { id = string.Empty });
        }
        [Authorize(Roles = "User")]
        public IActionResult BorrowResources()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _db.Users.Include(u => u.Resources).FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                // User not found, handle the error or redirect to an appropriate page
                return NotFound();
            }

            IEnumerable<Resource> objResourcesList = user.Resources;
            return View(objResourcesList);
        }


        
    }
}
