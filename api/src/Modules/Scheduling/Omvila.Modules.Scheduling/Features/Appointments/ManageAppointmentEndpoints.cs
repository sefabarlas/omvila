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
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Appointments;

/// <summary>
/// Panel randevu uçları (sözleşme manage-calendar: YeniRandevu, RandevuOnayli, TakvimSurukle, TekrarIptal; dilim-02 kabul
/// kriterleri 3, 11–14). Okuma calendar.view; oluşturma appointment.create (Idempotency-Key); saat değişikliği
/// appointment.update, iptal appointment.cancel, Geldi ve geri alma appointment.checkin (If-Match). Personel (K) yalnız
/// kendi kalemi olan randevuları görür ve değiştirir; kapsam dışı randevu 404.
/// </summary>
internal static class ManageAppointmentEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/appointments", Audiences.Manage);
        group.MapPost(string.Empty, CreateAsync).WithName("manageCreateAppointment")
            .RequirePermission(Permissions.Appointment.Create).RequireIdempotencyKey();
        group.MapGet("/{appointmentId:guid}", GetAsync).WithName("manageGetAppointment").RequirePermission(Permissions.Calendar.View);
        group.MapPost("/{appointmentId:guid}/reschedule", RescheduleAsync).WithName("manageRescheduleAppointment")
            .RequirePermission(Permissions.Appointment.Update).RequireIfMatch();
        group.MapPost("/{appointmentId:guid}/cancel", CancelAsync).WithName("manageCancelAppointment")
            .RequirePermission(Permissions.Appointment.Cancel).RequireIfMatch();
        group.MapPost("/{appointmentId:guid}/check-in", CheckInAsync).WithName("manageCheckInAppointment")
            .RequirePermission(Permissions.Appointment.CheckIn).RequireIfMatch();
        group.MapPost("/{appointmentId:guid}/undo-no-show", UndoNoShowAsync).WithName("manageUndoNoShow")
            .RequirePermission(Permissions.Appointment.CheckIn).RequireIfMatch();
    }

    private static async Task<IResult> CreateAsync(
        ManageAppointmentCreate body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        ManageAppointmentCreator creator,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.Create);
        var created = await creator.CreateAsync(brandId, body, access, UserId(http.User), ct);
        if (!created.IsSuccess)
        {
            return created.Error.ToProblem();
        }

        var (appointment, skipped) = created.Value;
        var detail = await reader.MapAsync(brandId, appointment, access, ct);
        http.Response.Headers.ETag = ETagVersion.Format(appointment.Version);
        return TypedResults.Created($"/api/v1/manage/appointments/{appointment.Id}", new AppointmentCreated(detail, skipped));
    }

    private static async Task<IResult> GetAsync(
        Guid appointmentId, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, AppointmentDetailReader reader, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Calendar.View);
        var appointment = await reader.LoadAsync(appointmentId, ct);
        if (appointment is null || !access.Covers(appointment))
        {
            return Error.NotFound().ToProblem();
        }

        return VersionedResults.Versioned(http, appointment.Version, await reader.MapAsync(brandId, appointment, access, ct));
    }

    private static async Task<IResult> RescheduleAsync(
        Guid appointmentId,
        ManageRescheduleRequest body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        ManageAppointmentChanges changes,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.Update);
        var result = await changes.RescheduleAsync(brandId, appointmentId, body, access, UserId(http.User), http.GetIfMatchVersion(), ct);
        return await RespondAsync(result, brandId, access, http, reader, ct);
    }

    private static async Task<IResult> CancelAsync(
        Guid appointmentId,
        ManageCancelRequest body,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        ManageAppointmentChanges changes,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.Cancel);
        var result = await changes.CancelAsync(brandId, appointmentId, body, access, UserId(http.User), http.GetIfMatchVersion(), ct);
        return await RespondAsync(result, brandId, access, http, reader, ct);
    }

    private static async Task<IResult> CheckInAsync(
        Guid appointmentId,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        ManageAppointmentChanges changes,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.CheckIn);
        var result = await changes.CheckInAsync(appointmentId, access, UserId(http.User), http.GetIfMatchVersion(), ct);
        return await RespondAsync(result, brandId, access, http, reader, ct);
    }

    private static async Task<IResult> UndoNoShowAsync(
        Guid appointmentId,
        HttpContext http,
        ITenantContext tenant,
        IPermissionResolver permissions,
        ManageAppointmentChanges changes,
        AppointmentDetailReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Appointment.CheckIn);
        var result = await changes.UndoNoShowAsync(brandId, appointmentId, access, UserId(http.User), http.GetIfMatchVersion(), ct);
        return await RespondAsync(result, brandId, access, http, reader, ct);
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
