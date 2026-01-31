namespace SmartInventory.Model;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty; // e.g., "Product", "Order", "Payment"
    public string Action { get; set; } = string.Empty; // e.g., "Create", "Read", "Update", "Delete"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
