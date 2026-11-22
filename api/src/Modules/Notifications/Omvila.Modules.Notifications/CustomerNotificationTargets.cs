using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications;

/// <summary>
/// Müşteri bildirim listesine yazılan olaylar ve dokununca açılacak ekran (sözleşme MeNotificationEvent, MeNotificationTarget).
/// Listede yalnız push metni olan müşteri olayları yer alır (bildirimler.md § Müşteri olayları); yalnız SMS olanlar (otp.code,
/// kapora mesajları, walkin.almost_turn) yazılmaz.
/// </summary>
internal static class CustomerNotificationTargets
{
    /// <summary>Bekleme teklifi SMS değişkeni: "/b/{kod}" bağlantısı (kod hedefte döner).</summary>
    public const string WaitlistLinkValue = "b bağlantısı";

    private static readonly Dictionary<string, string> Targets = new(StringComparer.Ordinal)
    {
        [NotificationEvents.AppointmentConfirmed] = CustomerNotification.AppointmentTarget,
        [NotificationEvents.AppointmentRequested] = CustomerNotification.AppointmentTarget,
        [NotificationEvents.AppointmentApproved] = CustomerNotification.AppointmentTarget,
        [NotificationEvents.AppointmentReminder] = CustomerNotification.AppointmentTarget,
        [NotificationEvents.AppointmentRescheduled] = CustomerNotification.AppointmentTarget,
        [NotificationEvents.AppointmentCancelledBySalon] = CustomerNotification.AppointmentTarget,
        [NotificationEvents.DepositReceived] = CustomerNotification.AppointmentTarget,

        // Talep iptal edildi: "Başka bir saat seçin" (rezervasyon akışı).
        [NotificationEvents.AppointmentDeclined] = CustomerNotification.BookingTarget,
        [NotificationEvents.AppointmentApprovalExpired] = CustomerNotification.BookingTarget,
        [NotificationEvents.WaitlistOffer] = CustomerNotification.WaitlistOfferTarget,
        [NotificationEvents.ReviewInvite] = CustomerNotification.ReviewTarget,
    };

    /// <summary>Olay müşteri listesinde yer alıyorsa hedef türü.</summary>
    public static bool TryGetTarget(string eventCode, out string targetType) => Targets.TryGetValue(eventCode, out targetType!);

    /// <summary>Bekleme teklifi bağlantısındaki kod (bağlantının son parçası); yoksa null.</summary>
    public static string? AccessCodeFromLink(string? link)
    {
        if (string.IsNullOrWhiteSpace(link))
        {
            return null;
        }

        var code = link.TrimEnd('/');
        var slash = code.LastIndexOf('/');
        return slash < 0 || slash == code.Length - 1 ? null : code[(slash + 1)..];
    }
}
