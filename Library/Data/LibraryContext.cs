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

            string ADMIN_ID = "B21EA37D-66D2-4C2C-D01C-08DB4A9AC978";

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

            var appUser = new User
            {
                Id = Guid.Parse(ADMIN_ID),
                Email = "wojciech@gmail.com",
                FirstName = "Admin",
                LastName = "Nimda",
                RoleId = 3
            };
            var hashedPassword = _passwordHasher.HashPassword(appUser, "Password");
            appUser.PasswordHash = hashedPassword;


            modelBuilder.Entity<User>()
                .HasData(appUser);

            modelBuilder.Entity<User>(eb =>
            {
                eb.Property(u => u.FirstName).IsRequired();

                eb.HasOne(u => u.Address)
                .WithOne(a => a.User)
                .HasForeignKey<Address>(a => a.UserId);

                eb.HasMany(u => u.Borrows)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);
            });



            modelBuilder.Entity<Resource>()
                .Property(r => r.Title)
                .IsRequired();

            modelBuilder.Entity<BorrowDto>()
                .HasMany(b => b.Resources)
                .WithOne(r => r.BorrowDto);

            modelBuilder.Entity<Address>()
                .Property(a => a.Street).IsRequired();


        }



    }
}
