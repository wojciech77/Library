using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library.Controllers
{
    public class UsersController : Controller
    {
        private readonly LibraryContext _db;

        public UsersController(LibraryContext db)
        {
            _db = db; // Initialize the database context
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            // Retrieve a list of users with their associated addresses
            IEnumerable<User> objUsersList = _db.Users.Include(u => u.Address);
            return View(objUsersList); // Return the view with the user list
        }

        // GET: Update user details
        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult UpdateUser()
        {
            // Retrieve the user's ID from the claims
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            // Fetch the user details including the address
            var user = _db.Users.Include(u => u.Address).FirstOrDefault(u => u.Id == userId);
            return View(user); // Return the view with the user details for editing
        }

        // POST: Update user details
        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult UpdateUser(User user)
        {
            // Retrieve the existing user from the database
            var existingUser = _db.Users.Include(u => u.Address).FirstOrDefault(u => u.Id == user.Id);

            // If the user does not exist, return a 404 Not Found response
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update the user's properties
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.PersonalIdNumber = user.PersonalIdNumber;

            // Update address if provided
            if (user.Address != null)
            {
                // Create a new Address if none exists
                if (existingUser.Address == null)
                {
                    existingUser.Address = new Address();
                }

                // Update the address properties
                existingUser.Address.Street = user.Address.Street;
                existingUser.Address.PostalCode = user.Address.PostalCode;
                existingUser.Address.City = user.Address.City;
                existingUser.Address.Country = user.Address.Country;
            }

            // Save changes to the database
            _db.SaveChanges();
            return View(user); // Return the view with the updated user details
        }

        // GET: Edit user details for Admin/Manager
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult EditUser(Guid id)
        {
            // Retrieve the user by ID including their address
            var user = _db.Users.Include(u => u.Address).FirstOrDefault(u => u.Id == id);
            return View(user); // Return the view for editing user details
        }

        // POST: Save edited user details
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult EditUser(User user)
        {
            // Load the existing user from the database
            var existingUser = _db.Users.FirstOrDefault(u => u.Id == user.Id);

            // Check if the email is unique (not used by another user)
            var existingUserWithEmail = _db.Users.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id);
            if (existingUserWithEmail != null)
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                return View(user); // Return the view with an error message
            }

            // Update the user's properties
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.PersonalIdNumber = user.PersonalIdNumber;

            // Check if the user already has an address
            var existingAddress = _db.Addresses.FirstOrDefault(a => a.UserId == user.Id);

            if (existingAddress != null)
            {
                // Update the existing address
                existingAddress.Street = user.Address.Street;
                existingAddress.PostalCode = user.Address.PostalCode;
                existingAddress.City = user.Address.City;
                existingAddress.Country = user.Address.Country;
                _db.Addresses.Update(existingAddress); // Mark the address as updated
            }
            else
            {
                // Create a new address if none exists
                var newAddress = new Address
                {
                    UserId = user.Id,
                    Street = user.Address.Street ?? string.Empty,
                    PostalCode = user.Address.PostalCode ?? string.Empty,
                    City = user.Address.City ?? string.Empty,
                    Country = user.Address.Country ?? string.Empty
                };

                _db.Addresses.Add(newAddress); // Add the new address to the database
            }

            // Update the user record
            _db.Users.Update(existingUser);
            _db.SaveChanges(); // Save changes to the database
            return RedirectToAction("Users"); // Redirect back to the users list
        }

        // GET: Delete a user
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _db.Users.Find(id);
            // Prevent deletion of Admin users
            if (user.RoleId == 3)
            {
                return RedirectToAction("Users"); // Redirect to users list if trying to delete an Admin
            }
            else
            {
                _db.Users.Remove(user); // Remove the user from the database
                _db.SaveChanges(); // Save changes to the database
                return RedirectToAction("Users"); // Redirect to the users list
            }
        }
    }
}
