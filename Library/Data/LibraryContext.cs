using System;
using Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryContext : DbContext
    {


        private readonly IPasswordHasher<User> _passwordHasher;

        public LibraryContext(DbContextOptions<LibraryContext> options, IPasswordHasher<User> passwordHasher) : base(options)
        {
            _passwordHasher = passwordHasher;
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<BorrowDto> Borrows { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role>()
                .HasData(
                    new Role
                    {

                        Id = 1,
                        Name = "User"
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "Manager"
                    },
                    new Role
                    {
                        Id = 3,
                        Name = "Admin"
                    }
                );


            modelBuilder.Entity<User>(eb =>
            {

                eb.HasOne(u => u.Address)
                    .WithOne(a => a.User)
                    .HasForeignKey<Address>(a => a.UserId);


                eb.HasMany(u => u.Borrows)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UserId);
            });

            modelBuilder.Entity<BorrowDto>(eb =>
            {


            });


            modelBuilder.Entity<Resource>()
                .Property(r => r.Title)
                .IsRequired();

            

              

        }



    }
}
