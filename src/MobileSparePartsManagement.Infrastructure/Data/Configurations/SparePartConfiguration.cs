using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileSparePartsManagement.Domain.Entities;

namespace MobileSparePartsManagement.Infrastructure.Data.Configurations;

public class SparePartConfiguration : IEntityTypeConfiguration<SparePart>
{
    public void Configure(EntityTypeBuilder<SparePart> builder)
    {
        builder.ToTable("spare_parts");
        
        builder.HasKey(sp => sp.Id);
        
        builder.Property(sp => sp.Id)
            .HasColumnName("id");

        builder.Property(sp => sp.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(sp => sp.Quantity)
            .IsRequired()
            .HasColumnName("quantity");

        builder.Property(sp => sp.Price)
            .IsRequired()
            .HasPrecision(10, 2)
            .HasColumnName("price");

        builder.Property(sp => sp.SupplierId)
            .HasColumnName("supplier_id");

        builder.Property(sp => sp.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.HasOne(sp => sp.Supplier)
            .WithMany(s => s.SpareParts)
            .HasForeignKey(sp => sp.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Add check constraints for non-negative values
        builder.ToTable(t => t.HasCheckConstraint("CK_SparePart_Quantity", "\"quantity\" >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_SparePart_Price", "\"price\" >= 0"));
    }
}
