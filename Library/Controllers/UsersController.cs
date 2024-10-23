using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly LibraryContext _db;

        // Constructor that initializes the database context
        public UsersController(LibraryContext db)
        {
            _db = db;
        }

        // Displays a list of users with their addresses
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            var objUsersList = _db.Users.Include(u => u.Address);
            return View(objUsersList);
        }

        // GET: Edit a specific user by id
        [Authorize(Roles = "Admin, Manager")]
        [Route("EditUser/{id:Guid}")]
        public IActionResult EditUser(Guid id)
        {
            var user = _db.Users
                           .Include(u => u.Address)
                           .FirstOrDefault(u => u.Id == id);
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        [ValidateAntiForgeryToken] // Protect the POST request from CSRF attacks
        public IActionResult EditUser(User user)
        {
            var existingUser = _db.Users.FirstOrDefault(u => u.Id == user.Id);

            // Ensure the new email is unique among other users
            var existingUserWithEmail = _db.Users.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id);
            if (existingUserWithEmail != null)
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                return View(user); // Return to view with error message
            }

            // Update only modified fields
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.PersonalIdNumber = user.PersonalIdNumber;
            existingUser.RoleId = user.RoleId;

            // Check if user has an address and update or add accordingly
            var existingAddress = _db.Addresses.FirstOrDefault(a => a.UserId == user.Id);

            if (existingAddress != null)
            {
                existingAddress.Street = user.Address.Street;
                existingAddress.PostalCode = user.Address.PostalCode;
                existingAddress.City = user.Address.City;
                existingAddress.Country = user.Address.Country;

                _db.Addresses.Update(existingAddress); // Update existing address
            }
            else
            {
                var newAddress = new Address
                {
                    UserId = user.Id,
                    Street = user.Address.Street ?? "",
                    PostalCode = user.Address.PostalCode ?? "",
                    City = user.Address.City ?? "",
                    Country = user.Address.Country ?? ""
                };

                _db.Addresses.Add(newAddress); // Add new address if none exists
            }

            // Save updated user and related data
            _db.Users.Update(existingUser);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }

        // Deletes a user unless they are an admin (RoleId == 3)
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _db.Users.Find(id);

            // Prevent deletion of admin users
            if (user.RoleId == 3)
            {
                return RedirectToAction("Users");
            }

            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }
    }
}
