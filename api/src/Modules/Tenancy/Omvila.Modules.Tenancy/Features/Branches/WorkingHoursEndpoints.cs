using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>
/// Şubenin haftalık saatleri (kabul kriteri 5). Kayıt bütün haftayı değiştirir. Sürüm şube satırının sürümüdür:
/// saat kaydı şubenin sürümünü de ilerletir (T-29).
/// </summary>
internal static class WorkingHoursEndpoints
{
    public static void Map(RouteGroupBuilder branches)
    {
        branches.MapGet("/{branchId:guid}/working-hours", GetAsync).WithName("manageGetBranchWorkingHours").RequireAuthenticated(Audiences.Manage);
        branches.MapPut("/{branchId:guid}/working-hours", SetAsync).WithName("manageSetBranchWorkingHours")
            .RequirePermission(Permissions.Settings.Manage).RequireIfMatch();
    }

    private static async Task<IResult> GetAsync(Guid branchId, HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var branch = await BranchEndpoints.FindAsync(db, tenant, http.User, branchId, track: false, ct);
        if (branch is null)
        {
            return Error.NotFound().ToProblem();
        }

        var rows = await db.BranchHours.AsNoTracking().Where(h => h.BranchId == branchId).ToListAsync(ct);
        return Versioned(http, branch.Version, ToDto(rows, branch.Version));
    }

    private static async Task<IResult> SetAsync(
        Guid branchId, WeeklyHoursDto body, HttpContext http, ITenantContext tenant, TenancyDbContext db, IIdGenerator ids, CancellationToken ct)
    {
        var branch = await BranchEndpoints.FindAsync(db, tenant, http.User, branchId, track: true, ct);
        if (branch is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (!Matches(db, branch, branch.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var days = body.Days ?? [];
        if (body.Days is null)
        {
            errors["days"] = ["required"];
        }
        else if (days.Count > 7)
        {
            errors["days"] = ["too_long"];
        }

        var raw = new List<(IsoDayOfWeek, IReadOnlyList<(LocalTime, LocalTime)>)>();
        for (var i = 0; i < days.Count && errors.Count == 0; i++)
        {
            if (days[i].DayOfWeek is < 1 or > 7)
            {
                errors[$"days[{i}].dayOfWeek"] = ["out_of_range"];
                break;
            }

            if (TimeText.Parse(days[i].Intervals, $"days[{i}].intervals", errors) is { } intervals)
            {
                raw.Add(((IsoDayOfWeek)days[i].DayOfWeek, intervals));
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var hours = WeeklyHours.Create(raw);
        if (!hours.IsSuccess)
        {
            return hours.Error.ToProblem();
        }

        await db.BranchHours.Where(h => h.BranchId == branchId).ExecuteDeleteAsync(ct);
        db.BranchHours.AddRange(BranchWorkingHours.From(branch.BrandId, branchId, hours.Value, ids.New));
        db.Entry(branch).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);

        var rows = await db.BranchHours.AsNoTracking().Where(h => h.BranchId == branchId).ToListAsync(ct);
        return Versioned(http, branch.Version, ToDto(rows, branch.Version));
    }

    private static WeeklyHoursDto ToDto(IEnumerable<BranchWorkingHours> rows, uint version) => new(
        [.. rows.GroupBy(r => r.DayOfWeek).OrderBy(g => g.Key).Select(g => new WeeklyHoursDayDto(
            (int)g.Key,
            [.. g.OrderBy(r => r.StartTime).Select(r => new TimeIntervalDto(TimeText.Format(r.StartTime), TimeText.Format(r.EndTime)))]))],
        version.ToString(CultureInfo.InvariantCulture));
}
