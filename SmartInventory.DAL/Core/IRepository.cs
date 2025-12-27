using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace SmartInventory.DAL.Core;

public interface IRepository<TEntity,TKey, TContext>
    where TEntity : class
    where TContext : DbContext
{
    Task<IList<TResult>> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                        Expression<Func<TEntity, bool>>? predicate = null,
                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                        bool disableTracking = true);

    Task<(IList<TResult> Items, int Total, int TotalFilter)> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>>? predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                            int pageIndex = 1, int pageSize = 10,
                            bool disableTracking = true);

    Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                        Expression<Func<TEntity, bool>>? predicate = null,
                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                        bool disableTracking = true);

    Task<TEntity> GetByIdAsync(object id);

    Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate);

    Task AddAsync(TEntity entity);

    Task UpdateAsync(TEntity entity, params string[] updateProperties);

    Task DeleteAsync(object id);
}
