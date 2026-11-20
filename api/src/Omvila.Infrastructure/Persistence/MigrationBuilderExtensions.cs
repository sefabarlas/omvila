using Microsoft.EntityFrameworkCore.Migrations;

namespace Omvila.Infrastructure.Persistence;

/// <summary>Modül migration'larının ortak SQL'i (ADR-0001 roller, ADR-0006 RLS).</summary>
public static class MigrationBuilderExtensions
{
    public const string AppRole = "omvila_app";

    /// <summary>Şemayı uygulama rolüne açar. Her modülün ilk migration'ında çağrılır.</summary>
    public static void GrantSchemaUsageToApp(this MigrationBuilder migrationBuilder, string schema)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);
        migrationBuilder.Sql($"GRANT USAGE ON SCHEMA {Quote(schema)} TO {AppRole};");
    }

    /// <summary>
    /// Markaya ait tabloyu kiracıya kilitler: ENABLE + FORCE RLS ve tenant_isolation politikası.
    /// <paramref name="customerColumn"/> verilirse müşteri kendi satırlarını görür (me yüzeyi).
    /// <paramref name="allowPlatformRowInsert"/>: brand_id'si boş platform satırı (ör. OTP bildirimi) her bağlamdan
    /// eklenebilir ama yalnız platform bağlamı okuyabilir.
    /// <paramref name="tenantColumn"/>: marka kimliğini taşıyan sütun (marka tablosunun kendisinde "id").
    /// </summary>
    public static void EnableTenantIsolation(
        this MigrationBuilder migrationBuilder,
        string schema,
        string table,
        string? customerColumn = null,
        bool allowPlatformRowInsert = false,
        string tenantColumn = "brand_id")
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);
        migrationBuilder.Sql(TenantIsolationSql(schema, table, customerColumn, allowPlatformRowInsert, tenantColumn));
    }

    internal static string TenantIsolationSql(
        string schema, string table, string? customerColumn = null, bool allowPlatformRowInsert = false, string tenantColumn = "brand_id")
    {
        var qualified = $"{Quote(schema)}.{Quote(table)}";
        var customerClause = customerColumn is null
            ? string.Empty
            : $"\n    OR {Quote(customerColumn)} = nullif(current_setting('app.customer_id', true), '')::uuid";
        return $"""
            ALTER TABLE {qualified} ENABLE ROW LEVEL SECURITY;
            ALTER TABLE {qualified} FORCE ROW LEVEL SECURITY;
            CREATE POLICY tenant_isolation ON {qualified}
              USING (
                {Quote(tenantColumn)} = nullif(current_setting('app.brand_id', true), '')::uuid
                OR current_setting('app.is_platform', true) = 'true'{customerClause}
              );
            {(allowPlatformRowInsert ? $"CREATE POLICY platform_row_insert ON {qualified} FOR INSERT WITH CHECK (brand_id IS NULL);" : string.Empty)}
            """;
    }

    private static string Quote(string identifier)
    {
        if (identifier.Length == 0 || !identifier.All(c => char.IsAsciiLetterLower(c) || char.IsAsciiDigit(c) || c == '_'))
        {
            throw new ArgumentException($"Geçersiz tanımlayıcı: {identifier}", nameof(identifier));
        }

        return $"\"{identifier}\"";
    }
}
