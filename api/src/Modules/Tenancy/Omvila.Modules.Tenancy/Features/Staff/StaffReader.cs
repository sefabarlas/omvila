using System.Globalization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Features.Branches;
using Omvila.Modules.Tenancy.Infrastructure;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>
/// Personel okuma: kapsam (jetondaki şubeler), liste özeti ve ayrıntı. Rol ve üyelik durumu Identity'den,
/// hizmetler Catalog'dan gelir. Kapsam dışı personel bulunamaz (404).
/// </summary>
internal sealed class StaffReader(TenancyDbContext db, IStaffMemberships memberships, IStaffServiceAssignments services, Integrations.Contracts.ICalendarLinkQueries calendars)
{
    public async Task<Domain.Staff?> FindAsync(Guid brandId, Guid staffId, ClaimsPrincipal user, bool track, CancellationToken ct)
    {
        var query = track ? db.Staff : db.Staff.AsNoTracking();
        var staff = await query.SingleOrDefaultAsync(s => s.Id == staffId && s.BrandId == brandId, ct);
        if (staff is null)
        {
            return null;
        }

        var scope = BranchScope.From(user);
        if (scope.All)
        {
            return staff;
        }

        var branches = await db.StaffBranches.AsNoTracking().Where(b => b.StaffId == staffId && b.IsActive).Select(b => b.BranchId).ToListAsync(ct);
        return branches.Exists(scope.Includes) ? staff : null;
    }

    public async Task<List<StaffSummaryResponse>> ListAsync(Guid brandId, BranchScope scope, Guid? branchId, bool includeInactive, CancellationToken ct)
    {
        var links = await db.StaffBranches.AsNoTracking().Where(b => b.BrandId == brandId && b.IsActive).ToListAsync(ct);
        var branchesByStaff = links.ToLookup(l => l.StaffId, l => l.BranchId);
        var query = db.Staff.AsNoTracking().Where(s => s.BrandId == brandId);
        if (!includeInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        var staff = (await query.OrderBy(s => s.CreatedUtc).ToListAsync(ct))
            .Where(s => branchesByStaff[s.Id].Any(scope.Includes) && (branchId is null || branchesByStaff[s.Id].Contains(branchId.Value)))
            .ToList();
        var info = await memberships.GetByStaffAsync(brandId, [.. staff.Select(s => s.Id)], ct);
        return [.. staff.Select(s => Summary(s, [.. branchesByStaff[s.Id]], info.GetValueOrDefault(s.Id)))];
    }

    public async Task<StaffDetailResponse> DetailAsync(Domain.Staff staff, CancellationToken ct)
    {
        var links = await db.StaffBranches.AsNoTracking().Where(b => b.StaffId == staff.Id && b.IsActive).OrderByDescending(b => b.IsPrimary).ToListAsync(ct);
        var linkIds = links.Select(l => l.Id).ToList();
        var hours = (await db.StaffHours.AsNoTracking().Where(h => linkIds.Contains(h.StaffBranchId)).ToListAsync(ct)).ToLookup(h => h.StaffBranchId);
        var info = (await memberships.GetByStaffAsync(staff.BrandId, [staff.Id], ct)).GetValueOrDefault(staff.Id);
        var assigned = await services.GetAsync(staff.BrandId, staff.Id, ct);
        var summary = Summary(staff, [.. links.Select(l => l.BranchId)], info);
        var calendar = (await calendars.GetStatusesAsync(staff.BrandId, [staff.Id], ct)).GetValueOrDefault(staff.Id);
        return new StaffDetailResponse(
            summary.Id, summary.FullName, summary.DisplayName, summary.Title, summary.PhotoUrl, summary.CalendarColor, summary.IsBookableOnline,
            summary.IsActive, summary.BranchIds, summary.Role, summary.MembershipStatus, info?.PhoneMasked, staff.Bio,
            staff.ServiceCommissionPercent, staff.SaleCommissionPercent,
            [.. assigned.Select(a => new StaffServiceDto(a.ServiceId, a.DurationOverrideMinutes, a.Surcharge, a.CommissionPercentOverride))],
            [.. links.Select(l => new StaffBranchHoursDto(l.BranchId, new WeeklyHoursDto(
                [.. hours[l.Id].GroupBy(h => h.DayOfWeek).OrderBy(g => g.Key).Select(g => new WeeklyHoursDayDto(
                    (int)g.Key,
                    [.. g.OrderBy(h => h.StartTime).Select(h => new TimeIntervalDto(TimeText.Format(h.StartTime), TimeText.Format(h.EndTime)))]))])))],
            calendar,
            staff.Version.ToString(CultureInfo.InvariantCulture));
    }

    private static StaffSummaryResponse Summary(Domain.Staff s, IReadOnlyList<Guid> branchIds, MembershipInfo? info) => new(
        s.Id, s.FullName, s.DisplayName, s.Title, null, s.CalendarColor, s.IsBookableOnline, s.IsActive, branchIds, info?.Role, info?.Status);
}
