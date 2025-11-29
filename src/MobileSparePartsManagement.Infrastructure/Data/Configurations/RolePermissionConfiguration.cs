using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileSparePartsManagement.Domain.Entities;

namespace MobileSparePartsManagement.Infrastructure.Data.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("rolepermissions");
        
        // Composite primary key
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
        
        builder.Property(rp => rp.RoleId)
            .HasColumnName("roleid");

        builder.Property(rp => rp.PermissionId)
            .HasColumnName("permissionid");

        // Relationships are configured in RoleConfiguration and PermissionConfiguration
    }
}