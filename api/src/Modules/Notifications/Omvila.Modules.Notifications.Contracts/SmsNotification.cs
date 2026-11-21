using NodaTime;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Telefona gidecek SMS bildirimi.</summary>
/// <param name="EventCode">Olay kodu (bildirimler.md kataloğu), ör. <see cref="NotificationEvents.OtpCode"/>.</param>
/// <param name="PhoneE164">Alıcı telefon, E.164.</param>
/// <param name="Values">Şablon değişkenleri (ör. "kod").</param>
/// <param name="BrandId">Salon mesajında marka; platform mesajında (OTP) boş.</param>
/// <param name="Reference">Ait olduğu kayıt (ör. randevu); bekleyen satırlar bu bağla iptal edilir.</param>
/// <param name="ScheduledUtc">Gönderim zamanı (hatırlatma); boşsa hemen.</param>
/// <param name="Variant">Aynı olay kodunun şablon çeşidi (ör. aynı gün hatırlatması); boşsa temel metin.</param>
public sealed record SmsNotification(
    string EventCode,
    string PhoneE164,
    IReadOnlyDictionary<string, string> Values,
    Guid? BrandId = null,
    NotificationReference? Reference = null,
    Instant? ScheduledUtc = null,
    string? Variant = null);
