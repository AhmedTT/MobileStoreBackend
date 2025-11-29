namespace MobileSparePartsManagement.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Foreign key for Role
    public Guid RoleId { get; set; }

    // Navigation property
    public Role Role { get; set; } = null!;
}
