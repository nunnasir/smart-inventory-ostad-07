using SmartInventory.Contract.Request;
using SmartInventory.Model;

namespace SmartInventory.BLL.Mapping;

public static class ContractMapping
{
    public static Product MapToProduct(this CreateProductRequest request)
    {
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CreatedTime = DateTime.Now,
            CreatedBy = 1
        };
    }

    public static UpdateProductRequest ToUpdateRequest(this Product product)
    {
        return new UpdateProductRequest
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity
        };
    }
}

