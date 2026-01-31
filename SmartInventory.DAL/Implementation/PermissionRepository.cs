using Microsoft.EntityFrameworkCore;
using SmartInventory.DAL.Context;
using SmartInventory.DAL.Core;
using SmartInventory.DAL.Interfaces;
using SmartInventory.Model;

namespace SmartInventory.DAL.Implementation;

public class PermissionRepository 
    : Repository<Permission, int, SmartInventoryDbContext>, 
    IPermissionRepository
{
    public PermissionRepository(SmartInventoryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Permission?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<IList<Permission>> GetByResourceAsync(string resource)
    {
        return await _dbSet.Where(p => p.Resource == resource).ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbSet.AnyAsync(p => p.Name == name);
    }
}
