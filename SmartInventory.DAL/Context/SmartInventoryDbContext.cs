using Microsoft.EntityFrameworkCore;
using SmartInventory.Model;

namespace SmartInventory.DAL.Context;
public class SmartInventoryDbContext : DbContext
{
    public SmartInventoryDbContext(DbContextOptions<SmartInventoryDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
}
