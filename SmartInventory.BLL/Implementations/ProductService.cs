using SmartInventory.BLL.Interfaces;
using SmartInventory.DAL.Interfaces;
using SmartInventory.Model;

namespace SmartInventory.BLL.Implementations;

public class ProductService : IProductService
{
    private readonly IProductUnitOfWork _productUnitOfWork;

    public ProductService(IProductUnitOfWork productUnitOfWork)
    {
        _productUnitOfWork = productUnitOfWork;
    }

    public async Task AddAsync(Product product)
    
    {
        try
        {
            await _productUnitOfWork.ProductRepository.AddAsync(product);
            await _productUnitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }
}
