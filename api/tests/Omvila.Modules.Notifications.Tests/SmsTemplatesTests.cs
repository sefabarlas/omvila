using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;

namespace Omvila.Modules.Notifications.Tests;

// Kural: bildirimler.md — metinler birebir; Türkçe tekli kaydırma tablosuyla ≤ 155 = 1 kontör; her şablon en uzun değerle
// denetlenir (salon adı 30 karakter, en uzun Türkçe gün/ay kısaltması); kullanıcı verisi aşarsa değişken kırpılır.
public sealed class SmsTemplatesTests
{
    private const string Salon30 = "Güzellik ve Bakım Şişli Çağdaş"; // 30 karakter, 5 kaydırma karakteri
    private const string ManageLink = "omvila.com/r/AbCdEfGh12";
    private const string LongDate = "Çar 30 Ağu";
    private const string LongTime = "23:45";

    // Her (olay kodu, çeşit) için en uzun gerçekçi değişken değerleri.
    private static readonly Dictionary<(string, string?), Dictionary<string, string>> LongestValues = new()
    {
        [(NotificationEvents.OtpCode, null)] = Values(("kod", "999999")),
        [(NotificationEvents.StaffInvite, null)] = Values(("salon", Salon30), ("bağlantı", "https://omvila.com/giris")),
        [(NotificationEvents.AppointmentConfirmed, null)] = Values(
            ("salon", Salon30), ("tarih", LongDate), ("saat", LongTime), ("hizmet", "Saç kesimi +2"), ("personel", "Zeynep"), ("bağlantı", ManageLink)),
        [(NotificationEvents.AppointmentConfirmed, SmsTemplates.WithoutStaffVariant)] = Values(
            ("salon", Salon30), ("tarih", LongDate), ("saat", LongTime), ("hizmet", "Saç kesimi ve fön +2"), ("bağlantı", ManageLink)),
        [(NotificationEvents.AppointmentRescheduled, null)] = Values(
            ("salon", Salon30), ("yeni tarih", LongDate), ("saat", LongTime), ("bağlantı", ManageLink)),
        [(NotificationEvents.AppointmentCancelledBySalon, null)] = Values(
            ("salon", Salon30), ("tarih", LongDate), ("saat", LongTime), ("rezervasyon bağlantısı", "omvila.com/guzellik-ve-bakim-sisli-cagdas")),
        [(NotificationEvents.AppointmentReminder, null)] = Values(("salon", Salon30), ("saat", LongTime), ("bağlantı", ManageLink)),
        [(NotificationEvents.AppointmentReminder, SmsTemplates.SameDayVariant)] = Values(("salon", Salon30), ("saat", LongTime), ("bağlantı", ManageLink)),
    };

    [Fact]
    public void EveryTemplate_HasLongestValues_AndFitsSingleSms_WithoutTrimming()
    {
        SmsTemplates.Keys.OrderBy(k => k.EventCode + k.Variant, StringComparer.Ordinal)
            .ShouldBe(LongestValues.Keys.Select(k => (EventCode: k.Item1, Variant: k.Item2)).OrderBy(k => k.EventCode + k.Variant, StringComparer.Ordinal));

        foreach (var ((eventCode, variant), values) in LongestValues)
        {
            var body = SmsTemplates.Render(eventCode, values, variant);
            var units = GsmLength.Units(body);
            units.ShouldNotBeNull($"{eventCode}: GSM dışı karakter (UCS-2'ye düşer)");
            units.Value.ShouldBeLessThanOrEqualTo(GsmLength.SingleSmsLimit, $"{eventCode} {variant}: {units} birim");
            foreach (var value in values.Values)
            {
                body.ShouldContain(value, Case.Sensitive, $"{eventCode} {variant}: 30 karakterlik salon adıyla değişken kırpılmamalı");
            }
        }
    }

    [Fact]
    public void EveryTemplate_WithOverlongUserValues_IsTrimmedToSingleSms()
    {
        var salon = new string('Ş', 160);
        var service = new string('ğ', 120);
        var staff = new string('ı', 120);
        foreach (var ((eventCode, variant), longest) in LongestValues)
        {
            var values = new Dictionary<string, string>(longest, StringComparer.Ordinal);
            foreach (var (key, value) in new[] { ("salon", salon), ("hizmet", service), ("personel", staff) })
            {
                if (values.ContainsKey(key))
                {
                    values[key] = value;
                }
            }

            if (values.ContainsKey("rezervasyon bağlantısı"))
            {
                values["rezervasyon bağlantısı"] = "omvila.com/" + new string('a', 60);
            }

            var body = SmsTemplates.Render(eventCode, values, variant);
            GsmLength.Units(body)!.Value.ShouldBeLessThanOrEqualTo(GsmLength.SingleSmsLimit, $"{eventCode} {variant}");
            body.ShouldContain(values.GetValueOrDefault("bağlantı") ?? values.GetValueOrDefault("rezervasyon bağlantısı") ?? values["kod"]);
        }
    }

