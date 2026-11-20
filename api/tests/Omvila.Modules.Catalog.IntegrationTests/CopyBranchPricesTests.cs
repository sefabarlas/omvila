using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Features;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.TestKit;

namespace Omvila.Modules.Catalog.IntegrationTests;

// Kural: SubeEkle "Hizmet ve fiyatlar: Kadıköy'den kopyala" (sözleşme BranchCreate.copyServicePricesFromBranchId).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class CopyBranchPricesTests(PostgresFixture db)
{
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-00000000c0a1");
    private static readonly Guid Category = Guid.Parse("0193a8f0-0000-7000-8000-00000000c0a2");
    private static readonly Guid Service = Guid.Parse("0193a8f0-0000-7000-8000-00000000c0a3");
    private static readonly Guid Kadikoy = Guid.Parse("0193a8f0-0000-7000-8000-00000000c0b1");
    private static readonly Guid Moda = Guid.Parse("0193a8f0-0000-7000-8000-00000000c0b2");

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task CopiesSourceBranchPrices_AndIsSafeToRepeat()
    {
        await ExecuteAsync($"""
            TRUNCATE catalog.service_categories CASCADE;
            INSERT INTO catalog.service_categories (id, brand_id, name, sort_order, is_active, created_utc) VALUES ('{Category}', '{Brand}', 'Saç', 1, true, now());
            INSERT INTO catalog.services (id, brand_id, category_id, name, duration_minutes, base_price_minor, currency_code, price_display,
                buffer_before_minutes, buffer_after_minutes, deposit_mode, is_online_bookable, sort_order, is_active, created_utc)
              VALUES ('{Service}', '{Brand}', '{Category}', 'Saç kesimi', 45, 45000, 'TRY', 'Fixed', 0, 0, 'Inherit', true, 1, true, now());
            INSERT INTO catalog.branch_service_prices (id, brand_id, branch_id, service_id, price_minor, duration_override_minutes, is_available)
              VALUES (gen_random_uuid(), '{Brand}', '{Kadikoy}', '{Service}', 50000, 50, true);
            """);
        await using var services = db.CreateServices(configure: s => s.AddCatalogModule());

        await HandleAsync(services, new BranchCreated(Brand, Moda, Kadikoy));
        await HandleAsync(services, new BranchCreated(Brand, Moda, Kadikoy));

        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(
            $"SELECT count(*) || '/' || max(price_minor) || '/' || max(duration_override_minutes) FROM catalog.branch_service_prices WHERE branch_id = '{Moda}'", connection);
        ((string)(await command.ExecuteScalarAsync(Ct))!).ShouldBe("1/50000/50");
    }

    private static async Task HandleAsync(ServiceProvider services, BranchCreated integrationEvent)
    {
        await using var scope = services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.Platform, Ct);
        await session.BeginAsync(Ct);
        await scope.ServiceProvider.GetRequiredService<CopyBranchPricesOnBranchCreated>().HandleAsync(integrationEvent, Ct);
        await session.CommitAsync(Ct);
    }

    private async Task ExecuteAsync(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }
}
