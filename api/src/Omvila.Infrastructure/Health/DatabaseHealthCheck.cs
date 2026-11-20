using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Omvila.Infrastructure.Health;

/// <summary>/health/ready: uygulama rolüyle veritabanına bağlanılabiliyor mu.</summary>
internal sealed class DatabaseHealthCheck(NpgsqlDataSource dataSource) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = dataSource.CreateCommand("SELECT 1");
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (NpgsqlException exception)
        {
            return HealthCheckResult.Unhealthy("Veritabanına bağlanılamadı.", exception);
        }
    }
}
