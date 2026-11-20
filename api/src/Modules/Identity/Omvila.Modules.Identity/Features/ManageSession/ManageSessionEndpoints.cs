using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.Modules.Identity.Security;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Features.ManageSession;

/// <summary>
/// Panel açılış özeti (sözleşme manageGetSession). Rol, şube kapsamı ve izinler jetondan değil veritabanındaki üyelikten
/// okunur (T-19): panel menüsü her açılışta güncel yetkiyle kurulur. Üyelik etkin değilse 401.
/// </summary>
internal static class ManageSessionEndpoints
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapApiGroup("/api/v1/manage/session", Audiences.Manage)
            .MapGet(string.Empty, GetAsync)
            .WithName("manageGetSession")
            .RequireAuthenticated(Audiences.Manage);

    private static async Task<IResult> GetAsync(
        ClaimsPrincipal principal, IdentityDbContext db, ITenantQueries tenants, ISubscriptionQueries subscriptions, CancellationToken ct)
    {
        if (!Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var userId)
            || !Guid.TryParse(principal.FindFirst(OmvilaClaims.MembershipId)?.Value, out var membershipId))
        {
            return Error.Unauthorized().ToProblem();
        }

        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId && u.IsActive, ct);
        var membership = await db.Memberships.AsNoTracking()
            .SingleOrDefaultAsync(m => m.Id == membershipId && m.UserId == userId && m.Status == MembershipStatus.Active, ct);
        if (user is null || membership is null)
        {
            return Error.Unauthorized().ToProblem();
        }

        // Owner ve boş kapsam = bütün şubeler (sözleşme: boş dizi).
        var allBranches = string.Equals(membership.Role, Roles.Owner, StringComparison.Ordinal) || membership.BranchIds.Length == 0;
        var tenant = await tenants.GetSummaryAsync(membership.BrandId, allBranches ? null : membership.BranchIds, ct);
        var subscription = await subscriptions.GetBannerAsync(membership.BrandId, ct);
        if (tenant is null || subscription is null)
        {
            return Error.Unauthorized().ToProblem();
        }

        var permissions = RolePermissions.ByRole.TryGetValue(membership.Role, out var granted)
            ? granted.Keys.Order(StringComparer.Ordinal).ToArray()
            : [];
        var phone = PhoneNumber.TryParse(user.PhoneE164, out var parsed) ? parsed.Masked : string.Empty;

        return TypedResults.Ok(new ManageSessionResponse(
            new SessionUser(user.Id, user.FullName ?? string.Empty, phone),
            new SessionBrand(tenant.BrandId, tenant.Name, tenant.Slug, tenant.LogoUrl, tenant.Status),
            new SessionMembership(membership.Role, allBranches ? [] : membership.BranchIds, membership.StaffId),
            permissions,
            [.. tenant.Branches.Select(b => new SessionBranch(b.Id, b.Name))],
            new SessionSubscription(subscription.Status, subscription.TrialEndsOn, subscription.PastDueSince, subscription.ReadOnlyOn)));
    }
}
