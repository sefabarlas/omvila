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

namespace Omvila.Modules.Scheduling.Features.Completion;

/// <summary>
/// "Tamamla ve ödeme al" ucu (sözleşme manageCompleteAppointment). appointment.complete ister (Owner, BranchManager kendi
/// şubesinde; personel 403). Para kaydettiği için If-Match ve Idempotency-Key ister; yanıt güncel AppointmentDetail + ETag.
/// </summary>
internal static class CompletionEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var appointments = app.MapApiGroup("/api/v1/manage/appointments", Audiences.Manage);
        appointments.MapPost("/{appointmentId:guid}/complete", CompleteAsync).WithName("manageCompleteAppointment")
            .RequirePermission(Permissions.Appointment.Complete).RequireIfMatch().RequireIdempotencyKey();
    }

    private static async Task<IResult> CompleteAsync(
        Guid appointmentId,
        ManageCompleteRequest body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        AppointmentCompletions completions,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.Complete);
        var result = await completions.CompleteAsync(brandId, appointmentId, body, access, UserId(http.User), http.GetIfMatchVersion(), ct);
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
