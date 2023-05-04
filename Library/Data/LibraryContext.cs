using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {

        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Role> Roles { get; set; }


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
                eb.Property(u => u.FirstName).IsRequired();

                eb.HasOne(u => u.Address)
                .WithOne(a => a.User)
                .HasForeignKey<Address>(a => a.UserId);

                eb.HasMany(u => u.Resources)
                .WithMany(r => r.Users);

            });



            modelBuilder.Entity<Resource>()
                .Property(r => r.Title)
                .IsRequired();

            modelBuilder.Entity<Address>()
                .Property(a => a.Street).IsRequired();


        }



    }
}
