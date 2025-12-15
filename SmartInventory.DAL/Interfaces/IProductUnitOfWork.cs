using SmartInventory.DAL.Core;

namespace SmartInventory.DAL.Interfaces;

public interface IProductUnitOfWork : IUnitOfWork
{
    IProductRepository ProductRepository { get; }
}
