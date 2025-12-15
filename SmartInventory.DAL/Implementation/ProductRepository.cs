using SmartInventory.DAL.Context;
using SmartInventory.DAL.Core;
using SmartInventory.DAL.Interfaces;
using SmartInventory.Model;

namespace SmartInventory.DAL.Implementation;

public class ProductRepository 
    : Repository<Product, int, SmartInventoryDbContext>, 
    IProductRepository
{
    public ProductRepository(SmartInventoryDbContext dbContext) : base(dbContext)
    {
        
    }

    public int CountPorudct()
    {
        return _dbSet.Count();
    }
}
