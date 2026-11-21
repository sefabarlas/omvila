using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Tenancy.Contracts;

namespace Omvila.Modules.Tenancy.Infrastructure;

/// <summary>
/// Müsaitlik girdileri: en çok altı sorgu (şube + ayarlar, şube saatleri, özel günler, personel, personel saatleri, izinler);
/// gün ve personel sayısından bağımsız, döngüde sorgu yok (musaitlik.md § Performans hedefi).
/// </summary>
internal sealed class BranchScheduleQueries(TenancyDbContext db) : IBranchScheduleQueries
{
    public async Task<BranchSchedule?> GetAsync(
        Guid brandId, Guid branchId, LocalDate from, LocalDate to, IReadOnlyCollection<Guid>? staffIds, CancellationToken ct)
    {
        var branch = await (from b in db.Branches.AsNoTracking()
                            join s in db.BrandSettings.AsNoTracking() on b.BrandId equals s.BrandId
                            where b.BrandId == brandId && b.Id == branchId
                            select new
                            {
                                b.TimeZoneId,
                                b.IsActive,
                                b.SlotIntervalMinutes,
                                BrandSlotInterval = s.SlotIntervalMinutes,
                                s.MinNoticeMinutes,
                                s.MaxAdvanceDays,
                                s.DefaultBranchTransferMinutes,
                            }).SingleOrDefaultAsync(ct);
        if (branch is null)
        {
            return null;
        }

        var hours = await db.BranchHours.AsNoTracking()
            .Where(h => h.BrandId == brandId && h.BranchId == branchId)
            .Select(h => new WeeklyHoursInterval(h.DayOfWeek, h.StartTime, h.EndTime))
            .ToListAsync(ct);

        var overrides = (await db.Overrides.AsNoTracking()
                .Where(o => o.BrandId == brandId && o.BranchId == branchId && o.Date >= from && o.Date <= to)
                .ToListAsync(ct))
            .Select(o => new HoursOverride(o.Date, o.IsClosed, [.. o.OpeningIntervals.Select(i => new LocalTimeInterval(i.Start, i.End))]))
            .ToList();

        var staffBranches = StaffInBranch(brandId, branchId, staffIds);
        var staff = await staffBranches.ToListAsync(ct);

        var staffHours = (await db.StaffHours.AsNoTracking()
                .Join(staffBranches, h => h.StaffBranchId, sb => sb.StaffBranchId, (h, sb) => new { sb.Id, h.DayOfWeek, h.StartTime, h.EndTime })
                .ToListAsync(ct))
            .ToLookup(h => h.Id, h => new WeeklyHoursInterval(h.DayOfWeek, h.StartTime, h.EndTime));

        var zone = DateTimeZoneProviders.Tzdb[branch.TimeZoneId];
        var rangeStart = zone.AtStartOfDay(from).ToInstant();
        var rangeEnd = zone.AtStartOfDay(to.PlusDays(1)).ToInstant();
        var timeOffs = await db.TimeOffs.AsNoTracking()
            .Where(t => t.BrandId == brandId && t.StartUtc < rangeEnd && t.EndUtc > rangeStart)
            .Join(staffBranches, t => t.StaffId, sb => sb.Id, (t, _) => new StaffTimeOff(t.StaffId, t.StartUtc, t.EndUtc))
            .ToListAsync(ct);

        return new BranchSchedule(
            branchId,
            branch.TimeZoneId,
            branch.IsActive,
            new BookingRules(branch.MinNoticeMinutes, branch.MaxAdvanceDays, branch.SlotIntervalMinutes ?? branch.BrandSlotInterval, branch.DefaultBranchTransferMinutes),
            hours,
            overrides,
            [.. staff.Select(s => new StaffWorkSchedule(s.Id, s.IsBookableOnline, [.. staffHours[s.Id]], []))],
            timeOffs,
            // BranchTransferTime tablosu henüz yok (alan-modeli.md § BranchTransferTime); marka varsayılanı geçerli (T-48).
            []);
    }

    /// <summary>Şubede etkin çalışan, etkin personel (StaffBranch.IsActive, Staff.IsActive); diğer sorgularda alt sorgu.</summary>
    private IQueryable<StaffInBranchRow> StaffInBranch(Guid brandId, Guid branchId, IReadOnlyCollection<Guid>? staffIds)
    {
        var query = db.StaffBranches.AsNoTracking()
            .Where(sb => sb.BrandId == brandId && sb.BranchId == branchId && sb.IsActive)
            .Join(db.Staff.AsNoTracking().Where(st => st.IsActive), sb => sb.StaffId, st => st.Id, (sb, st) => new StaffInBranchRow { StaffBranchId = sb.Id, Id = st.Id, IsBookableOnline = st.IsBookableOnline });
        return staffIds is null ? query : query.Where(x => staffIds.Contains(x.Id));
    }

    /// <summary>Üye atamalı projeksiyon: EF sonraki Join/Where'de alanlara çevirebilsin diye.</summary>
    private sealed class StaffInBranchRow
    {
        public Guid StaffBranchId { get; init; }

        public Guid Id { get; init; }

        public bool IsBookableOnline { get; init; }
    }
}
