using Microsoft.AspNetCore.Identity;

namespace Library.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly DateOfUserCreation { get; set; }
        public string? PersonalIdNumber { get; set; }

        public Address? Address { get; set; }

        public List<Resource>? Resources { get; set; }

        public string? PasswordHash { get; set; }
        public virtual Role? Role { get; set; }
        public int RoleId { get; set; }

    }
}
