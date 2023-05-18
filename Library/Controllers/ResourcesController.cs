using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Controllers
{
    [Authorize]
    public class ResourcesController : Controller
    {
        private readonly LibraryContext _db;
        

        public ResourcesController(LibraryContext db)
        {
            _db = db;
        }
        public IActionResult Resources()
        {
            IEnumerable<Resource> objResourcesList = _db.Resources;
            return View(objResourcesList);
        }
        
        public IActionResult Borrow()
        {
            IEnumerable<Resource> objResourcesList = _db.Resources;
            return View(objResourcesList);
        }
        [HttpPost]
        public IActionResult Borrow(int id)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            var resource = _db.Resources.FirstOrDefault(x => x.Id == id);
            _db.Users
                .FirstOrDefault(x => x.Id == userId).Resources.Add(resource);
            return View();
        }

        public IActionResult BorrowResources(User user)
        {

            return View();
        }

        //GET
        public IActionResult AddResource()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddResource(Resource obj)
        {
                _db.Resources.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Resources");
            
        }
    }
}
