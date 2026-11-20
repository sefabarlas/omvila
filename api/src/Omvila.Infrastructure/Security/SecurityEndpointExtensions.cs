using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Omvila.Infrastructure.Security;

public static class SecurityEndpointExtensions
{
    /// <summary>Uç bir izin ister (roller-yetkiler.md § Uygulama kuralları 1). Yoksa 403.</summary>
    public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, string permission) =>
        builder
            .WithMetadata(new PermissionMetadata(permission))
            .RequireAuthorization(policy => policy.AddRequirements(new PermissionRequirement(permission)))
            .ProducesProblem(StatusCodes.Status403Forbidden);
}
