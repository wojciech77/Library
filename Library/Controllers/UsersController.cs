using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Library.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly LibraryContext _db;
        public UsersController(LibraryContext db)
        {
            _db = db;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            IEnumerable<User> objUsersList = _db.Users;
            return View(objUsersList);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditUser(User obj)
        {
            _db.Users.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _db.Users.Find(id);
            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }
    }
}
