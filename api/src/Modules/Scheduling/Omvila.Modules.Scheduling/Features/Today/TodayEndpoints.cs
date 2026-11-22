using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Scheduling.Features.Today;

/// <summary>
/// İşletme uygulaması "Bugün" (sözleşme manageGetToday). calendar.view (personel K: yalnız kendi randevuları). Kapsam dışı veya
/// başka markanın şubesi 404. Onay, kapıda sıra ve prim kartları ilgili izin yoksa null döner.
/// </summary>
internal static class TodayEndpoints
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapApiGroup("/api/v1/manage/today", Audiences.Manage)
            .MapGet(string.Empty, GetAsync)
            .WithName("manageGetToday")
            .RequirePermission(Permissions.Calendar.View);

    private static async Task<IResult> GetAsync(
        Guid? branchId, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, TodayReader reader, CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.Calendar.View);
        var role = http.User.FindFirst(OmvilaClaims.Role)?.Value;
        bool Has(string permission) => role is not null && permissions.Resolve(role, permission) is not null;
        var cards = new TodayReader.Cards(Has(Permissions.Appointment.Approve), Has(Permissions.WalkIn.Manage), Has(Permissions.Commission.View));
        var result = await reader.GetAsync(tenant.RequireBrandId(), branchId, access, cards, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }
}
