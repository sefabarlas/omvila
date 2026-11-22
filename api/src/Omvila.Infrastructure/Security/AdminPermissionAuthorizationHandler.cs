using Microsoft.AspNetCore.Authorization;

namespace Omvila.Infrastructure.Security;

/// <summary>
/// Jetondaki platform_role iddiasının izni var mı (admin yüzeyi; ADR-0007). Salon jetonunun role iddiası burada sayılmaz;
/// jeton zaten admin şemasıyla (aud=admin) doğrulanmıştır. Yoksa 403 forbidden.
/// </summary>
internal sealed class AdminPermissionAuthorizationHandler(IAdminPermissionResolver resolver) : AuthorizationHandler<AdminPermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminPermissionRequirement requirement)
    {
        var role = context.User.FindFirst(OmvilaClaims.PlatformRole)?.Value;
        if (role is not null && resolver.HasPermission(role, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
