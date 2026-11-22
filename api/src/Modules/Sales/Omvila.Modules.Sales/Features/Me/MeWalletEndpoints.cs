using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Me;

/// <summary>
/// Müşteri uygulaması cüzdanı (sözleşme me-wallet: meListPackages, meListPayments). Salt okunur: paketler salondan satın alınır,
/// uygulamadan ödeme alınmaz (ürün kuralı 1). Kimlik me jetonunun sub'ıdır.
/// </summary>
internal static class MeWalletEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/me", Audiences.Me);
        group.MapGet("/packages", ListPackagesAsync).WithName("meListPackages").RequireAuthenticated(Audiences.Me);
        group.MapGet("/payments", ListPaymentsAsync).WithName("meListPayments").RequireAuthenticated(Audiences.Me);
    }

    private static async Task<IResult> ListPackagesAsync(ITenantContext tenant, MeWalletReader reader, CancellationToken ct) =>
        TypedResults.Ok(await reader.ListPackagesAsync(CustomerId(tenant), ct));

    private static async Task<IResult> ListPaymentsAsync(int? limit, string? cursor, ITenantContext tenant, MeWalletReader reader, CancellationToken ct) =>
        (await reader.ListPaymentsAsync(CustomerId(tenant), limit, cursor, ct)).ToHttp(p => TypedResults.Ok(p));

    private static Guid CustomerId(ITenantContext tenant) =>
        tenant.Scope.CustomerId ?? throw new DomainException("me isteğinde müşteri bağlamı yok.");
}
