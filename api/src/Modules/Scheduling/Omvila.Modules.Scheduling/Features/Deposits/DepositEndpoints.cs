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

namespace Omvila.Modules.Scheduling.Features.Deposits;

/// <summary>
/// Kapora uçları (sözleşme manage-calendar: manageMarkDepositReceived, manageRemindDeposit, manageMarkDepositRefunded). Hepsi
/// appointment.deposit.mark ister (Owner, BranchManager kendi şubesinde); personel v1'de 403 (V5). "Kapora geldi" ve
/// "Salon iade etti" tahsilat kaydı yazdığı için If-Match ve Idempotency-Key ister, yanıt güncel AppointmentDetail + ETag;
/// "Hatırlat" gövdesiz, 204.
/// </summary>
internal static class DepositEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var appointments = app.MapApiGroup("/api/v1/manage/appointments", Audiences.Manage);
        appointments.MapPost("/{appointmentId:guid}/deposit/mark-received", MarkReceivedAsync).WithName("manageMarkDepositReceived")
            .RequirePermission(Permissions.Appointment.DepositMark).RequireIfMatch().RequireIdempotencyKey();
        appointments.MapPost("/{appointmentId:guid}/deposit/remind", RemindAsync).WithName("manageRemindDeposit")
            .RequirePermission(Permissions.Appointment.DepositMark);
        appointments.MapPost("/{appointmentId:guid}/deposit/refund", MarkRefundedAsync).WithName("manageMarkDepositRefunded")
            .RequirePermission(Permissions.Appointment.DepositMark).RequireIfMatch().RequireIdempotencyKey();
    }

    private static async Task<IResult> RemindAsync(
        Guid appointmentId, HttpContext http, IPermissionResolver permissions, DepositFollowUps deposits, CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.DepositMark);
        var result = await deposits.RemindAsync(appointmentId, access, UserId(http.User), ct);
        return result.IsSuccess ? TypedResults.NoContent() : result.Error.ToProblem();
    }

    private static async Task<IResult> MarkRefundedAsync(
        Guid appointmentId,
        DepositRefundRequest body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        DepositFollowUps deposits,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.DepositMark);
        var result = await deposits.MarkRefundedAsync(appointmentId, body, access, UserId(http.User), http.GetIfMatchVersion(), ct);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        var appointment = result.Value;
        return VersionedResults.Versioned(http, appointment.Version, await reader.MapAsync(brandId, appointment, access, ct));
    }

    private static async Task<IResult> MarkReceivedAsync(
        Guid appointmentId,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        DepositReceipts receipts,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.DepositMark);
        var result = await receipts.MarkReceivedAsync(appointmentId, access, UserId(http.User), http.GetIfMatchVersion(), ct);
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
