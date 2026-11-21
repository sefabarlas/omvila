using System.Text.RegularExpressions;
using Omvila.Modules.Notifications.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// Push başlık ve metinleri (bildirimler.md § Müşteri olayları ve § İşletme olayları, metinler birebir). Başlık
/// <see cref="PlatformDefaults.PushTitleMaxChars"/>, metin <see cref="PlatformDefaults.PushBodyMaxChars"/> karakteri aşarsa
/// sondan kısaltılır ("…"). Müşteri adı çağıran tarafından "Ad S." biçiminde verilir; not ve sağlık içeriği yazılmaz.
/// Olayın push metni yoksa (ör. otp.code) <see cref="TryRender"/> false döner: o satır yalnız SMS'tir.
/// </summary>
internal static partial class PushTemplates
{
    /// <summary>staff.appointment_changed: iptal metni.</summary>
    public const string CancelledVariant = "cancelled";

    /// <summary>staff.appointment_changed: saat değişikliği metni.</summary>
    public const string RescheduledVariant = "rescheduled";

    /// <summary>owner.credits_low: tüketim geçmişi yokken tahmini gün yazılmaz.</summary>
    public const string WithoutEstimateVariant = "without_estimate";

    private const int RegexTimeoutMilliseconds = 1000;

    private static readonly Dictionary<string, (string Title, string Body)> Templates = new(StringComparer.Ordinal)
    {
        [Key(NotificationEvents.AppointmentConfirmed, null)] = ("Randevunuz onaylandı", "{salon} · {tarih} {saat}. Görüşmek üzere."),
        [Key(NotificationEvents.AppointmentConfirmed, SmsTemplates.WithoutStaffVariant)] = ("Randevunuz onaylandı", "{salon} · {tarih} {saat}. Görüşmek üzere."),
        [Key(NotificationEvents.AppointmentReminder, null)] = ("Hatırlatma", "Yarın {saat} {salon}."),
        [Key(NotificationEvents.AppointmentReminder, SmsTemplates.SameDayVariant)] = ("Bugün {saat} {salon}", "Görüşmek üzere. Yol tarifi için dokunun."),
        [Key(NotificationEvents.WaitlistOffer, null)] = ("Beklediğiniz saat boşaldı", "{salon} · {tarih} {saat} · {dakika} dk."),
        [Key(NotificationEvents.StaffApprovalNeeded, null)] = ("Onay bekliyor · {kalan süre}", "{müşteri} · {tarih saat} {hizmet}. Yanıtlanmazsa otomatik iptal olur."),
        [Key(NotificationEvents.StaffAppointmentCreated, null)] = ("Yeni randevu", "{müşteri} · {saat} {hizmet} · sizinle."),
        [Key(NotificationEvents.StaffAppointmentChanged, CancelledVariant)] = ("İptal", "{müşteri} · {saat} iptal etti. Neden: {neden}."),
        [Key(NotificationEvents.StaffAppointmentChanged, RescheduledVariant)] = ("Saat değişti", "{eski} → {yeni}"),
        [Key(NotificationEvents.StaffCustomerCheckedIn, null)] = ("Müşteri geldi", "{müşteri} check-in yaptı."),
        [Key(NotificationEvents.StaffDepositReported, null)] = ("Havale bildirildi", "{müşteri} · {tutar} kaporayı gönderdiğini söyledi. Kontrol edin."),
        [Key(NotificationEvents.StaffReviewReceived, null)] = ("Yeni değerlendirme", "{puan} yıldız · {müşteri}"),
        [Key(NotificationEvents.StaffWalkInQueue, null)] = ("Sırada {n} kişi", "Kapıda sıra · tahmini bekleme {dk} dk."),
        [Key(NotificationEvents.StaffDailySummary, null)] = ("Gün sonu özeti", "{n} randevu tamamlandı, {tutar} tahsilat."),
        [Key(NotificationEvents.OwnerCreditsLow, null)] = ("SMS kontörü azaldı", "{kalan} SMS kaldı, tahmini {gün} gün yeter."),

        // Katalog metninin tahminsiz hâli (son 30 günde tüketim yok; T-71).
        [Key(NotificationEvents.OwnerCreditsLow, WithoutEstimateVariant)] = ("SMS kontörü azaldı", "{kalan} SMS kaldı."),
    };

    /// <summary>Tanımlı (olay kodu, çeşit) çiftleri.</summary>
    public static IReadOnlyList<(string EventCode, string? Variant)> Keys =>
        [.. Templates.Keys.Select(k => k.Split('#') is [var code, var variant] ? (code, (string?)variant) : (k, null))];

    public static bool Has(string eventCode, string? variant) => Templates.ContainsKey(Key(eventCode, variant));

    /// <summary>Push metni; olayın push şablonu yoksa false. Eksik değişken <see cref="InvalidOperationException"/>.</summary>
    public static bool TryRender(string eventCode, IReadOnlyDictionary<string, string> values, string? variant, out string title, out string body)
    {
        ArgumentNullException.ThrowIfNull(values);
        title = body = string.Empty;
        if (!Templates.TryGetValue(Key(eventCode, variant), out var template))
        {
            return false;
        }

        title = Fit(Fill(eventCode, template.Title, values), PlatformDefaults.PushTitleMaxChars);
        body = Fit(Fill(eventCode, template.Body, values), PlatformDefaults.PushBodyMaxChars);
        return true;
    }

    private static string Fill(string eventCode, string text, IReadOnlyDictionary<string, string> values) =>
        Placeholder().Replace(text, match => values.TryGetValue(match.Groups["name"].Value, out var value)
            ? value
            : throw new InvalidOperationException($"Push şablonunda doldurulmamış değişken: {eventCode} {match.Groups["name"].Value}"));

    private static string Fit(string text, int max) => text.Length <= max ? text : string.Concat(text.AsSpan(0, max - 1).TrimEnd(), "…");

    private static string Key(string eventCode, string? variant) => variant is null ? eventCode : eventCode + "#" + variant;

    [GeneratedRegex(@"\{(?<name>[^{}]+)\}", RegexOptions.ExplicitCapture, RegexTimeoutMilliseconds)]
    private static partial Regex Placeholder();
}
