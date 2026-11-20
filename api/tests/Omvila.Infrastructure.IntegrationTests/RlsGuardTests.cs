using Npgsql;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

// Kural: ADR-0006 § Sonuçlar — brand_id sütunu olan her tabloda ENABLE + FORCE RLS ve politika zorunlu.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class RlsGuardTests(PostgresFixture db)
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task AfterAllMigrations_EveryBrandOwnedTableIsProtected()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);

        (await RlsGuard.FindUnprotectedTablesAsync(connection, null, Ct)).ShouldBeEmpty();
    }

    [Fact]
    public async Task Guard_DetectsTableWithoutPolicyOrForcedRls()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var transaction = await connection.BeginTransactionAsync(Ct);
        await using var create = new NpgsqlCommand(
            """
            CREATE SCHEMA guard_probe;
            CREATE TABLE guard_probe.no_policy (brand_id uuid);
            CREATE TABLE guard_probe.not_forced (brand_id uuid);
            ALTER TABLE guard_probe.not_forced ENABLE ROW LEVEL SECURITY;
            CREATE POLICY p ON guard_probe.not_forced USING (true);
            """,
            connection,
            transaction);
        await create.ExecuteNonQueryAsync(Ct);

        var unprotected = await RlsGuard.FindUnprotectedTablesAsync(connection, transaction, Ct);

        unprotected.Contains("guard_probe.no_policy", StringComparer.Ordinal).ShouldBeTrue();
        unprotected.Contains("guard_probe.not_forced", StringComparer.Ordinal).ShouldBeTrue();
        await transaction.RollbackAsync(Ct);
    }
}
