using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Integrations.Features.GoogleCalendar;

/// <summary>
/// Google Takvim (sözleşme manageGetGoogleCalendarStatus, manageDisconnectGoogleCalendar, manageGetGoogleCalendarConnectUrl,
/// manageCompleteGoogleCalendarConnect; K7). Uçlar oturumdaki kullanıcının kendi personel kaydı içindir (jetondaki staff_id);
/// personel kaydı yoksa 404. İzin: calendar.view.
/// </summary>
internal static class GoogleCalendarEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/integrations/google", Audiences.Manage);
        group.MapGet(string.Empty, GetAsync).WithName("manageGetGoogleCalendarStatus").RequirePermission(Permissions.Calendar.View);
        group.MapDelete(string.Empty, DisconnectAsync).WithName("manageDisconnectGoogleCalendar").RequirePermission(Permissions.Calendar.View);
        group.MapGet("/connect-url", ConnectUrl).WithName("manageGetGoogleCalendarConnectUrl").RequirePermission(Permissions.Calendar.View);
        group.MapPost("/callback", CompleteAsync).WithName("manageCompleteGoogleCalendarConnect").RequirePermission(Permissions.Calendar.View);
    }

    private static async Task<IResult> GetAsync(ClaimsPrincipal user, ITenantContext tenant, CalendarConnections connections, CancellationToken ct) =>
        Actor(user, tenant) is { } actor
            ? TypedResults.Ok(GoogleCalendarConnection.From(await connections.FindAsync(actor, ct)))
            : Error.NotFound().ToProblem();

    private static async Task<IResult> DisconnectAsync(ClaimsPrincipal user, ITenantContext tenant, CalendarConnections connections, CancellationToken ct)
    {
        if (Actor(user, tenant) is not { } actor)
        {
            return Error.NotFound().ToProblem();
        }

        await connections.DisconnectAsync(actor, ct);
        return TypedResults.NoContent();
    }

    private static IResult ConnectUrl(string? client, ClaimsPrincipal user, ITenantContext tenant, CalendarConnections connections) =>
        Actor(user, tenant) is { } actor
            ? connections.CreateConnectUrl(actor, client).ToHttp(r => TypedResults.Ok(r))
            : Error.NotFound().ToProblem();

    private static async Task<IResult> CompleteAsync(
        GoogleCalendarCallbackRequest body, ClaimsPrincipal user, ITenantContext tenant, CalendarConnections connections, CancellationToken ct)
    {
        if (Actor(user, tenant) is not { } actor)
        {
            return Error.NotFound().ToProblem();
        }

        var result = await connections.CompleteAsync(actor, body, ct);
        return result.ToHttp(link => TypedResults.Ok(GoogleCalendarConnection.From(link)));
    }

    private static CalendarActor? Actor(ClaimsPrincipal user, ITenantContext tenant) =>
        Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var userId)
        && Guid.TryParse(user.FindFirst(OmvilaClaims.StaffId)?.Value, out var staffId)
            ? new CalendarActor(tenant.RequireBrandId(), userId, staffId)
            : null;
}
