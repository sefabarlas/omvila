namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Telefona gidecek SMS bildirimi.</summary>
/// <param name="EventCode">Olay kodu (bildirimler.md kataloğu), ör. <see cref="NotificationEvents.OtpCode"/>.</param>
/// <param name="PhoneE164">Alıcı telefon, E.164.</param>
/// <param name="Values">Şablon değişkenleri (ör. "kod").</param>
/// <param name="BrandId">Salon mesajında marka; platform mesajında (OTP) boş.</param>
public sealed record SmsNotification(
    string EventCode,
    string PhoneE164,
    IReadOnlyDictionary<string, string> Values,
    Guid? BrandId = null);
