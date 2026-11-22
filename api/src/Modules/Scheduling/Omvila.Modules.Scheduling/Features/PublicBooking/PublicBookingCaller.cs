using Microsoft.AspNetCore.Http;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Public rezervasyon uçlarını çağıran (sözleşme publicCreateAppointment, publicListMyPackages, publicJoinWaitlist: publicBookingBearer
/// veya meBearer). Rezervasyon jetonunda telefon jetondadır; müşteri uygulaması jetonunda (aud=me, T-101 (3)) telefon ve profildeki ad
/// global kayıttan okunur, kod adımı yoktur, istemci her zaman müşteri uygulamasıdır.
/// </summary>
/// <param name="Phone">Doğrulanmış telefon.</param>
/// <param name="AppCustomerId">Müşteri uygulaması jetonunda global müşteri; rezervasyon jetonunda null.</param>
/// <param name="ProfileName">Uygulama profilindeki ad (yoksa null).</param>
internal sealed record PublicBookingCaller(PhoneNumber Phone, Guid? AppCustomerId, string? ProfileName)
{
    private const string CustomerAppClient = "customer-app";

    /// <summary>Jetondan çağıranı çözer; geçersizse (telefonsuz rezervasyon jetonu, silinmiş hesap) null → 401.</summary>
    public static async Task<PublicBookingCaller?> ResolveAsync(HttpContext http, ICustomerAccounts accounts, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(accounts);
        if (string.Equals(http.User.FindFirst("aud")?.Value, Audiences.Me, StringComparison.Ordinal))
        {
            return Guid.TryParse(http.User.FindFirst(OmvilaClaims.Subject)?.Value, out var customerId)
                && await accounts.GetIdentityAsync(customerId, ct) is { } identity
                ? new PublicBookingCaller(identity.Phone, customerId, identity.FullName)
                : null;
        }

        return PhoneNumber.TryParse(http.User.FindFirst(OmvilaClaims.Phone)?.Value, out var phone) ? new PublicBookingCaller(phone, null, null) : null;
    }

    /// <summary>Online isteğin bağlamı; müşteri uygulaması jetonu veya <c>X-Client: customer-app</c> başlığı uygulama kanalıdır.</summary>
    public PublicBookingRequest Request(HttpContext http, PublicBranchScope scope)
    {
        ArgumentNullException.ThrowIfNull(http);
        var customerApp = AppCustomerId is not null
            || string.Equals(http.Request.Headers["X-Client"].ToString(), CustomerAppClient, StringComparison.Ordinal);
        return new PublicBookingRequest(
            scope,
            Phone,
            customerApp ? ClientApp.CustomerApp : ClientApp.BookingPage,
            customerApp ? ConsentChannel.CustomerApp : ConsentChannel.Web,
            http.Connection.RemoteIpAddress?.ToString());
    }

    /// <summary>Uygulamada ad profilden gelir; profilde ad yoksa istekteki ad kullanılır (ve ilk randevuda profile yazılır).</summary>
    public string? NameOr(string? requested) => ProfileName ?? requested;
}
