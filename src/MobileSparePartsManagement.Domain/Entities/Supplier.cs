namespace MobileSparePartsManagement.Domain.Entities;

public class Supplier
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public ICollection<SparePart> SpareParts { get; set; } = new List<SparePart>();
}
