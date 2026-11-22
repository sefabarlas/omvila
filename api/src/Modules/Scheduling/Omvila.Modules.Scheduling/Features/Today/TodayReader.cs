using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.WalkIns;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Today;

/// <summary>
/// İşletme uygulaması "Bugün" okuma modeli (sözleşme manageGetToday; T-105). Randevular Scheduling'in kendi verisi; kapıda sıra
/// özeti <see cref="WalkInQueueReader"/>, kişisel prim kartı Sales.Contracts <see cref="ICommissionQueries"/>, şube ve personel
/// Tenancy.Contracts, müşteri adları Customers.Contracts üzerinden toplu okunur. Gün şubenin saat dilimindeki bugündür; personel
/// (calendar.view = K) yalnız kendi kalemi olan randevuları görür (KK10). İptal edilen randevu gün akışında yoktur.
/// </summary>
internal sealed class TodayReader(
    SchedulingDbContext db,
    ITenantQueries tenants,
    ICustomerDirectory customers,
    ICommissionQueries commissions,
    WalkInQueueReader walkIns,
    IClock clock)
{
    private const string ServiceSeparator = " + ";

    /// <summary>Yetkiye bağlı kartlar (sözleşme: izin yoksa null).</summary>
    /// <param name="Approvals">appointment.approve.</param>
    /// <param name="WalkIns">walkin.manage.</param>
    /// <param name="Commission">commission.view.</param>
    internal sealed record Cards(bool Approvals, bool WalkIns, bool Commission);

    // Kural: sözleşme ManageToday; dilim-05 kabul kriteri 10; roller-yetkiler.md calendar.view (K), appointment.approve, walkin.manage, commission.view
    public async Task<Result<ManageToday>> GetAsync(Guid brandId, Guid? branchId, ManageAccess access, Cards cards, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(access);
        ArgumentNullException.ThrowIfNull(cards);
        var branch = await ResolveBranchAsync(brandId, branchId, access.Branches, ct);
        if (branch is null || await tenants.GetBranchTimeZoneAsync(brandId, branch.Id, ct) is not { } zoneId)
        {
            return Error.NotFound();
        }

        var zone = DateTimeZoneProviders.Tzdb[zoneId];
        var now = clock.GetCurrentInstant();
        var today = now.InZone(zone).Date;
        var dayStart = zone.AtStartOfDay(today).ToInstant();
        var dayEnd = zone.AtStartOfDay(today.PlusDays(1)).ToInstant();

        var query = db.Appointments.AsNoTracking().Include(a => a.Items)
            .Where(a => a.BranchId == branch.Id && a.StartUtc >= dayStart && a.StartUtc < dayEnd
                && a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Draft);
        if (access.OwnOnly)
        {
            var own = access.OwnStaffId ?? Guid.Empty;
            query = query.Where(a => a.Items.Any(i => i.StaffId == own && i.Status != AppointmentItemStatus.Cancelled));
        }

        var appointments = await query.OrderBy(a => a.StartUtc).ThenBy(a => a.Id).ToListAsync(ct);
        var items = await MapAsync(brandId, appointments, ct);
        var next = appointments.Zip(items)
            .FirstOrDefault(x => x.First.StartUtc > now && x.First.Status is AppointmentStatus.Confirmed or AppointmentStatus.PendingApproval).Second;

        int? pending = cards.Approvals
            ? await db.Appointments.CountAsync(
                a => a.BranchId == branch.Id && a.Status == AppointmentStatus.PendingApproval && a.HoldExpiresUtc != null && a.HoldExpiresUtc > now, ct)
            : null;
        var queue = cards.WalkIns ? await walkIns.LoadAsync(brandId, branch.Id, ct) : null;
        var commission = cards.Commission && access.OwnStaffId is { } staffId ? await CommissionAsync(brandId, staffId, today, zoneId, ct) : null;

        return new ManageToday(
            branch.Id,
            branch.Name,
            zoneId,
            today,
            now,
            next,
            pending,
            queue is null ? null : new TodayWalkInSummary(queue.WaitingCount, queue.EstimatedWaitMinutesForNew),
            commission,
            appointments.Count(a => a.Status == AppointmentStatus.Completed),
            appointments.Count,
            items);
    }

    // branchId verilmezse kullanıcının birincil şubesi: kapsamındaki etkin şubelerin ilki (oluşturulma sırası; T-105).
    private async Task<BranchSummary?> ResolveBranchAsync(Guid brandId, Guid? branchId, BranchScope scope, CancellationToken ct)
    {
        if (branchId is { } requested && !scope.Includes(requested))
        {
            return null;
        }

        var summary = await tenants.GetSummaryAsync(brandId, scope.All ? null : scope.BranchIds, ct);
        return branchId is { } id ? summary?.Branches.FirstOrDefault(b => b.Id == id) : summary?.Branches is [var first, ..] ? first : null;
    }

    private async Task<List<TodayAppointment>> MapAsync(Guid brandId, List<Appointment> appointments, CancellationToken ct)
    {
        var people = await customers.GetDisplayInfoAsync(brandId, [.. appointments.Select(a => a.BrandCustomerId).Distinct()], ct);
        var staffIds = appointments.SelectMany(Active).Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
        var staff = await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        return [.. appointments.Select(a =>
        {
            var info = people.GetValueOrDefault(a.BrandCustomerId);
            var active = Active(a).ToList();
            return new TodayAppointment(
                a.Id,
                a.StartUtc,
                a.EndUtc,
                a.Status.ToString(),
                a.DisplayStatus.ToString(),
                info?.FullName ?? string.Empty,
                // Sözleşme: "İlk randevusu mu" (tamamlanmış ziyareti yok).
                info is null || info.VisitCount == 0,
                string.Join(ServiceSeparator, active.Select(i => i.ServiceNameSnapshot)),
                [.. active.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct()
                    .Select(s => staff.TryGetValue(s, out var card) ? card.FullName : null).OfType<string>()],
                a.Status == AppointmentStatus.PendingApproval ? a.HoldExpiresUtc : null,
                a.DepositStatus?.ToString());
        })];
    }

    private async Task<TodayCommission> CommissionAsync(Guid brandId, Guid staffId, LocalDate today, string zoneId, CancellationToken ct)
    {
        var days = await commissions.GetDailyTotalsAsync(brandId, staffId, today.Year, today.Month, zoneId, ct);
        return new TodayCommission(
            string.Create(CultureInfo.InvariantCulture, $"{today.Year:D4}-{today.Month:D2}"),
            days.Sum(d => d.AmountMinor),
            (days.Count > 0 ? days[0].CurrencyCode : null) ?? PlatformDefaults.DefaultCurrency.ToString(),
            [.. days.Select(d => new TodayCommissionDay(d.Date, d.AmountMinor))]);
    }

    private static IEnumerable<AppointmentItem> Active(Appointment a) =>
        a.Items.Where(i => i.Status != AppointmentItemStatus.Cancelled).OrderBy(i => i.SeqNo);
}
