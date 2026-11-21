using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Scheduling.Infrastructure.Availability;

/// <summary>
/// Müsaitlik anlık görüntüsünü yükler (musaitlik.md § Performans hedefi): şube, saatler, özel günler, personel, izinler
/// (Tenancy, en çok 6 sorgu), teklifler (Catalog, 4 sorgu) ve personelin bütün şubelerdeki meşguliyeti (1 sorgu). Sorgu
/// sayısı gün, personel ve hizmet sayısından bağımsızdır; döngüde sorgu yoktur. Markanın kiracı bağlamında, açık
/// oturumda çağrılır. Kaynak (oda/cihaz) tabloları henüz yok; envanter boştur (T-48).
/// </summary>
internal sealed class AvailabilitySnapshotLoader(
    SchedulingDbContext db, IBranchScheduleQueries schedules, IServiceOffers offers, IClock clock)
{
    /// <summary>
    /// Şube markada yoksa null. <paramref name="staffIds"/> null ise şubenin bütün etkin personeli adaydır ("fark etmez").
    /// </summary>
    public async Task<AvailabilitySnapshot?> LoadAsync(
        Guid brandId,
        Guid branchId,
        LocalDate from,
        LocalDate to,
        IReadOnlyCollection<Guid> serviceIds,
        IReadOnlyCollection<Guid>? staffIds,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(serviceIds);
        if (to < from)
        {
            throw new ArgumentOutOfRangeException(nameof(to), "Bitiş başlangıçtan önce olamaz.");
        }

        var now = clock.GetCurrentInstant();
        var schedule = await schedules.GetAsync(brandId, branchId, from, to, staffIds, ct);
        if (schedule is null)
        {
            return null;
        }

        var zone = DateTimeZoneProviders.Tzdb[schedule.TimeZoneId];
        var staffList = schedule.Staff.Select(s => s.StaffId).ToList();
        var services = serviceIds.Distinct().ToList();

        // Personelsiz çözüm hizmet kurallarını (online, pencere) verir; personelli çözümler kimin verdiğini ve süresini.
        var requests = services.SelectMany(serviceId =>
            new[] { new ServiceOfferRequest(serviceId, null) }.Concat(staffList.Select(staffId => new ServiceOfferRequest(serviceId, staffId)))).ToList();
        var resolved = await offers.ResolveAsync(brandId, branchId, requests, ct);

        var transfers = new TransferTimes(
            schedule.Rules.DefaultBranchTransferMinutes,
            schedule.Transfers.ToDictionary(t => (t.FromBranchId, t.ToBranchId), t => t.Minutes));
        var maxTransfer = schedule.Transfers.Select(t => t.Minutes).Append(schedule.Rules.DefaultBranchTransferMinutes).Max();

        // Gün sınırları + bir gün (gece yarısını aşan kalem) + en uzun geçiş süresi.
        var pad = Duration.FromDays(1) + Duration.FromMinutes(maxTransfer);
        var rangeStart = zone.AtStartOfDay(from).ToInstant() - pad;
        var rangeEnd = zone.AtStartOfDay(to.PlusDays(1)).ToInstant() + pad;
        var busy = staffList.Count == 0 ? [] : await LoadBusyAsync(staffList, rangeStart, rangeEnd, ct);

        return new AvailabilitySnapshot(
            branchId,
            zone,
            now,
            new AvailabilitySettings(schedule.Rules.MinNoticeMinutes, schedule.Rules.MaxAdvanceDays, schedule.Rules.SlotIntervalMinutes),
            ToSchedule(schedule.Hours, schedule.Overrides),
            BuildStaff(schedule, busy),
            BuildServices(requests, resolved, staffList),
            transfers,
            ResourceInventory.None);
    }

    private static Dictionary<Guid, StaffSchedule> BuildStaff(BranchSchedule schedule, List<BusyRow> busy)
    {
        var busyByStaff = busy.ToLookup(b => b.StaffId);
        var timeOffsByStaff = schedule.TimeOffs.ToLookup(t => t.StaffId);
        return schedule.Staff.ToDictionary(
            s => s.StaffId,
            s => new StaffSchedule(
                s.StaffId,
                s.IsBookableOnline,
                ToSchedule(s.Hours, s.Overrides),
                [.. timeOffsByStaff[s.StaffId].Select(t => new Interval(t.StartUtc, t.EndUtc))],
                [.. busyByStaff[s.StaffId].Select(b => new BusySegment(
                    b.AppointmentId,
                    b.BranchId,
                    b.During,
                    b.Status == AppointmentStatus.PendingApproval ? b.HoldExpiresUtc : null))]));
    }

    /// <summary>İstekler hizmet başına [personelsiz, personel1, personel2…] sırasındadır; yanıt aynı sıradadır.</summary>
    private static Dictionary<Guid, ServiceAvailability> BuildServices(
        List<ServiceOfferRequest> requests, IReadOnlyList<SharedKernel.Result<ServiceOffer>> resolved, List<Guid> staffList)
    {
        var serviceAvailability = new Dictionary<Guid, ServiceAvailability>();
        for (var i = 0; i < requests.Count; i += staffList.Count + 1)
        {
            var rules = resolved[i];
            if (!rules.IsSuccess)
            {
                continue;
            }

            var byStaff = new Dictionary<Guid, ItemOffer>();
            for (var k = 1; k <= staffList.Count; k++)
            {
                if (resolved[i + k] is { IsSuccess: true } offer)
                {
                    byStaff[staffList[k - 1]] = offer.Value.ToItemOffer();
                }
            }

            var serviceId = requests[i].ServiceId;
            serviceAvailability[serviceId] = new ServiceAvailability(
                serviceId, rules.Value.IsOnlineBookable, rules.Value.MinNoticeMinutes, rules.Value.MaxAdvanceDays, byStaff, []);
        }

        return serviceAvailability;
    }
    private static WeeklySchedule ToSchedule(IReadOnlyList<WeeklyHoursInterval> hours, IReadOnlyList<HoursOverride> overrides) => new(
        hours.GroupBy(h => h.Day).ToDictionary(
            g => g.Key,
            g => (IReadOnlyList<LocalTimeRange>)[.. g.OrderBy(h => h.Start).Select(h => new LocalTimeRange(h.Start, h.End))]),
        overrides.ToDictionary(
            o => o.Date,
            o => (IReadOnlyList<LocalTimeRange>)(o.IsClosed ? [] : [.. o.Intervals.OrderBy(i => i.Start).Select(i => new LocalTimeRange(i.Start, i.End))])));

    /// <summary>
    /// Personelin bütün şubelerdeki aktif, personel-meşgul dilimleri (musaitlik.md § 4). Çakışma kısıtı gibi şubeyle
    /// filtrelenmez.
    /// </summary>
    private async Task<List<BusyRow>> LoadBusyAsync(List<Guid> staffIds, Instant rangeStart, Instant rangeEnd, CancellationToken ct) =>
        await (from segment in db.AppointmentItemSegments.AsNoTracking()
               join item in db.AppointmentItems.AsNoTracking() on segment.AppointmentItemId equals item.Id
               join appointment in db.Appointments.AsNoTracking() on item.AppointmentId equals appointment.Id
               where segment.IsActive && segment.StaffBusy && segment.StaffId != null && staffIds.Contains(segment.StaffId.Value)
                     && item.StartUtc < rangeEnd && item.EndUtc > rangeStart
                     && segment.During.Start < rangeEnd && segment.During.End > rangeStart
               select new BusyRow(segment.StaffId!.Value, appointment.Id, appointment.BranchId, segment.During, appointment.Status, appointment.HoldExpiresUtc))
            .ToListAsync(ct);

    private sealed record BusyRow(Guid StaffId, Guid AppointmentId, Guid BranchId, Interval During, AppointmentStatus Status, Instant? HoldExpiresUtc);
}
