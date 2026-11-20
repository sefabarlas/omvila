using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Omvila.Infrastructure.Persistence;

/// <summary>
/// Kayıtlı bütün modül DbContext'lerinin migration'larını migrator rolüyle uygular
/// (`dotnet run --project src/Omvila.Host -- migrate`).
/// </summary>
public sealed partial class MigrationRunner(IServiceProvider services, ILogger<MigrationRunner> logger)
{
    public async Task RunAsync(string migratorConnectionString, CancellationToken ct)
    {
        foreach (var context in services.GetServices<MigratableContext>())
        {
            await using var connection = new NpgsqlConnection(migratorConnectionString);
            var builder = (DbContextOptionsBuilder)Activator.CreateInstance(
                typeof(DbContextOptionsBuilder<>).MakeGenericType(context.ContextType))!;
            builder.UseNpgsql(connection, npgsql => npgsql
                    .UseNodaTime()
                    .MigrationsHistoryTable(ModuleDbContexts.MigrationsHistoryTable, context.Schema))
                .UseSnakeCaseNamingConvention();

            await using var db = (DbContext)ActivatorUtilities.CreateInstance(services, context.ContextType, builder.Options);
            await db.Database.MigrateAsync(ct);
            LogMigrated(logger, context.ContextType.Name);
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Migrations applied for {Context}")]
    private static partial void LogMigrated(ILogger logger, string context);
}
