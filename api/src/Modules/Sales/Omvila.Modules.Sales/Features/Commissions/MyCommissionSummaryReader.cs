using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Commissions;

/// <summary>
/// Kişisel prim özeti (sözleşme manageGetMyCommissionSummary; K2, KK10). Yalnız oturumdaki kullanıcının personel kaydı; rol fark
/// etmez. Ay ve günler markanın ana şubesinin saat dilimindedir (ay = PeriodYearMonth, gün = kaydın oluştuğu anın yerel günü).
/// Toplam = ayın bütün kayıtları (ters kayıtlar eksi); randevu sayısı ve gelmedi oranı ayda başlayan randevulardan (Scheduling),
/// puan son <see cref="PlatformDefaults.ReviewOverviewWindowDays"/> günün değerlendirmelerinden (Reviews), müşteri adları
/// Customers'tan toplu okunur.
/// </summary>
internal sealed class MyCommissionSummaryReader(
    SalesDbContext db,
    ITenantQueries tenants,
    IAppointmentQueries appointments,
    ICustomerDirectory customers,
    IReviewRatings ratings,
    IClock clock)
{
    private const string Completed = "Completed";
    private const string NoShow = "NoShow";
    private const string CheckedIn = "CheckedIn";
    private static readonly YearMonthPattern MonthPattern = YearMonthPattern.CreateWithInvariantCulture("uuuu'-'MM");

    // Kural: sözleşme MyCommissionSummary; roller-yetkiler.md commission.view (K: "Primim", kendi prim kaydı)
    public async Task<Result<MyCommissionSummary>> GetAsync(Guid brandId, Guid? staffId, string? month, CancellationToken ct)
    {
        var parsed = month is null ? null : MonthPattern.Parse(month);
        if (parsed is { Success: false })
        {
            return Error.Validation("month", "invalid_format");
        }

        if (staffId is not { } id || !(await tenants.GetStaffCardsAsync(brandId, [id], ct)).TryGetValue(id, out var staff))
        {
            return Error.NotFound();
        }

        var settings = await tenants.GetCustomerSettingsAsync(brandId, ct) ?? throw new DomainException("Marka ayarları bulunamadı.");
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(settings.TimeZoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var now = clock.GetCurrentInstant();
        var window = Window.For(zone, now.InZone(zone).Date, parsed?.Value);

        var entries = await CommissionQueries.MonthEntries(db, brandId, id, window.Month.Year, window.Month.Month)
            .OrderByDescending(e => e.CreatedUtc).ThenByDescending(e => e.Id)
            .ToListAsync(ct);
        var weekEntries = await db.CommissionEntries.AsNoTracking()
            .Where(e => e.BrandId == brandId && e.StaffId == id && e.CreatedUtc >= window.WeekStartUtc && e.CreatedUtc < window.TomorrowUtc)
            .ToListAsync(ct);
        var outcomes = await appointments.ListStaffOutcomesAsync(
            brandId, id, Instant.Min(window.MonthStartUtc, window.WeekStartUtc), Instant.Max(window.MonthEndUtc, window.TomorrowUtc), ct);
        var inMonth = outcomes.Where(o => o.StartUtc >= window.MonthStartUtc && o.StartUtc < window.MonthEndUtc).ToList();
        var completed = inMonth.Count(o => Is(o, Completed));
        var settled = completed + inMonth.Count(o => Is(o, NoShow));
        var next = outcomes.FirstOrDefault(o => Is(o, CheckedIn) && o.StartUtc.InZone(zone).Date == window.Today);
        var (rows, names) = await RowsAsync(brandId, entries, next, ct);

        return new MyCommissionSummary(
            id,
            staff.FullName,
            zone.Id,
            MonthPattern.Format(window.Month),
            window.From,
            window.To,
            entries.Sum(e => e.AmountMinor),
            entries.Where(e => e.Type == CommissionType.Service).Sum(e => e.BasisMinor),
            entries.FirstOrDefault()?.CurrencyCode ?? settings.CurrencyCode,
            completed,
            settled == 0 ? null : Math.Round((settled - completed) * 100d / settled, 1, MidpointRounding.AwayFromZero),
            await ratings.GetStaffAverageAsync(brandId, id, now - Duration.FromDays(PlatformDefaults.ReviewOverviewWindowDays), ct),
            [.. CommissionQueries.DailyTotals(entries, zone).Select(d => new CommissionDayAmount(d.Date, d.AmountMinor))],
            Week(window, zone, weekEntries, outcomes),
            rows,
            next is null ? null : new NextAccrual(next.AppointmentId, names.GetValueOrDefault(next.BrandCustomerId)?.FullName ?? string.Empty));
    }

    private static bool Is(StaffAppointmentOutcome outcome, string status) => string.Equals(outcome.Status, status, StringComparison.Ordinal);

    // "Bu hafta": Pazartesiden bugüne her gün (kaydı olmayan gün 0), gün başına prim ve tamamlanan randevu.
    private static List<CommissionWeekDay> Week(
        Window window, DateTimeZone zone, List<CommissionEntry> weekEntries, IReadOnlyList<StaffAppointmentOutcome> outcomes)
    {
        var amounts = CommissionQueries.DailyTotals(weekEntries, zone).ToDictionary(d => d.Date, d => d.AmountMinor);
        var counts = outcomes.Where(o => Is(o, Completed)).GroupBy(o => o.StartUtc.InZone(zone).Date).ToDictionary(g => g.Key, g => g.Count());
        var days = Period.Between(window.WeekStart, window.Today, PeriodUnits.Days).Days + 1;
        return [.. Enumerable.Range(0, days).Select(window.WeekStart.PlusDays)
            .Select(d => new CommissionWeekDay(d, amounts.GetValueOrDefault(d), counts.GetValueOrDefault(d)))];
    }

    // Hizmet primi: randevu başlangıcı, müşteri ve hizmet adı Scheduling'den; satış primi: satış anı, müşteri ve paket adı Sales'ten.
    private async Task<(List<MyCommissionEntry> Rows, IReadOnlyDictionary<Guid, CustomerDisplayInfo> Names)> RowsAsync(
        Guid brandId, List<CommissionEntry> entries, StaffAppointmentOutcome? next, CancellationToken ct)
    {
        var itemIds = entries.Where(e => e.AppointmentItemId is not null).Select(e => e.AppointmentItemId!.Value).Distinct().ToList();
        var items = await appointments.GetItemContextsAsync(brandId, itemIds, ct);
        var packageIds = entries.Where(e => e.CustomerPackageId is not null).Select(e => e.CustomerPackageId!.Value).Distinct().ToList();
        var packages = await db.CustomerPackages.AsNoTracking().Where(p => packageIds.Contains(p.Id))
            .Select(p => new { p.Id, p.BrandCustomerId, p.PackageNameSnapshot })
            .ToDictionaryAsync(p => p.Id, ct);
        var customerIds = items.Values.Select(i => i.BrandCustomerId).Concat(packages.Values.Select(p => p.BrandCustomerId)).ToHashSet();
        if (next is not null)
        {
            customerIds.Add(next.BrandCustomerId);
        }

        var names = await customers.GetDisplayInfoAsync(brandId, [.. customerIds], ct);
        string NameOf(Guid brandCustomerId) => names.GetValueOrDefault(brandCustomerId)?.FullName ?? string.Empty;
        var rows = entries.Select(e =>
        {
            if (e.AppointmentItemId is { } itemId && items.TryGetValue(itemId, out var item))
            {
                return new MyCommissionEntry(e.Id, e.Type.ToString(), e.AmountMinor, e.Status.ToString(), item.StartUtc,
                    NameOf(item.BrandCustomerId), item.ServiceName, item.AppointmentId, null);
            }

            var package = e.CustomerPackageId is { } packageId ? packages.GetValueOrDefault(packageId) : null;
            return new MyCommissionEntry(e.Id, e.Type.ToString(), e.AmountMinor, e.Status.ToString(), e.CreatedUtc,
                package is null ? string.Empty : NameOf(package.BrandCustomerId), package?.PackageNameSnapshot ?? string.Empty, null, e.CustomerPackageId);
        }).ToList();
        return (rows, names);
    }

    /// <summary>Dönem: ay (verilmezse bu ay), ayın bugüne kadarki kısmı ve bu haftanın Pazartesisi.</summary>
    private sealed record Window(
        YearMonth Month, LocalDate Today, LocalDate From, LocalDate To, LocalDate WeekStart,
        Instant MonthStartUtc, Instant MonthEndUtc, Instant WeekStartUtc, Instant TomorrowUtc)
    {
        public static Window For(DateTimeZone zone, LocalDate today, YearMonth? requested)
        {
            var month = requested ?? today.ToYearMonth();
            var from = month.OnDayOfMonth(1);
            var last = month.ToDateInterval().End;
            var weekStart = today.With(DateAdjusters.PreviousOrSame(IsoDayOfWeek.Monday));
            return new Window(
                month,
                today,
                from,
                month == today.ToYearMonth() ? today : last,
                weekStart,
                zone.AtStartOfDay(from).ToInstant(),
                zone.AtStartOfDay(last.PlusDays(1)).ToInstant(),
                zone.AtStartOfDay(weekStart).ToInstant(),
                zone.AtStartOfDay(today.PlusDays(1)).ToInstant());
        }
    }
}
