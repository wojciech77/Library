using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{

    public class ResourcesController : Controller
    {
        private readonly LibraryContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public ResourcesController(LibraryContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }


        [Authorize(Roles = "Admin, Manager")]
        public IActionResult Resources()
        {
            IEnumerable<Resource> objResourcesList = _db.Resources;
            return View(objResourcesList);
        }

        [Authorize(Roles = "Admin, Manager")]
        public IActionResult SearchResources(string searchString)
        {
            IQueryable<Resource> resourcesQuery = _db.Resources;

            if (!string.IsNullOrEmpty(searchString))
            {
                resourcesQuery = resourcesQuery.Where(r =>
                    r.Title.Contains(searchString) ||
                    r.Author.Contains(searchString) ||
                    r.Type.Contains(searchString) ||
                    r.Category.Contains(searchString));
            }

            IEnumerable<Resource> objResourcesList = resourcesQuery.ToList();
            return View("Resources", objResourcesList);
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
            if (ModelState.IsValid)
            {
                _db.Resources.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Resources");
            }
            else
            {
                // Sprawdź, czy wystąpiły błędy związane z polami Title, Author, Type, Category i Quantity
                if (ModelState.TryGetValue("Title", out var titleEntry) && titleEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Title
                    ModelState.AddModelError("Title", "Invalid title.");
                }

                if (ModelState.TryGetValue("Author", out var authorEntry) && authorEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Author
                    ModelState.AddModelError("Author", "Invalid author.");
                }

                if (ModelState.TryGetValue("Type", out var typeEntry) && typeEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Type
                    ModelState.AddModelError("Type", "Invalid type.");
                }

                if (ModelState.TryGetValue("Category", out var categoryEntry) && categoryEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Category
                    ModelState.AddModelError("Category", "Invalid category.");
                }

                if (ModelState.TryGetValue("Quantity", out var quantityEntry) && quantityEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Quantity
                    ModelState.AddModelError("Quantity", "Invalid quantity.");
                }

                // Zwróć widok logowania z modelem zawierającym błędy walidacji
                return View("AddResource", obj);
            }

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
            if (ModelState.IsValid)
            {
                _db.Resources.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Resources");
            }
            else
            {
                // Sprawdź, czy wystąpiły błędy związane z polami Title, Author, Type, Category i Quantity
                if (ModelState.TryGetValue("Title", out var titleEntry) && titleEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Title
                    ModelState.AddModelError("Title", "Invalid title.");
                }

                if (ModelState.TryGetValue("Author", out var authorEntry) && authorEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Author
                    ModelState.AddModelError("Author", "Invalid author.");
                }

                if (ModelState.TryGetValue("Type", out var typeEntry) && typeEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Type
                    ModelState.AddModelError("Type", "Invalid type.");
                }

                if (ModelState.TryGetValue("Category", out var categoryEntry) && categoryEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Category
                    ModelState.AddModelError("Category", "Invalid category.");
                }

                if (ModelState.TryGetValue("Quantity", out var quantityEntry) && quantityEntry.Errors.Any())
                {
                    // Dodaj odpowiedni komunikat o błędzie dla pola Quantity
                    ModelState.AddModelError("Quantity", "Invalid quantity.");
                }

                // Zwróć widok logowania z modelem zawierającym błędy walidacji
                return View("EditResource", obj);
            }


        }




        [Authorize(Roles = "Admin, Manager")]
        public IActionResult DeleteResource(int id)
        {
            var resource = _db.Resources.Find(id);
            _db.Resources.Remove(resource);
            _db.SaveChanges();
            return RedirectToAction("Resources");
        }





    }
}
