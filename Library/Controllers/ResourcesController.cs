using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
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
