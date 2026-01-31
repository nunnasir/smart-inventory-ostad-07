using SmartInventory.BLL.Model;
using SmartInventory.Contract.Request;
using SmartInventory.Model;

namespace SmartInventory.BLL.Interfaces;

public interface IPermissionService
{
    Task<Result<IList<Permission>>> GetAllAsync();
    Task<Result<Permission>> GetByIdAsync(int id);
    Task<Result<int>> CreateAsync(CreatePermissionRequest request);
    Task<Result<bool>> UpdateAsync(UpdatePermissionRequest request);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<bool>> AssignPermissionToRoleAsync(string roleId, int permissionId, string assignedBy);
    Task<Result<bool>> RemovePermissionFromRoleAsync(string roleId, int permissionId);
    Task<Result<IList<Permission>>> GetPermissionsByRoleIdAsync(string roleId);
    Task<Result<IList<string>>> GetRolesByPermissionIdAsync(int permissionId);
}
