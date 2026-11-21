using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Approvals;

/// <summary>
/// Onay uçları (sözleşme manage-calendar: manageApproveAppointment, manageDeclineAppointment, manageListPendingApprovals;
/// dilim-03 kabul kriterleri 2, 3, 5). Hepsi appointment.approve ister (Owner, BranchManager kendi şubesinde); personel
/// rolü 403. Onay ve red If-Match ister, yanıt güncel AppointmentDetail + ETag.
/// </summary>
internal static class ApprovalEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var appointments = app.MapApiGroup("/api/v1/manage/appointments", Audiences.Manage);
        appointments.MapPost("/{appointmentId:guid}/approve", ApproveAsync).WithName("manageApproveAppointment")
            .RequirePermission(Permissions.Appointment.Approve).RequireIfMatch();
        appointments.MapPost("/{appointmentId:guid}/decline", DeclineAsync).WithName("manageDeclineAppointment")
            .RequirePermission(Permissions.Appointment.Approve).RequireIfMatch();

        var queue = app.MapApiGroup("/api/v1/manage/approval-queue", Audiences.Manage);
        queue.MapGet(string.Empty, GetQueueAsync).WithName("manageListPendingApprovals").RequirePermission(Permissions.Appointment.Approve);
    }

    private static async Task<IResult> ApproveAsync(
        Guid appointmentId,
        ManageApproveRequest? body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        AppointmentApprovals approvals,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.Approve);
        var result = await approvals.ApproveAsync(appointmentId, access, UserId(http.User), http.GetIfMatchVersion(), body?.WaiveDeposit ?? false, ct);
        return await RespondAsync(result, brandId, access, http, reader, ct);
    }

    private static async Task<IResult> DeclineAsync(
        Guid appointmentId,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        AppointmentApprovals approvals,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.Approve);
        var result = await approvals.DeclineAsync(appointmentId, access, UserId(http.User), http.GetIfMatchVersion(), ct);
        return await RespondAsync(result, brandId, access, http, reader, ct);
    }

    private static async Task<IResult> GetQueueAsync(
        Guid? branchId, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, ApprovalQueueReader reader, CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.Approve);
        var result = await reader.GetAsync(tenant.RequireBrandId(), branchId, access, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static async Task<IResult> RespondAsync(
        Result<Appointment> result, Guid brandId, ManageAccess access, HttpContext http, AppointmentDetailReader reader, CancellationToken ct)
    {
        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        var appointment = result.Value;
        return VersionedResults.Versioned(http, appointment.Version, await reader.MapAsync(brandId, appointment, access, ct));
    }

    private static Guid? UserId(ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : null;
}
