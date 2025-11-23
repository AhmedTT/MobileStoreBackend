using System.ComponentModel.DataAnnotations;

namespace MobileSparePartsManagement.Api.DTOs.Suppliers;

public class CreateSupplierRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    public string? ContactEmail { get; set; }

    [MaxLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
    public string? Phone { get; set; }
}
