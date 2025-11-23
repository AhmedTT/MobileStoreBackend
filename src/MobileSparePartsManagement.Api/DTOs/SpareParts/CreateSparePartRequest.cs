using System.ComponentModel.DataAnnotations;

namespace MobileSparePartsManagement.Api.DTOs.SpareParts;

public class CreateSparePartRequest
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be non-negative")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "SupplierId is required")]
    public Guid SupplierId { get; set; }
}
