using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;

namespace Omvila.Modules.Scheduling.Features.Calendar;

/// <summary>
/// Takvim (sözleşme manageGetCalendar). calendar.view; calendar.view.all-staff yoksa (personel) yalnız kendi kolonu.
/// Kapsam dışı şube 404. Tarihler ve staffIds sorgu metninden ayrıştırılır (NodaTime tipleri sorgu bağlamada çözülmez).
/// </summary>
internal static class CalendarEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/calendar", Audiences.Manage);
        group.MapGet(string.Empty, GetAsync).WithName("manageGetCalendar").RequirePermission(Permissions.Calendar.View);
    }

    private static async Task<IResult> GetAsync(
        Guid? branchId,
        string? from,
        string? to,
        string? staffIds,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        CalendarReader reader,
        CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.Calendar.View);
        var result = await reader.GetAsync(tenant.RequireBrandId(), branchId, from, to, staffIds, access, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }
}
