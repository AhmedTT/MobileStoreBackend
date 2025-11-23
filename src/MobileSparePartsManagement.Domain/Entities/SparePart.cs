namespace MobileSparePartsManagement.Domain.Entities;

public class SparePart
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Guid SupplierId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public Supplier Supplier { get; set; } = null!;
}
