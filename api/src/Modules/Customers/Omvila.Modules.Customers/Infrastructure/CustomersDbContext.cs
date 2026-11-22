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

    public DbSet<DataDeletionRequest> DataDeletionRequests => Set<DataDeletionRequest>();

    public DbSet<CustomerImport> CustomerImports => Set<CustomerImport>();

    /// <summary>Müşteri uygulamasında eklenen salonlar (RLS: marka veya müşterinin kendisi).</summary>
    public DbSet<CustomerSavedBrand> SavedBrands => Set<CustomerSavedBrand>();

    /// <summary>"Tüm verileri dışa aktar" istekleri ve arşivleri (T-121).</summary>
    public DbSet<DataExport> DataExports => Set<DataExport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersDbContext).Assembly);
    }
}
