namespace Omvila.Modules.Notifications.Contracts;

/// <summary>İşletme uygulaması push'u (bildirimler.md § İşletme olayları); SMS'e düşmez.</summary>
/// <param name="EventCode">Olay kodu, ör. <see cref="NotificationEvents.StaffApprovalNeeded"/>.</param>
/// <param name="BrandId">Marka.</param>
/// <param name="UserIds">Alıcı işletme kullanıcıları (rol ve tercih süzgeci çağıranda).</param>
/// <param name="Values">Şablon değişkenleri; müşteri adı "Ad S." biçiminde.</param>
/// <param name="TimeZoneId">Şubenin IANA saat dilimi (sessiz saat bu dilimde).</param>
/// <param name="Reference">Ait olduğu kayıt (ör. randevu); bekleyen push'lar bu bağla düşürülür.</param>
/// <param name="Variant">Şablon çeşidi (ör. staff.appointment_changed için "cancelled" / "rescheduled").</param>
public sealed record StaffPushNotification(
    string EventCode,
    Guid BrandId,
    IReadOnlyCollection<Guid> UserIds,
    IReadOnlyDictionary<string, string> Values,
    string TimeZoneId,
    NotificationReference? Reference = null,
    string? Variant = null);
