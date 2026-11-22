using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Notifications;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Features.PaymentNotices;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Tenancy;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Platform.IntegrationTests;

// Kural: alan-modeli.md § SubscriptionInvoice (boşluksuz numara, yıl başına kilitli sayaç satırı); § PaymentNotice "Eşleştirme
// sonucu"; durum-makinesi.md § 8 (PastDue/ReadOnly → Active), § 10 PaymentNotice; dilim-06 kabul kriteri 2; T-118.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class BillingIntegrationTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly Guid Owner = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
    private static readonly Guid Admin = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a9");
    private static readonly Guid BankTransaction = Guid.Parse("0193a8f0-0000-7000-8000-0000000000aa");

    private ServiceProvider _services = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ExecuteAsync("""
            TRUNCATE platform.payment_notices, platform.subscription_invoices, platform.invoice_number_counters, platform.subscriptions, platform.audit_logs;
            TRUNCATE notifications.credit_transactions;
            TRUNCATE infra.outbox_events CASCADE;
            TRUNCATE tenancy.brands CASCADE;
            UPDATE platform.plans SET monthly_price_minor = 100000, included_branch_count = 1, extra_branch_price_minor = 25000;
            """);
        var configuration = db.Configuration();
        _services = db.CreateServices(
            maxPoolSize: 20,
            configure: s => s.AddTenancyModule(configuration).AddNotificationsModule(configuration).AddPlatformModule(configuration));
    }

    public async ValueTask DisposeAsync()
    {
        await ExecuteAsync("UPDATE platform.plans SET monthly_price_minor = NULL, included_branch_count = NULL, extra_branch_price_minor = NULL");
        await _services.DisposeAsync();
    }

    [Fact]
    public async Task InvoiceNumbers_ConcurrentTransactions_AreGaplessAndUnique_RollbackDoesNotConsume()
    {
        const int year = 2031;
        const int parallel = 8;
        var numbers = await Task.WhenAll(Enumerable.Range(0, parallel).Select(_ => AllocateAsync(year, commit: true)));
        var rolledBack = await AllocateAsync(year, commit: false);
        var next = await AllocateAsync(year, commit: true);

        numbers.Order(StringComparer.Ordinal).ShouldBe(Enumerable.Range(1, parallel).Select(i => InvoiceNumbers.Format(year, i)));
        rolledBack.ShouldBe(InvoiceNumbers.Format(year, parallel + 1));
        next.ShouldBe(InvoiceNumbers.Format(year, parallel + 1), "geri alınan numara boşa gitmez");
    }

    [Fact]
    public async Task Match_SubscriptionPastDue_IssuesAndPaysInvoice_Activates_WritesAuditAndEvent()
    {
        var brandId = await NewBrandAsync("Kuaför Elif");
        await ExecuteAsync($"UPDATE platform.subscriptions SET status = 'PastDue', past_due_since = trial_ends_on, current_period_start = trial_ends_on, current_period_end = trial_ends_on + 30 WHERE brand_id = '{brandId}'");
        var noticeId = await NotifyAsync(brandId, credits: false);

        var settled = await InScopeAsync(null, sp => sp.GetRequiredService<IPaymentNoticeSettlement>().MatchAsync(noticeId, BankTransaction, Admin, Ct));

        settled.IsSuccess.ShouldBeTrue();
        settled.Value.InvoiceId.ShouldNotBeNull();
        (await ScalarAsync<string>($"SELECT status FROM platform.subscriptions WHERE brand_id = '{brandId}'")).ShouldBe("Active");
        (await ScalarAsync<string>($"SELECT status || ':' || paid_via || ':' || amount_minor || ':' || invoice_number FROM platform.subscription_invoices WHERE brand_id = '{brandId}'"))
            .ShouldMatch("^Paid:BankTransfer:100000:OMV-[0-9]{4}-000001$");
        (await ScalarAsync<string>($"SELECT status FROM platform.payment_notices WHERE id = '{noticeId}'")).ShouldBe("Matched");
        (await ScalarAsync<long>($"SELECT count(*) FROM platform.audit_logs WHERE brand_id = '{brandId}' AND action = 'Match' AND actor_type = 'PlatformUser' AND actor_id = '{Admin}'"))
            .ShouldBe(1);
        (await ScalarAsync<long>($"SELECT count(*) FROM infra.outbox_events WHERE type = '{typeof(PaymentNoticeMatched).FullName}' AND payload->>'brandId' = '{brandId}'"))
            .ShouldBe(1);
    }

    [Fact]
    public async Task Match_ReadOnly_Reactivates_AndAlreadyMatchedIsConflict()
    {
        var brandId = await NewBrandAsync("Berber Ali");
        await ExecuteAsync($"UPDATE platform.subscriptions SET status = 'ReadOnly', past_due_since = trial_ends_on, read_only_since = trial_ends_on + 7, current_period_start = trial_ends_on, current_period_end = trial_ends_on + 30 WHERE brand_id = '{brandId}'");
        var noticeId = await NotifyAsync(brandId, credits: false);
        var settlement = (IServiceProvider sp) => sp.GetRequiredService<IPaymentNoticeSettlement>().MatchAsync(noticeId, BankTransaction, Admin, Ct);

        (await InScopeAsync(null, settlement)).IsSuccess.ShouldBeTrue();
        var again = await InScopeAsync(null, settlement);

        (await ScalarAsync<string>($"SELECT status FROM platform.subscriptions WHERE brand_id = '{brandId}'")).ShouldBe("Active");
        (await ScalarAsync<object>($"SELECT read_only_since FROM platform.subscriptions WHERE brand_id = '{brandId}'")).ShouldBe(DBNull.Value);
        again.Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Fact]
    public async Task Match_Credits_WritesPurchase()
    {
        var brandId = await NewBrandAsync("Güzellik Sena");
        var noticeId = await NotifyAsync(brandId, credits: true);

        var settled = await InScopeAsync(null, sp => sp.GetRequiredService<IPaymentNoticeSettlement>().MatchAsync(noticeId, BankTransaction, Admin, Ct));

        settled.Value.CreditsAdded.ShouldBe(5000);
        (await ScalarAsync<long>($"SELECT count(*) FROM notifications.credit_transactions WHERE brand_id = '{brandId}' AND type = 'Purchase' AND amount = 5000 AND reference_id = '{noticeId}'"))
            .ShouldBe(1);
        (await ScalarAsync<string>($"SELECT status FROM platform.subscriptions WHERE brand_id = '{brandId}'")).ShouldBe("Trialing", "kontör aboneliği değiştirmez");
    }

    [Fact]
    public async Task Match_UnknownNotice_NotFound() =>
        (await InScopeAsync(null, sp => sp.GetRequiredService<IPaymentNoticeSettlement>().MatchAsync(Guid.Parse("0193a8f0-0000-7000-8000-0000000000ab"), BankTransaction, Admin, Ct)))
            .Error!.Code.ShouldBe(ErrorCodes.NotFound);

    [Fact]
    public async Task StartTrial_AssignsUniqueTransferCodesFromBrandName()
    {
        var first = await NewBrandAsync("Kuaför Elif");
        var second = await NewBrandAsync("Kuaför Elif");

        var codes = await ScalarAsync<string>($"SELECT string_agg(transfer_code || '/' || credit_transfer_code, ',' ORDER BY transfer_code) FROM platform.subscriptions WHERE brand_id IN ('{first}', '{second}')");
        var pairs = codes.Split(',');
        pairs.Length.ShouldBe(2);
        pairs.ShouldAllBe(p => p.StartsWith("ABN-KE-", StringComparison.Ordinal) && p.Contains("/KNT-KE-", StringComparison.Ordinal));
        pairs[0].ShouldNotBe(pairs[1], StringComparer.Ordinal);
    }

    private async Task<string> AllocateAsync(int year, bool commit)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(TenantScope.Platform, Ct);
        await session.BeginAsync(Ct);
        var number = await scope.ServiceProvider.GetRequiredService<InvoiceNumberAllocator>().NextAsync(year, Ct);
        await Task.Delay(20, Ct);
        if (commit)
        {
            await session.CommitAsync(Ct);
        }
        else
        {
            await session.RollbackAsync(Ct);
        }

        return number;
    }

    private Task<Guid> NewBrandAsync(string name) =>
        InScopeAsync(null, async sp =>
        {
            var tenant = await sp.GetRequiredService<ITenantProvisioning>().ProvisionBrandAsync(name, "hair_salon", Owner, null, Ct);
            await sp.GetRequiredService<IDbSession>().SetTenantAsync(TenantScope.ForBrand(tenant.BrandId), Ct);
            await sp.GetRequiredService<ISubscriptionProvisioning>().StartTrialAsync(tenant.BrandId, PlatformDefaults.DefaultTimeZoneId, Ct);
            return tenant.BrandId;
        });

    private Task<Guid> NotifyAsync(Guid brandId, bool credits) =>
        InScopeAsync(brandId, async sp =>
        {
            var platform = sp.GetRequiredService<PlatformDbContext>();
            var subscription = await platform.Subscriptions.AsNoTracking().SingleAsync(s => s.BrandId == brandId, Ct);
            var ids = sp.GetRequiredService<IIdGenerator>();
            var now = sp.GetRequiredService<NodaTime.IClock>().GetCurrentInstant();
            var notice = credits
                ? PaymentNotice.ForCredits(
                    ids.New(), subscription, await platform.CreditPackages.SingleAsync(p => p.Code == "medium", Ct), PaymentNoticeChannel.Panel, null, Owner, now)
                : PaymentNotice.ForSubscription(ids.New(), subscription, 100000, PaymentNoticeChannel.Panel, "Elif Kaya", Owner, now);
            platform.PaymentNotices.Add(notice);
            await platform.SaveChangesAsync(Ct);
            return notice.Id;
        });

    private async Task<T> InScopeAsync<T>(Guid? brandId, Func<IServiceProvider, Task<T>> work)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.SetTenantAsync(brandId is { } brand ? TenantScope.ForBrand(brand) : TenantScope.Platform, Ct);
        await session.BeginAsync(Ct);
        var result = await work(scope.ServiceProvider);
        await session.CommitAsync(Ct);
        return result;
    }

    private async Task ExecuteAsync(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
