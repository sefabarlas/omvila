using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Modules.Integrations.Infrastructure;

/// <summary>Yalnız `dotnet ef migrations add` için; veritabanına bağlanmaz.</summary>
internal sealed class IntegrationsDbContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationsDbContext>
{
    public IntegrationsDbContext CreateDbContext(string[] args) =>
        new(ModuleDbContexts.Configure(
            new DbContextOptionsBuilder<IntegrationsDbContext>(), new NpgsqlConnection("Host=localhost"), IntegrationsDbContext.Schema).Options);
}
