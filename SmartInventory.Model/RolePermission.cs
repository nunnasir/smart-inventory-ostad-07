using Microsoft.AspNetCore.Identity;

namespace SmartInventory.Model;

public class RolePermission
{
    public int Id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public int PermissionId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string AssignedBy { get; set; } = string.Empty;

    // Navigation properties
    public virtual IdentityRole Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
