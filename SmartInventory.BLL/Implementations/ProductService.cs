using SmartInventory.BLL.Helpers;
using SmartInventory.BLL.Interfaces;
using SmartInventory.BLL.Mapping;
using SmartInventory.BLL.Model;
using SmartInventory.Contract.Request;
using SmartInventory.Contract.Response;
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

    public async Task<Result<int>> AddAsync(CreateProductRequest product)
    {
        if (product is null)
        {
            return Result<int>.FaileResult("Product cannot be null.");
        }

        var existingProduct = await _productUnitOfWork.ProductRepository.GetAsync(
            x => x.Id, x => x.Name == product.Name, null, null, false);

        if (existingProduct.Any())
        {
            return Result<int>.FaileResult("A product with the same name already exists.");
        }

        try
        {
            var newProduct = product.MapToProduct();

            await _productUnitOfWork.ProductRepository.AddAsync(newProduct);
            var saved = await _productUnitOfWork.SaveChangesAsync();

            if (!saved)
            {
                return Result<int>.FaileResult("Failed to save the product.");
            }

            return Result<int>.SuccessResult(newProduct.Id);
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

        await _productUnitOfWork.ProductRepository.DeleteAsync(product.Id);
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

    public async Task<Result<int>> UpdateAsync(UpdateProductRequest model)
    {
        if (model is null)
        {
            return Result<int>.FaileResult("Product data cannot be null.");
        }

        var product = await _productUnitOfWork.ProductRepository.GetByIdAsync(model.Id);
        if (product is null)
        {
            return Result<int>.FaileResult($"Product with id {model.Id} was not found.");
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.StockQuantity = model.StockQuantity;

        await _productUnitOfWork.ProductRepository.UpdateAsync(product);

        var saved = await _productUnitOfWork.SaveChangesAsync();

        if (!saved)
        {
            return Result<int>.FaileResult("Failed to update product.");
        }

        return Result<int>.SuccessResult(model.Id);
    }

    public async Task<DataTablesResponse<Product>> GetDataTablesAsync(DataTablesRequest request)
    {
        try
        {
            // Build search predicate
            var searchPredicate = DataTablesHelper.BuildSearchPredicate<Product>(
                request,
                searchValue =>
                {
                    var lowerSearch = searchValue.ToLower();
                    return p =>
                        p.Name.ToLower().Contains(lowerSearch) ||
                        p.Description.ToLower().Contains(lowerSearch) ||
                        p.Price.ToString().Contains(searchValue) ||
                        p.StockQuantity.ToString().Contains(searchValue);
                }
            );

            // Build order by expression
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null;

            if (request.Order != null && request.Order.Any() && request.Columns != null)
            {
                var order = request.Order.First();
                var columnIndex = order.Column;
                var isAscending = order.Dir.ToLower() == "asc";

                if (columnIndex >= 0 && columnIndex < request.Columns.Count)
                {
                    var column = request.Columns[columnIndex];
                    var columnKey = column.Data.ToLower();

                    orderBy = columnKey switch
                    {
                        "name" => isAscending
                            ? q => q.OrderBy(p => p.Name)
                            : q => q.OrderByDescending(p => p.Name),
                        "description" => isAscending
                            ? q => q.OrderBy(p => p.Description)
                            : q => q.OrderByDescending(p => p.Description),
                        "price" => isAscending
                            ? q => q.OrderBy(p => p.Price)
                            : q => q.OrderByDescending(p => p.Price),
                        "stockquantity" => isAscending
                            ? q => q.OrderBy(p => p.StockQuantity)
                            : q => q.OrderByDescending(p => p.StockQuantity),
                        _ => null
                    };
                }
            }

            // Default ordering if no order specified
            orderBy ??= q => q.OrderByDescending(p => p.Id);

            // Calculate pagination
            var (pageIndex, pageSize) = DataTablesHelper.CalculatePagination(request);

            // Get Data from repository
            var (items, total, totalFilter) = await _productUnitOfWork.ProductRepository.GetAsync(
                p => p,
                searchPredicate,
                orderBy,
                null,
                pageIndex,
                pageSize,
                true);

            return new DataTablesResponse<Product>
            {
                Draw = request.Draw,
                RecordsTotal = total,
                RecordsFiltered = totalFilter,
                Data = items.ToList()
            };

        }
        catch (Exception)
        {

            throw;
        }
    }

}
