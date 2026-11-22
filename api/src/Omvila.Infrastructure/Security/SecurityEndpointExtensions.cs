using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    /// Uç yalnız verilen yüzeylerin geçerli oturumunu ister, izin istemez (ör. çıkış). Birden çok yüzey verilirse (ör. rezervasyon
    /// jetonu veya müşteri uygulaması jetonu; sözleşmede iki security seçeneği) uç her birini kabul eder. Her şema kendi aud değeriyle doğrular; jeton yalnız birinde geçer.
    /// </summary>
    public static RouteHandlerBuilder RequireAuthenticated(this RouteHandlerBuilder builder, string audience, params string[] otherAudiences)
    {
        ArgumentNullException.ThrowIfNull(otherAudiences);
        var schemes = string.Join(',', [audience, .. otherAudiences]);
        return builder
            .WithMetadata(new AuthenticatedOnlyMetadata(schemes))
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = schemes });
    }
}
