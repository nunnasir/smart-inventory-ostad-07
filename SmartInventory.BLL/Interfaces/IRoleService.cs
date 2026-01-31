using SmartInventory.BLL.Model;
using SmartInventory.Contract.Request;
using Microsoft.AspNetCore.Identity;

namespace SmartInventory.BLL.Interfaces;

public interface IRoleService
{
    Task<Result<IList<IdentityRole>>> GetAllRolesAsync();
    Task<Result<IdentityRole>> GetRoleByIdAsync(string roleId);
    Task<Result<IdentityRole>> GetRoleByNameAsync(string roleName);
    Task<Result<string>> CreateRoleAsync(CreateRoleRequest request);
    Task<Result<bool>> UpdateRoleAsync(UpdateRoleRequest request);
    Task<Result<bool>> DeleteRoleAsync(string roleId);
    Task<Result<bool>> AssignUserToRoleAsync(string userId, string roleName);
    Task<Result<bool>> RemoveUserFromRoleAsync(string userId, string roleName);
    Task<Result<IList<string>>> GetUserRolesAsync(string userId);
}
