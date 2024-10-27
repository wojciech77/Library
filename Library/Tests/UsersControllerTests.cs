using Library.Controllers;
using Library.Data;
using Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Library.Tests
{
    public class UsersControllerTests
    {
        private readonly LibraryContext _dbContext;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "TestDb") // Ensure the InMemory package is added
                .Options;

            var passwordHasherMock = new Mock<IPasswordHasher<User>>(); // Mock the IPasswordHasher<User>
            _dbContext = new LibraryContext(options, passwordHasherMock.Object);
            _controller = new UsersController(_dbContext);
        }

        [Fact]
        public void UpdateUser_UserExists_UpdatesUserDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                Id = userId,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123456789",
                DateOfBirth = DateTime.Now.AddYears(-25),
                PersonalIdNumber = "12345678901",
                Address = new Address { Street = "123 Main St", City = "Test City", PostalCode = "12345", Country = "Test Country" }
            };

            _dbContext.Users.Add(existingUser);
            _dbContext.SaveChanges();

            var updatedUser = new User
            {
                Id = userId,
                Email = "updated@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                PhoneNumber = "987654321",
                DateOfBirth = DateTime.Now.AddYears(-30),
                PersonalIdNumber = "10987654321",
                Address = new Address { Street = "456 Another St", City = "Updated City", PostalCode = "54321", Country = "Updated Country" }
            };

            // Act
            var result = _controller.UpdateUser(updatedUser) as ViewResult;

            // Assert
            var userInDb = _dbContext.Users.Include(u => u.Address).FirstOrDefault(u => u.Id == userId);
            Assert.NotNull(result);
            Assert.Equal("updated@example.com", userInDb.Email);
            Assert.Equal("Jane", userInDb.FirstName);
            Assert.Equal("Smith", userInDb.LastName);
            Assert.Equal("987654321", userInDb.PhoneNumber);
            Assert.Equal("456 Another St", userInDb.Address.Street);
        }

        [Fact]
        public void UpdateUser_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var nonExistingUser = new User { Id = Guid.NewGuid() };

            // Act
            var result = _controller.UpdateUser(nonExistingUser) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void EditUser_UserExists_ReturnsView()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                Id = userId,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "123456789"
            };

            _dbContext.Users.Add(existingUser);
            _dbContext.SaveChanges();

            // Act
            var result = _controller.EditUser(userId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<User>(result.Model);
            Assert.Equal(userId, ((User)result.Model).Id);
        }

        [Fact]
        public void EditUser_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = _controller.EditUser(userId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        // Additional tests can be added here for other actions in UsersController.
    }
}
