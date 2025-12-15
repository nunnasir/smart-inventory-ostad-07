using SmartInventory.DAL.Context;
using SmartInventory.DAL.Core;
using SmartInventory.Model;

namespace SmartInventory.DAL.Interfaces;

public interface IProductRepository 
    : IRepository<Product, int, SmartInventoryDbContext>
{
    int CountPorudct();
}
