using System.ComponentModel.DataAnnotations;

namespace SmartInventory.Contract.Request;

public class UpdatePermissionRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Resource { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Action { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
