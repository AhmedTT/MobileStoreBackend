using System.ComponentModel.DataAnnotations;

namespace MobileSparePartsManagement.Api.DTOs.Permissions;

public class CreatePermissionRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}