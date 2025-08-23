using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using User_Management.Database.Models;
using HostingEnvironmentExtensions = Microsoft.AspNetCore.Hosting.HostingEnvironmentExtensions;

namespace User_Management.Database;

public class Context : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ServiceToken> Tokens { get; set; }

    public Context(DbContextOptions<Context> options) : base(options)
    {
        Database.EnsureCreated();
        if (!Users.Any(u => u.Email == "admin@innoshop.com"))
        {
            var hasher = new PasswordHasher<User>();
            var admin = new User()
            {
                Name = "admin",
                Email = "admin@innoshop.com",
                IsActive = true,
                Role = "Admin"
            };
            admin.Password = hasher.HashPassword(admin, "admin");

            Users.Add(admin);

            SaveChanges();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasOne(p => p.User)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}