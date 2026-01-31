using System.ComponentModel.DataAnnotations;

namespace SmartInventory.Contract.Request;

public class UpdateRoleRequest
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
