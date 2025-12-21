using SmartInventory.BLL.Interfaces;
using SmartInventory.BLL.Model;
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

    public async Task<Result<int>> AddAsync(Product product)
    {
        if (product is null)
        {
            return Result<int>.FaileResult("Product cannot be null.");
        }

        try
        {
            await _productUnitOfWork.ProductRepository.AddAsync(product);
            var saved = await _productUnitOfWork.SaveChangesAsync();

            if (!saved)
            {
                return Result<int>.FaileResult("Failed to save the product.");
            }

            return Result<int>.SuccessResult(product.Id);
        }
        catch (Exception)
        {
            return Result<int>.FaileResult("An error occurred while adding the product.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var product = await _productUnitOfWork.ProductRepository.GetByIdAsync(id);
        if (product is null)
        {
            return Result<bool>.FaileResult("Product not found.");
        }

        await _productUnitOfWork.ProductRepository.DeleteAsync(product);
        var saved = await _productUnitOfWork.SaveChangesAsync();

        if (!saved)
        {
            return Result<bool>.FaileResult("Failed to delete product.");
        }

        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<IList<Product>>> GetAllAsync()
    {
        var products = await _productUnitOfWork.ProductRepository.GetAsync(
            x => x, null,
            x => x.OrderByDescending(x => x.Id), null, true);

        return Result<IList<Product>>.SuccessResult(products);
    }

    public async Task<Result<Product>> GetByIdAsync(int id)
    {
        var product = await _productUnitOfWork.ProductRepository.GetByIdAsync(id);

        if (product is null)
        {
            return Result<Product>.FaileResult($"Product with id {id} was not found.");
        }

        return Result<Product>.SuccessResult(product);
    }

    public async Task<Result<int>> UpdateAsync(Product product)
    {
        if (product is null)
        {
            return Result<int>.FaileResult("Product data cannot be null.");
        }

        var existing = await _productUnitOfWork.ProductRepository.GetByIdAsync(product.Id);
        if (existing is null)
        {
            return Result<int>.FaileResult($"Product with id {product.Id} was not found.");
        }

        await _productUnitOfWork.ProductRepository.UpdateAsync(product);

        var saved = await _productUnitOfWork.SaveChangesAsync();

        if (!saved)
        {
            return Result<int>.FaileResult("Failed to update product.");
        }

        return Result<int>.SuccessResult(existing.Id);
    }
}
