using System.Security.Claims;
using Omvila.Infrastructure.Security;

namespace Omvila.Infrastructure.Persistence;

/// <summary>Kiracı bağlamı yalnız doğrulanmış jetondan (ADR-0006); istek gövdesinden veya başlıktan okunmaz.</summary>
internal static class TenantResolution
{
    public static TenantScope? FromUser(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return user.FindFirst("aud")?.Value switch
        {
            Audiences.Manage when Guid.TryParse(user.FindFirst(OmvilaClaims.BrandId)?.Value, out var brand) => TenantScope.ForBrand(brand),
            Audiences.Admin => TenantScope.Platform,
            Audiences.Me when Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var customer) => TenantScope.ForCustomer(customer),
            _ => null,
        };
    }
}
