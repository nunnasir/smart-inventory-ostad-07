using SmartInventory.BLL.Model;
using SmartInventory.Model;

namespace SmartInventory.BLL.Interfaces;

public interface IProductService
{
    Task<Result<IList<Product>>> GetAllAsync();
    Task<Result<Product>> GetByIdAsync(int id);
    Task<Result<int>> AddAsync(Product product);
    Task<Result<int>> UpdateAsync(Product product);
    Task<Result<bool>> DeleteAsync(int id);
}
