using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartInventory.DAL.Context;
using SmartInventory.Model;

namespace SmartInventory.Api.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly SmartInventoryDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PermissionAuthorizationHandler(
        SmartInventoryDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User == null || context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userId = _userManager.GetUserId(context.User);
        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        // Get user's roles
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return;
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        // Check if any of the user's roles have the required permission
        var hasPermission = await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Include(rp => rp.Role)
            .Where(rp => rp.Role != null && 
                         rp.Permission != null &&
                         userRoles.Contains(rp.Role.Name!) && 
                         rp.Permission.Name == requirement.Permission &&
                         rp.Permission.IsActive)
            .AnyAsync();

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
