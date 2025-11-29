using System.ComponentModel.DataAnnotations;

namespace MobileSparePartsManagement.Api.DTOs.Roles;

public class AssignPermissionsRequest
{
    [Required]
    public List<Guid> PermissionIds { get; set; } = new();
}