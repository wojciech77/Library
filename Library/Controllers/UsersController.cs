using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            IEnumerable<User> objUsersList = _db.Users.Include(u => u.Address);
            return View(objUsersList);
        }

        [Authorize(Roles = "Admin, Manager")]
        public IActionResult EditUser(Guid id)
        {
            var user = _db.Users
                           .Include(u => u.Address) 
                           .FirstOrDefault(u => u.Id == id);
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public IActionResult EditUser(User user)
        {
            // Wczytanie istniejącego użytkownika z bazy danych
            var existingUser = _db.Users.FirstOrDefault(u => u.Id == user.Id);

            // Sprawdzenie, czy nowy adres e-mail jest unikatowy
            var existingUserWithEmail = _db.Users.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id);
            if (existingUserWithEmail != null)
            {
                ModelState.AddModelError("Email", "This email is already in use.");
                return View(user); // lub inną odpowiedź w przypadku błędu
            }

            // Aktualizacja tylko tych pól, które zostały zmienione
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.PersonalIdNumber = user.PersonalIdNumber;



            // Sprawdzenie, czy użytkownik ma już wpis w tabeli Addresses
            var existingAddress = _db.Addresses.FirstOrDefault(a => a.UserId == user.Id);

            if (existingAddress != null)
            {
                // Aktualizacja istniejącego wpisu
                existingAddress.Street = user.Address.Street;
                existingAddress.PostalCode = user.Address.PostalCode;
                existingAddress.City = user.Address.City;
                existingAddress.Country = user.Address.Country;

                _db.Addresses.Update(existingAddress);
            }
            else
            {
                // Dodanie nowego wpisu
                var newAddress = new Address
                {
                    UserId = user.Id,
                    Street = user.Address.Street ?? "",
                    PostalCode = user.Address.PostalCode ?? "",
                    City = user.Address.City ?? "",
                    Country = user.Address.Country ?? ""
                };

                _db.Addresses.Add(newAddress);
            }

            // Aktualizacja reszty danych użytkownika
            _db.Users.Update(existingUser);
            _db.SaveChanges();
            return RedirectToAction("Users");
        }




        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(Guid id)
        {
            var user = _db.Users.Find(id);
            if(user.RoleId == 3)
            {
                return RedirectToAction("Users");
            }
            else
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                return RedirectToAction("Users");
            }
            
        }
    }
}
