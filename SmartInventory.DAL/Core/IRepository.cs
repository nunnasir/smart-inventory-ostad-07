using Microsoft.EntityFrameworkCore;

namespace SmartInventory.DAL.Core;

public interface IRepository<TEntity,TKey, TContext>
    where TEntity : class
    where TContext : DbContext
{
    Task AddAsync(TEntity entity);
}
