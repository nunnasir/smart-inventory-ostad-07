using Microsoft.AspNetCore.Authorization;

namespace SmartInventory.Api.Authorization;

/// <summary>
/// Custom authorization attribute that requires a specific permission
/// Usage: [RequirePermission("Product.Read")]
/// </summary>
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}
