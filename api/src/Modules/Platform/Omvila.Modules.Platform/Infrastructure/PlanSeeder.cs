using Microsoft.Extensions.Options;
using Npgsql;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Infrastructure;

/// <summary>
/// Geliştirme ve test planları (S-14: "geliştirmede sınırsız plan seed'i"). Yalnız Platform:SeedPlans doluysa çalışır;
/// üretim yapılandırmasında bu bölüm yoktur. Var olan koda dokunmaz.
/// </summary>
public sealed class PlanSeeder(IOptions<PlatformOptions> options, IIdGenerator ids)
{
    public async Task SeedAsync(string migratorConnectionString, CancellationToken ct)
    {
        if (options.Value.SeedPlans.Count == 0)
        {
            return;
        }

        await using var connection = new NpgsqlConnection(migratorConnectionString);
        await connection.OpenAsync(ct);
        foreach (var seed in options.Value.SeedPlans)
        {
            await using var command = new NpgsqlCommand(
                "INSERT INTO platform.plans (id, code, name, is_public, is_active) VALUES ($1, $2, $3, false, true) ON CONFLICT (code) DO NOTHING",
                connection);
            command.Parameters.Add(new NpgsqlParameter { Value = ids.New() });
            command.Parameters.Add(new NpgsqlParameter { Value = seed.Code });
            command.Parameters.Add(new NpgsqlParameter { Value = seed.Name });
            await command.ExecuteNonQueryAsync(ct);
        }
    }
}
