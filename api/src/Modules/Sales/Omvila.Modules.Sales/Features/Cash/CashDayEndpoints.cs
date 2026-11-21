using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>
/// Kasa uçları (sözleşme manageGetCashDay, manageCloseCashDay). Görüntüleme payment.collect, kapatma cash.close ister (Owner,
/// BranchManager kendi şubesinde; personel 403). Kapsam dışı şube 404. Kapatma para kaydı oluşturduğu için Idempotency-Key ister.
/// Tarih sorgu metninden ayrıştırılır (NodaTime tipleri sorgu bağlamada çözülmez).
/// </summary>
internal static class CashDayEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/cash", Audiences.Manage);
        group.MapGet(string.Empty, GetAsync).WithName("manageGetCashDay").RequirePermission(Permissions.Payment.Collect);
        group.MapPost("/close", CloseAsync).WithName("manageCloseCashDay").RequirePermission(Permissions.Cash.Close).RequireIdempotencyKey();
    }

    private static async Task<IResult> GetAsync(
        Guid? branchId, string? date, HttpContext http, ITenantContext tenant, CashDays cash, CancellationToken ct)
    {
        var result = await cash.GetAsync(tenant.RequireBrandId(), branchId, date, BranchScope.From(http.User), ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static async Task<IResult> CloseAsync(
        CashDayCloseRequest body, HttpContext http, ITenantContext tenant, CashDays cash, CancellationToken ct)
    {
        var result = await cash.CloseAsync(tenant.RequireBrandId(), body, BranchScope.From(http.User), UserId(http.User), ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static Guid? UserId(ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : null;
}
