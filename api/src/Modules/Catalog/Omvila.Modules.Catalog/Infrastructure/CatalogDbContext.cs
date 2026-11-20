using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Catalog.Domain;

namespace Omvila.Modules.Catalog.Infrastructure;

internal sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public const string Schema = "catalog";

    public DbSet<ServiceCategory> Categories => Set<ServiceCategory>();

    public DbSet<Service> Services => Set<Service>();

    public DbSet<ServiceStageRow> Stages => Set<ServiceStageRow>();

    public DbSet<ServiceStaff> ServiceStaff => Set<ServiceStaff>();

    public DbSet<BranchServicePrice> BranchPrices => Set<BranchServicePrice>();

    public DbSet<ServiceTemplate> Templates => Set<ServiceTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
