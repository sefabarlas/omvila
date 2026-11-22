using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using NodaTime;
using Omvila.Infrastructure.Email;
using Omvila.Modules.Notifications.Contracts;

namespace Omvila.Modules.Notifications.Templates;

/// <summary>
/// İşletmeye giden OMVILA e-postaları (bildirimler.md § E-posta; tasarım bildirim-dagitim/EpostaDeneme, EpostaOdemeHata,
/// EpostaKontor). Sade HTML ve aynı içerikte düz metin. Kart akışı (T-06) olmadığından kart butonları yoktur; ödeme yolu "Havale
/// ile öde". Değerler HTML'de kaçırılır. Renk kullanılmaz (e-postada tasarım tokenı yok; istemci varsayılanı).
/// </summary>
internal static class EmailTemplates
{
    public const string SubscriptionPath = "ayarlar/abonelik";
    private const string Footer = "OMVILA · İşletme hesabınızla ilgili bilgilendirme.";
    private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");

    // Türkçe harfler olduğu gibi kalır; yalnız HTML'de anlamı olan karakterler kaçırılır.
    private static readonly HtmlEncoder Html = HtmlEncoder.Create(UnicodeRanges.All);

    // Kural: bildirimler.md owner.trial_ending; tasarım EpostaDeneme
    public static EmailMessage TrialEnding(
        string to, string brandName, string planName, int trialDays, LocalDate trialEndsOn, int daysLeft, int graceDays, Uri panel)
    {
        var title = $"Deneme sürenizin bitmesine {daysLeft} gün kaldı";
        var builder = new EmailBuilder(title, "Havale ile ödeyin; online randevularınız kesintisiz devam etsin.")
            .Paragraph($"Merhaba, {brandName} için {trialDays} günlük deneme süresi {TurkishSuffix.Locative(Day(trialEndsOn))} bitiyor. "
                + $"Havale ile ödeyerek {planName} planıyla devam edebilirsiniz.")
            .Button("Havale ile öde", new Uri(panel, SubscriptionPath))
            .Note($"Ödeme yapılmazsa {graceDays} günlük hatırlatma süresi başlar. Sonra panel salt okunur olur ve online randevu kapanır. "
                + "Verileriniz silinmez.");
        return builder.Build(to, title, NotificationEvents.OwnerTrialEnding);
    }

    // Kural: bildirimler.md owner.subscription_payment_failed ("{gün} gün içinde … havale ile ödeyin; {tarih} salt okunur"); EpostaOdemeHata
    public static EmailMessage PaymentFailed(
        string to, string brandName, string planName, LocalDate periodStart, LocalDate today, LocalDate readOnlyOn, int graceDay, Uri panel)
    {
        const string title = "Abonelik ödemeniz alınamadı";
        var daysLeft = Math.Max(0, Period.DaysBetween(today, readOnlyOn));
        var builder = new EmailBuilder(title, $"{daysLeft} gün içinde havale ile ödeyin; aksi hâlde panel salt okunur olur.")
            .Paragraph($"Merhaba, {brandName} için {Day(periodStart)} dönemine ait {planName} planı ödemesi henüz ulaşmadı. Havaleniz bekleniyor.")
            .Row("Bugün", $"Hatırlatma süresi · {graceDay}. gün")
            .Row(Day(readOnlyOn), "Panel salt okunur olur")
            .Row("Verileriniz", "Silinmez")
            .Paragraph("Salt okunurda randevularınızı görmeye devam edersiniz; yeni randevu, online rezervasyon ve SMS durur. "
                + "Ödeme tamamlanınca hepsi hemen geri açılır.")
            .Button("Havale ile öde", new Uri(panel, SubscriptionPath));
        return builder.Build(to, title, NotificationEvents.OwnerSubscriptionPaymentFailed);
    }

