using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.Deposits;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.AppointmentLink;

/// <summary>
/// SMS bağlantısı "Randevum" (sözleşme public-appointment-link: publicGetAppointmentByCode, publicCancelAppointment,
/// publicSearchRescheduleSlots, publicRescheduleAppointment; dilim-02 kabul kriteri 10) ve Havale ekranı "Havaleyi yaptım"
/// (publicReportDepositTransfer; dilim-04 kabul kriteri 4). Kimlik erişim kodudur: kodu bilen
/// o randevu için doğrulanmış sayılır (alan-modeli.md § AppointmentAccessCode). İptal ve saat değişikliği If-Match ister
/// (GET'teki ETag). Salt okunur abonelikte de çalışır: var olan randevular durur ve sözleşme bu uçlarda 403 tanımlamaz (T-52).
/// </summary>
internal static class PublicAppointmentLinkEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/public/appointments");

        // Üyeliksiz (ürün kuralı 7): yetki 10 karakterlik kriptografik rastgele erişim kodudur (62^10), jeton yoktur.
        group.MapGet("/{accessCode}", GetAsync).WithName("publicGetAppointmentByCode").AllowAnonymous();

        // Üyeliksiz: erişim kodu randevuya yetki verir; sürüm If-Match ile denetlenir.
        group.MapPost("/{accessCode}/cancel", CancelAsync).WithName("publicCancelAppointment").AllowAnonymous().RequireIfMatch();

        // Üyeliksiz: erişim kodu randevuya yetki verir; yan etkisiz okuma.
        group.MapPost("/{accessCode}/availability/slots", SlotsAsync).WithName("publicSearchRescheduleSlots").AllowAnonymous();

        // Üyeliksiz: erişim kodu randevuya yetki verir; sürüm If-Match ile denetlenir.
        group.MapPost("/{accessCode}/reschedule", RescheduleAsync).WithName("publicRescheduleAppointment").AllowAnonymous().RequireIfMatch();

        // Üyeliksiz: erişim kodu randevuya yetki verir. Kendiliğinden idempotent (ikinci çağrı aynı sonuç, bildirim tek kez);
        // sözleşme gereği If-Match ve Idempotency-Key almaz. Hız sınırı diğer bağlantı uçları gibi yoktur (T-83).
        group.MapPost("/{accessCode}/deposit/report", ReportDepositAsync).WithName("publicReportDepositTransfer").AllowAnonymous();
    }

    private static async Task<IResult> ReportDepositAsync(
        string accessCode, HttpContext http, AppointmentLinkResolver resolver, DepositFollowUps deposits, CancellationToken ct)
    {
        var appointment = await resolver.EnterAsync(accessCode, ct);
        if (appointment is null)
        {
            return NotFound();
        }

        var result = await deposits.ReportTransferAsync(appointment, ct);
        return result.IsSuccess ? await RespondAsync(appointment, accessCode, http, resolver, ct) : result.Error.ToProblem();
    }

    private static async Task<IResult> GetAsync(string accessCode, HttpContext http, AppointmentLinkResolver resolver, CancellationToken ct)
    {
        var appointment = await resolver.EnterAsync(accessCode, ct);
        return appointment is null ? NotFound() : await RespondAsync(appointment, accessCode, http, resolver, ct);
    }

    private static async Task<IResult> CancelAsync(
        string accessCode,
        CustomerCancelRequest body,
        HttpContext http,
        AppointmentLinkResolver resolver,
        PublicAppointmentLinkChanges changes,
        CancellationToken ct)
    {
        var appointment = await resolver.EnterAsync(accessCode, ct);
        if (appointment is null)
        {
            return NotFound();
        }

        var result = await changes.CancelAsync(appointment, body, http.GetIfMatchVersion(), ct);
        return result.IsSuccess ? await RespondAsync(appointment, accessCode, http, resolver, ct) : result.Error.ToProblem();
    }

    private static async Task<IResult> SlotsAsync(
        string accessCode,
        RescheduleSlotSearchRequest body,
        AppointmentLinkResolver resolver,
        PublicAppointmentLinkChanges changes,
        CancellationToken ct)
    {
        var appointment = await resolver.EnterAsync(accessCode, ct);
        if (appointment is null)
        {
            return NotFound();
        }

        var result = await changes.SearchSlotsAsync(appointment, body, ct);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.Error.ToProblem();
    }

    private static async Task<IResult> RescheduleAsync(
        string accessCode,
        PublicRescheduleRequest body,
        HttpContext http,
        AppointmentLinkResolver resolver,
        PublicAppointmentLinkChanges changes,
        CancellationToken ct)
    {
        var appointment = await resolver.EnterAsync(accessCode, ct);
        if (appointment is null)
        {
            return NotFound();
        }

        var result = await changes.RescheduleAsync(appointment, body, http.GetIfMatchVersion(), ct);
        return result.IsSuccess ? await RespondAsync(appointment, accessCode, http, resolver, ct) : result.Error.ToProblem();
    }

    private static async Task<IResult> RespondAsync(
        Appointment appointment, string accessCode, HttpContext http, AppointmentLinkResolver resolver, CancellationToken ct)
    {
        var view = await resolver.MapAsync(appointment, accessCode, ct);
        return view is null ? NotFound() : VersionedResults.Versioned(http, appointment.Version, view);
    }

    /// <summary>Bilinmeyen, süresi dolmuş veya iptal edilmiş kod: "Bu randevu bulunamadı".</summary>
    private static IResult NotFound() => Error.NotFound().ToProblem();
}
