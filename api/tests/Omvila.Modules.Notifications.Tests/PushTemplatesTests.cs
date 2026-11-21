using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Tests;

// Kural: bildirimler.md § Push (başlık ≤ 40, metin ≤ 90; müşteri adı "Ad S."), § İşletme olayları (staff.* metinleri
// birebir), § Kanal planı (giriş kodunun push metni yok: her zaman SMS).
public sealed class PushTemplatesTests
{
    private const string Salon30 = "Güzellik ve Bakım Şişli Çağdaş";
    private const string Customer = "Abdurrahman Ş.";

    private static readonly Dictionary<(string, string?), Dictionary<string, string>> LongestValues = new()
    {
        [(NotificationEvents.AppointmentConfirmed, null)] = Values(("salon", Salon30), ("tarih", "Çar 30 Ağu"), ("saat", "23:45")),
        [(NotificationEvents.AppointmentConfirmed, SmsTemplates.WithoutStaffVariant)] = Values(("salon", Salon30), ("tarih", "Çar 30 Ağu"), ("saat", "23:45")),
        [(NotificationEvents.AppointmentReminder, null)] = Values(("salon", Salon30), ("saat", "23:45")),
        [(NotificationEvents.AppointmentReminder, SmsTemplates.SameDayVariant)] = Values(("salon", Salon30), ("saat", "23:45")),
        [(NotificationEvents.WaitlistOffer, null)] = Values(("salon", Salon30), ("tarih", "Çar 30 Ağu"), ("saat", "23:45"), ("dakika", "15")),
        [(NotificationEvents.StaffApprovalNeeded, null)] = Values(("kalan süre", "1 sa 58 dk"), ("müşteri", Customer), ("tarih saat", "Çar 30 Ağu 23:45"), ("hizmet", "Saç kesimi")),
        [(NotificationEvents.StaffAppointmentCreated, null)] = Values(("müşteri", Customer), ("saat", "23:45"), ("hizmet", "Saç kesimi ve fön +2")),
        [(NotificationEvents.StaffAppointmentChanged, PushTemplates.CancelledVariant)] = Values(("müşteri", Customer), ("saat", "23:45"), ("neden", "Başka saat istiyorum")),
        [(NotificationEvents.StaffAppointmentChanged, PushTemplates.RescheduledVariant)] = Values(("eski", "Çar 30 Ağu 23:45"), ("yeni", "Per 31 Ağu 23:45")),
        [(NotificationEvents.StaffCustomerCheckedIn, null)] = Values(("müşteri", Customer)),
        [(NotificationEvents.StaffDepositReported, null)] = Values(("müşteri", Customer), ("tutar", "1.500 TL")),
        [(NotificationEvents.StaffReviewReceived, null)] = Values(("puan", "5"), ("müşteri", Customer)),
        [(NotificationEvents.StaffWalkInQueue, null)] = Values(("n", "12"), ("dk", "120")),
        [(NotificationEvents.StaffDailySummary, null)] = Values(("n", "120"), ("tutar", "45.000 TL")),
        [(NotificationEvents.OwnerCreditsLow, null)] = Values(("kalan", "199"), ("gün", "1.000")),
        [(NotificationEvents.OwnerCreditsLow, PushTemplates.WithoutEstimateVariant)] = Values(("kalan", "199")),
    };

    [Fact]
    public void EveryTemplate_WithLongestValues_FitsLimits_WithoutTrimming()
    {
        PushTemplates.Keys.OrderBy(k => k.EventCode + k.Variant, StringComparer.Ordinal)
            .ShouldBe(LongestValues.Keys.Select(k => (EventCode: k.Item1, Variant: k.Item2)).OrderBy(k => k.EventCode + k.Variant, StringComparer.Ordinal));

        foreach (var ((eventCode, variant), values) in LongestValues)
        {
            PushTemplates.TryRender(eventCode, values, variant, out var title, out var body).ShouldBeTrue();
            title.Length.ShouldBeLessThanOrEqualTo(PlatformDefaults.PushTitleMaxChars, $"{eventCode} {variant}: {title}");
            body.Length.ShouldBeLessThanOrEqualTo(PlatformDefaults.PushBodyMaxChars, $"{eventCode} {variant}: {body}");
            body.ShouldNotContain("…", Case.Sensitive, $"{eventCode} {variant}: gerçekçi değerle kırpılmamalı");
            if (!(string.Equals(eventCode, NotificationEvents.AppointmentReminder, StringComparison.Ordinal) && string.Equals(variant, SmsTemplates.SameDayVariant, StringComparison.Ordinal)))
            {
                title.ShouldNotContain("…", Case.Sensitive, $"{eventCode} {variant}: gerçekçi değerle kırpılmamalı");
            }
        }
    }

    [Fact]
    public void SameDayReminderTitle_WithThirtyCharacterSalon_IsTrimmedToForty()
    {
        // "Bugün {saat} {salon}" 30 karakterlik salon adıyla 42 karakter: başlık sınırında "…" ile kısalır (T-66).
        PushTemplates.TryRender(NotificationEvents.AppointmentReminder, LongestValues[(NotificationEvents.AppointmentReminder, SmsTemplates.SameDayVariant)], SmsTemplates.SameDayVariant, out var title, out _);

        title.ShouldBe("Bugün 23:45 Güzellik ve Bakım Şişli Çağ…");
    }

    [Fact]
    public void ApprovalNeeded_MatchesCatalogText()
    {
        PushTemplates.TryRender(NotificationEvents.StaffApprovalNeeded, LongestValues[(NotificationEvents.StaffApprovalNeeded, null)], null, out var title, out var body);

        title.ShouldBe("Onay bekliyor · 1 sa 58 dk");
        body.ShouldBe("Abdurrahman Ş. · Çar 30 Ağu 23:45 Saç kesimi. Yanıtlanmazsa otomatik iptal olur.");
    }

    [Fact]
    public void CreditsLow_MatchesCatalogText()
    {
        PushTemplates.TryRender(NotificationEvents.OwnerCreditsLow, Values(("kalan", "180"), ("gün", "3")), null, out var title, out var body);

        title.ShouldBe("SMS kontörü azaldı");
        body.ShouldBe("180 SMS kaldı, tahmini 3 gün yeter.");
    }

    [Fact]
    public void OverlongValues_AreTrimmedToTheLimits()
    {
        var values = Values(("kalan süre", new string('x', 60)), ("müşteri", Customer), ("tarih saat", "Çar 30 Ağu 23:45"), ("hizmet", new string('ğ', 120)));

        PushTemplates.TryRender(NotificationEvents.StaffApprovalNeeded, values, null, out var title, out var body);

        title.Length.ShouldBe(PlatformDefaults.PushTitleMaxChars);
        body.Length.ShouldBe(PlatformDefaults.PushBodyMaxChars);
        body.ShouldEndWith("…");
    }

    [Fact]
    public void OtpCode_HasNoPushText()
    {
        PushTemplates.Has(NotificationEvents.OtpCode, null).ShouldBeFalse();
        PushTemplates.TryRender(NotificationEvents.OtpCode, Values(("kod", "123456")), null, out _, out _).ShouldBeFalse();
    }

    [Fact]
    public void MissingValue_Throws()
    {
        Should.Throw<InvalidOperationException>(() => PushTemplates.TryRender(NotificationEvents.StaffCustomerCheckedIn, Values(), null, out _, out _));
    }

    private static Dictionary<string, string> Values(params (string Key, string Value)[] pairs) =>
        pairs.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal);
}
