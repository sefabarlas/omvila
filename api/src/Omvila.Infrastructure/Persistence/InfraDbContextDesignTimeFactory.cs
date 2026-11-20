using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Omvila.Infrastructure.Persistence;

/// <summary>Yalnız `dotnet ef migrations add` için; veritabanına bağlanmaz.</summary>
internal sealed class InfraDbContextDesignTimeFactory : IDesignTimeDbContextFactory<InfraDbContext>
{
    public InfraDbContext CreateDbContext(string[] args) =>
        new(ModuleDbContexts.Configure(
            new DbContextOptionsBuilder<InfraDbContext>(), new NpgsqlConnection("Host=localhost"), InfraDbContext.Schema).Options);
}
