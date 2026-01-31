using SmartInventory.DAL.Context;
using SmartInventory.DAL.Core;
using SmartInventory.Model;

namespace SmartInventory.DAL.Interfaces;

public interface IRolePermissionRepository : IRepository<RolePermission, int, SmartInventoryDbContext>
{
    Task<RolePermission?> GetByRoleIdAndPermissionIdAsync(string roleId, int permissionId);
    Task<IList<RolePermission>> GetByRoleIdAsync(string roleId);
    Task<IList<RolePermission>> GetByPermissionIdAsync(int permissionId);
    Task<bool> ExistsAsync(string roleId, int permissionId);
    Task<bool> DeleteByRoleIdAndPermissionIdAsync(string roleId, int permissionId);
}
