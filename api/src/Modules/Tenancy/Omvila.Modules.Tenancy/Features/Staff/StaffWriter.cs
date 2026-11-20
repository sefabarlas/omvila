using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.Staff;

/// <summary>
/// Personel ekleme ve düzenleme. Tenancy personel kaydını ve şube bağlarını, Identity üyeliği ve daveti, Catalog
/// personelin hizmetlerini tutar; hepsi aynı istek transaction'ındadır.
/// </summary>
internal sealed class StaffWriter(
    TenancyDbContext db,
    ITenantContext tenant,
    StaffReader reader,
    IStaffMemberships memberships,
    IStaffServiceAssignments services,
    ISubscriptionQueries subscriptions,
    IIdGenerator ids,
    IClock clock)
{
    public async Task<IResult> CreateAsync(StaffCreateRequest body, HttpContext http, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var scope = BranchScope.From(http.User);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var role = ParseRole(body.Role, errors);
        var branchIds = (body.BranchIds ?? []).Distinct().ToList();
        var draft = new StaffDraft(
            body.FullName ?? string.Empty, body.Phone, role ?? StaffRole.Staff, body.CalendarColor ?? string.Empty, branchIds,
            body.ServiceCommissionPercent ?? 0, body.SaleCommissionPercent ?? 0, body.SendInvite ?? true);
        var phone = StaffRules.Validate(draft);
        if (!phone.IsSuccess)
        {
            Merge(errors, phone.Error);
        }

        if (body.Title?.Trim().Length > 60)
        {
            errors["title"] = ["too_long"];
        }

        await CheckBranchesAsync(brandId, branchIds, errors, ct);
        var assignments = StaffServiceInput.ToAssignments(body.Services, "services", errors);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        // Şube yöneticisi yalnız kendi şubelerine ve yalnız Staff rolüyle personel ekler.
        if (!scope.All && (role != StaffRole.Staff || !branchIds.TrueForAll(scope.Includes)))
        {
            return Error.Forbidden().ToProblem();
        }

        var allowed = await CanAddStaffAsync(brandId, ct);
        if (!allowed.IsSuccess)
        {
            return allowed.Error.ToProblem();
        }

        var staff = await AddStaffAsync(body, draft, branchIds, ct);

        var assigned = await services.SetAsync(brandId, staff.Id, assignments!, "services", ct);
        if (!assigned.IsSuccess)
        {
            return assigned.Error.ToProblem();
        }

        var invited = phone.Value is { } mobile ? await InviteAsync(staff, mobile, role!.Value, branchIds, draft.SendInvite, ct) : Result.Success();
        if (!invited.IsSuccess)
        {
            return invited.Error.ToProblem();
        }

        http.Response.Headers.ETag = ETagVersion.Format(staff.Version);
        return TypedResults.Created($"/api/v1/manage/staff/{staff.Id}", await reader.DetailAsync(staff, ct));
    }

    public async Task<IResult> UpdateAsync(Guid staffId, Brand.JsonBody input, HttpContext http, CancellationToken ct)
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

        var patch = StaffPatch.Read(input, staff);
        if (patch.BranchIds is { } requested)
        {
            await CheckBranchesAsync(brandId, requested, input.Errors, ct);
        }

        if (input.Errors.Count > 0)
        {
            return Error.Validation(input.Errors).ToProblem();
        }

        var membership = (await memberships.GetByStaffAsync(brandId, [staffId], ct)).GetValueOrDefault(staffId);
        var denied = Authorize(http.User, membership, patch);
        if (denied is not null)
        {
            return denied;
        }

        staff.Update(patch.Profile);
        db.Entry(staff).State = EntityState.Modified;
        if (patch.BranchIds is { } branchIds)
        {
            await SyncBranchesAsync(staff, branchIds, ct);
        }

        await db.SaveChangesAsync(ct);
        await memberships.UpdateAsync(brandId, staffId, new MembershipChange(patch.Role?.ToString(), patch.BranchIds, patch.Profile.IsActive ? null : false), ct);
        return VersionedResults.Versioned(http, staff.Version, await reader.DetailAsync(staff, ct));
    }

    private async Task<Domain.Staff> AddStaffAsync(StaffCreateRequest body, StaffDraft draft, List<Guid> branchIds, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var staff = Domain.Staff.Create(
            ids.New(), brandId, draft.FullName, string.IsNullOrWhiteSpace(body.Title) ? null : body.Title.Trim(), draft.CalendarColor,
            draft.ServiceCommissionPercent, draft.SaleCommissionPercent, body.IsBookableOnline ?? true, clock.GetCurrentInstant());
        db.Staff.Add(staff);
        await db.SaveChangesAsync(ct);
        var links = branchIds.Select((branchId, i) => StaffBranch.Create(ids.New(), brandId, staff.Id, branchId, isPrimary: i == 0)).ToList();
        db.StaffBranches.AddRange(links);
        await db.SaveChangesAsync(ct);
        if (body.CopyBranchWorkingHours ?? true)
        {
            await CopyPrimaryBranchHoursAsync(links[0], ct);
        }

        return staff;
    }

    private async Task<Result> CanAddStaffAsync(Guid brandId, CancellationToken ct)
    {
        var limits = await subscriptions.GetPlanLimitsAsync(brandId, ct);
        var plan = limits is null ? PlanLimits.Unlimited : new PlanLimits(limits.StaffLimit, limits.IncludedBranchCount, limits.AllowsExtraBranches);
        return plan.CanAddStaff(await db.Staff.CountAsync(s => s.BrandId == brandId && s.IsActive, ct));
    }

    private async Task<Result> InviteAsync(Domain.Staff staff, PhoneNumber mobile, StaffRole role, IReadOnlyCollection<Guid> branchIds, bool sendInvite, CancellationToken ct)
    {
        var brandName = await db.Brands.Where(b => b.Id == staff.BrandId).Select(b => b.Name).SingleAsync(ct);
        var invited = await memberships.InviteAsync(
            new StaffInvitation(staff.BrandId, brandName, staff.Id, mobile.E164, staff.FullName, role.ToString(), branchIds, sendInvite), ct);
        return invited.IsSuccess ? Result.Success() : invited.Error;
    }

    // Sahibin rolü ve etkinliği değişmez; şube yöneticisi yönetici atayamaz ve kapsamı dışına taşıyamaz.
    private static IResult? Authorize(System.Security.Claims.ClaimsPrincipal user, MembershipInfo? membership, StaffPatch patch)
    {
        var isOwner = string.Equals(membership?.Role, Roles.Owner, StringComparison.Ordinal);
        if (isOwner && (patch.Role is not null || !patch.Profile.IsActive))
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition).ToProblem();
        }

        var scope = BranchScope.From(user);
        if (!scope.All && (patch.Role == StaffRole.BranchManager || isOwner
            || string.Equals(membership?.Role, Roles.BranchManager, StringComparison.Ordinal)
            || (patch.BranchIds is { } ids && !ids.All(scope.Includes))))
        {
            return Error.Forbidden().ToProblem();
        }

        return null;
    }

    private async Task CheckBranchesAsync(Guid brandId, IReadOnlyCollection<Guid> branchIds, Dictionary<string, string[]> errors, CancellationToken ct)
    {
        var known = await db.Branches.CountAsync(b => b.BrandId == brandId && b.IsActive && branchIds.Contains(b.Id), ct);
        if (branchIds.Count > 0 && known != branchIds.Count)
        {
            errors["branchIds"] = ["invalid_format"];
        }
    }

    // "İşletme saatleriyle aynı": birincil şubenin saatleri kopyalanır. Diğer şubeler boş başlar; bir personelin
    // şubeler arası saatleri örtüşemeyeceği için aynı saatler bütün şubelere kopyalanmaz (T-32).
    private async Task CopyPrimaryBranchHoursAsync(StaffBranch link, CancellationToken ct)
    {
        var rows = await db.BranchHours.AsNoTracking().Where(h => h.BranchId == link.BranchId).ToListAsync(ct);
        db.StaffHours.AddRange(rows.Select(h => StaffWorkingHours.Create(ids.New(), link.BrandId, link.Id, h.DayOfWeek, h.StartTime, h.EndTime)));
        await db.SaveChangesAsync(ct);
    }

    private async Task SyncBranchesAsync(Domain.Staff staff, IReadOnlyList<Guid> branchIds, CancellationToken ct)
    {
        var links = await db.StaffBranches.Where(b => b.StaffId == staff.Id).ToListAsync(ct);
        foreach (var link in links)
        {
            link.SetActive(branchIds.Contains(link.BranchId));
        }

        db.StaffBranches.AddRange(branchIds.Where(id => links.TrueForAll(l => l.BranchId != id))
            .Select(id => StaffBranch.Create(ids.New(), staff.BrandId, staff.Id, id, isPrimary: links.Count == 0 && id == branchIds[0])));
    }

    private static StaffRole? ParseRole(string? role, Dictionary<string, string[]> errors)
    {
        if (role is not null && Enum.GetNames<StaffRole>().Contains(role, StringComparer.Ordinal))
        {
            return Enum.Parse<StaffRole>(role);
        }

        errors["role"] = [role is null ? "required" : "invalid_format"];
        return null;
    }

    private static void Merge(Dictionary<string, string[]> errors, Error error)
    {
        foreach (var (field, codes) in error.FieldErrors)
        {
            errors[field] = codes;
        }
    }
}
