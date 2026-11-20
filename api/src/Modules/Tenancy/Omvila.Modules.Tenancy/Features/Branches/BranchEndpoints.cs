using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Features.Brand;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;

namespace Omvila.Modules.Tenancy.Features.Branches;

/// <summary>
/// Şube uçları (sözleşme manage-branches; AyarlarSube, SubeEkle). Okuma jetondaki şube kapsamıyla süzülür, kapsam
/// dışı şube 404 (roller-yetkiler.md). Değiştirmek settings.manage ister.
/// </summary>
internal static class BranchEndpoints
{
    private static readonly int?[] SlotIntervals = [5, 10, 15];

    public static void Map(IEndpointRouteBuilder app)
    {
        var branches = app.MapApiGroup("/api/v1/manage/branches", Audiences.Manage);

        branches.MapGet(string.Empty, ListAsync).WithName("manageListBranches").RequireAuthenticated(Audiences.Manage);
        branches.MapPost(string.Empty, CreateAsync).WithName("manageCreateBranch")
            .RequirePermission(Permissions.Settings.Manage).RequireIdempotencyKey();
        branches.MapGet("/{branchId:guid}", GetAsync).WithName("manageGetBranch").RequireAuthenticated(Audiences.Manage);
        branches.MapPatch("/{branchId:guid}", UpdateAsync).WithName("manageUpdateBranch")
            .RequirePermission(Permissions.Settings.Manage).RequireIfMatch();
        WorkingHoursEndpoints.Map(branches);
        OverrideEndpoints.Map(branches);
    }

    /// <summary>Kapsamdaki şube; yoksa veya kapsam dışıysa null (404).</summary>
    public static async Task<Domain.Branch?> FindAsync(TenancyDbContext db, ITenantContext tenant, ClaimsPrincipal user, Guid branchId, bool track, CancellationToken ct)
    {
        if (!BranchScope.From(user).Includes(branchId))
        {
            return null;
        }

        var query = track ? db.Branches : db.Branches.AsNoTracking();
        return await query.SingleOrDefaultAsync(b => b.Id == branchId && b.BrandId == BrandOf(tenant), ct);
    }

    private static async Task<IResult> ListAsync(ClaimsPrincipal user, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var scope = BranchScope.From(user);
        var query = db.Branches.AsNoTracking().Where(b => b.BrandId == BrandOf(tenant));
        if (!scope.All)
        {
            query = query.Where(b => scope.BranchIds.Contains(b.Id));
        }

        var items = await query.OrderBy(b => b.CreatedUtc).ToListAsync(ct);
        return TypedResults.Ok(new BranchList([.. items.Select(BranchResponse.From)]));
    }

    private static async Task<IResult> GetAsync(Guid branchId, HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var branch = await FindAsync(db, tenant, http.User, branchId, track: false, ct);
        return branch is null ? Error.NotFound().ToProblem() : Versioned(http, branch.Version, BranchResponse.From(branch));
    }

    private static async Task<IResult> CreateAsync(
        BranchCreateRequest body,
        HttpContext http,
        ITenantContext tenant,
        TenancyDbContext db,
        ISubscriptionQueries subscriptions,
        IOutbox outbox,
        IIdGenerator ids,
        IClock clock,
        CancellationToken ct)
    {
        var brandId = BrandOf(tenant);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var details = body.ToDetails(errors);
        foreach (var (field, sourceId) in new[] { ("copyWorkingHoursFromBranchId", body.CopyWorkingHoursFromBranchId), ("copyServicePricesFromBranchId", body.CopyServicePricesFromBranchId) })
        {
            if (sourceId is { } id && !await db.Branches.AnyAsync(b => b.Id == id && b.BrandId == brandId, ct))
            {
                errors[field] = ["invalid_format"];
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        // Plan sınırı (sözleşme: 403 + meta.planLimit). Sınırlar Platform'daki plan verisinden.
        var limits = await subscriptions.GetPlanLimitsAsync(brandId, ct);
        var plan = limits is null ? PlanLimits.Unlimited : new PlanLimits(limits.StaffLimit, limits.IncludedBranchCount, limits.AllowsExtraBranches);
        var allowed = plan.CanAddBranch(await db.Branches.CountAsync(b => b.BrandId == brandId && b.IsActive, ct));
        if (!allowed.IsSuccess)
        {
            return allowed.Error.ToProblem();
        }

        var taken = (await db.Branches.Where(b => b.BrandId == brandId).Select(b => b.Slug).ToListAsync(ct)).ToHashSet(StringComparer.Ordinal);
        var slug = SlugGenerator.FirstAvailable(details!.Name, taken.Contains).Value;
        var branch = Domain.Branch.Create(ids.New(), brandId, details, slug, PlatformDefaults.DefaultTimeZoneId, clock.GetCurrentInstant());
        db.Branches.Add(branch);
        await db.SaveChangesAsync(ct);

        if (body.CopyWorkingHoursFromBranchId is { } hoursSource)
        {
            var rows = await db.BranchHours.AsNoTracking().Where(h => h.BranchId == hoursSource).ToListAsync(ct);
            var copy = WeeklyHours.Create([.. rows.GroupBy(r => r.DayOfWeek).Select(g =>
                (g.Key, (IReadOnlyList<(LocalTime, LocalTime)>)[.. g.Select(r => (r.StartTime, r.EndTime))]))]).Value;
            db.BranchHours.AddRange(BranchWorkingHours.From(brandId, branch.Id, copy, ids.New));
            await db.SaveChangesAsync(ct);
        }

        await outbox.EnqueueAsync(new BranchCreated(brandId, branch.Id, body.CopyServicePricesFromBranchId), ct);
        http.Response.Headers.ETag = ETagVersion.Format(branch.Version);
        return TypedResults.Created($"/api/v1/manage/branches/{branch.Id}", BranchResponse.From(branch));
    }

    private static async Task<IResult> UpdateAsync(
        Guid branchId, JsonElement body, HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var branch = await FindAsync(db, tenant, http.User, branchId, track: true, ct);
        if (branch is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (!Matches(db, branch, branch.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var input = new JsonBody(body);
        if (!input.IsObject)
        {
            return Error.Validation("body", "invalid_format").ToProblem();
        }

        var details = BranchPatch.Apply(input, branch);
        if (input.Errors.Count > 0)
        {
            return Error.Validation(input.Errors).ToProblem();
        }

        // Rezervasyon ve takvim en az bir etkin şubeye dayanır: son etkin şube kapatılamaz.
        if (branch.IsActive && !details.IsActive
            && !await db.Branches.AnyAsync(b => b.BrandId == branch.BrandId && b.Id != branch.Id && b.IsActive, ct))
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition).ToProblem();
        }

        branch.Update(details);
        await db.SaveChangesAsync(ct);
        return Versioned(http, branch.Version, BranchResponse.From(branch));
    }

    internal static bool IsValidSlotInterval(int? minutes) => minutes is null || SlotIntervals.Contains(minutes);
}
