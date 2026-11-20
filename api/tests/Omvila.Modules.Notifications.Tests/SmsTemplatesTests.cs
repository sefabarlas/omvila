using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Templates;

namespace Omvila.Modules.Notifications.Tests;

// Kural: bildirimler.md — Türkçe tekli kaydırma tablosuyla ≤ 155 = 1 kontör; her şablon en uzun değerle denetlenir.
public sealed class SmsTemplatesTests
{
    // Her olay kodu için en uzun gerçekçi değişken değerleri.
    private static readonly Dictionary<string, Dictionary<string, string>> LongestValues = new(StringComparer.Ordinal)
    {
        [NotificationEvents.OtpCode] = new(StringComparer.Ordinal) { ["kod"] = "999999" },
        [NotificationEvents.StaffInvite] = new(StringComparer.Ordinal)
        {
            ["salon"] = "Güzellik ve Bakım Merkezi Nişantaşı Şubesi",
            ["bağlantı"] = "https://omvila.com/giris",
        },
    };

    [Fact]
    public void EveryTemplate_HasLongestValues_AndFitsSingleSms()
    {
        SmsTemplates.EventCodes.Order(StringComparer.Ordinal).ShouldBe(LongestValues.Keys.Order(StringComparer.Ordinal));

        foreach (var (eventCode, values) in LongestValues)
        {
            var body = SmsTemplates.Render(eventCode, values);
            var units = GsmLength.Units(body);
            units.ShouldNotBeNull($"{eventCode}: GSM dışı karakter (UCS-2'ye düşer)");
            units.Value.ShouldBeLessThanOrEqualTo(GsmLength.SingleSmsLimit, $"{eventCode}: {units} birim");
        }
    }

    [Fact]
    public void OtpCode_RendersDocumentText() =>
        SmsTemplates.Render(NotificationEvents.OtpCode, new Dictionary<string, string>(StringComparer.Ordinal) { ["kod"] = "482193" })
            .ShouldBe("OMVILA giriş kodunuz: 482193. Bu kodu kimseyle paylaşmayın.");

    [Fact]
    public void StaffInvite_StartsWithSalonName() =>
        SmsTemplates.Render(NotificationEvents.StaffInvite, new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["salon"] = "Kuaför Elif",
            ["bağlantı"] = "https://omvila.com/giris",
        }).ShouldBe("Kuaför Elif: Ekibimize eklendiniz. Bu numarayla OMVILA'ya giriş yapın: https://omvila.com/giris");

    [Fact]
    public void Render_WithMissingValue_Throws() =>
        Should.Throw<InvalidOperationException>(() =>
            SmsTemplates.Render(NotificationEvents.OtpCode, new Dictionary<string, string>(StringComparer.Ordinal)));

    [Fact]
    public void Render_WithUnknownEvent_Throws() =>
        Should.Throw<InvalidOperationException>(() =>
            SmsTemplates.Render("bilinmeyen", new Dictionary<string, string>(StringComparer.Ordinal)));

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
}
