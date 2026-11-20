using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>Yalnız `dotnet ef migrations add` için; veritabanına bağlanmaz.</summary>
internal sealed class SchedulingDbContextDesignTimeFactory : IDesignTimeDbContextFactory<SchedulingDbContext>
{
    public SchedulingDbContext CreateDbContext(string[] args) =>
        new(ModuleDbContexts.Configure(
            new DbContextOptionsBuilder<SchedulingDbContext>(), new NpgsqlConnection("Host=localhost"), SchedulingDbContext.Schema).Options);
}
