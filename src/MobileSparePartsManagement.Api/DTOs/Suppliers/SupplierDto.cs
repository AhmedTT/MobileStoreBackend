namespace MobileSparePartsManagement.Api.DTOs.Suppliers;

public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}
