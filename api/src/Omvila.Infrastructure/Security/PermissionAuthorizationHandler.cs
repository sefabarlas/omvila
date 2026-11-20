using Microsoft.AspNetCore.Authorization;

namespace Omvila.Infrastructure.Security;

/// <summary>Jetondaki rolün izni var mı (kapsam sorguda uygulanır). Yoksa 403 forbidden.</summary>
internal sealed class PermissionAuthorizationHandler(IPermissionResolver resolver) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var role = context.User.FindFirst(OmvilaClaims.Role)?.Value;
        if (role is not null && resolver.Resolve(role, requirement.Permission) is not null)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
