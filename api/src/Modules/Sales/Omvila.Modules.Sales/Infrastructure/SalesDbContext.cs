using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Sales.Domain;

namespace Omvila.Modules.Sales.Infrastructure;

internal sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options)
{
    public const string Schema = "sales";

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<PaymentAllocation> PaymentAllocations => Set<PaymentAllocation>();

    public DbSet<CommissionEntry> CommissionEntries => Set<CommissionEntry>();

    public DbSet<Package> Packages => Set<Package>();

    public DbSet<PackageItem> PackageItems => Set<PackageItem>();

    public DbSet<CustomerPackage> CustomerPackages => Set<CustomerPackage>();

    public DbSet<CustomerPackageItem> CustomerPackageItems => Set<CustomerPackageItem>();

    public DbSet<PackageSessionUsage> PackageSessionUsages => Set<PackageSessionUsage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
    }
}
