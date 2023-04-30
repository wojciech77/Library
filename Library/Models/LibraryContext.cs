using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
    public class LibraryContext :   DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) :   base(options)
        {

        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Resource> Resources { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
