using NodaTime;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>İşletme uygulaması push'u (bildirimler.md § İşletme olayları); SMS'e düşmez.</summary>
/// <param name="EventCode">Olay kodu, ör. <see cref="NotificationEvents.StaffApprovalNeeded"/>.</param>
/// <param name="BrandId">Marka.</param>
/// <param name="UserIds">Alıcı işletme kullanıcıları (rolü çağıran seçer; kişisel tercih ve sessiz saat süzgeci outbox'ta).</param>
/// <param name="Values">Şablon değişkenleri; müşteri adı "Ad S." biçiminde.</param>
/// <param name="TimeZoneId">Şubenin IANA saat dilimi (sessiz saat bu dilimde).</param>
/// <param name="Reference">Ait olduğu kayıt (ör. randevu); bekleyen push'lar bu bağla düşürülür.</param>
/// <param name="Variant">Şablon çeşidi (ör. staff.appointment_changed için "cancelled" / "rescheduled").</param>
/// <param name="NotAfterUtc">
/// Bu anda veya sonra gidecek push yazılmaz (ör. sessiz saatte sabaha ertelenen onay push'u tutma süresinden sonra; T-66 (8)).
/// </param>
public sealed record StaffPushNotification(
    string EventCode,
    Guid BrandId,
    IReadOnlyCollection<Guid> UserIds,
    IReadOnlyDictionary<string, string> Values,
    string TimeZoneId,
    NotificationReference? Reference = null,
    string? Variant = null,
    Instant? NotAfterUtc = null);
