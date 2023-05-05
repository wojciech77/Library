using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class UsersController : Controller
    {
        private readonly LibraryContext _db;
        public UsersController(LibraryContext db)
        {
            _db = db;
        }
        public IActionResult Users()
        {
            IEnumerable<User> objUsersList = _db.Users;
            return View(objUsersList);
        }
        //GET
        public IActionResult AddUser()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(User obj)
        {

            obj.DateOfUserCreation = DateOnly.FromDateTime(DateTime.Now);
            _db.Users.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Users");
            
        }
    }
}
