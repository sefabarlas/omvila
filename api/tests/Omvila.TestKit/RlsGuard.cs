using Npgsql;

namespace Omvila.TestKit;

/// <summary>
/// ADR-0006 § Sonuçlar: brand_id sütunu olup ENABLE + FORCE RLS ve politikası olmayan tabloları bulur.
/// </summary>
public static class RlsGuard
{
    private const string Query = """
        SELECT n.nspname || '.' || c.relname
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        JOIN pg_attribute a ON a.attrelid = c.oid AND a.attname = 'brand_id' AND NOT a.attisdropped
        WHERE c.relkind IN ('r', 'p')
          AND n.nspname NOT IN ('pg_catalog', 'information_schema')
          AND (NOT c.relrowsecurity
               OR NOT c.relforcerowsecurity
               OR NOT EXISTS (SELECT 1 FROM pg_policies p WHERE p.schemaname = n.nspname AND p.tablename = c.relname))
        ORDER BY 1
        """;

    public static async Task<IReadOnlyList<string>> FindUnprotectedTablesAsync(
        NpgsqlConnection connection, NpgsqlTransaction? transaction, CancellationToken ct)
    {
        await using var command = new NpgsqlCommand(Query, connection, transaction);
        await using var reader = await command.ExecuteReaderAsync(ct);
        var tables = new List<string>();
        while (await reader.ReadAsync(ct))
        {
            tables.Add(reader.GetString(0));
        }

        return tables;
    }
}
