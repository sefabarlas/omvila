using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Features.Branches;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>
/// Personelin verdiği hizmetler (kabul kriteri 11) ve şube saatleri (PersonelEkle "Çalışma saatleri"). İkisi de
/// personelin sürümüyle (If-Match) korunur ve kayıt personelin sürümünü ilerletir. staff.manage; şube yöneticisi
/// kapsamındaki personel ve şube için.
/// </summary>
internal static class StaffScheduleEndpoints
{
    public static void Map(RouteGroupBuilder staff)
    {
        staff.MapPut("/{staffId:guid}/services", SetServicesAsync).WithName("manageSetStaffServices")
            .RequirePermission(Permissions.Staff.Manage).RequireIfMatch();
        staff.MapPut("/{staffId:guid}/branches/{branchId:guid}/working-hours", SetHoursAsync).WithName("manageSetStaffWorkingHours")
            .RequirePermission(Permissions.Staff.Manage).RequireIfMatch();
    }

    private static async Task<IResult> SetServicesAsync(
        Guid staffId, StaffServicesRequest body, HttpContext http, ITenantContext tenant, TenancyDbContext db, StaffReader reader,
        IStaffServiceAssignments services, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var staff = await reader.FindAsync(brandId, staffId, http.User, track: true, ct);
        if (staff is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (!db.MatchesVersion(staff, staff.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.Items is null)
        {
            errors["items"] = ["required"];
        }

        var assignments = StaffServiceInput.ToAssignments(body.Items, "items", errors);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var saved = await services.SetAsync(brandId, staffId, assignments!, "items", ct);
        if (!saved.IsSuccess)
        {
            return saved.Error.ToProblem();
        }

        db.Entry(staff).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return VersionedResults.Versioned(http, staff.Version, await reader.DetailAsync(staff, ct));
    }

    private static async Task<IResult> SetHoursAsync(
        Guid staffId, Guid branchId, WeeklyHoursDto body, HttpContext http, ITenantContext tenant, TenancyDbContext db, StaffReader reader,
        IIdGenerator ids, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var staff = await reader.FindAsync(brandId, staffId, http.User, track: true, ct);
        var link = staff is null || !BranchScope.From(http.User).Includes(branchId)
            ? null
            : await db.StaffBranches.SingleOrDefaultAsync(b => b.StaffId == staffId && b.BranchId == branchId && b.IsActive, ct);
        if (staff is null || link is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (!db.MatchesVersion(staff, staff.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var parsed = WeeklyHoursInput.Parse(body);
        if (!parsed.IsSuccess)
        {
            return parsed.Error.ToProblem();
        }

        var (days, hours) = parsed.Value;
        var otherLinks = await db.StaffBranches.AsNoTracking().Where(b => b.StaffId == staffId && b.IsActive && b.Id != link.Id).Select(b => b.Id).ToListAsync(ct);
        var otherRows = await db.StaffHours.AsNoTracking().Where(h => otherLinks.Contains(h.StaffBranchId)).ToListAsync(ct);
        var others = otherRows.GroupBy(h => h.StaffBranchId).Select(g => WeeklyHours.Create(
            [.. g.GroupBy(h => h.DayOfWeek).Select(d => (d.Key, (IReadOnlyList<(LocalTime, LocalTime)>)[.. d.Select(h => (h.StartTime, h.EndTime))]))]).Value);
        var overlap = StaffHoursRules.CheckAgainstOtherBranches(days, hours, others);
        if (!overlap.IsSuccess)
        {
            return overlap.Error.ToProblem();
        }

        await db.StaffHours.Where(h => h.StaffBranchId == link.Id).ExecuteDeleteAsync(ct);
        db.StaffHours.AddRange(hours.Days.SelectMany(d => d.Value.Select(i => StaffWorkingHours.Create(ids.New(), brandId, link.Id, d.Key, i.Start, i.End))));
        db.Entry(staff).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        var dto = new WeeklyHoursDto(
            [.. hours.Days.OrderBy(d => d.Key).Select(d => new WeeklyHoursDayDto(
                (int)d.Key, [.. d.Value.Select(i => new TimeIntervalDto(TimeText.Format(i.Start), TimeText.Format(i.End)))]))],
            staff.Version.ToString(CultureInfo.InvariantCulture));
        return VersionedResults.Versioned(http, staff.Version, dto);
    }
}
