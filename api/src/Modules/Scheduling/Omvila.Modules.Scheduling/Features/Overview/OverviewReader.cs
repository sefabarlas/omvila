using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Today;
using Omvila.Modules.Scheduling.Features.WalkIns;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Overview;

/// <summary>
/// Salon paneli Genel bakış (sözleşme manageGetOverview; T-120). Randevular, onay kuyruğu, kapıda sıra ve doluluk Scheduling'in
/// verisi (çalışma saatleri müsaitlik yükleyicisinden, motorla aynı kurallar); bugünkü ciro Sales.Contracts
/// <see cref="ICollectionQueries"/>, personel kartları Tenancy.Contracts ile okunur. Gün ve hafta (Pazartesi–Pazar) şubenin saat
/// dilimindedir; şube verilmezse kullanıcının birincil şubesi (<see cref="TodayReader.ResolveBranchAsync"/>).
/// </summary>
internal sealed class OverviewReader(
    SchedulingDbContext db,
    AvailabilitySnapshotLoader loader,
    ITenantQueries tenants,
    ICollectionQueries collections,
    WalkInQueueReader walkIns,
    IClock clock)
{
    private static readonly StringComparer TurkishOrder = StringComparer.Create(CultureInfo.GetCultureInfo("tr-TR"), ignoreCase: true);

    /// <summary>Yetkiye bağlı kartlar (sözleşme: izin yoksa null).</summary>
    /// <param name="Approvals">appointment.approve.</param>
    /// <param name="WalkIns">walkin.manage.</param>
    internal sealed record Cards(bool Approvals, bool WalkIns);

    // Kural: sözleşme ManageOverview; roller-yetkiler.md report.view (BranchManager Ş), appointment.approve, walkin.manage
    public async Task<Result<ManageOverview>> GetAsync(Guid brandId, Guid? branchId, BranchScope scope, Cards cards, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(cards);
        var branch = await TodayReader.ResolveBranchAsync(tenants, brandId, branchId, scope, ct);
        if (branch is null)
        {
            return Error.NotFound();
        }

        var now = clock.GetCurrentInstant();
        var weekStart = await LocalTodayAsync(brandId, branch.Id, now, ct) is { } localToday
            ? localToday.With(DateAdjusters.PreviousOrSame(IsoDayOfWeek.Monday))
            : (LocalDate?)null;
        var snapshot = weekStart is { } monday ? await loader.LoadAsync(brandId, branch.Id, monday.PlusDays(-7), monday.PlusDays(6), [], null, ct) : null;
        if (snapshot is null)
        {
            return Error.NotFound();
        }

        var zone = snapshot.Zone;
        var today = snapshot.Today;
        var dayStart = zone.AtStartOfDay(today).ToInstant();
        var dayEnd = zone.AtStartOfDay(today.PlusDays(1)).ToInstant();
        var settings = await tenants.GetCustomerSettingsAsync(brandId, ct) ?? throw new DomainException("Marka ayarları bulunamadı.");
        var currency = Enum.TryParse<Currency>(settings.CurrencyCode, out var c) ? c : PlatformDefaults.DefaultCurrency;

        var todayCount = await db.Appointments.CountAsync(
            a => a.BranchId == branch.Id && a.StartUtc >= dayStart && a.StartUtc < dayEnd
                && a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Draft,
            ct);
        OverviewPendingApproval? pending = null;
        if (cards.Approvals)
        {
            var holds = await db.Appointments.AsNoTracking()
                .Where(a => a.BranchId == branch.Id && a.Status == AppointmentStatus.PendingApproval && a.HoldExpiresUtc != null && a.HoldExpiresUtc > now)
                .Select(a => a.HoldExpiresUtc!.Value)
                .ToListAsync(ct);
            pending = new OverviewPendingApproval(holds.Count, holds.Count == 0 ? null : holds.Min());
        }

        var queue = cards.WalkIns ? await walkIns.LoadAsync(brandId, branch.Id, ct) : null;
        return new ManageOverview(
            branch.Id,
            branch.Name,
            zone.Id,
            today,
            now,
            new Money(await collections.GetNetCollectedAsync(brandId, branch.Id, dayStart, dayEnd, ct), currency),
            todayCount,
            pending,
            queue is null ? null : new OverviewWalkIns(queue.WaitingCount, queue.EstimatedWaitMinutesForNew),
            await OccupancyAsync(brandId, snapshot, weekStart!.Value, ct));
    }

    private async Task<LocalDate?> LocalTodayAsync(Guid brandId, Guid branchId, Instant now, CancellationToken ct) =>
        await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) is { } zoneId && DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) is { } zone
            ? now.InZone(zone).Date
            : null;

    // Kural: sözleşme ManageOverview.weeklyOccupancy (çalışma dakikası olmayan personel listede yok; hiç yoksa percent null); T-120
    private async Task<WeeklyOccupancy> OccupancyAsync(Guid brandId, AvailabilitySnapshot snapshot, LocalDate monday, CancellationToken ct)
    {
        var from = snapshot.Zone.AtStartOfDay(monday.PlusDays(-7)).ToInstant();
        var to = snapshot.Zone.AtStartOfDay(monday.PlusDays(7)).ToInstant();
        var appointments = await db.Appointments.AsNoTracking()
            .Include(a => a.Items).ThenInclude(i => i.Segments)
            .Where(a => a.BranchId == snapshot.BranchId && a.StartUtc < to && a.EndUtc > from
                && a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Draft)
            .AsSplitQuery()
            .ToListAsync(ct);
        var busy = appointments
            .SelectMany(a => a.Items.Where(i => i.StaffId is not null && i.Status != AppointmentItemStatus.Cancelled))
            .SelectMany(i => i.CurrentSegments.Where(s => s.StaffBusy).Select(s => (StaffId: i.StaffId!.Value, s.During)))
            .ToLookup(x => x.StaffId, x => x.During);

        var measured = snapshot.Staff.Values.Select(s => (
            s.StaffId,
            Current: Occupancy.Measure(snapshot, s, monday, monday.PlusDays(6), busy[s.StaffId]),
            Previous: Occupancy.Measure(snapshot, s, monday.PlusDays(-7), monday.PlusDays(-1), busy[s.StaffId]))).ToList();
        var working = measured.Where(m => m.Current.WorkingSeconds > 0).ToList();
        var cards = await tenants.GetStaffCardsAsync(brandId, [.. working.Select(m => m.StaffId)], ct);
        var staff = working
            .Where(m => cards.ContainsKey(m.StaffId))
            .Select(m => new StaffOccupancy(
                m.StaffId, cards[m.StaffId].FullName, cards[m.StaffId].CalendarColor, Occupancy.Percent(m.Current.BookedSeconds, m.Current.WorkingSeconds)!.Value))
            .OrderByDescending(s => s.Percent).ThenBy(s => s.StaffName, TurkishOrder).ThenBy(s => s.StaffId)
            .ToList();
        return new WeeklyOccupancy(
            monday,
            Occupancy.Percent(measured.Sum(m => m.Current.BookedSeconds), measured.Sum(m => m.Current.WorkingSeconds)),
            Occupancy.Percent(measured.Sum(m => m.Previous.BookedSeconds), measured.Sum(m => m.Previous.WorkingSeconds)),
            staff);
    }
}
