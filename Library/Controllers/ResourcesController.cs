using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
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

        // Display a list of resources, accessible to Admin and Manager roles
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Resources()
        {
            var resourcesList = _db.Resources.ToList();
            return View(resourcesList);
        }

        // Search resources by title, author, type, or category
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult SearchResources(string searchString)
        {
            var resourcesQuery = _db.Resources.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                resourcesQuery = resourcesQuery.Where(r =>
                    r.Title.Contains(searchString) ||
                    r.Author.Contains(searchString) ||
                    r.Type.Contains(searchString) ||
                    r.Category.Contains(searchString));
            }

            return View("Resources", resourcesQuery.ToList());
        }

        // GET: Show form for adding a new resource
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult AddResource()
        {
            return View();
        }

        // POST: Add a new resource to the database
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddResource(Resource resource)
        {
            if (ModelState.IsValid)
            {
                _db.Resources.Add(resource);
                _db.SaveChanges();
                return RedirectToAction("Resources");
            }

            // Handle validation errors for resource fields
            ValidateResourceFields();
            return View("AddResource", resource);
        }

        // GET: Show form to edit an existing resource
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult EditResource(int id)
        {
            var resource = _db.Resources.Find(id);
            return View(resource);
        }

        // POST: Save changes to an existing resource
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public IActionResult EditResource(Resource resource)
        {
            if (ModelState.IsValid)
            {
                _db.Resources.Update(resource);
                _db.SaveChanges();
                return RedirectToAction("Resources");
            }

            // Handle validation errors for resource fields
            ValidateResourceFields();
            return View("EditResource", resource);
        }

        // DELETE: Remove a resource from the database
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult DeleteResource(int id)
        {
            var resource = _db.Resources.Find(id);
            if (resource != null)
            {
                _db.Resources.Remove(resource);
                _db.SaveChanges();
            }
            return RedirectToAction("Resources");
        }

        // Helper method to handle validation errors for resource fields
        private void ValidateResourceFields()
        {
            if (ModelState.TryGetValue("Title", out var titleEntry) && titleEntry.Errors.Any())
            {
                ModelState.AddModelError("Title", "Invalid title.");
            }

            if (ModelState.TryGetValue("Author", out var authorEntry) && authorEntry.Errors.Any())
            {
                ModelState.AddModelError("Author", "Invalid author.");
            }

            if (ModelState.TryGetValue("Type", out var typeEntry) && typeEntry.Errors.Any())
            {
                ModelState.AddModelError("Type", "Invalid type.");
            }

            if (ModelState.TryGetValue("Category", out var categoryEntry) && categoryEntry.Errors.Any())
            {
                ModelState.AddModelError("Category", "Invalid category.");
            }

            if (ModelState.TryGetValue("Quantity", out var quantityEntry) && quantityEntry.Errors.Any())
            {
                ModelState.AddModelError("Quantity", "Invalid quantity.");
            }
        }
    }
}
