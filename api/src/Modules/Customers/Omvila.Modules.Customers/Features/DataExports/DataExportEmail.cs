using System.Text.Encodings.Web;
using System.Text.Unicode;
using Omvila.Infrastructure.Email;
using Omvila.Modules.Notifications.Contracts;

namespace Omvila.Modules.Customers.Features.DataExports;

/// <summary>
/// "Tüm verileri dışa aktar" e-postası (owner.data_export_ready; T-121). Bildirim e-postalarının biçimi: sade HTML ve aynı içerikte
/// düz metin, değerler HTML'de kaçırılır, renk yok. Metin taslaktır (tasarımda e-posta ekranı yok).
/// </summary>
internal static class DataExportEmail
{
    public const string Subject = "Verileriniz hazır";
    private static readonly HtmlEncoder Html = HtmlEncoder.Create(UnicodeRanges.All);

    public static EmailMessage Build(string to, string brandName, Uri link, int validityDays)
    {
        var intro = $"Merhaba, {brandName} için istediğiniz dışa aktarma hazır. Müşteriler, randevular ve ödemeler CSV dosyaları olarak tek arşivdedir.";
        var validity = $"Bağlantı {validityDays} gün geçerlidir; süre dolunca dosya silinir.";
        const string note = "Bu isteği siz yapmadıysanız bu e-postayı dikkate almayın ve hesabınızın güvenliği için bize yazın.";
        var text = $"{Subject}\n\n{intro}\n\nVerileri indirin: {link}\n\n{validity}\n\n{note}\n\nOMVILA · İşletme hesabınızla ilgili bilgilendirme.\n";
        var html =
            $"<!doctype html><html lang=\"tr\"><body><h1>{Html.Encode(Subject)}</h1><p>{Html.Encode(intro)}</p>"
            + $"<p><a href=\"{Html.Encode(link.ToString())}\">Verileri indir</a></p><p>{Html.Encode(validity)}</p>"
            + $"<p><small>{Html.Encode(note)}</small></p><p><small>OMVILA · İşletme hesabınızla ilgili bilgilendirme.</small></p></body></html>";
        return new EmailMessage(to, Subject, text, html, NotificationEvents.OwnerDataExportReady);
    }
}
