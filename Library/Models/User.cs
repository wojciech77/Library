﻿namespace Library.Models
{

    public class User
    {
        public User()
        {
            BorrowsCount = 0;
        }
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime DateOfUserCreation { get; set; }
        public string? PersonalIdNumber { get; set; }

        public Address? Address { get; set; }
        public int BorrowsCount { get; set; }

        public List<BorrowDto> Borrows { get; set; } = new List<BorrowDto> { };

        public string? PasswordHash { get; set; }
        public virtual Role? Role { get; set; }
        public int RoleId { get; set; }

    }

}
