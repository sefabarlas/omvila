using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.Contracts;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Catalog.IntegrationTests;

// Kural: alan-modeli.md § BranchServicePrice "Fiyat ve süre çözümü" (şube fiyatı + personel ek ücreti; personel süresi
// son personel-meşgul aşamaya), § ServiceStage (aşamasız hizmet tek örtük aşama), § ServiceStaff (hizmeti yalnız
// atanmış personel verir); dilim-02 kabul kriteri 2 (Elif'in ek ücreti ve Nişantaşı şube fiyatı kalem fiyatına yansır).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class ServiceOffersTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-00000000d0a1");
    private static readonly Guid OtherBrand = Guid.Parse("0193a8f0-0000-7000-8000-00000000d0a2");
    private static readonly Guid Category = Guid.Parse("0193a8f0-0000-7000-8000-00000000d0c1");
    private static readonly Guid Color = Guid.Parse("0193a8f0-0000-7000-8000-00000000d051");
    private static readonly Guid Haircut = Guid.Parse("0193a8f0-0000-7000-8000-00000000d052");
    private static readonly Guid Kadikoy = Guid.Parse("0193a8f0-0000-7000-8000-00000000d0b1");
    private static readonly Guid Nisantasi = Guid.Parse("0193a8f0-0000-7000-8000-00000000d0b2");
    private static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-00000000d0e1");
    private static readonly Guid Zeynep = Guid.Parse("0193a8f0-0000-7000-8000-00000000d0e2");

    private ServiceProvider _services = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand($"""
                TRUNCATE catalog.service_categories CASCADE;
                INSERT INTO catalog.service_categories (id, brand_id, name, sort_order, is_active, created_utc) VALUES ('{Category}', '{Brand}', 'Renk', 1, true, now());
                INSERT INTO catalog.services (id, brand_id, category_id, name, duration_minutes, base_price_minor, currency_code, price_display,
                    buffer_before_minutes, buffer_after_minutes, deposit_mode, is_online_bookable, sort_order, is_active, created_utc)
                  VALUES ('{Color}', '{Brand}', '{Category}', 'Boya', 120, 120000, 'TRY', 'StartingFrom', 0, 10, 'Inherit', true, 1, true, now()),
                         ('{Haircut}', '{Brand}', '{Category}', 'Saç kesimi', 45, 45000, 'TRY', 'Fixed', 0, 0, 'Inherit', true, 2, true, now());
                INSERT INTO catalog.service_stages (id, brand_id, service_id, seq_no, name, duration_minutes, staff_busy) VALUES
                  (gen_random_uuid(), '{Brand}', '{Color}', 1, 'Uygulama', 45, true),
                  (gen_random_uuid(), '{Brand}', '{Color}', 2, 'Bekleme', 40, false),
                  (gen_random_uuid(), '{Brand}', '{Color}', 3, 'Yıkama ve fön', 35, true);
                INSERT INTO catalog.branch_service_prices (id, brand_id, branch_id, service_id, price_minor, duration_override_minutes, is_available) VALUES
                  (gen_random_uuid(), '{Brand}', '{Nisantasi}', '{Color}', 130000, NULL, true),
                  (gen_random_uuid(), '{Brand}', '{Nisantasi}', '{Haircut}', 55000, 50, true);
                INSERT INTO catalog.service_staff (id, brand_id, service_id, staff_id, duration_override_minutes, surcharge_minor, is_active) VALUES
                  (gen_random_uuid(), '{Brand}', '{Color}', '{Elif}', 130, 30000, true),
                  (gen_random_uuid(), '{Brand}', '{Haircut}', '{Elif}', NULL, 0, true),
                  (gen_random_uuid(), '{Brand}', '{Haircut}', '{Zeynep}', NULL, 0, true);
                """, connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        _services = db.CreateServices(configure: s => s.AddCatalogModule());
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task BranchPrice_PlusStaffSurcharge_AndStaffDurationOnTheLastBusyStage()
    {
        var offer = (await ResolveAsync(Brand, Nisantasi, Color, Elif)).Value;

        offer.Price.ShouldBe(Money.Try(160_000));
        offer.StaffSurcharge.ShouldBe(Money.Try(30_000));
        offer.IsBranchPrice.ShouldBeTrue();
        offer.PriceDisplay.ShouldBe(ServicePriceDisplay.StartingFrom);
        offer.DurationMinutes.ShouldBe(130);
        offer.Stages.ShouldBe([new("Uygulama", 45, true), new("Bekleme", 40, false), new("Yıkama ve fön", 45, true)]);
        offer.BufferAfterMinutes.ShouldBe(10);
        offer.ServiceName.ShouldBe("Boya");
    }

    [Fact]
    public async Task WithoutBranchPrice_UsesBasePrice_AndStagelessServiceHasOneImplicitStage()
    {
        var offer = (await ResolveAsync(Brand, Kadikoy, Haircut, Zeynep)).Value;

        offer.Price.ShouldBe(Money.Try(45_000));
        offer.StaffSurcharge.ShouldBe(Money.Zero(Currency.TRY));
        offer.IsBranchPrice.ShouldBeFalse();
        offer.Stages.ShouldBe([new("Saç kesimi", 45, true)]);
    }

    [Fact]
    public async Task BranchDurationOverride_AppliesWhenStaffHasNone()
    {
        var offer = (await ResolveAsync(Brand, Nisantasi, Haircut, Elif)).Value;

        offer.Price.ShouldBe(Money.Try(55_000));
        offer.DurationMinutes.ShouldBe(50);
        offer.Stages.ShouldBe([new("Saç kesimi", 50, true)]);
    }

    [Fact]
    public async Task StaffWhoDoesNotProvideTheService_IsNotFound() =>
        (await ResolveAsync(Brand, Kadikoy, Color, Zeynep)).Error!.Code.ShouldBe(ErrorCodes.NotFound);

    [Fact]
    public async Task OtherBrand_CannotResolve() =>
        (await ResolveAsync(OtherBrand, Kadikoy, Haircut, null)).Error!.Code.ShouldBe(ErrorCodes.NotFound);

    [Fact]
    public async Task Batch_KeepsRequestOrder()
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(Brand), Ct);
        await session.BeginAsync(Ct);

        var results = await scope.ServiceProvider.GetRequiredService<IServiceOffers>().ResolveAsync(
            Brand, Nisantasi, [new(Haircut, Elif), new(Color, Zeynep), new(Color, Elif)], Ct);
        await session.RollbackAsync(Ct);

        results.Select(r => r.IsSuccess ? r.Value.Price.AmountMinor : -1).ShouldBe([55_000, -1, 160_000]);
    }

    private async Task<Result<ServiceOffer>> ResolveAsync(Guid brandId, Guid branchId, Guid serviceId, Guid? staffId)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        var result = await scope.ServiceProvider.GetRequiredService<IServiceOffers>().ResolveAsync(brandId, branchId, serviceId, staffId, Ct);
        await session.RollbackAsync(Ct);
        return result;
    }
}
