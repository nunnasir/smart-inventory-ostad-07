using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartInventory.BLL.Interfaces;
using SmartInventory.BLL.Model;
using SmartInventory.Contract.Request;
using SmartInventory.DAL.Context;
using SmartInventory.DAL.Interfaces;
using SmartInventory.Model;

namespace SmartInventory.BLL.Implementations;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly SmartInventoryDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        SmartInventoryDbContext context,
        RoleManager<IdentityRole> roleManager)
    {
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<Result<IList<Permission>>> GetAllAsync()
    {
        try
        {
            var permissions = await _permissionRepository.GetAsync(
                p => p, null, x => x.OrderBy(p => p.Name), null, true);
            return Result<IList<Permission>>.SuccessResult(permissions);
        }
        catch (Exception ex)
        {
            return Result<IList<Permission>>.FaileResult($"Error retrieving permissions: {ex.Message}");
        }
    }

    public async Task<Result<Permission>> GetByIdAsync(int id)
    {
        try
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
            {
                return Result<Permission>.FaileResult($"Permission with id {id} was not found.");
            }
            return Result<Permission>.SuccessResult(permission);
        }
        catch (Exception ex)
        {
            return Result<Permission>.FaileResult($"Error retrieving permission: {ex.Message}");
        }
    }

    public async Task<Result<int>> CreateAsync(CreatePermissionRequest request)
    {
        if (request == null)
        {
            return Result<int>.FaileResult("Permission request cannot be null.");
        }

        try
        {
            // Check if permission with same name exists
            var exists = await _permissionRepository.ExistsByNameAsync(request.Name);
            if (exists)
            {
                return Result<int>.FaileResult("A permission with the same name already exists.");
            }

            var permission = new Permission
            {
                Name = request.Name,
                Description = request.Description,
                Resource = request.Resource,
                Action = request.Action,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _permissionRepository.AddAsync(permission);
            await _context.SaveChangesAsync();

            return Result<int>.SuccessResult(permission.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.FaileResult($"Error creating permission: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateAsync(UpdatePermissionRequest request)
    {
        if (request == null)
        {
            return Result<bool>.FaileResult("Permission request cannot be null.");
        }

        try
        {
            var permission = await _permissionRepository.GetByIdAsync(request.Id);
            if (permission == null)
            {
                return Result<bool>.FaileResult($"Permission with id {request.Id} was not found.");
            }

            // Check if another permission with same name exists
            var existing = await _permissionRepository.GetByNameAsync(request.Name);
            if (existing != null && existing.Id != request.Id)
            {
                return Result<bool>.FaileResult("A permission with the same name already exists.");
            }

            permission.Name = request.Name;
            permission.Description = request.Description;
            permission.Resource = request.Resource;
            permission.Action = request.Action;
            permission.IsActive = request.IsActive;

            await _permissionRepository.UpdateAsync(permission);
            await _context.SaveChangesAsync();

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.FaileResult($"Error updating permission: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            if (permission == null)
            {
                return Result<bool>.FaileResult("Permission not found.");
            }

            await _permissionRepository.DeleteAsync(id);
            await _context.SaveChangesAsync();

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.FaileResult($"Error deleting permission: {ex.Message}");
        }
    }

    public async Task<Result<bool>> AssignPermissionToRoleAsync(string roleId, int permissionId, string assignedBy)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return Result<bool>.FaileResult("Role not found.");
            }

            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            if (permission == null)
            {
                return Result<bool>.FaileResult("Permission not found.");
            }

            var exists = await _rolePermissionRepository.ExistsAsync(roleId, permissionId);
            if (exists)
            {
                return Result<bool>.FaileResult("Permission is already assigned to this role.");
            }

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            };

            await _rolePermissionRepository.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.FaileResult($"Error assigning permission: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RemovePermissionFromRoleAsync(string roleId, int permissionId)
    {
        try
        {
            var deleted = await _rolePermissionRepository.DeleteByRoleIdAndPermissionIdAsync(roleId, permissionId);
            if (!deleted)
            {
                return Result<bool>.FaileResult("Permission assignment not found.");
            }

            await _context.SaveChangesAsync();

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.FaileResult($"Error removing permission: {ex.Message}");
        }
    }

    public async Task<Result<IList<Permission>>> GetPermissionsByRoleIdAsync(string roleId)
    {
        try
        {
            var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(roleId);
            var permissions = rolePermissions.Select(rp => rp.Permission).ToList();
            return Result<IList<Permission>>.SuccessResult(permissions);
        }
        catch (Exception ex)
        {
            return Result<IList<Permission>>.FaileResult($"Error retrieving permissions: {ex.Message}");
        }
    }

    public async Task<Result<IList<string>>> GetRolesByPermissionIdAsync(int permissionId)
    {
        try
        {
            var rolePermissions = await _rolePermissionRepository.GetByPermissionIdAsync(permissionId);
            var roleIds = rolePermissions.Select(rp => rp.RoleId).ToList();
            
            var roles = await _roleManager.Roles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name!)
                .ToListAsync();

            return Result<IList<string>>.SuccessResult(roles);
        }
        catch (Exception ex)
        {
            return Result<IList<string>>.FaileResult($"Error retrieving roles: {ex.Message}");
        }
    }
}
