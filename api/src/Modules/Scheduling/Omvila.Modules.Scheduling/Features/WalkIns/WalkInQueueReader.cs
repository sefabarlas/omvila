using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Infrastructure;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.WalkIns;

/// <summary>
/// Kapıda sıra okuması (sözleşme manageListWalkIns; KapidaSira, işletme uygulaması Sıra): şubenin bugünkü (yerel gün) Waiting ve
/// Called biletleri numara sırasıyla, özet sayılar ve tahmini bekleme (<see cref="WalkInWaitEstimator"/>). Müsait personel =
/// şubede şu an çalışma saatinde olan ve izinde olmayan personel; o an randevuda olması düşülmez (kapasite; T-91).
/// </summary>
internal sealed class WalkInQueueReader(SchedulingDbContext db, AvailabilitySnapshotLoader loader, ITenantQueries tenants, IClock clock)
{
    public async Task<Result<WalkInQueue>> GetAsync(Guid brandId, Guid? branchId, ManageAccess access, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(access);
        if (branchId is not { } id)
        {
            return Error.Validation("branchId", "required");
        }

        if (!access.Branches.Includes(id))
        {
            return Error.NotFound();
        }

        var queue = await BuildAsync(brandId, id, ct);
        return queue is null ? Error.NotFound() : queue;
    }

    /// <summary>Tek biletin görünümü (oluşturma, başlatma, sıradan çıkarma yanıtları); sıradaysa tahmini beklemesiyle.</summary>
    public async Task<WalkInTicketView> ViewAsync(WalkInTicket ticket, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        if (ticket.IsInQueue && await BuildAsync(ticket.BrandId, ticket.BranchId, ct) is { } queue
            && queue.Items.SingleOrDefault(i => i.Id == ticket.Id) is { } inQueue)
        {
            return inQueue;
        }

        var names = await StaffNamesAsync(ticket.BrandId, [ticket], ct);
        return Map(ticket, null, names);
    }

    private async Task<WalkInQueue?> BuildAsync(Guid brandId, Guid branchId, CancellationToken ct)
    {
        if (await tenants.GetBranchTimeZoneAsync(brandId, branchId, ct) is not { } zoneId)
        {
            return null;
        }

        var zone = DateTimeZoneProviders.Tzdb[zoneId];
        var now = clock.GetCurrentInstant();
        var today = now.InZone(zone).Date;
        var tickets = await db.WalkInTickets.AsNoTracking()
            .Where(t => t.BrandId == brandId && t.BranchId == branchId && t.QueueDate == today)
            .OrderBy(t => t.QueueNumber)
            .ToListAsync(ct);
        var lastNumber = await db.BranchQueueCounters.AsNoTracking()
            .Where(c => c.BrandId == brandId && c.BranchId == branchId && c.QueueDate == today)
            .Select(c => (int?)c.LastNumber)
            .SingleOrDefaultAsync(ct);
        var inQueue = tickets.Where(t => t.IsInQueue).ToList();

        // Hizmetsiz biletin süresi bugünkü biletlerin hizmet sürelerinin ortalamasıdır (WalkInAverageServiceMinutes, T-91).
        var known = tickets.Where(t => t.ServiceMinutes is not null).Select(t => t.ServiceMinutes!.Value).ToList();
        int? fallback = known.Count > 0 ? (int)Math.Round(known.Average(), MidpointRounding.AwayFromZero) : null;
        var available = await AvailableStaffAsync(brandId, branchId, today, ct);
        var (waits, forNew) = WalkInWaitEstimator.Estimate([.. inQueue.Select(t => t.ServiceMinutes)], fallback, available);
        var names = await StaffNamesAsync(brandId, inQueue, ct);

        return new WalkInQueue(
            branchId,
            today,
            zoneId,
            now,
            [.. inQueue.Select((t, i) => Map(t, waits[i], names))],
            inQueue.Count,
            inQueue.Count > 0 ? inQueue.Min(t => t.JoinedUtc) : null,
            forNew,
            tickets.Count(t => t.Status == WalkInTicketStatus.Served),
            (lastNumber ?? 0) + 1);
    }

    private async Task<int> AvailableStaffAsync(Guid brandId, Guid branchId, LocalDate today, CancellationToken ct)
    {
        var snapshot = await loader.LoadAsync(brandId, branchId, today, today, [], staffIds: null, ct);
        return snapshot is null ? 0 : snapshot.Staff.Values.Count(s => IsWorkingNow(snapshot, s, today));
    }

    private static bool IsWorkingNow(AvailabilitySnapshot snapshot, StaffSchedule staff, LocalDate today) =>
        ZoneTime.ToSet(today, AvailabilityEngine.StaffHoursOn(snapshot, staff, today), snapshot.Zone).Contains(snapshot.Now)
        && !staff.TimeOffs.Any(t => t.Contains(snapshot.Now));

    private async Task<IReadOnlyDictionary<Guid, StaffCard>> StaffNamesAsync(Guid brandId, IEnumerable<WalkInTicket> tickets, CancellationToken ct)
    {
        var ids = tickets.Where(t => t.PreferredStaffId is not null).Select(t => t.PreferredStaffId!.Value).Distinct().ToList();
        return ids.Count == 0 ? new Dictionary<Guid, StaffCard>() : await tenants.GetStaffCardsAsync(brandId, ids, ct);
    }

    private static WalkInTicketView Map(WalkInTicket t, int? wait, IReadOnlyDictionary<Guid, StaffCard> staff) => new(
        t.Id,
        t.BranchId,
        t.QueueDate,
        t.QueueNumber,
        t.Status.ToString(),
        t.CustomerName,
        t.BrandCustomerId,
        PhoneNumber.TryParse(t.PhoneE164, out var phone) ? phone.Masked : null,
        t.ServiceId,
        t.ServiceName,
        t.PreferredStaffId,
        t.PreferredStaffId is { } staffId && staff.TryGetValue(staffId, out var card) ? card.FullName : null,
        t.JoinedUtc,
        wait,
        t.AppointmentId,
        t.Version.ToString(CultureInfo.InvariantCulture));
}
