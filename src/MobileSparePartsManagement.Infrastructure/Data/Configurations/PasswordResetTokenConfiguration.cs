using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobileSparePartsManagement.Domain.Entities;

namespace MobileSparePartsManagement.Infrastructure.Data.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("password_reset_tokens");
        
        builder.HasKey(prt => prt.Id);
        
        builder.Property(prt => prt.Id)
            .HasColumnName("id");

        builder.Property(prt => prt.UserId)
            .HasColumnName("user_id");

        builder.Property(prt => prt.Token)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("token");

        builder.Property(prt => prt.ExpiresAt)
            .IsRequired()
            .HasColumnName("expires_at");

        builder.Property(prt => prt.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.HasOne(prt => prt.User)
            .WithMany()
            .HasForeignKey(prt => prt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(prt => prt.Token);
    }
}