    // Kural: bildirimler.md owner.credits_low ("{kalan} SMS kaldı, tahmini {gün} gün yeter"); EpostaKontor
    public static EmailMessage CreditsLow(string to, string brandName, int balance, int? daysLeft, Uri panel)
    {
        var remaining = $"{StaffText.Count(balance)} SMS";
        var preview = daysLeft is { } d
            ? $"Mevcut kullanımla yaklaşık {StaffText.Count(d)} gün yeter. Hatırlatmalar durmadan paket alın."
            : "Hatırlatmalar durmadan paket alın.";
        var builder = new EmailBuilder("SMS kontörünüz azaldı", preview)
            .Paragraph($"Merhaba, {brandName} için SMS bakiyesi azaldı.")
            .Row("Kalan", remaining);
        if (daysLeft is { } days)
        {
            builder.Row("Tahmini", $"{StaffText.Count(days)} gün yeter");
        }

        builder.Paragraph("Kontör biterse onay, hatırlatma ve bekleme listesi SMS'leri gönderilmez. "
                + "Uygulaması olan müşterilere push bildirimleri gitmeye devam eder.")
            .Button("Kontör paketi al", new Uri(panel, SubscriptionPath));
        return builder.Build(to, $"SMS kontörünüz azaldı: {remaining} kaldı", NotificationEvents.OwnerCreditsLow);
    }

    private static string Day(LocalDate date) => date.ToString("d MMMM", Turkish);

    /// <summary>Başlık, önizleme metni, paragraflar, satırlar ve buton; HTML ve düz metin birlikte.</summary>
    private sealed class EmailBuilder
    {
        private readonly StringBuilder _html = new();
        private readonly StringBuilder _text = new();

        public EmailBuilder(string heading, string preview)
        {
            _html.Append("<!doctype html><html lang=\"tr\"><head><meta charset=\"utf-8\"><title>")
                .Append(Encode(heading))
                .Append("</title></head><body style=\"font-family:Arial,Helvetica,sans-serif;line-height:1.5;margin:0;padding:24px\">")
                .Append("<div style=\"display:none;max-height:0;overflow:hidden\">").Append(Encode(preview)).Append("</div>")
                .Append("<table role=\"presentation\" width=\"100%\" style=\"max-width:560px;margin:0 auto\"><tr><td>")
                .Append("<p style=\"font-weight:bold;margin:0 0 16px\">OMVILA</p>")
                .Append("<h1 style=\"font-size:22px;margin:0 0 12px\">").Append(Encode(heading)).Append("</h1>");
            _text.AppendLine(heading).AppendLine();
        }

        public EmailBuilder Paragraph(string text)
        {
            _html.Append("<p style=\"margin:0 0 16px\">").Append(Encode(text)).Append("</p>");
            _text.AppendLine(text).AppendLine();
            return this;
        }

        public EmailBuilder Note(string text)
        {
            _html.Append("<p style=\"margin:0 0 16px;padding:12px;border:1px solid;border-radius:8px\">").Append(Encode(text)).Append("</p>");
            _text.AppendLine(text).AppendLine();
            return this;
        }

        public EmailBuilder Row(string label, string value)
        {
            _html.Append("<p style=\"margin:0 0 8px\">").Append(Encode(label)).Append(": <strong>").Append(Encode(value)).Append("</strong></p>");
            _text.Append(label).Append(": ").AppendLine(value);
            return this;
        }

        public EmailBuilder Button(string label, Uri url)
        {
            var href = Encode(url.AbsoluteUri);
            _html.Append("<p style=\"margin:24px 0\"><a href=\"").Append(href)
                .Append("\" style=\"display:inline-block;padding:12px 20px;border:1px solid;border-radius:10px;font-weight:bold;text-decoration:none\">")
                .Append(Encode(label)).Append("</a></p>");
            _text.AppendLine().Append(label).Append(": ").AppendLine(url.AbsoluteUri).AppendLine();
            return this;
        }

        public EmailMessage Build(string to, string subject, string eventCode)
        {
            _html.Append("<hr><p style=\"font-size:12px;margin:16px 0 0\">").Append(Encode(Footer)).Append("</p></td></tr></table></body></html>");
            _text.AppendLine("--").AppendLine(Footer);
            return new EmailMessage(to, subject, _text.ToString(), _html.ToString(), eventCode);
        }

        private static string Encode(string value) => Html.Encode(value);
    }
}
