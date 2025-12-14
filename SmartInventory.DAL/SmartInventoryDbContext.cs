using Microsoft.EntityFrameworkCore;
using SmartInventory.Model;

namespace SmartInventory.DAL;
public class SmartInventoryDbContext : DbContext
{
    public SmartInventoryDbContext(DbContextOptions<SmartInventoryDbContext> options) : base(options)
    {
    }

    DbSet<Product> Products { get; set; }
}
