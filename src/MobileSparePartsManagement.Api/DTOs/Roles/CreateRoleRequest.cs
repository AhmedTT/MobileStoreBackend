using System.ComponentModel.DataAnnotations;

namespace MobileSparePartsManagement.Api.DTOs.Roles;

public class CreateRoleRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}