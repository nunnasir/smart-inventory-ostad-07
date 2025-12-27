using SmartInventory.BLL.Model;
using SmartInventory.Contract.Request;
using SmartInventory.Model;

namespace SmartInventory.BLL.Interfaces;

public interface IProductService
{
    Task<Result<IList<Product>>> GetAllAsync();
    Task<Result<Product>> GetByIdAsync(int id);
    Task<Result<int>> AddAsync(CreateProductRequest product);
    Task<Result<int>> UpdateAsync(UpdateProductRequest model);
    Task<Result<bool>> DeleteAsync(int id);
}
