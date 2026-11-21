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
using Omvila.Modules.Scheduling.Features.Appointments;

namespace Omvila.Modules.Scheduling.Features.WalkIns;

/// <summary>
/// Kapıda sıra uçları (sözleşme manage-walk-ins; KapidaSira, SiraEkle; dilim-05 kabul kriteri 4). Hepsi walkin.manage ister
/// (Owner, BranchManager ve personel kendi şubelerinde); "Başlat" ayrıca appointment.create (personel yalnız kendine). Başka
/// şubenin veya markanın bileti 404.
/// </summary>
internal static class WalkInEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/walk-in-tickets", Audiences.Manage);
        group.MapGet(string.Empty, ListAsync).WithName("manageListWalkIns").RequirePermission(Permissions.WalkIn.Manage);
        group.MapPost(string.Empty, CreateAsync).WithName("manageCreateWalkIn")
            .RequirePermission(Permissions.WalkIn.Manage).RequireIdempotencyKey();
        group.MapPost("/{ticketId:guid}/start", StartAsync).WithName("manageStartWalkIn")
            .RequirePermission(Permissions.WalkIn.Manage).RequirePermission(Permissions.Appointment.Create).RequireIfMatch();
        group.MapPost("/{ticketId:guid}/leave", LeaveAsync).WithName("manageLeaveWalkIn")
            .RequirePermission(Permissions.WalkIn.Manage).RequireIfMatch();
    }

    private static async Task<IResult> ListAsync(
        Guid? branchId, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, WalkInQueueReader reader, CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.WalkIn.Manage);
        var result = await reader.GetAsync(tenant.RequireBrandId(), branchId, access, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static async Task<IResult> CreateAsync(
        WalkInCreate body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        WalkInTickets tickets,
        WalkInQueueReader reader,
        CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.WalkIn.Manage);
        var created = await tickets.CreateAsync(tenant.RequireBrandId(), body, access, UserId(http.User), ct);
        if (!created.IsSuccess)
        {
            return created.Error.ToProblem();
        }

        http.Response.Headers.ETag = ETagVersion.Format(created.Value.Version);
        return TypedResults.Created((string?)null, await reader.ViewAsync(created.Value, ct));
    }

    private static async Task<IResult> StartAsync(
        Guid ticketId,
        WalkInStartRequest? body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        WalkInTickets tickets,
        WalkInQueueReader reader,
        AppointmentDetailReader appointments,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.WalkIn.Manage);
        var createAccess = ManageAccess.For(http.User, permissions, Permissions.Appointment.Create);
        var started = await tickets.StartAsync(brandId, ticketId, body, access, createAccess, UserId(http.User), http.GetIfMatchVersion(), ct);
        if (!started.IsSuccess)
        {
            return started.Error.ToProblem();
        }

        var (ticket, appointment) = started.Value;
        var detail = await appointments.MapAsync(brandId, appointment, createAccess, ct);
        return VersionedResults.Versioned(http, ticket.Version, new WalkInStarted(await reader.ViewAsync(ticket, ct), detail));
    }

    private static async Task<IResult> LeaveAsync(
        Guid ticketId, HttpContext http, IPermissionResolver permissions, WalkInTickets tickets, WalkInQueueReader reader, CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.WalkIn.Manage);
        var left = await tickets.LeaveAsync(ticketId, access, http.GetIfMatchVersion(), ct);
        if (!left.IsSuccess)
        {
            return left.Error.ToProblem();
        }

        return VersionedResults.Versioned(http, left.Value.Version, await reader.ViewAsync(left.Value, ct));
    }

    private static Guid? UserId(ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : null;
}
