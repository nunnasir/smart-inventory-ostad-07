using SmartInventory.DAL.Context;
using SmartInventory.DAL.Core;
using SmartInventory.Model;

namespace SmartInventory.DAL.Interfaces;

public interface IPermissionRepository : IRepository<Permission, int, SmartInventoryDbContext>
{
    Task<Permission?> GetByNameAsync(string name);
    Task<IList<Permission>> GetByResourceAsync(string resource);
    Task<bool> ExistsByNameAsync(string name);
}
