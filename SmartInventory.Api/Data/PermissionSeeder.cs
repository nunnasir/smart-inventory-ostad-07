using Microsoft.AspNetCore.Identity;
using SmartInventory.BLL.Interfaces;
using SmartInventory.Contract.Request;
using SmartInventory.Model;

namespace SmartInventory.Api.Data;

public static class PermissionSeeder
{
    public static async Task SeedAsync(
        IPermissionService permissionService,
        IRoleService roleService,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        // Define default permissions
        var permissions = new[]
        {
            // Product Permissions
            new CreatePermissionRequest { Name = "Product.Read", Resource = "Product", Action = "Read", Description = "Read products" },
            new CreatePermissionRequest { Name = "Product.Create", Resource = "Product", Action = "Create", Description = "Create products" },
            new CreatePermissionRequest { Name = "Product.Update", Resource = "Product", Action = "Update", Description = "Update products" },
            new CreatePermissionRequest { Name = "Product.Delete", Resource = "Product", Action = "Delete", Description = "Delete products" },
            
            // Permission Management
            new CreatePermissionRequest { Name = "Permission.Read", Resource = "Permission", Action = "Read", Description = "Read permissions" },
            new CreatePermissionRequest { Name = "Permission.Manage", Resource = "Permission", Action = "Manage", Description = "Manage permissions" },
            
            // Role Management
            new CreatePermissionRequest { Name = "Role.Read", Resource = "Role", Action = "Read", Description = "Read roles" },
            new CreatePermissionRequest { Name = "Role.Manage", Resource = "Role", Action = "Manage", Description = "Manage roles" },
        };

        // Create permissions
        var createdPermissions = new Dictionary<string, int>();
        foreach (var perm in permissions)
        {
            var result = await permissionService.CreateAsync(perm);
            if (result.Success)
            {
                createdPermissions[perm.Name] = result.Data;
                Console.WriteLine($"Created permission: {perm.Name}");
            }
            else
            {
                // Permission might already exist, try to get it
                var allPerms = await permissionService.GetAllAsync();
                if (allPerms.Success && allPerms.Data != null)
                {
                    var existing = allPerms.Data.FirstOrDefault(p => p.Name == perm.Name);
                    if (existing != null)
                    {
                        createdPermissions[perm.Name] = existing.Id;
                        Console.WriteLine($"→ Permission already exists: {perm.Name}");
                    }
                }
            }
        }

        // Create roles
        var roles = new[]
        {
            new { Name = "Admin", Description = "Administrator with full access" },
            new { Name = "Manager", Description = "Manager with read, create, and update access" },
            new { Name = "User", Description = "Regular user with read access" },
        };

        var createdRoles = new Dictionary<string, string>();
        foreach (var role in roles)
        {
            var roleRequest = new CreateRoleRequest { Name = role.Name, Description = role.Description };
            var result = await roleService.CreateRoleAsync(roleRequest);
            if (result.Success && !string.IsNullOrEmpty(result.Data))
            {
                createdRoles[role.Name] = result.Data;
                Console.WriteLine($"✓ Created role: {role.Name}");
            }
            else
            {
                // Role might already exist
                var existingRole = await roleManager.FindByNameAsync(role.Name);
                if (existingRole != null)
                {
                    createdRoles[role.Name] = existingRole.Id;
                    Console.WriteLine($"→ Role already exists: {role.Name}");
                }
            }
        }

        // Assign permissions to Admin role (all permissions)
        if (createdRoles.ContainsKey("Admin"))
        {
            var adminRoleId = createdRoles["Admin"];
            foreach (var perm in createdPermissions)
            {
                await permissionService.AssignPermissionToRoleAsync(
                    adminRoleId,
                    perm.Value,
                    "System");
            }
            Console.WriteLine($"✓ Assigned all permissions to Admin role");
        }

        // Assign permissions to Manager role
        if (createdRoles.ContainsKey("Manager"))
        {
            var managerRoleId = createdRoles["Manager"];
            var managerPermissions = new[]
            {
                "Product.Read", "Product.Create", "Product.Update",
                "Order.Read", "Order.Create", "Order.Update"
            };

            foreach (var permName in managerPermissions)
            {
                if (createdPermissions.ContainsKey(permName))
                {
                    await permissionService.AssignPermissionToRoleAsync(
                        managerRoleId,
                        createdPermissions[permName],
                        "System");
                }
            }
            Console.WriteLine($"✓ Assigned permissions to Manager role");
        }

        // Assign permissions to User role
        if (createdRoles.ContainsKey("User"))
        {
            var userRoleId = createdRoles["User"];
            var userPermissions = new[]
            {
                "Product.Read",
                "Order.Read", "Order.Create"
            };

            foreach (var permName in userPermissions)
            {
                if (createdPermissions.ContainsKey(permName))
                {
                    await permissionService.AssignPermissionToRoleAsync(
                        userRoleId,
                        createdPermissions[permName],
                        "System");
                }
            }
            Console.WriteLine($"✓ Assigned permissions to User role");
        }

        Console.WriteLine("\n✅ Permission seeding completed!");
    }
}
