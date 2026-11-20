using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Modules.Catalog.Infrastructure;

/// <summary>Yalnız `dotnet ef migrations add` için; veritabanına bağlanmaz.</summary>
internal sealed class CatalogDbContextDesignTimeFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args) =>
        new(ModuleDbContexts.Configure(
            new DbContextOptionsBuilder<CatalogDbContext>(), new NpgsqlConnection("Host=localhost"), CatalogDbContext.Schema).Options);
}
