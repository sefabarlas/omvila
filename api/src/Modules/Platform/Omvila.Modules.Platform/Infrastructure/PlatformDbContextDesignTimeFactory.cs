using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Modules.Platform.Infrastructure;

/// <summary>Yalnız `dotnet ef migrations add` için; veritabanına bağlanmaz.</summary>
internal sealed class PlatformDbContextDesignTimeFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args) =>
        new(ModuleDbContexts.Configure(
            new DbContextOptionsBuilder<PlatformDbContext>(), new NpgsqlConnection("Host=localhost"), PlatformDbContext.Schema).Options);
}
