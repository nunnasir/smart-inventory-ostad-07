using Microsoft.EntityFrameworkCore;

namespace SmartInventory.DAL.Core;

public abstract class Repository<TEntity, TKey, TContext> 
    : IRepository<TEntity, TKey, TContext>
    where TEntity : class
    where TContext : DbContext
{
    protected readonly TContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(TContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }
}

