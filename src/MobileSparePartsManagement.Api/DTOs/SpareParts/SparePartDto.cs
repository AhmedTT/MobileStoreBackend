namespace MobileSparePartsManagement.Api.DTOs.SpareParts;

public class SparePartDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal? WholesalePrice { get; set; } // Null for regular users
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
