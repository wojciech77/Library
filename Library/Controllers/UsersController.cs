using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace Library.Controllers
{
    public class UsersController : Controller
    {
        private readonly LibraryContext _db;
        public UsersController(LibraryContext db)
        {
            _db = db;
        }

        // Display a list of all users for admins
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            var objUsersList = _db.Users.Include(u => u.Address);
            return View(objUsersList);
        }

        /*
        // Allow regular users to edit their own details
        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("Users/EditUser")] // Route for regular users to edit their own details
        public IActionResult EditUser()
        {
            var userId = Guid.Parse(HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _db.Users.Include(u => u.Address).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user); // Specify the view name directly
        }

        // Allow admins and managers to edit any user's details
        [Authorize(Roles = "Admin, Manager")]
        [HttpGet]
        [Route("Users/EditUserAdmin/{id:Guid}")] // Route for admins/managers to edit users
        public IActionResult EditUserAdmin(Guid id)
        {
            var user = _db.Users.Include(u => u.Address).FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user); // Ensure this matches your view name
        }

        // Process the user edit request (for regular users)
        [HttpPost]
        [Authorize(Roles = "User")]
        [Route("Users/EditUser")] // Same route for the POST request
        public IActionResult EditUser(User user)
        {
            // Fetch the existing user from the database
            var existingUser = _db.Users.Include(u => u.Address).FirstOrDefault(u => u.Id == user.Id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Ensure the email is unique across users (except for the same user)
            var existingUserWithEmail = _db.Users.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id);
            if (existingUserWithEmail != null)
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                return View("EditUser", user); // Return to the same view for regular users
            }

            // Update user details without changing the RoleId
            UpdateUserDetails(existingUser, user, false);
            _db.SaveChanges(); // Save changes to the database
            return RedirectToAction("EditUser"); // Redirect back to the user's edit view
        }

        // Process the user edit request (for admins/managers)
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        [Route("Users/EditUserAdmin")] // Same route for the POST request
        public IActionResult EditUserAdmin(User user)
        {
            // Fetch the existing user from the database
            var existingUser = _db.Users.Include(u => u.Address).FirstOrDefault(u => u.Id == user.Id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Ensure the email is unique across users (except for the same user)
            var existingUserWithEmail = _db.Users.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id);
            if (existingUserWithEmail != null)
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                return View(user); // Return to the admin view
            }

            // Update user details with RoleId changes allowed
            UpdateUserDetails(existingUser, user, true);
            _db.SaveChanges(); // Save changes to the database
            return RedirectToAction("Users"); // Redirect to the Users view
        }


        // Helper method to update user details
        private void UpdateUserDetails(User existingUser, User updatedUser, bool allowRoleUpdate)
        {
            // Update common fields
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.PhoneNumber = updatedUser.PhoneNumber;
            existingUser.DateOfBirth = updatedUser.DateOfBirth;
            existingUser.PersonalIdNumber = updatedUser.PersonalIdNumber;

            // Update RoleId only if admin or manager
            if (allowRoleUpdate)
            {
                existingUser.RoleId = updatedUser.RoleId;
            }

            // Update or insert address
            var existingAddress = _db.Addresses.FirstOrDefault(a => a.UserId == updatedUser.Id);

            if (existingAddress != null)
            {
                // Update existing address
                existingAddress.Street = updatedUser.Address.Street;
                existingAddress.PostalCode = updatedUser.Address.PostalCode;
                existingAddress.City = updatedUser.Address.City;
                existingAddress.Country = updatedUser.Address.Country;

                _db.Addresses.Update(existingAddress);
            }
            else
            {
                // Insert new address
                var newAddress = new Address
                {
                    UserId = updatedUser.Id,
                    Street = updatedUser.Address.Street ?? "",
                    PostalCode = updatedUser.Address.PostalCode ?? "",
                    City = updatedUser.Address.City ?? "",
                    Country = updatedUser.Address.Country ?? ""
                };

                _db.Addresses.Add(newAddress);
            }

            // Mark user entity as updated
            _db.Users.Update(existingUser);
        }
        */

        // Admin-only: Delete a user based on their ID
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _db.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            // Prevent deletion of the admin role itself
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
