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

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>
/// Özel günler (kabul kriteri 6). Çok günlük kapanış tarih başına satıra açılır; aynı tarihte var olan özel gün
/// yenisiyle değiştirilir (T-29). Randevular otomatik iptal edilmez (randevu Dilim 2).
/// </summary>
internal static class OverrideEndpoints
{
    // Tek istekte açılabilecek gün sayısı: yıllık kapanıştan uzun aralık giriş hatasıdır.
    private const int MaxDaysPerRequest = 366;

    public static void Map(RouteGroupBuilder branches)
    {
        branches.MapGet("/{branchId:guid}/overrides", ListAsync).WithName("manageListBranchOverrides").RequireAuthenticated(Audiences.Manage);
        branches.MapPost("/{branchId:guid}/overrides", CreateAsync).WithName("manageCreateBranchOverride")
            .RequirePermission(Permissions.Settings.Manage);
        branches.MapDelete("/{branchId:guid}/overrides/{overrideId:guid}", DeleteAsync).WithName("manageDeleteBranchOverride")
            .RequirePermission(Permissions.Settings.Manage);
    }

    private static async Task<IResult> ListAsync(
        Guid branchId, string? from, string? to, HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        if (await BranchEndpoints.FindAsync(db, tenant, http.User, branchId, track: false, ct) is null)
        {
            return Error.NotFound().ToProblem();
        }

        // Sorgu tarihleri ISO (yyyy-MM-dd); NodaTime tipleri sorgu bağlamada doğrudan çözülmez.
        var fromDate = ParseDate(from);
        var toDate = ParseDate(to);
        if ((from is not null && fromDate is null) || (to is not null && toDate is null))
        {
            return Error.Validation(fromDate is null && from is not null ? "from" : "to", "invalid_format").ToProblem();
        }

        var query = db.Overrides.AsNoTracking().Where(o => o.BranchId == branchId);
        if (fromDate is { } f)
        {
            query = query.Where(o => o.Date >= f);
        }

        if (toDate is { } t)
        {
            query = query.Where(o => o.Date <= t);
        }

        var items = await query.OrderBy(o => o.Date).ToListAsync(ct);
        return TypedResults.Ok(new OverrideList([.. items.Select(ToDto)]));
    }

    private static async Task<IResult> CreateAsync(
        Guid branchId, OverrideCreateRequest body, HttpContext http, ITenantContext tenant, TenancyDbContext db, IIdGenerator ids, IClock clock, CancellationToken ct)
    {
        var branch = await BranchEndpoints.FindAsync(db, tenant, http.User, branchId, track: false, ct);
        if (branch is null)
        {
            return Error.NotFound().ToProblem();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.FromDate is null)
        {
            errors["fromDate"] = ["required"];
        }

        if (!Enum.TryParse<OverrideKind>(body.Kind, out var kind) || !Enum.GetNames<OverrideKind>().Contains(body.Kind, StringComparer.Ordinal))
        {
            errors["kind"] = [body.Kind is null ? "required" : "invalid_format"];
        }

        if (body.IsClosed is null)
        {
            errors["isClosed"] = ["required"];
        }

        if (body.Note?.Trim().Length > 200)
        {
            errors["note"] = ["too_long"];
        }

        if (body.FromDate is { } start && body.ToDate is { } end && Period.Between(start, end, PeriodUnits.Days).Days >= MaxDaysPerRequest)
        {
            errors["toDate"] = ["out_of_range"];
        }

        var intervals = TimeText.Parse(body.Intervals, "intervals", errors);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var days = DayOverrides.Create(body.FromDate!.Value, body.ToDate, kind, body.IsClosed!.Value, intervals!, body.Note);
        if (!days.IsSuccess)
        {
            return days.Error.ToProblem();
        }

        var dates = days.Value.Select(d => d.Date).ToList();
        await db.Overrides.Where(o => o.BranchId == branchId && dates.Contains(o.Date)).ExecuteDeleteAsync(ct);
        var now = clock.GetCurrentInstant();
        var rows = days.Value.Select(day => WorkingHoursOverride.ForBranch(ids.New(), branch.BrandId, branchId, day, now)).ToList();
        db.Overrides.AddRange(rows);
        await db.SaveChangesAsync(ct);

        // Sözleşme tek özel gün döner; çok günlü kapanışta ilk gün.
        return TypedResults.Created($"/api/v1/manage/branches/{branchId}/overrides/{rows[0].Id}", ToDto(rows[0]));
    }

    private static async Task<IResult> DeleteAsync(
        Guid branchId, Guid overrideId, HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        if (await BranchEndpoints.FindAsync(db, tenant, http.User, branchId, track: false, ct) is null)
        {
            return Error.NotFound().ToProblem();
        }

        var deleted = await db.Overrides.Where(o => o.Id == overrideId && o.BranchId == branchId).ExecuteDeleteAsync(ct);
        return deleted == 0 ? Error.NotFound().ToProblem() : TypedResults.NoContent();
    }

    private static LocalDate? ParseDate(string? text) =>
        text is not null && NodaTime.Text.LocalDatePattern.Iso.Parse(text) is { Success: true } parsed ? parsed.Value : null;

    private static OverrideDto ToDto(WorkingHoursOverride o) => new(
        o.Id,
        o.Date,
        o.Kind.ToString(),
        o.IsClosed,
        [.. o.OpeningIntervals.Select(i => new TimeIntervalDto(TimeText.Format(i.Start), TimeText.Format(i.End)))],
        o.Note);
}
