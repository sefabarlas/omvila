using Npgsql;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

/// <summary>Koleksiyondaki testler aynı veritabanını sırayla kullanır; infra tabloları her testte boşaltılır.</summary>
internal static class InfraTables
{
    public static async Task ResetAsync(PostgresFixture db, CancellationToken ct)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(ct);
        await using var command = new NpgsqlCommand(
            "TRUNCATE infra.outbox_events, infra.processed_events, infra.job_runs", connection);
        await command.ExecuteNonQueryAsync(ct);
    }

    public static async Task<T> ScalarAsync<T>(PostgresFixture db, string sql, CancellationToken ct)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(ct))!;
    }
}
