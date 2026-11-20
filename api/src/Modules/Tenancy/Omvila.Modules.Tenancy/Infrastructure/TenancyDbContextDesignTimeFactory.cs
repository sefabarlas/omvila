using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Modules.Tenancy.Infrastructure;

/// <summary>Yalnız `dotnet ef migrations add` için; veritabanına bağlanmaz.</summary>
internal sealed class TenancyDbContextDesignTimeFactory : IDesignTimeDbContextFactory<TenancyDbContext>
{
    public TenancyDbContext CreateDbContext(string[] args) =>
        new(ModuleDbContexts.Configure(
            new DbContextOptionsBuilder<TenancyDbContext>(), new NpgsqlConnection("Host=localhost"), TenancyDbContext.Schema).Options);
}
