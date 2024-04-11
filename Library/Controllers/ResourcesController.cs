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
            if(ModelState.IsValid)
            {
                _db.Resources.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Resources");
            }
            return RedirectToAction("AddResource");

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
        




    }
}
