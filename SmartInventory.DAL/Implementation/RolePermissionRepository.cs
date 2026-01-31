using Microsoft.EntityFrameworkCore;
using SmartInventory.DAL.Context;
using SmartInventory.DAL.Core;
using SmartInventory.DAL.Interfaces;
using SmartInventory.Model;

namespace SmartInventory.DAL.Implementation;

public class RolePermissionRepository 
    : Repository<RolePermission, int, SmartInventoryDbContext>, 
    IRolePermissionRepository
{
    public RolePermissionRepository(SmartInventoryDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<RolePermission?> GetByRoleIdAndPermissionIdAsync(string roleId, int permissionId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
    }

    public async Task<IList<RolePermission>> GetByRoleIdAsync(string roleId)
    {
        return await _dbSet
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<IList<RolePermission>> GetByPermissionIdAsync(int permissionId)
    {
        return await _dbSet
            .Include(rp => rp.Role)
            .Where(rp => rp.PermissionId == permissionId)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string roleId, int permissionId)
    {
        return await _dbSet
            .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
    }

    public async Task<bool> DeleteByRoleIdAndPermissionIdAsync(string roleId, int permissionId)
    {
        var rolePermission = await GetByRoleIdAndPermissionIdAsync(roleId, permissionId);
        if (rolePermission == null)
        {
            return false;
        }

        _dbSet.Remove(rolePermission);
        return true;
    }
}
