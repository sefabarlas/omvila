using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Customers.IntegrationTests;

// Kural: mimari.md § Modüller arası iletişim (ICustomerDirectory.EnsureBrandCustomer randevu transaction'ında),
// dilim-02 § Alan kuralları (online oluşturmada BrandCustomer yoksa Source=Online, Customer yoksa IsVerified=true;
// aydınlatma metni ConsentRecord), alan-modeli.md § Customer (global, telefon tek) ve § ConsentRecord (yalnız eklenir),
// ADR-0006 (kart başka markaya görünmez).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class CustomerDirectoryTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid BrandA = Guid.Parse("0193a8f0-0000-7000-8000-00000000cc01");
    private static readonly Guid BrandB = Guid.Parse("0193a8f0-0000-7000-8000-00000000cc02");
    private static readonly PhoneNumber Selin = PhoneNumber.Parse("+905550004418");

    private ServiceProvider _services = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand("TRUNCATE customers.customers CASCADE;", connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        _services = db.CreateServices(configure: s => s.AddCustomersModule());
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task Ensure_CreatesVerifiedCustomerAndOnlineCard_ThenFindsTheSameCard()
    {
        var first = await InBrandAsync(BrandA, d => d.EnsureBrandCustomerAsync(new(BrandA, Selin, "Selin Öztürk", CustomerSource.Online, PhoneVerified: true), Ct));
        var again = await InBrandAsync(BrandA, d => d.EnsureBrandCustomerAsync(new(BrandA, Selin, "Selin Ö.", CustomerSource.Phone, PhoneVerified: false), Ct));

        first.Created.ShouldBeTrue();
        again.Created.ShouldBeFalse();
        again.BrandCustomerId.ShouldBe(first.BrandCustomerId);
        (await ScalarAsync<string>("SELECT full_name || '/' || is_verified FROM customers.customers")).ShouldBe("Selin Öztürk/true");
        (await ScalarAsync<string>("SELECT source || '/' || display_name FROM customers.brand_customers")).ShouldBe("Online/Selin Öztürk");
    }

    [Fact]
    public async Task OtherBrand_ReusesGlobalCustomer_ButCannotSeeTheFirstBrandsCard()
    {
        var a = await InBrandAsync(BrandA, d => d.EnsureBrandCustomerAsync(new(BrandA, Selin, "Selin Öztürk", CustomerSource.Online, true), Ct));
        var b = await InBrandAsync(BrandB, d => d.EnsureBrandCustomerAsync(new(BrandB, Selin, "Selin", CustomerSource.Phone, false), Ct));

        b.CustomerId.ShouldBe(a.CustomerId);
        b.BrandCustomerId.ShouldNotBe(a.BrandCustomerId);
        (await InBrandAsync(BrandB, d => d.GetDisplayInfoAsync(BrandB, [a.BrandCustomerId, b.BrandCustomerId], Ct))).Keys.ShouldBe([b.BrandCustomerId]);
        var info = (await InBrandAsync(BrandB, d => d.GetDisplayInfoAsync(BrandB, [b.BrandCustomerId], Ct)))[b.BrandCustomerId];
        info.FullName.ShouldBe("Selin", "B markası A'nın adını görmez; kendi kart adını görür");
        info.PhoneMasked.ShouldBe("0555 *** ** 18");
        (await InBrandAsync(BrandB, d => d.ApplyCountersAsync(BrandB, a.BrandCustomerId, new CustomerCounterChange(Visits: 1), Ct))).Error!.Code.ShouldBe(ErrorCodes.NotFound);
    }

    [Fact]
    public async Task ConcurrentEnsure_MeetsOnOneCustomerAndOneCard()
    {
        var results = await Task.WhenAll(Enumerable.Range(0, 4).Select(_ =>
            InBrandAsync(BrandA, d => d.EnsureBrandCustomerAsync(new(BrandA, Selin, "Selin Öztürk", CustomerSource.Online, true), Ct))));

        results.Select(r => r.BrandCustomerId).Distinct().Count().ShouldBe(1);
        results.Count(r => r.Created).ShouldBe(1);
        (await ScalarAsync<long>("SELECT count(*) FROM customers.customers")).ShouldBe(1);
    }

    [Fact]
    public async Task Counters_AreAppliedToTheCard()
    {
        var card = await InBrandAsync(BrandA, d => d.EnsureBrandCustomerAsync(new(BrandA, Selin, "Selin Öztürk", CustomerSource.Panel, false), Ct));

        await InBrandAsync(BrandA, d => d.ApplyCountersAsync(BrandA, card.BrandCustomerId, new CustomerCounterChange(Visits: 1, SpentMinor: 45_000, VisitedOn: new LocalDate(2026, 11, 25)), Ct));
        await InBrandAsync(BrandA, d => d.ApplyCountersAsync(BrandA, card.BrandCustomerId, new CustomerCounterChange(NoShows: 1), Ct));

        (await ScalarAsync<string>("SELECT visit_count || '/' || no_show_count || '/' || total_spent_minor || '/' || last_visit_on FROM customers.brand_customers"))
            .ShouldBe("1/1/45000/2026-11-25");
    }

    [Fact]
    public async Task Consent_IsAppendOnly_AndBrandScoped()
    {
        var card = await InBrandAsync(BrandA, d => d.EnsureBrandCustomerAsync(new(BrandA, Selin, "Selin Öztürk", CustomerSource.Online, true), Ct));
        var now = Instant.FromUtc(2026, 11, 25, 9, 0);

        await InBrandAsync(BrandA, async d =>
        {
            await d.RecordConsentAsync(new ConsentEntry(card.CustomerId, BrandA, ConsentType.PrivacyNoticeAcknowledged, true, "2026-11-01", ConsentChannel.Web, null, now), Ct);
            await d.RecordConsentAsync(new ConsentEntry(card.CustomerId, BrandA, ConsentType.MarketingSms, false, "2026-11-01", ConsentChannel.Web, null, now), Ct);
            return true;
        });

        (await CountInBrandAsync(BrandA, "consent_records")).ShouldBe(2);
        (await CountInBrandAsync(BrandB, "consent_records")).ShouldBe(0);
        (await CountInBrandAsync(BrandB, "brand_customers")).ShouldBe(0);

        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(BrandA), Ct);
        await session.BeginAsync(Ct);
        var context = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
        var error = await Should.ThrowAsync<PostgresException>(() => context.Database.ExecuteSqlRawAsync("UPDATE customers.consent_records SET is_granted = true", Ct));
        error.SqlState.ShouldBe(PostgresErrorCodes.InsufficientPrivilege);
        await session.RollbackAsync(Ct);
    }

    private async Task<T> InBrandAsync<T>(Guid brandId, Func<ICustomerDirectory, Task<T>> action)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        var result = await action(scope.ServiceProvider.GetRequiredService<ICustomerDirectory>());
        await session.CommitAsync(Ct);
        return result;
    }

    private async Task<long> CountInBrandAsync(Guid brandId, string table)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        await using var command = new NpgsqlCommand($"SELECT count(*) FROM customers.{table}", session.Connection, session.Transaction);
        var count = (long)(await command.ExecuteScalarAsync(Ct))!;
        await session.RollbackAsync(Ct);
        return count;
    }

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
