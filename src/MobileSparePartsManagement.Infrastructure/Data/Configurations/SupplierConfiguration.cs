using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileSparePartsManagement.Domain.Entities;

namespace MobileSparePartsManagement.Infrastructure.Data.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasColumnName("id");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(s => s.ContactEmail)
            .HasMaxLength(256)
            .HasColumnName("contact_email");

        builder.Property(s => s.Phone)
            .HasMaxLength(50)
            .HasColumnName("phone");

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.HasMany(s => s.SpareParts)
            .WithOne(sp => sp.Supplier)
            .HasForeignKey(sp => sp.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
