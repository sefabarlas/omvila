using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Sales.Features.Commissions;

/// <summary>
/// Kişisel prim özeti (sözleşme manageGetMyCommissionSummary). İzin commission.view; personel kaydı jetondaki staff_id'dir, yoksa
/// veya markada bulunmazsa 404. Başka personelin primi bu uçtan görülmez (salon geneli rapor Dilim 6, K2).
/// </summary>
internal static class CommissionEndpoints
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapApiGroup("/api/v1/manage/me/commission-summary", Audiences.Manage)
            .MapGet(string.Empty, GetAsync)
            .WithName("manageGetMyCommissionSummary")
            .RequirePermission(Permissions.Commission.View);

    private static async Task<IResult> GetAsync(
        string? month, ClaimsPrincipal user, ITenantContext tenant, MyCommissionSummaryReader reader, CancellationToken ct)
    {
        var staffId = Guid.TryParse(user.FindFirst(OmvilaClaims.StaffId)?.Value, out var id) ? id : (Guid?)null;
        var result = await reader.GetAsync(tenant.RequireBrandId(), staffId, month, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }
}
