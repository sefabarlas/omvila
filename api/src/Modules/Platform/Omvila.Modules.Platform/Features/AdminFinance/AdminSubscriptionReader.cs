using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;
using static Omvila.Modules.Platform.Features.AdminFinance.AdminFinanceViews;

namespace Omvila.Modules.Platform.Features.AdminFinance;

/// <summary>
/// Abonelikler tablosu (sözleşme adminListSubscriptions; PAbonelikler). Sıralama aciliyete göre: Ödeme gecikti (en uzun gecikme
/// önce), Salt okunur, Denemede (bitişi yakın önce), Aktif, İptal; sonra marka adı. Sayfalama sıralı listede konumla (imleç sıra
/// numarası); platform ölçeğinde marka sayısı küçüktür. Günler OMVILA'nın faturalama takviminde (Europe/Istanbul).
/// </summary>
internal sealed class AdminSubscriptionReader(
    PlatformDbContext db, IPlatformBrandDirectory brands, IPlatformCredits credits, IClock clock, IOptions<PlatformOptions> options)
{
    public const int DefaultLimit = 50;

    // Kural: sözleşme adminListSubscriptions — "9'u bu hafta bitiyor" (7 gün; AdminBrandReader ile aynı pencere).
    private const int EndingThisWeekDays = 7;

    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");
    private static readonly StringComparer NameOrder = StringComparer.Create(Turkish, ignoreCase: true);

    public async Task<Result<AdminSubscriptionPage>> ListAsync(string? filter, string? q, int? limit, string? cursor, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var predicate = Filter(filter);
        if (predicate is null)
        {
            errors["filter"] = ["invalid_format"];
        }

        var search = q?.Trim();
        if (search is { Length: < 2 or > 100 })
        {
            errors["q"] = ["out_of_range"];
        }

        if (limit is < 1 or > 100)
        {
            errors["limit"] = ["out_of_range"];
        }

        var offset = 0;
        if (cursor is not null && (!int.TryParse(cursor, NumberStyles.None, CultureInfo.InvariantCulture, out offset) || offset < 1))
        {
            errors["cursor"] = ["invalid_format"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var rows = await RowsAsync(ct);
        var counts = new SubscriptionCounts(
            rows.Count(r => r.Status == SubscriptionStatus.Active),
            rows.Count(r => r.Status == SubscriptionStatus.Trialing),
            rows.Count(r => r.TrialDaysLeft is <= EndingThisWeekDays),
            rows.Count(r => r.Status == SubscriptionStatus.PastDue),
            rows.Count(r => r.Status == SubscriptionStatus.ReadOnly));
        var filtered = rows.Where(predicate!);
        if (!string.IsNullOrEmpty(search))
        {
            filtered = filtered.Where(r => Turkish.CompareInfo.IndexOf(r.BrandName, search, CompareOptions.IgnoreCase) >= 0);
        }

        var ordered = filtered
            .OrderBy(r => Urgency(r.Status))
            .ThenByDescending(r => r.PastDueDays ?? 0)
            .ThenBy(r => r.TrialDaysLeft ?? int.MaxValue)
            .ThenBy(r => r.BrandName, NameOrder)
            .ThenBy(r => r.BrandId)
            .ToList();
        var take = limit ?? DefaultLimit;
        var page = ordered.Skip(offset).Take(take).ToList();
        var next = ordered.Count > offset + take ? (offset + take).ToString(CultureInfo.InvariantCulture) : null;
        return new AdminSubscriptionPage(counts, page, next);
    }

    private async Task<List<AdminSubscriptionListItem>> RowsAsync(CancellationToken ct)
    {
        var settings = options.Value;
        var names = (await brands.ListAsync(ct)).ToDictionary(b => b.Id, b => b.Name);
        var balances = await credits.GetBalancesAsync(ct);
        var today = clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId]).Date;
        var subscriptions = await (from s in db.Subscriptions.AsNoTracking()
                                   join p in db.Plans.AsNoTracking() on s.PlanId equals p.Id
                                   select new { Subscription = s, PlanName = p.Name }).ToListAsync(ct);
        return [.. subscriptions
            .Where(x => names.ContainsKey(x.Subscription.BrandId))
            .Select(x =>
            {
                var s = x.Subscription;
                var balance = Math.Max(0, balances.GetValueOrDefault(s.BrandId));
                int? trialLeft = s.Status == SubscriptionStatus.Trialing && s.TrialEndsOn is { } ends ? Math.Max(0, Period.DaysBetween(today, ends)) : null;
                int? pastDueDays = s.Status == SubscriptionStatus.PastDue && s.PastDueSince is { } since ? Math.Max(0, Period.DaysBetween(since, today)) : null;
                var step = SubscriptionNextStep.For(s, balance, settings.PastDueGraceDays, settings.CancelledArchiveDays);
                return new AdminSubscriptionListItem(
                    s.BrandId, names[s.BrandId], x.PlanName, s.Status, pastDueDays, trialLeft, balance, SubscriptionNextStep.IsCreditsLow(balance),
                    new NextStepView(step.Type, step.Date));
            })];
    }

    // Kural: sözleşme adminListSubscriptions filter — Tümü, Denemede, Aktif, Ödeme gecikti, Salt okunur, İptal edildi, Kontörü azalan.
    private static Func<AdminSubscriptionListItem, bool>? Filter(string? filter) => (filter ?? "All") switch
    {
        "All" => _ => true,
        "Trialing" => r => r.Status == SubscriptionStatus.Trialing,
        "Active" => r => r.Status == SubscriptionStatus.Active,
        "PastDue" => r => r.Status == SubscriptionStatus.PastDue,
        "ReadOnly" => r => r.Status == SubscriptionStatus.ReadOnly,
        "Cancelled" => r => r.Status == SubscriptionStatus.Cancelled,
        "CreditsLow" => r => r.CreditsLow,
        _ => null,
    };

    private static int Urgency(SubscriptionStatus status) => status switch
    {
        SubscriptionStatus.PastDue => 0,
        SubscriptionStatus.ReadOnly => 1,
        SubscriptionStatus.Trialing => 2,
        SubscriptionStatus.Active => 3,
        _ => 4,
    };
}
