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
}
