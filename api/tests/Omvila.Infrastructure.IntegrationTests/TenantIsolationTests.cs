using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

// Kural: ADR-0006 (RLS, set_config transaction'a yerel), ADR-0001 (omvila_app RLS'e tabidir)
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class TenantIsolationTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid BrandB = Guid.Parse("0193a8f0-0000-7000-8000-00000000000b");

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    // Her testte aynı politikayı taşıyan deneme tablosu: modüllerin kullanacağı SQL'in kendisi.
    public async ValueTask InitializeAsync()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        var sql = $"""
            DROP SCHEMA IF EXISTS rls_probe CASCADE;
            CREATE SCHEMA rls_probe;
            GRANT USAGE ON SCHEMA rls_probe TO omvila_app;
            CREATE TABLE rls_probe.items (id int PRIMARY KEY, brand_id uuid NOT NULL);
            {MigrationBuilderExtensions.TenantIsolationSql("rls_probe", "items")}
            INSERT INTO rls_probe.items VALUES (1, '{BrandA}'), (2, '{BrandA}'), (3, '{BrandB}');
            """;
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task AppRole_IsNotSuperuser_AndCannotBypassRls()
    {
        await using var services = db.CreateServices();
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(Ct);

        await using var command = new NpgsqlCommand(
            "SELECT rolsuper, rolbypassrls FROM pg_roles WHERE rolname = current_user", session.Connection, session.Transaction);
        await using var reader = await command.ExecuteReaderAsync(Ct);
        (await reader.ReadAsync(Ct)).ShouldBeTrue();
        reader.GetBoolean(0).ShouldBeFalse();
        reader.GetBoolean(1).ShouldBeFalse();
    }

    [Fact]
    public async Task BrandScope_SeesOnlyOwnRows() =>
        (await VisibleIdsAsync(TenantScope.ForBrand(BrandA))).ShouldBe([1, 2]);

    [Fact]
    public async Task NoScope_SeesNothing() =>
        (await VisibleIdsAsync(TenantScope.None)).ShouldBeEmpty();

    [Fact]
    public async Task PlatformScope_SeesAllBrands() =>
        (await VisibleIdsAsync(TenantScope.Platform)).ShouldBe([1, 2, 3]);

    [Fact]
    public async Task BrandScope_CannotInsertRowForAnotherBrand()
    {
        await using var services = db.CreateServices();
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(BrandA), Ct);
        await session.BeginAsync(Ct);

        await using var command = new NpgsqlCommand(
            $"INSERT INTO rls_probe.items VALUES (9, '{BrandB}')", session.Connection, session.Transaction);
        var error = await Should.ThrowAsync<PostgresException>(() => command.ExecuteNonQueryAsync(Ct));
        error.SqlState.ShouldBe(PostgresErrorCodes.InsufficientPrivilege);
    }

    [Fact]
    public async Task SetTenantAsync_DuringTransaction_AppliesImmediately()
    {
        await using var services = db.CreateServices();
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(Ct);
        (await ReadIdsAsync(session)).ShouldBeEmpty();

        // public uç: slug çözüldükten sonra marka bağlamı kurulur.
        await session.SetTenantAsync(TenantScope.ForBrand(BrandB), Ct);

        (await ReadIdsAsync(session)).ShouldBe([3]);
    }

    [Fact]
    public async Task TenantSetting_DoesNotLeakToNextTransactionOnPooledConnection()
    {
        await using var services = db.CreateServices(maxPoolSize: 1);

        await using (var first = services.CreateAsyncScope())
        {
            var session = first.ServiceProvider.GetRequiredService<IDbSession>();
            await session.SetTenantAsync(TenantScope.ForBrand(BrandA), Ct);
            await session.BeginAsync(Ct);
            (await ReadIdsAsync(session)).ShouldBe([1, 2]);
            await session.CommitAsync(Ct);
        }

        await using var second = services.CreateAsyncScope();
        var next = second.ServiceProvider.GetRequiredService<IDbSession>();
        await next.BeginAsync(Ct);
        await using var command = new NpgsqlCommand(
            "SELECT coalesce(current_setting('app.brand_id', true), ''), pg_backend_pid()", next.Connection, next.Transaction);
        await using var reader = await command.ExecuteReaderAsync(Ct);
        (await reader.ReadAsync(Ct)).ShouldBeTrue();
        reader.GetString(0).ShouldBeEmpty();
    }

    [Fact]
    public async Task Rollback_DiscardsWrites()
    {
        await using var services = db.CreateServices();
        await using (var scope = services.CreateAsyncScope())
        {
            var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
            await session.SetTenantAsync(TenantScope.ForBrand(BrandA), Ct);
            await session.BeginAsync(Ct);
            await using var insert = new NpgsqlCommand(
                $"INSERT INTO rls_probe.items VALUES (10, '{BrandA}')", session.Connection, session.Transaction);
            await insert.ExecuteNonQueryAsync(Ct);
            await session.RollbackAsync(Ct);
        }

        (await VisibleIdsAsync(TenantScope.ForBrand(BrandA))).ShouldBe([1, 2]);
    }

    private async Task<int[]> VisibleIdsAsync(TenantScope scope)
    {
        await using var services = db.CreateServices();
        await using var serviceScope = services.CreateAsyncScope();
        var session = serviceScope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(scope, Ct);
        await session.BeginAsync(Ct);
        return await ReadIdsAsync(session);
    }

    private static async Task<int[]> ReadIdsAsync(IDbSession session)
    {
        await using var command = new NpgsqlCommand("SELECT id FROM rls_probe.items ORDER BY id", session.Connection, session.Transaction);
        await using var reader = await command.ExecuteReaderAsync(Ct);
        var ids = new List<int>();
        while (await reader.ReadAsync(Ct))
        {
            ids.Add(reader.GetInt32(0));
        }

        return [.. ids];
    }
}