    [Fact]
    public void Confirmed_TrimsLongSalonNameOnlyAsMuchAsNeeded_BeforeServiceAndStaff()
    {
        var body = SmsTemplates.Render(NotificationEvents.AppointmentConfirmed, Values(
            ("salon", "Kuaför Elif Güzellik ve Bakım Salonu Kadıköy Moda Şubesi"), ("tarih", "Sal 25 Kas"), ("saat", "14:30"),
            ("hizmet", "Saç kesimi +1"), ("personel", "Elif Kaya"), ("bağlantı", ManageLink)));

        body.ShouldBe("Kuaför Elif Güzellik ve Bakım Salon: Randevunuz onaylandı. Sal 25 Kas 14:30, Saç kesimi +1 (Elif Kaya). Değişiklik ve iptal: omvila.com/r/AbCdEfGh12");
        GsmLength.Units(body)!.Value.ShouldBeLessThanOrEqualTo(GsmLength.SingleSmsLimit);
    }

    [Fact]
    public void Confirmed_RendersDocumentText() =>
        SmsTemplates.Render(NotificationEvents.AppointmentConfirmed, Values(
            ("salon", "Kuaför Elif"), ("tarih", "Sal 25 Kas"), ("saat", "14:30"), ("hizmet", "Saç kesimi +1"), ("personel", "Elif"), ("bağlantı", ManageLink)))
            .ShouldBe("Kuaför Elif: Randevunuz onaylandı. Sal 25 Kas 14:30, Saç kesimi +1 (Elif). Değişiklik ve iptal: omvila.com/r/AbCdEfGh12");

    [Fact]
    public void Rescheduled_RendersDocumentText() =>
        SmsTemplates.Render(NotificationEvents.AppointmentRescheduled, Values(
            ("salon", "Kuaför Elif"), ("yeni tarih", "Per 26 Kas"), ("saat", "11:00"), ("bağlantı", ManageLink)))
            .ShouldBe("Kuaför Elif: Randevunuz Per 26 Kas 11:00 olarak güncellendi. Detay: omvila.com/r/AbCdEfGh12");

    [Fact]
    public void CancelledBySalon_RendersDocumentText_WithBookingPageLink() =>
        SmsTemplates.Render(NotificationEvents.AppointmentCancelledBySalon, Values(
            ("salon", "Kuaför Elif"), ("tarih", "Sal 25 Kas"), ("saat", "14:30"), ("rezervasyon bağlantısı", "omvila.com/kuafor-elif")))
            .ShouldBe("Kuaför Elif, Sal 25 Kas 14:30 randevunuzu iptal etti. omvila.com/kuafor-elif");

    [Fact]
    public void Reminder_DayBefore_And_SameDay_RenderDocumentTexts_WithSuffixHarmony()
    {
        var values = Values(("saat", "14:30"), ("salon", "Kuaför Elif"), ("bağlantı", ManageLink));

        SmsTemplates.Render(NotificationEvents.AppointmentReminder, values)
            .ShouldBe("Hatırlatma: Yarın 14:30'da Kuaför Elif'te randevunuz var. Değişiklik için: omvila.com/r/AbCdEfGh12");
        SmsTemplates.Render(NotificationEvents.AppointmentReminder, values, SmsTemplates.SameDayVariant)
            .ShouldBe("Bugün 14:30'da Kuaför Elif'te görüşmek üzere. Yol tarifi: omvila.com/r/AbCdEfGh12");
        SmsTemplates.Render(NotificationEvents.AppointmentReminder, Values(("saat", "10:00"), ("salon", "Berber Ali"), ("bağlantı", ManageLink)))
            .ShouldStartWith("Hatırlatma: Yarın 10:00'da Berber Ali'de randevunuz var.");
    }

    [Fact]
    public void OtpCode_RendersDocumentText() =>
        SmsTemplates.Render(NotificationEvents.OtpCode, Values(("kod", "482193")))
            .ShouldBe("OMVILA giriş kodunuz: 482193. Bu kodu kimseyle paylaşmayın.");

    [Fact]
    public void StaffInvite_StartsWithSalonName() =>
        SmsTemplates.Render(NotificationEvents.StaffInvite, Values(("salon", "Kuaför Elif"), ("bağlantı", "https://omvila.com/giris")))
            .ShouldBe("Kuaför Elif: Ekibimize eklendiniz. Bu numarayla OMVILA'ya giriş yapın: https://omvila.com/giris");

    [Fact]
    public void Render_WithMissingValue_Throws() =>
        Should.Throw<InvalidOperationException>(() => SmsTemplates.Render(NotificationEvents.OtpCode, Values()));

    [Fact]
    public void Render_WithUnknownEventOrVariant_Throws()
    {
        Should.Throw<InvalidOperationException>(() => SmsTemplates.Render("bilinmeyen", Values()));
        Should.Throw<InvalidOperationException>(() => SmsTemplates.Render(NotificationEvents.OtpCode, Values(("kod", "1")), "bilinmeyen"));
    }

    [Fact]
    public void Render_ValueContainingBraces_IsNotTreatedAsPlaceholder() =>
        SmsTemplates.Render(NotificationEvents.StaffInvite, Values(("salon", "{Elif}"), ("bağlantı", "x")))
            .ShouldStartWith("{Elif}: Ekibimize");

    [Theory]
    [InlineData("Kuafor Elif", 11)]
    [InlineData("şğı", 6)]
    [InlineData("Çöü", 3)]
    [InlineData("{}", 4)]
    public void GsmLength_CountsTurkishShiftCharactersAsTwo(string text, int expected) =>
        GsmLength.Units(text).ShouldBe(expected);

    [Fact]
    public void GsmLength_WithCharacterOutsideTables_ReturnsNull() =>
        GsmLength.Units("Randevu ✂").ShouldBeNull();

    private static Dictionary<string, string> Values(params (string Key, string Value)[] pairs) =>
        pairs.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal);
}
