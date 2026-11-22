using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.AppointmentLink;
using Omvila.Modules.Scheduling.Features.Deposits;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Me;

/// <summary>
/// Müşteri uygulaması "Randevularım" (sözleşme me-appointments: meListAppointments, meGetAppointment, meCancelAppointment,
/// meSearchRescheduleSlots, meRescheduleAppointment, meReportDepositTransfer; dilim-05 KK2, dilim-04 KK4). Değişiklikler SMS
/// bağlantısındaki uçlarla aynı alan kurallarıdır (<see cref="PublicAppointmentLinkChanges"/>, <see cref="DepositFollowUps"/>):
/// iptal A6 (seans iadesi, bekleme listesine teklif olay işleyicisinde), saat değişikliği A14/A13 (kapora yeni saate aktarılır).
/// Kimlik me jetonunun sub'ıdır; başka müşterinin randevusu 404.
/// </summary>
internal static class MeAppointmentEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/me/appointments", Audiences.Me);
        group.MapGet(string.Empty, ListAsync).WithName("meListAppointments").RequireAuthenticated(Audiences.Me);
        group.MapGet("/{appointmentId:guid}", GetAsync).WithName("meGetAppointment").RequireAuthenticated(Audiences.Me);
        group.MapPost("/{appointmentId:guid}/cancel", CancelAsync).WithName("meCancelAppointment").RequireAuthenticated(Audiences.Me).RequireIfMatch();

        // Yan etkisiz okuma (POST: gövdede gün).
        group.MapPost("/{appointmentId:guid}/availability/slots", SlotsAsync).WithName("meSearchRescheduleSlots").RequireAuthenticated(Audiences.Me);
        group.MapPost("/{appointmentId:guid}/reschedule", RescheduleAsync).WithName("meRescheduleAppointment").RequireAuthenticated(Audiences.Me).RequireIfMatch();

        // Kendiliğinden idempotent (ikinci çağrı aynı sonuç, bildirim tek kez); sözleşme gereği If-Match ve Idempotency-Key almaz.
        group.MapPost("/{appointmentId:guid}/deposit/report", ReportDepositAsync).WithName("meReportDepositTransfer").RequireAuthenticated(Audiences.Me);
    }

    private static async Task<IResult> ListAsync(
        string? scope, bool? notReviewed, int? limit, string? cursor, ITenantContext tenant, MeAppointmentReader reader, CancellationToken ct)
    {
        var page = await reader.ListAsync(CustomerId(tenant), scope, notReviewed == true, limit, cursor, ct);
        return page.ToHttp(p => TypedResults.Ok(p));
    }

    private static async Task<IResult> GetAsync(Guid appointmentId, HttpContext http, ITenantContext tenant, MeAppointmentReader reader, CancellationToken ct)
    {
        var appointment = await reader.EnterAsync(CustomerId(tenant), appointmentId, ct);
        return appointment is null ? Error.NotFound().ToProblem() : await RespondAsync(appointment, http, reader, ct);
    }

    private static async Task<IResult> CancelAsync(
        Guid appointmentId,
        CustomerCancelRequest body,
        HttpContext http,
        ITenantContext tenant,
        MeAppointmentReader reader,
        PublicAppointmentLinkChanges changes,
        CancellationToken ct)
    {
        var appointment = await reader.EnterAsync(CustomerId(tenant), appointmentId, ct);
        if (appointment is null)
        {
            return Error.NotFound().ToProblem();
        }

        var result = await changes.CancelAsync(appointment, body, http.GetIfMatchVersion(), ct);
        return result.IsSuccess ? await RespondAsync(appointment, http, reader, ct) : result.Error.ToProblem();
    }

    private static async Task<IResult> SlotsAsync(
        Guid appointmentId,
        RescheduleSlotSearchRequest body,
        ITenantContext tenant,
        MeAppointmentReader reader,
        PublicAppointmentLinkChanges changes,
        CancellationToken ct)
    {
        var appointment = await reader.EnterAsync(CustomerId(tenant), appointmentId, ct);
        if (appointment is null)
        {
            return Error.NotFound().ToProblem();
        }

        var result = await changes.SearchSlotsAsync(appointment, body, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static async Task<IResult> RescheduleAsync(
        Guid appointmentId,
        PublicRescheduleRequest body,
        HttpContext http,
        ITenantContext tenant,
        MeAppointmentReader reader,
        PublicAppointmentLinkChanges changes,
        CancellationToken ct)
    {
        var appointment = await reader.EnterAsync(CustomerId(tenant), appointmentId, ct);
        if (appointment is null)
        {
            return Error.NotFound().ToProblem();
        }

        var result = await changes.RescheduleAsync(appointment, body, http.GetIfMatchVersion(), ct);
        return result.IsSuccess ? await RespondAsync(appointment, http, reader, ct) : result.Error.ToProblem();
    }

    private static async Task<IResult> ReportDepositAsync(
        Guid appointmentId, HttpContext http, ITenantContext tenant, MeAppointmentReader reader, DepositFollowUps deposits, CancellationToken ct)
    {
        var appointment = await reader.EnterAsync(CustomerId(tenant), appointmentId, ct);
        if (appointment is null)
        {
            return Error.NotFound().ToProblem();
        }

        var result = await deposits.ReportTransferAsync(appointment, ct);
        return result.IsSuccess ? await RespondAsync(appointment, http, reader, ct) : result.Error.ToProblem();
    }

    private static async Task<IResult> RespondAsync(Appointment appointment, HttpContext http, MeAppointmentReader reader, CancellationToken ct)
    {
        var view = await reader.MapAsync(appointment, ct);
        return view is null ? Error.NotFound().ToProblem() : VersionedResults.Versioned(http, appointment.Version, view);
    }

    // me yüzeyinde kiracı bağlamı jetondan (sub = global Customer); oturum markaya geçmeden önce okunur.
    private static Guid CustomerId(ITenantContext tenant) =>
        tenant.Scope.CustomerId ?? throw new DomainException("me isteğinde müşteri bağlamı yok.");
}
