using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Security;

namespace Omvila.TestKit;

/// <summary>
/// roller-yetkiler.md § Uygulama kuralları 1: her API ucu ya bir izin ister ya da açıkça anonimdir
/// (izin gerektirmeyen oturumlu işlemler RequireAuthenticated ile işaretlenir).
/// İkisini de taşımayan "/api/" uçlarını döner.
/// </summary>
public static class EndpointAudit
{
    public static IReadOnlyList<string> FindUnmarkedApiEndpoints(IServiceProvider services) =>
        services.GetServices<EndpointDataSource>()
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Where(e => e.RoutePattern.RawText?.StartsWith("/api/", StringComparison.Ordinal) == true)
            .Where(e => e.Metadata.GetMetadata<PermissionMetadata>() is null
                && e.Metadata.GetMetadata<IAllowAnonymous>() is null
                && e.Metadata.GetMetadata<AuthenticatedOnlyMetadata>() is null)
            .Select(e => e.RoutePattern.RawText!)
            .Order(StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// roller-yetkiler.md § Platform rolleri, ADR-0007 admin yüzeyi: "/api/v1/admin" uçları ya platform izni (platform.*) ister
    /// ve admin şemasıyla doğrulanır, ya yalnız admin oturumu ister, ya da giriş öncesi (auth/otp, auth/refresh) anonimdir. Salon
    /// uçları platform izni istemez. Kuralı çiğneyen uçları "yol: neden" olarak döner.
    /// </summary>
    public static IReadOnlyList<string> FindMisdeclaredAdminEndpoints(IServiceProvider services)
    {
        var problems = new List<string>();
        foreach (var e in services.GetServices<EndpointDataSource>().SelectMany(s => s.Endpoints).OfType<RouteEndpoint>())
        {
            var route = e.RoutePattern.RawText ?? string.Empty;
            if (!route.StartsWith("/api/", StringComparison.Ordinal))
            {
                continue;
            }

            var permission = e.Metadata.GetMetadata<PermissionMetadata>()?.Permission;
            var isPlatformPermission = permission?.StartsWith("platform.", StringComparison.Ordinal) == true;
            var isAdmin = route.StartsWith("/api/v1/admin/", StringComparison.Ordinal);
            if (!isAdmin)
            {
                if (isPlatformPermission)
                {
                    problems.Add($"{route}: admin dışı uç platform izni istiyor");
                }

                continue;
            }

            var schemes = e.Metadata.GetOrderedMetadata<IAuthorizeData>().Select(a => a.AuthenticationSchemes).Where(s => s is not null).ToList();
            if (e.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            {
                if (!route.StartsWith("/api/v1/admin/auth/", StringComparison.Ordinal) || route.EndsWith("/logout", StringComparison.Ordinal))
                {
                    problems.Add($"{route}: anonim admin ucu yalnız giriş öncesi olabilir");
                }
            }
            else if (e.Metadata.GetMetadata<AuthenticatedOnlyMetadata>() is { } authenticated)
            {
                if (!string.Equals(authenticated.Audience, Audiences.Admin, StringComparison.Ordinal))
                {
                    problems.Add($"{route}: oturum admin yüzeyinden olmalı");
                }
            }
            else if (!isPlatformPermission || !schemes.Contains(Audiences.Admin, StringComparer.Ordinal))
            {
                problems.Add($"{route}: platform izni (RequireAdminPermission) ve admin şeması gerekli");
            }
        }

        return [.. problems.Order(StringComparer.Ordinal)];
    }
}
