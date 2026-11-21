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
/// Kapora uçları (sözleşme manage-calendar: manageMarkDepositReceived). appointment.deposit.mark ister (Owner, BranchManager
/// kendi şubesinde); personel v1'de 403 (V5). Tahsilat kaydı oluşturduğu için If-Match ve Idempotency-Key ister; yanıt güncel
/// AppointmentDetail + ETag. Gövde yoktur: tutar istenen kaporadır.
/// </summary>
internal static class DepositEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var appointments = app.MapApiGroup("/api/v1/manage/appointments", Audiences.Manage);
        appointments.MapPost("/{appointmentId:guid}/deposit/mark-received", MarkReceivedAsync).WithName("manageMarkDepositReceived")
            .RequirePermission(Permissions.Appointment.DepositMark).RequireIfMatch().RequireIdempotencyKey();
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
