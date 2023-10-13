using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartFood.Domain.Models;

namespace SmartFood.Domain;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<StoredDish> StoredDishes { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Filial> Filials { get; set; }
    public DbSet<Fridge> Fridges { get; set; }
    public DbSet<FridgeInstallationRequest> FridgeInstallationRequests { get; set; }
    public DbSet<FridgeDeinstallationRequest> FridgeDeinstallationRequests { get; set; }
    public DbSet<FridgeUsageRequest> FridgeUsageRequests { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<IssuedToken> IssuedTokens { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().HasIndex(c => c.Email).IsUnique();

        builder.Entity<Employee>().HasOne(c => c.Organization).WithMany().OnDelete(DeleteBehavior.NoAction);
        
        builder.Entity<StoredDish>().HasOne(c => c.Fridge).WithMany(c => c.DishesServed).OnDelete(DeleteBehavior.NoAction);

        builder.Entity<FridgeUsageRequest>().HasOne(c => c.Fridge).WithMany().OnDelete(DeleteBehavior.NoAction);
        builder.Entity<FridgeUsageRequest>().HasOne(c => c.Supplier).WithMany().OnDelete(DeleteBehavior.NoAction);
    }
}