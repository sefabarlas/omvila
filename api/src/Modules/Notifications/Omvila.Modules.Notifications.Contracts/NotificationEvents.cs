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

    /// <summary>Randevu saati değişti (A14): "{salon}: Randevunuz {yeni tarih} {saat} olarak güncellendi…".</summary>
    public const string AppointmentRescheduled = "appointment.rescheduled";

    /// <summary>Salon iptal etti (A7): "{salon}, {tarih} {saat} randevunuzu iptal etti…".</summary>
    public const string AppointmentCancelledBySalon = "appointment.cancelled_by_salon";

    /// <summary>Hatırlatma (ReminderOffsetsMinutes): zamanlanmış outbox satırı; randevu iptal olunca veya saati değişince düşer.</summary>
    public const string AppointmentReminder = "appointment.reminder";

    /// <summary>İşletme push'u (Owner, BranchManager): "Onay bekliyor · {kalan süre}". Tercih: newOnlineAppointment.</summary>
    public const string StaffApprovalNeeded = "staff.approval_needed";

    /// <summary>İşletme push'u (ilgili personel): "Yeni randevu".</summary>
    public const string StaffAppointmentCreated = "staff.appointment_created";

    /// <summary>İşletme push'u (ilgili personel): "İptal" / "Saat değişti" (çeşit cancelled / rescheduled).</summary>
    public const string StaffAppointmentChanged = "staff.appointment_changed";

    /// <summary>İşletme push'u (ilgili personel): "Müşteri geldi".</summary>
    public const string StaffCustomerCheckedIn = "staff.customer_checked_in";

    /// <summary>İşletme push'u (Owner, BranchManager): "Havale bildirildi".</summary>
    public const string StaffDepositReported = "staff.deposit_reported";

    /// <summary>İşletme push'u: "Yeni değerlendirme".</summary>
    public const string StaffReviewReceived = "staff.review_received";

    /// <summary>İşletme push'u: "Sırada {n} kişi".</summary>
    public const string StaffWalkInQueue = "staff.walkin_queue";

    /// <summary>İşletme push'u: "Gün sonu özeti".</summary>
    public const string StaffDailySummary = "staff.daily_summary";
}
