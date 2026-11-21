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

    /// <summary>
    /// Kaporalı online randevu oluştu (Kapora bekleniyor): "{salon}: {tarih} {saat} için {tutar} TL kaporayı {süre} içinde
    /// gönderin. IBAN …". Her zaman SMS (push denenmez), sessiz saatte kaydırılmaz.
    /// </summary>
    public const string DepositAwaiting = "deposit.awaiting";

    /// <summary>
    /// Salon "Kapora geldi" dedi (A3): "{salon}: {tutar} TL kaporanız alındı, {tarih} {saat} randevunuz kesinleşti…". Yalnız SMS
    /// (V3; push denenmez), sessiz saatte kaydırılmaz.
    /// </summary>
    public const string DepositReceived = "deposit.received";

    /// <summary>
    /// Kapora süresinde gelmedi, randevu sistemce iptal edildi (A5, kapora vardı): "{salon}: {tutar} TL kapora {süre} içinde
    /// gelmediği için {tarih} {saat} randevunuz iptal edildi. Havale yaptıysanız salonu arayın." Yalnız SMS.
    /// </summary>
    public const string DepositNotReceived = "deposit.not_received";

    /// <summary>Hatırlatma (ReminderOffsetsMinutes): zamanlanmış outbox satırı; randevu iptal olunca veya saati değişince düşer.</summary>
    public const string AppointmentReminder = "appointment.reminder";

    /// <summary>
    /// Değerlendirme daveti (A11; tamamlandıktan ReviewInviteDelayMinutes sonra): "{salon}'teki randevunuz nasıldı? 10 saniyenizi
    /// alır: {d bağlantısı}". Push → SMS; randevu iptal olunca bekleyen satır düşer.
    /// </summary>
    public const string ReviewInvite = "review.invite";

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

    /// <summary>İşletme push'u ve bildirim paneli (Owner): "SMS kontörü azaldı" (eşik altına iniş veya yetersiz kontör).</summary>
    public const string OwnerCreditsLow = "owner.credits_low";
}
