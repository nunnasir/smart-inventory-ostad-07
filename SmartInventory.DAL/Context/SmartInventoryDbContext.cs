using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Model;

namespace SmartInventory.DAL.Context;
public class SmartInventoryDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public SmartInventoryDbContext(DbContextOptions<SmartInventoryDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
}
