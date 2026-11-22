using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;
using Omvila.Modules.Platform.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.AdminBrands;

/// <summary>
/// Platform paneli okuma modeli: Genel bakış, Markalar ve marka detayı (sözleşme adminGetOverview, adminListBrands, adminGetBrand,
/// adminListBrandCreditTransactions). Marka verisi diğer modüllerin platform okuma servislerinden (Contracts; ADR-0006) gelir ve
/// burada birleştirilir; telefon yalnız maskeli döner. "Bugün" ve "bu ay" platform saat dilimindedir (Europe/Istanbul).
/// </summary>
internal sealed class AdminBrandReader(
    PlatformDbContext db,
    IPlatformBrandDirectory brands,
    IPlatformCredits credits,
    IPlatformAppointmentStats appointments,
    IPlatformCatalogStats catalog,
    IPlatformCustomerStats customers,
    IBrandOwnerDirectory owners,
    IPendingApprovalCounts approvals,
    Integrations.Contracts.IPlatformCalendarLinks calendars,
    IClock clock,
    IOptions<PlatformOptions> options)
{
    /// <summary>Sözleşme Limit varsayılanı.</summary>
    public const int DefaultLimit = 50;

    // Kural: sözleşme adminGetOverview ve T-116 (7) — gösterim pencereleri (ayar değil, ekran tanımı).
    private const int EndingThisWeekDays = 7;
    private const int ReadOnlySoonDays = 2;
    private const int AttentionTrialDays = 3;
    private const int AttentionLimit = 10;
    private const int FunnelWindowDays = 30;
    private const int RecentCreditDays = 30;
    private const int RecentCreditLimit = 10;
    private const string CreditTransactionEntity = "CreditTransaction";

    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");
    private static readonly StringComparer NameOrder = StringComparer.Create(Turkish, ignoreCase: true);

    private static readonly SubscriptionStatus[] LiveStatuses = [SubscriptionStatus.Trialing, SubscriptionStatus.Active, SubscriptionStatus.PastDue];

    public async Task<AdminOverview> GetOverviewAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var today = Today(now);
        var facts = await LoadAsync(null, ct);
        var active = facts.Where(IsActiveBrand).ToList();
        var monthStart = MonthStart(today);
        var pending = await approvals.GetAsync(ct);

        var trialing = facts.Where(f => f.Subscription.Status == SubscriptionStatus.Trialing).ToList();
        var pastDue = facts.Where(f => f.Subscription.Status == SubscriptionStatus.PastDue).ToList();

        var recent = facts.Where(f => f.Brand.CreatedUtc >= now - Duration.FromDays(FunnelWindowDays)).Select(f => f.Brand).ToList();
        var recentIds = recent.Select(b => b.Id).ToList();
        var withServices = recentIds.Count == 0 ? new HashSet<Guid>() : await catalog.GetBrandsWithServicesAsync(recentIds, ct);
        var withOnline = recentIds.Count == 0 ? new HashSet<Guid>() : await appointments.GetBrandsWithOnlineAppointmentAsync(recentIds, ct);

        return new AdminOverview(
            now,
            new AdminOverview.ActiveBrandsCard(active.Count, active.Count(f => f.Brand.CreatedUtc >= monthStart)),
            new AdminOverview.PendingApprovalsCard(pending.PaymentNotices + pending.SmsSenders, pending.PaymentNotices, pending.SmsSenders),
            new AdminOverview.TrialingCard(trialing.Count, trialing.Count(f => f.TrialDaysLeft is { } left && left <= EndingThisWeekDays)),
            new AdminOverview.PastDueCard(pastDue.Count, pastDue.Count(f => f.ReadOnlyOn is { } on && Period.Between(today, on, PeriodUnits.Days).Days <= ReadOnlySoonDays)),
            new AdminOverview.FunnelCard(
                FunnelWindowDays,
                recent.Count,
                recent.Count(b => withServices.Contains(b.Id)),
                recent.Count(b => b.HasWorkingHours),
                recent.Count(b => withOnline.Contains(b.Id))),
            Attention(facts));
    }

    public async Task<Result<AdminBrandPage>> ListAsync(string? filter, string? q, int? limit, string? cursor, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var predicate = FilterPredicate(filter);
        if (predicate is null)
        {
            errors["filter"] = ["invalid_format"];
        }

        var search = q?.Trim();
        if (search is not null && search.Length is < 2 or > 100)
        {
            errors["q"] = ["out_of_range"];
        }

        if (limit is < 1 or > 100)
        {
            errors["limit"] = ["out_of_range"];
        }

        (string Name, Guid Id)? after = null;
        if (cursor is not null && !TryDecodeNameCursor(cursor, out after))
        {
            errors["cursor"] = ["invalid_format"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var facts = await LoadAsync(null, ct);
        var counts = new AdminBrandPage.BrandCounts(facts.Count(IsActiveBrand), facts.Count(f => f.Subscription.Status == SubscriptionStatus.Trialing));
        var rows = facts.Where(predicate!);
        if (!string.IsNullOrEmpty(search))
        {
            var ownerPhones = await owners.GetOwnersAsync(null, ct);
            var digits = search.Length == 4 && search.All(char.IsAsciiDigit);
            rows = rows.Where(f =>
                Turkish.CompareInfo.IndexOf(f.Brand.Name, search, CompareOptions.IgnoreCase) >= 0
                || f.Brand.Slug.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (digits && ownerPhones.TryGetValue(f.Brand.Id, out var owner) && owner.PhoneE164.EndsWith(search, StringComparison.Ordinal)));
        }

        var ordered = rows.OrderBy(f => f.Brand.Name, NameOrder).ThenBy(f => f.Brand.Id).ToList();
        if (after is { } a)
        {
            ordered = [.. ordered.SkipWhile(f => Compare(f, a) <= 0)];
        }

        var take = limit ?? DefaultLimit;
        var page = ordered.Take(take).ToList();
        var next = ordered.Count > take ? EncodeNameCursor(page[^1].Brand.Name, page[^1].Brand.Id) : null;
        return new AdminBrandPage(counts, [.. page.Select(ListItem)], next);
    }

    /// <summary>Marka detayı ve sürümü (marka + abonelik satırı); marka veya abonelik yoksa null.</summary>
    public async Task<(AdminBrandDetail Detail, uint Version)?> GetDetailAsync(Guid brandId, CancellationToken ct)
    {
        var profile = await brands.GetAsync(brandId, ct);
        if (profile is null)
        {
            return null;
        }

        var facts = (await LoadAsync([profile.Brand], ct)).SingleOrDefault();
        if (facts is null)
        {
            return null;
        }

        var now = clock.GetCurrentInstant();
        var owner = (await owners.GetOwnersAsync([brandId], ct)).GetValueOrDefault(brandId);
        var sender = await credits.GetSmsSenderAsync(brandId, ct);
        var consumed = await credits.GetConsumedSinceAsync(brandId, now - Duration.FromDays(RecentCreditDays), ct);
        var balance = Math.Max(0, facts.CreditBalance);
        var recent = await CreditRowsAsync(brandId, RecentCreditLimit, null, now - Duration.FromDays(RecentCreditDays), ct);
        var subscription = facts.Subscription;
        var version = unchecked(profile.Brand.Version + await SubscriptionVersionAsync(brandId, ct));

        var detail = new AdminBrandDetail(
            profile.Brand.Id,
            profile.Brand.Name,
            profile.Brand.Slug,
            profile.Brand.IndustryCode,
            profile.Brand.City,
            profile.Brand.Status,
            profile.Brand.CreatedUtc,
            profile.BookingUrl,
            new AdminBrandDetail.OwnerInfo(owner?.FullName ?? string.Empty, Mask(owner?.PhoneE164)),
            [.. profile.Branches.Select(b => new AdminBrandDetail.BranchRef(b.Id, b.Name))],
            profile.Brand.ActiveStaffCount,
            await customers.CountBrandCustomersAsync(brandId, ct),
            await calendars.CountConnectedStaffAsync(brandId, ct),
            new AdminBrandDetail.SmsSenderInfo(sender.Name, sender.Status, sender.EffectiveName, sender.SubmittedUtc, sender.DecidedUtc, sender.RejectionReason),
            new AdminBrandDetail.SubscriptionCard(
                subscription.Status.ToString(),
                facts.Plan.Code,
                facts.Plan.Name,
                subscription.BillingCycle.ToString(),
                // Ödeme yolu, ABN- havale kodu ve bekleyen bildirim abonelik/faturalama PR'ında gelir (T-119).
                PaymentMethod: null,
                subscription.TrialEndsOn,
                facts.TrialDaysLeft,
                subscription.PastDueSince,
                facts.ReadOnlyOn,
                TransferCode: string.Empty,
                PendingNoticeId: null,
                PendingNoticeNotifiedUtc: null),
            new AdminBrandDetail.CreditsCard(balance, consumed > 0 ? (int)(balance * (long)RecentCreditDays / consumed) : null),
            new AdminBrandDetail.AppointmentsCard(
                facts.Month.Total,
                facts.Month.Total == 0 ? null : Math.Round(facts.Month.Online * 100.0 / facts.Month.Total, 0, MidpointRounding.AwayFromZero)),
            [.. recent.Take(RecentCreditLimit)],
            ETagVersion.Format(version));
        return (detail, version);
    }

    /// <summary>"Kontör" sekmesi (yeniden eskiye; gönderimler gün başına toplanmış).</summary>
    public async Task<Result<AdminPage<AdminCreditTransaction>>> CreditPageAsync(Guid brandId, int? limit, string? cursor, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (limit is < 1 or > 100)
        {
            errors["limit"] = ["out_of_range"];
        }

        PlatformCreditCursor? before = null;
        if (cursor is not null)
        {
            if (TryDecodeCreditCursor(cursor, out var decoded))
            {
                before = decoded;
            }
            else
            {
                errors["cursor"] = ["invalid_format"];
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var take = limit ?? DefaultLimit;
        var rows = await CreditRowsAsync(brandId, take, before, null, ct);
        var page = rows.Take(take).ToList();
        var next = rows.Count > take ? EncodeCreditCursor(page[^1].OccurredUtc, page[^1].Id) : null;
        return new AdminPage<AdminCreditTransaction>(page, next);
    }

    /// <summary>Platform eklemesi satırı (adminGrantCredits yanıtı; not = neden).</summary>
    public static AdminCreditTransaction GrantRow(PlatformCreditEntry entry, string reason) =>
        new(entry.Id, entry.OccurredUtc, entry.Type, entry.Credits, entry.BalanceAfter, reason);

    /// <summary>Marka satırı + abonelik + plan + kontör + bu ay randevu; aboneliği olmayan marka (olmamalı) dışarıda kalır.</summary>
    private async Task<List<BrandFacts>> LoadAsync(IReadOnlyList<PlatformBrandRow>? only, CancellationToken ct)
    {
        var rows = only ?? await brands.ListAsync(ct);
        var ids = rows.Select(r => r.Id).ToList();
        var subscriptions = await (from s in db.Subscriptions.AsNoTracking()
                                   join p in db.Plans.AsNoTracking() on s.PlanId equals p.Id
                                   where ids.Contains(s.BrandId)
                                   select new { Subscription = s, Plan = p }).ToDictionaryAsync(x => x.Subscription.BrandId, ct);
        var balances = await credits.GetBalancesAsync(ct);
        var now = clock.GetCurrentInstant();
        var today = Today(now);
        var monthStart = MonthStart(today);
        var monthEnd = MonthStart(today.PlusMonths(1));
        var months = await appointments.CountAsync(monthStart, monthEnd, only is null ? null : ids, ct);
        var grace = options.Value.PastDueGraceDays;
        var result = new List<BrandFacts>(rows.Count);
        foreach (var row in rows)
        {
            if (!subscriptions.TryGetValue(row.Id, out var sub))
            {
                continue;
            }

            var s = sub.Subscription;
            int? trialLeft = s.Status == SubscriptionStatus.Trialing && s.TrialEndsOn is { } ends
                ? Math.Max(0, Period.Between(today, ends, PeriodUnits.Days).Days)
                : null;
            int? pastDueDays = s.Status == SubscriptionStatus.PastDue && s.PastDueSince is { } since
                ? Math.Max(0, Period.Between(since, today, PeriodUnits.Days).Days)
                : null;
            LocalDate? readOnlyOn = s.Status == SubscriptionStatus.PastDue && s.PastDueSince is { } from ? from.PlusDays(grace) : null;
            result.Add(new BrandFacts(
                row, s, sub.Plan, balances.GetValueOrDefault(row.Id), months.GetValueOrDefault(row.Id) ?? new PlatformAppointmentCount(0, 0),
                trialLeft, pastDueDays, readOnlyOn));
        }

        return result;
    }

    private async Task<List<AdminCreditTransaction>> CreditRowsAsync(Guid brandId, int limit, PlatformCreditCursor? before, Instant? since, CancellationToken ct)
    {
        var entries = await credits.ListAsync(brandId, limit, before, since, ct);
        var grantIds = entries.Where(e => string.Equals(e.Type, "Grant", StringComparison.Ordinal)).Select(e => (Guid?)e.Id).ToList();
        // Platform eklemesinin nedeni denetim kaydında (adminGrantCredits: Create, CreditTransaction, changes.reason).
        var notes = grantIds.Count == 0
            ? []
            : (await db.AuditLogs.AsNoTracking()
                .Where(l => l.EntityType == CreditTransactionEntity && l.Action == AuditAction.Create && grantIds.Contains(l.EntityId))
                .Select(l => new { l.EntityId, l.Changes })
                .ToListAsync(ct))
            .Where(l => l.EntityId is not null)
            .ToDictionary(l => l.EntityId!.Value, l => ReadReason(l.Changes));
        return [.. entries.Select(e => new AdminCreditTransaction(e.Id, e.OccurredUtc, e.Type, e.Credits, e.BalanceAfter, notes.GetValueOrDefault(e.Id)))];
    }

    private async Task<uint> SubscriptionVersionAsync(Guid brandId, CancellationToken ct) =>
        (uint)await db.Database
            .SqlQuery<long>($"""SELECT xmin::text::bigint AS "Value" FROM platform.subscriptions WHERE brand_id = {brandId}""")
            .SingleAsync(ct);

    private static List<AdminOverview.AttentionBrand> Attention(List<BrandFacts> facts)
    {
        var candidates = new List<(int Rank, int Key, BrandFacts Facts, string Reason)>();
        foreach (var f in facts.Where(f => string.Equals(f.Brand.Status, "Active", StringComparison.Ordinal) || string.Equals(f.Brand.Status, "Onboarding", StringComparison.Ordinal)))
        {
            if (f.PastDueDays is { } days)
            {
                candidates.Add((0, -days, f, "PastDue"));
            }
            else if (f.TrialDaysLeft is { } left && left <= AttentionTrialDays)
            {
                candidates.Add((1, left, f, "TrialEnding"));
            }
            else if (f.CreditBalance <= 0 && LiveStatuses.Contains(f.Subscription.Status))
            {
                candidates.Add((2, 0, f, "CreditsDepleted"));
            }
        }

        return [.. candidates
            .OrderBy(c => c.Rank).ThenBy(c => c.Key).ThenBy(c => c.Facts.Brand.Name, NameOrder)
            .Take(AttentionLimit)
            .Select(c => new AdminOverview.AttentionBrand(
                c.Facts.Brand.Id, c.Facts.Brand.Name, c.Facts.Brand.IndustryCode, c.Facts.Brand.City, c.Facts.Plan.Name, c.Reason,
                c.Facts.PastDueDays, c.Facts.TrialDaysLeft, c.Facts.Brand.BranchCount, c.Facts.Month.Total))];
    }

    // Kural: sözleşme adminListBrands filter — Tümü, Denemede, Ödeme gecikti, Salt okunur, Kapora açık (etkin IBAN hesabı).
    private static Func<BrandFacts, bool>? FilterPredicate(string? filter) => (filter ?? "All") switch
    {
        "All" => _ => true,
        "Trialing" => f => f.Subscription.Status == SubscriptionStatus.Trialing,
        "PastDue" => f => f.Subscription.Status == SubscriptionStatus.PastDue,
        "ReadOnly" => f => f.Subscription.Status == SubscriptionStatus.ReadOnly,
        "DepositEnabled" => f => f.Brand.DepositEnabled,
        _ => null,
    };

    private static AdminBrandPage.Item ListItem(BrandFacts f) => new(
        f.Brand.Id, f.Brand.Name, f.Brand.IndustryCode, f.Brand.City, f.Brand.Status, f.Plan.Code, f.Plan.Name, f.Subscription.Status.ToString(),
        f.TrialDaysLeft, f.PastDueDays, f.Brand.BranchCount, f.Month.Total, Math.Max(0, f.CreditBalance), f.Brand.DepositEnabled);

    // Kural: sözleşme adminGetOverview — aktif marka = Brand Active ve abonelik Trialing, Active veya PastDue.
    private static bool IsActiveBrand(BrandFacts f) =>
        string.Equals(f.Brand.Status, "Active", StringComparison.Ordinal) && LiveStatuses.Contains(f.Subscription.Status);

    private static int Compare(BrandFacts f, (string Name, Guid Id) after)
    {
        var byName = NameOrder.Compare(f.Brand.Name, after.Name);
        return byName != 0 ? byName : f.Brand.Id.CompareTo(after.Id);
    }

    // Platform telefonları her zaman maskelidir (roller-yetkiler.md § Platform rolleri).
    private static string Mask(string? phoneE164) =>
        phoneE164 is not null && PhoneNumber.TryParse(phoneE164, out var phone) ? phone.Masked : string.Empty;

    private static string? ReadReason(string? changes)
    {
        if (changes is null)
        {
            return null;
        }

        using var json = JsonDocument.Parse(changes);
        return json.RootElement.TryGetProperty("reason", out var reason) && reason.ValueKind == JsonValueKind.String ? reason.GetString() : null;
    }

    private static LocalDate Today(Instant now) =>
        now.InZone(DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId]).Date;

    private static Instant MonthStart(LocalDate day) =>
        new LocalDate(day.Year, day.Month, 1).AtStartOfDayInZone(DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId]).ToInstant();

    private static string EncodeNameCursor(string name, Guid id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{id:N}|{name}"));

    private static bool TryDecodeNameCursor(string cursor, out (string Name, Guid Id)? value)
    {
        value = null;
        try
        {
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(cursor)).Split('|', 2);
            if (parts.Length == 2 && Guid.TryParseExact(parts[0], "N", out var id))
            {
                value = (parts[1], id);
                return true;
            }
        }
        catch (FormatException)
        {
        }

        return false;
    }

    private static string EncodeCreditCursor(Instant occurredUtc, Guid id) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(
            $"{occurredUtc.ToUnixTimeTicks().ToString(CultureInfo.InvariantCulture)}|{id:N}"));

    private static bool TryDecodeCreditCursor(string cursor, out PlatformCreditCursor? value)
    {
        value = null;
        try
        {
            var parts = Encoding.UTF8.GetString(Convert.FromBase64String(cursor)).Split('|');
            if (parts.Length == 2 && long.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var ticks)
                && Guid.TryParseExact(parts[1], "N", out var id))
            {
                value = new PlatformCreditCursor(Instant.FromUnixTimeTicks(ticks), id);
                return true;
            }
        }
        catch (FormatException)
        {
        }

        return false;
    }

    private sealed record BrandFacts(
        PlatformBrandRow Brand,
        Subscription Subscription,
        Plan Plan,
        int CreditBalance,
        PlatformAppointmentCount Month,
        int? TrialDaysLeft,
        int? PastDueDays,
        LocalDate? ReadOnlyOn);
}
