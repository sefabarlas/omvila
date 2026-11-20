using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Infrastructure;

internal sealed class CustomersDbContext(DbContextOptions<CustomersDbContext> options) : DbContext(options)
{
    public const string Schema = "customers";

    /// <summary>Global kişiler: markaya ait değildir, RLS'siz; sorgular yalnız markanın kartları üzerinden yapılır.</summary>
    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<BrandCustomer> BrandCustomers => Set<BrandCustomer>();

    public DbSet<ConsentRecord> ConsentRecords => Set<ConsentRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
    }
}
