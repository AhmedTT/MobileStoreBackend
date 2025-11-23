using Microsoft.EntityFrameworkCore;
using MobileSparePartsManagement.Domain.Entities;
using MobileSparePartsManagement.Infrastructure.Data.Configurations;

namespace MobileSparePartsManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SparePart> SpareParts => Set<SparePart>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        modelBuilder.ApplyConfiguration(new SparePartConfiguration());
    }
}
