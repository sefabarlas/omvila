namespace Omvila.Modules.Notifications.Contracts;

/// <summary>Bildirim olay kodları (docs/standartlar/bildirimler.md § Olay kataloğu).</summary>
public static class NotificationEvents
{
    /// <summary>OTP isteği: "OMVILA giriş kodunuz: {kod}…" (her zaman SMS, gönderen OMVILA).</summary>
    public const string OtpCode = "otp.code";

    /// <summary>Personel daveti: "{salon}: Ekibimize eklendiniz…" (SMS; metin taslak, T-32).</summary>
    public const string StaffInvite = "staff.invite";

    /// <summary>Onaylı randevu oluştu (online onaysız veya panel "Müşteriye SMS gönder" açık): "{salon}: Randevunuz onaylandı…".</summary>
    public const string AppointmentConfirmed = "appointment.confirmed";

    /// <summary>Onay gerektiren talep oluştu (A1) veya müşteri saati değiştirince yeniden onaya düştü (A13): "{salon}: Talebiniz alındı…".</summary>
    public const string AppointmentRequested = "appointment.requested";

    /// <summary>Salon talebi onayladı (A2): "{salon} talebinizi onayladı…"; hatırlatmalar bu anda planlanır.</summary>
    public const string AppointmentApproved = "appointment.approved";

    /// <summary>Salon talebi onaylamadı (A4): "…talebinizi onaylayamadı, talep iptal edildi…" ("reddedildi" yazılmaz).</summary>
    public const string AppointmentDeclined = "appointment.declined";

    /// <summary>Talep süresinde yanıtlanmadı (A5, kapora yok): "…talebinizi zamanında yanıtlayamadı, talep iptal edildi…".</summary>
    public const string AppointmentApprovalExpired = "appointment.approval_expired";

    /// <summary>Randevu saati değişti (A14): "{salon}: Randevunuz {yeni tarih} {saat} olarak güncellendi…".</summary>
    public const string AppointmentRescheduled = "appointment.rescheduled";

    /// <summary>Salon iptal etti (A7): "{salon}, {tarih} {saat} randevunuzu iptal etti…".</summary>
    public const string AppointmentCancelledBySalon = "appointment.cancelled_by_salon";

    /// <summary>Hatırlatma (ReminderOffsetsMinutes): zamanlanmış outbox satırı; randevu iptal olunca veya saati değişince düşer.</summary>
    public const string AppointmentReminder = "appointment.reminder";
}
