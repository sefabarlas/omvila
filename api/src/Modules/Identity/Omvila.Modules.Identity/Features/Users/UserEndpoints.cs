using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.Users;

/// <summary>
/// Ayarlar → Roller ve yetkiler (K6; sözleşme manageListUsers, manageUpdateUser, manageGetRoleMatrix). Kullanıcı ekleme ayrı
/// uç değildir (personel daveti, manageCreateStaff). Rol değişikliği yalnız üyeliği değiştirir; personelin çalıştığı şubeler
/// (personel kaydı) personel düzenlemesindedir. Açık oturumlar yeni izinleri bir sonraki jeton yenilemede alır.
/// </summary>
internal static class UserEndpoints
{
    private static readonly string[] AssignableRoles = [Roles.Owner, Roles.BranchManager, Roles.Staff];

    public static void Map(IEndpointRouteBuilder app)
    {
        var users = app.MapApiGroup("/api/v1/manage/users", Audiences.Manage);
        users.MapGet(string.Empty, ListAsync).WithName("manageListUsers").RequirePermission(Permissions.Settings.Roles);
        users.MapPatch("/{userId:guid}", UpdateAsync).WithName("manageUpdateUser").RequirePermission(Permissions.Settings.Roles).RequireIfMatch();

        app.MapApiGroup("/api/v1/manage/roles", Audiences.Manage)
            .MapGet(string.Empty, () => TypedResults.Ok(RoleMatrix.Build()))
            .WithName("manageGetRoleMatrix")
            .RequirePermission(Permissions.Settings.Roles);
    }

    private static async Task<IResult> ListAsync(
        ClaimsPrincipal principal, ITenantContext tenant, IdentityDbContext db, ITenantQueries tenants, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var currentUserId = CurrentUser(principal);
        var rows = await (from m in db.Memberships.AsNoTracking()
                          join u in db.Users.AsNoTracking() on m.UserId equals u.Id
                          where m.BrandId == brandId
                          select new { Membership = m, User = u }).ToListAsync(ct);
        var staffIds = rows.Where(r => r.Membership.StaffId is not null).Select(r => r.Membership.StaffId!.Value).ToList();
        var cards = await tenants.GetStaffCardsAsync(brandId, staffIds, ct);
        var items = rows
            .Select(r => ToResponse(r.Membership, r.User, Name(r.Membership, r.User, cards), currentUserId))
            .OrderBy(r => r.FullName, StringComparer.Create(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"), ignoreCase: true))
            .ThenBy(r => r.UserId)
            .ToList();
        return TypedResults.Ok(new ManageUserList(items));
    }

    // Kural: roller-yetkiler.md § Salon rolleri ("Owner en az bir tane olmak zorunda"), sözleşme manageUpdateUser
    private static async Task<IResult> UpdateAsync(
        Guid userId,
        ManageUserUpdate body,
        HttpContext http,
        ITenantContext tenant,
        IdentityDbContext db,
        ITenantQueries tenants,
        IAuditLog audit,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var (errors, role, branchIds) = await ValidateAsync(body, brandId, tenants, ct);
        var isOwner = string.Equals(role, Roles.Owner, StringComparison.Ordinal);
        var membership = await db.Memberships.SingleOrDefaultAsync(m => m.BrandId == brandId && m.UserId == userId, ct);
        if (membership is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (!db.MatchesVersion(membership, membership.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        if (string.Equals(membership.Role, Roles.Owner, StringComparison.Ordinal) && !isOwner && !await HasAnotherOwnerAsync(db, brandId, membership.Id, ct))
        {
            return Error.Conflict(ErrorCodes.LastOwnerRequired).ToProblem();
        }

        var previousRole = membership.Role;
        membership.Reassign(role!, branchIds);
        await db.SaveChangesAsync(ct);

        var user = await db.Users.AsNoTracking().SingleAsync(u => u.Id == membership.UserId, ct);
        var cards = membership.StaffId is { } staffId ? await tenants.GetStaffCardsAsync(brandId, [staffId], ct) : null;
        var actor = CurrentUser(http.User);
        await audit.WriteAsync(
            new AuditEntry(
                brandId,
                AuditActorType.User,
                actor,
                AuditAction.Update,
                nameof(UserMembership),
                membership.Id,
                new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["userId"] = membership.UserId,
                    ["previousRole"] = previousRole,
                    ["role"] = membership.Role,
                    ["branchIds"] = membership.BranchIds,
                },
                http.Connection.RemoteIpAddress?.ToString(),
                http.Request.Headers.UserAgent.ToString() is { Length: > 0 } ua ? ua : null),
            ct);
        return VersionedResults.Versioned(http, membership.Version, ToResponse(membership, user, Name(membership, user, cards), actor));
    }

    private static async Task<(Dictionary<string, string[]> Errors, string? Role, List<Guid> BranchIds)> ValidateAsync(
        ManageUserUpdate body, Guid brandId, ITenantQueries tenants, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var role = body.Role;
        if (role is null)
        {
            errors["role"] = ["required"];
        }
        else if (!AssignableRoles.Contains(role, StringComparer.Ordinal))
        {
            errors["role"] = ["invalid_format"];
        }

        var isOwner = string.Equals(role, Roles.Owner, StringComparison.Ordinal);
        var branchIds = (body.BranchIds ?? []).Distinct().ToList();
        if (body.BranchIds is null || (!isOwner && branchIds.Count == 0))
        {
            errors["branchIds"] = ["required"];
        }
        else if (!isOwner)
        {
            var summary = await tenants.GetSummaryAsync(brandId, null, ct);
            var known = summary?.Branches.Select(b => b.Id).ToHashSet() ?? [];
            if (!branchIds.TrueForAll(known.Contains))
            {
                errors["branchIds"] = ["invalid_format"];
            }
        }

        return (errors, role, branchIds);
    }

    // Diğer Owner üyelikleri kilitlenir: iki Owner'ın birbirini aynı anda başka role alması son Owner'ı düşüremez.
    private static async Task<bool> HasAnotherOwnerAsync(IdentityDbContext db, Guid brandId, Guid membershipId, CancellationToken ct)
    {
        var owners = await db.Memberships
            .FromSql($"SELECT *, xmin FROM identity.user_memberships WHERE brand_id = {brandId} AND role = 'Owner' FOR UPDATE")
            .AsNoTracking()
            .ToListAsync(ct);
        return owners.Exists(o => o.Id != membershipId && o.Status != MembershipStatus.Disabled);
    }

    private static Guid? CurrentUser(ClaimsPrincipal principal) =>
        Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : null;

    // Panelde düzenlenen ad personel kaydındadır; personel kaydı olmayan üyelikte kullanıcının adı.
    private static string Name(UserMembership m, User u, IReadOnlyDictionary<Guid, StaffCard>? cards) =>
        m.StaffId is { } id && cards is not null && cards.TryGetValue(id, out var card) ? card.FullName : u.FullName ?? string.Empty;

    private static ManageUserResponse ToResponse(UserMembership m, User u, string fullName, Guid? currentUserId) => new(
        u.Id,
        fullName,
        PhoneNumber.TryParse(u.PhoneE164, out var phone) ? phone.Masked : string.Empty,
        m.Role,
        string.Equals(m.Role, Roles.Owner, StringComparison.Ordinal) ? [] : m.BranchIds,
        m.Status.ToString(),
        m.StaffId,
        u.Id == currentUserId,
        ETagVersion.Format(m.Version));
}
