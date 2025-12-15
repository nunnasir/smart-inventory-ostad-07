using SmartInventory.Model;

namespace SmartInventory.BLL.Interfaces;

public interface IProductService
{
    Task AddAsync(Product product);
}
