using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using NodaTime.Text;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>
/// İzinler (IzinEkle; kabul kriteri 12). timeoff.request: Owner ve şube yöneticisi kapsamındaki personel için,
/// personel yalnız kendisi için (roller-yetkiler.md "K"); başkasınınki 404. Tüm gün izin personelin birincil
/// şubesinin saat diliminde hesaplanır. Randevular iptal edilmez; etkilenen randevular (Dilim 2) listelenir.
/// </summary>
internal static class TimeOffEndpoints
{
    public static void Map(RouteGroupBuilder staff)
    {
        staff.MapGet("/{staffId:guid}/time-off", ListAsync).WithName("manageListTimeOff").RequireAuthenticated(Audiences.Manage);
        staff.MapPost("/{staffId:guid}/time-off", CreateAsync).WithName("manageCreateTimeOff").RequirePermission(Permissions.TimeOff.Request);
        staff.MapDelete("/{staffId:guid}/time-off/{timeOffId:guid}", DeleteAsync).WithName("manageDeleteTimeOff").RequirePermission(Permissions.TimeOff.Request);
    }

    private static async Task<IResult> ListAsync(
        Guid staffId, string? from, string? to, ClaimsPrincipal user, ITenantContext tenant, TenancyDbContext db, StaffReader reader, CancellationToken ct)
    {
        var staff = await FindAsync(staffId, user, tenant, reader, ct);
        if (staff is null)
        {
            return Error.NotFound().ToProblem();
        }

        var zone = await ZoneAsync(db, staffId, ct);
        var query = db.TimeOffs.AsNoTracking().Where(t => t.StaffId == staffId);
        if (from is not null)
        {
            if (LocalDatePattern.Iso.Parse(from) is not { Success: true } f)
            {
                return Error.Validation("from", "invalid_format").ToProblem();
            }

            var start = zone.AtStartOfDay(f.Value).ToInstant();
            query = query.Where(t => t.EndUtc > start);
        }

        if (to is not null)
        {
            if (LocalDatePattern.Iso.Parse(to) is not { Success: true } t2)
            {
                return Error.Validation("to", "invalid_format").ToProblem();
            }

            var end = zone.AtStartOfDay(t2.Value.PlusDays(1)).ToInstant();
            query = query.Where(t => t.StartUtc < end);
        }

        var items = await query.OrderBy(t => t.StartUtc).ToListAsync(ct);
        return TypedResults.Ok(new TimeOffList([.. items.Select(TimeOffResponse.From)]));
    }

    private static async Task<IResult> CreateAsync(
        Guid staffId, TimeOffCreateRequest body, ClaimsPrincipal user, ITenantContext tenant, TenancyDbContext db, StaffReader reader,
        IIdGenerator ids, IClock clock, CancellationToken ct)
    {
        var staff = await FindAsync(staffId, user, tenant, reader, ct);
        if (staff is null)
        {
            return Error.NotFound().ToProblem();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.Type is null || !Enum.GetNames<TimeOffType>().Contains(body.Type, StringComparer.Ordinal))
        {
            errors["type"] = [body.Type is null ? "required" : "invalid_format"];
        }

        if (body.IsAllDay is null)
        {
            errors["isAllDay"] = ["required"];
        }

        if (body.Note?.Trim().Length > 200)
        {
            errors["note"] = ["too_long"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var period = body.IsAllDay!.Value
            ? TimeOffPeriod.AllDay(body.FromDate, body.ToDate, await ZoneAsync(db, staffId, ct))
            : TimeOffPeriod.Timed(body.StartUtc, body.EndUtc);
        if (!period.IsSuccess)
        {
            return period.Error.ToProblem();
        }

        var timeOff = TimeOff.Create(ids.New(), staff.BrandId, staffId, Enum.Parse<TimeOffType>(body.Type!), period.Value, body.Note, clock.GetCurrentInstant());
        db.TimeOffs.Add(timeOff);
        await db.SaveChangesAsync(ct);

        // Randevular Dilim 2'de gelir; o zamana kadar etkilenen randevu yoktur (kabul kriteri 12: boş liste).
        return TypedResults.Created(
            $"/api/v1/manage/staff/{staffId}/time-off/{timeOff.Id}", new TimeOffCreated(TimeOffResponse.From(timeOff), []));
    }

    private static async Task<IResult> DeleteAsync(
        Guid staffId, Guid timeOffId, ClaimsPrincipal user, ITenantContext tenant, TenancyDbContext db, StaffReader reader, CancellationToken ct)
    {
        if (await FindAsync(staffId, user, tenant, reader, ct) is null)
        {
            return Error.NotFound().ToProblem();
        }

        var deleted = await db.TimeOffs.Where(t => t.Id == timeOffId && t.StaffId == staffId).ExecuteDeleteAsync(ct);
        return deleted == 0 ? Error.NotFound().ToProblem() : TypedResults.NoContent();
    }

    // Personel rolü yalnız kendi kaydını görür ve değiştirir; diğerleri şube kapsamıyla.
    private static async Task<Domain.Staff?> FindAsync(Guid staffId, ClaimsPrincipal user, ITenantContext tenant, StaffReader reader, CancellationToken ct)
    {
        var isStaffRole = string.Equals(user.FindFirst(OmvilaClaims.Role)?.Value, Roles.Staff, StringComparison.Ordinal);
        if (isStaffRole && !string.Equals(user.FindFirst(OmvilaClaims.StaffId)?.Value, staffId.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return await reader.FindAsync(tenant.RequireBrandId(), staffId, user, track: false, ct);
    }

    private static async Task<DateTimeZone> ZoneAsync(TenancyDbContext db, Guid staffId, CancellationToken ct)
    {
        var zoneId = await (from l in db.StaffBranches.AsNoTracking()
                            join b in db.Branches.AsNoTracking() on l.BranchId equals b.Id
                            where l.StaffId == staffId && l.IsActive
                            orderby l.IsPrimary descending
                            select b.TimeZoneId).FirstOrDefaultAsync(ct);
        return DateTimeZoneProviders.Tzdb[zoneId ?? PlatformDefaults.DefaultTimeZoneId];
    }
}
