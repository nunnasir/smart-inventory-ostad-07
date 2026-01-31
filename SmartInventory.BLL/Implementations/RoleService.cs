using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartInventory.BLL.Interfaces;
using SmartInventory.BLL.Model;
using SmartInventory.Contract.Request;
using SmartInventory.Model;

namespace SmartInventory.BLL.Implementations;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<Result<IList<IdentityRole>>> GetAllRolesAsync()
    {
        try
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Result<IList<IdentityRole>>.SuccessResult(roles);
        }
        catch (Exception ex)
        {
            return Result<IList<IdentityRole>>.FaileResult($"Error retrieving roles: {ex.Message}");
        }
    }

    public async Task<Result<IdentityRole>> GetRoleByIdAsync(string roleId)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return Result<IdentityRole>.FaileResult($"Role with id {roleId} was not found.");
            }
            return Result<IdentityRole>.SuccessResult(role);
        }
        catch (Exception ex)
        {
            return Result<IdentityRole>.FaileResult($"Error retrieving role: {ex.Message}");
        }
    }

    public async Task<Result<IdentityRole>> GetRoleByNameAsync(string roleName)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return Result<IdentityRole>.FaileResult($"Role with name {roleName} was not found.");
            }
            return Result<IdentityRole>.SuccessResult(role);
        }
        catch (Exception ex)
        {
            return Result<IdentityRole>.FaileResult($"Error retrieving role: {ex.Message}");
        }
    }

    public async Task<Result<string>> CreateRoleAsync(CreateRoleRequest request)
    {
        if (request == null)
        {
            return Result<string>.FaileResult("Role request cannot be null.");
        }

        try
        {
            var roleExists = await _roleManager.RoleExistsAsync(request.Name);
            if (roleExists)
            {
                return Result<string>.FaileResult("A role with the same name already exists.");
            }

            var role = new IdentityRole(request.Name);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<string>.FaileResult($"Failed to create role: {errors}");
            }

            return Result<string>.SuccessResult(role.Id);
        }
        catch (Exception ex)
        {
            return Result<string>.FaileResult($"Error creating role: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UpdateRoleAsync(UpdateRoleRequest request)
    {
        if (request == null)
        {
            return Result<bool>.FaileResult("Role request cannot be null.");
        }

        try
        {
            var role = await _roleManager.FindByIdAsync(request.Id);
            if (role == null)
            {
                return Result<bool>.FaileResult($"Role with id {request.Id} was not found.");
            }

            // Check if another role with same name exists
            var existingRole = await _roleManager.FindByNameAsync(request.Name);
            if (existingRole != null && existingRole.Id != request.Id)
            {
                return Result<bool>.FaileResult("A role with the same name already exists.");
            }

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<bool>.FaileResult($"Failed to update role: {errors}");
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.FaileResult($"Error updating role: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DeleteRoleAsync(string roleId)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return Result<bool>.FaileResult("Role not found.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<bool>.FaileResult($"Failed to delete role: {errors}");
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.FaileResult($"Error deleting role: {ex.Message}");
        }
    }

    public async Task<Result<bool>> AssignUserToRoleAsync(string userId, string roleName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<bool>.FaileResult("User not found.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return Result<bool>.FaileResult("Role not found.");
            }

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (isInRole)
            {
                return Result<bool>.FaileResult("User is already assigned to this role.");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<bool>.FaileResult($"Failed to assign user to role: {errors}");
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.FaileResult($"Error assigning user to role: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RemoveUserFromRoleAsync(string userId, string roleName)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<bool>.FaileResult("User not found.");
            }

            var isInRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!isInRole)
            {
                return Result<bool>.FaileResult("User is not assigned to this role.");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<bool>.FaileResult($"Failed to remove user from role: {errors}");
            }

            return Result<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.FaileResult($"Error removing user from role: {ex.Message}");
        }
    }

    public async Task<Result<IList<string>>> GetUserRolesAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<IList<string>>.FaileResult("User not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            return Result<IList<string>>.SuccessResult(roles.ToList());
        }
        catch (Exception ex)
        {
            return Result<IList<string>>.FaileResult($"Error retrieving user roles: {ex.Message}");
        }
    }
}
