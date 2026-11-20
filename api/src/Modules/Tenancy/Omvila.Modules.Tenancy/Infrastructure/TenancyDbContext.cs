using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure;

internal sealed class TenancyDbContext(DbContextOptions<TenancyDbContext> options) : DbContext(options)
{
    public const string Schema = "tenancy";

    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<BrandSettings> BrandSettings => Set<BrandSettings>();

    public DbSet<BookingPageSettings> BookingPageSettings => Set<BookingPageSettings>();

    public DbSet<BrandSlugHistory> SlugHistory => Set<BrandSlugHistory>();

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<Staff> Staff => Set<Staff>();

    public DbSet<StaffBranch> StaffBranches => Set<StaffBranch>();

    public DbSet<BrandOnboarding> Onboarding => Set<BrandOnboarding>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenancyDbContext).Assembly);
    }
}
