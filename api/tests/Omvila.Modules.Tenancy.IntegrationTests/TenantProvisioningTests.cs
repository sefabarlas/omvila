using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Tenancy.IntegrationTests;

// Kural: dilim-01 § Alan kuralları (kayıtta marka Onboarding, "Merkez" şubesi Europe/Istanbul, sahip personel,
// bağlantı adı adından, alınmışsa -2), sureler-ve-ayarlar.md varsayılanları, ADR-0006 (RLS), T-11 (yönlendirmesi
// süren eski ad başkasına verilmez).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class TenantProvisioningTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid OwnerA = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
    private static readonly Guid OwnerB = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");

    private ServiceProvider _services = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ExecuteAsMigratorAsync("TRUNCATE tenancy.brands CASCADE");
        _services = db.CreateServices(configure: services => services.AddTenancyModule());
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task Provision_CreatesBrandInOnboarding_WithFirstBranchOwnerStaffAndDefaults()
    {
        var tenant = await ProvisionAsync("Kuaför Elif", OwnerA, "Elif Kaya");

        tenant.Slug.ShouldBe("kuafor-elif");
        await InBrandAsync(tenant.BrandId, async ctx =>
        {
            var brand = await ctx.Brands.SingleAsync(Ct);
            brand.Name.ShouldBe("Kuaför Elif");
            brand.Status.ShouldBe(BrandStatus.Onboarding);
            brand.IndustryCode.ShouldBe("hair_salon");
            brand.DefaultCurrencyCode.ShouldBe("TRY");
            brand.Version.ShouldNotBe(0u);

            var branch = await ctx.Branches.SingleAsync(Ct);
            branch.Id.ShouldBe(tenant.BranchId);
            branch.Name.ShouldBe("Merkez");
            branch.TimeZoneId.ShouldBe("Europe/Istanbul");
            branch.IsActive.ShouldBeTrue();

            var staff = await ctx.Staff.SingleAsync(Ct);
            staff.Id.ShouldBe(tenant.OwnerStaffId);
            staff.UserId.ShouldBe(OwnerA);
            staff.FullName.ShouldBe("Elif Kaya");
            staff.IsBookableOnline.ShouldBeTrue();

            var link = await ctx.StaffBranches.SingleAsync(Ct);
            (link.StaffId, link.BranchId, link.IsPrimary).ShouldBe((staff.Id, branch.Id, true));

            var settings = await ctx.BrandSettings.SingleAsync(Ct);
            settings.RequiresApproval.ShouldBeFalse();
            settings.ApprovalTimeoutMinutes.ShouldBe(120);
            settings.CancellationCutoffHours.ShouldBe(24);
            settings.SlotIntervalMinutes.ShouldBe(15);
            settings.DepositPolicy.ShouldBe("RiskyCustomersOnly");
            settings.ReminderOffsetsMinutes.ShouldBe([1440, 120]);
            settings.MaskCustomerPhoneFromStaff.ShouldBeTrue();

            var page = await ctx.BookingPageSettings.SingleAsync(Ct);
            (page.CoverColorToken, page.ShowPrices, page.ShowReviews).ShouldBe(("petrol", true, true));
        });
    }

    [Fact]
    public async Task Provision_WithoutOwnerName_UsesBusinessNameForOwnerStaff()
    {
        var tenant = await ProvisionAsync("Berber Ali", OwnerA, ownerFullName: null);

        await InBrandAsync(tenant.BrandId, async ctx => (await ctx.Staff.SingleAsync(Ct)).FullName.ShouldBe("Berber Ali"));
    }

    [Fact]
    public async Task Provision_SameNameTwice_SecondGetsSuffix()
    {
        (await ProvisionAsync("Kuaför Elif", OwnerA, null)).Slug.ShouldBe("kuafor-elif");
        (await ProvisionAsync("Kuafor Elif", OwnerB, null)).Slug.ShouldBe("kuafor-elif-2");
    }

    [Fact]
    public async Task Provision_ConcurrentSameName_NeverSharesSlug()
    {
        var results = await Task.WhenAll(
            Enumerable.Range(0, 4).Select(i => ProvisionAsync("Aynı Ad", NewId(), null)));

        results.Select(r => r.Slug).Order(StringComparer.Ordinal)
            .ShouldBe(["ayni-ad", "ayni-ad-2", "ayni-ad-3", "ayni-ad-4"]);
    }

    [Fact]
    public async Task Provision_SlugStillRedirectingForAnotherBrand_IsSkipped_ExpiredIsReused()
    {
        var other = await ProvisionAsync("Eski Ad", OwnerA, null);
        var now = TestClock.Start;
        await ExecuteAsMigratorAsync(
            $"""
            UPDATE tenancy.brands SET slug = 'yeni-ad' WHERE id = '{other.BrandId}';
            INSERT INTO tenancy.brand_slug_history (id, brand_id, slug, changed_utc, redirect_expires_utc)
            VALUES ('{NewId()}', '{other.BrandId}', 'eski-ad', '{(now - Duration.FromDays(1)).ToDateTimeUtc():O}',
                    '{(now + Duration.FromDays(29)).ToDateTimeUtc():O}');
            INSERT INTO tenancy.brand_slug_history (id, brand_id, slug, changed_utc, redirect_expires_utc)
            VALUES ('{NewId()}', '{other.BrandId}', 'kapali-ad', '{(now - Duration.FromDays(40)).ToDateTimeUtc():O}',
                    '{(now - Duration.FromDays(10)).ToDateTimeUtc():O}');
            """);

        (await ProvisionAsync("Eski Ad", OwnerB, null)).Slug.ShouldBe("eski-ad-2");
        (await ProvisionAsync("Kapalı Ad", NewId(), null)).Slug.ShouldBe("kapali-ad");
    }

    [Theory]
    [InlineData("brands")]
    [InlineData("brand_settings")]
    [InlineData("booking_page_settings")]
    [InlineData("branches")]
    [InlineData("staff")]
    [InlineData("staff_branches")]
    public async Task BrandScope_SeesOnlyItsOwnRows(string table)
    {
        var a = await ProvisionAsync("Salon A", OwnerA, null);
        await ProvisionAsync("Salon B", OwnerB, null);

        (await CountAsync(TenantScope.ForBrand(a.BrandId), table)).ShouldBe(1);
        (await CountAsync(TenantScope.None, table)).ShouldBe(0);
        (await CountAsync(TenantScope.Platform, table)).ShouldBe(2);
    }

    [Fact]
    public async Task BrandScope_CannotInsertRowForAnotherBrand()
    {
        var a = await ProvisionAsync("Salon A", OwnerA, null);
        var b = await ProvisionAsync("Salon B", OwnerB, null);

        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(a.BrandId), Ct);
        await session.BeginAsync(Ct);
        await using var command = new NpgsqlCommand(
            $"""
            INSERT INTO tenancy.branches (id, brand_id, name, slug, address_line, district, city, country_code, time_zone_id, is_active, created_utc)
            VALUES ('{NewId()}', '{b.BrandId}', 'Sızma', 'sizma', '', '', '', 'TR', 'Europe/Istanbul', true, now())
            """,
            session.Connection,
            session.Transaction);

        var error = await Should.ThrowAsync<PostgresException>(() => command.ExecuteNonQueryAsync(Ct));
        error.SqlState.ShouldBe(PostgresErrorCodes.InsufficientPrivilege);
        await session.RollbackAsync(Ct);
    }

    private Guid NewId() => _services.GetRequiredService<IIdGenerator>().New();

    private async Task<ProvisionedTenant> ProvisionAsync(string businessName, Guid owner, string? ownerFullName)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(Ct);
        var tenant = await scope.ServiceProvider.GetRequiredService<ITenantProvisioning>()
            .ProvisionBrandAsync(businessName, "hair_salon", owner, ownerFullName, Ct);
        session.Scope.ShouldBe(TenantScope.ForBrand(tenant.BrandId));
        await session.CommitAsync(Ct);
        return tenant;
    }

    private async Task InBrandAsync(Guid brandId, Func<TenancyDbContext, Task> assert)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), Ct);
        await session.BeginAsync(Ct);
        await assert(scope.ServiceProvider.GetRequiredService<TenancyDbContext>());
        await session.RollbackAsync(Ct);
    }

    private async Task<long> CountAsync(TenantScope tenant, string table)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(tenant, Ct);
        await session.BeginAsync(Ct);
        await using var command = new NpgsqlCommand($"SELECT count(*) FROM tenancy.{table}", session.Connection, session.Transaction);
        var count = (long)(await command.ExecuteScalarAsync(Ct))!;
        await session.RollbackAsync(Ct);
        return count;
    }

    private async Task ExecuteAsMigratorAsync(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }
}
