using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Scheduling.Features.Overview;

/// <summary>
/// Salon paneli Genel bakış (sözleşme manageGetOverview). report.view (Owner, BranchManager kendi şubeleri; personel 403, onun
/// özeti manageGetToday). Kapsam dışı veya başka markanın şubesi 404. Onay ve kapıda sıra kartları ilgili izin yoksa null.
/// </summary>
internal static class OverviewEndpoints
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapApiGroup("/api/v1/manage/overview", Audiences.Manage)
            .MapGet(string.Empty, GetAsync)
            .WithName("manageGetOverview")
            .RequirePermission(Permissions.Report.View);

    private static async Task<IResult> GetAsync(
        Guid? branchId, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, OverviewReader reader, CancellationToken ct)
    {
        var role = http.User.FindFirst(OmvilaClaims.Role)?.Value;
        bool Has(string permission) => role is not null && permissions.Resolve(role, permission) is not null;
        var cards = new OverviewReader.Cards(Has(Permissions.Appointment.Approve), Has(Permissions.WalkIn.Manage));
        var result = await reader.GetAsync(tenant.RequireBrandId(), branchId, BranchScope.From(http.User), cards, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }
}
