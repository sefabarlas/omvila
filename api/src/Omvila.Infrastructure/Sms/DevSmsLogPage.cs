using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NodaTime;
using NodaTime.Text;
using Omvila.Infrastructure.Hosting;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Sms;

/// <summary>
/// Sahte SMS günlüğü sayfası <c>GET /dev/sms</c> (T-76): sahte sağlayıcının infra.dev_sms_log'a yazdığı mesajlar
/// (giriş kodları, onay ve hatırlatma SMS'leri), test edenler kodu buradan okur. Sunucuda üretilen düz HTML; sözleşmede
/// (contracts/) yoktur ve OpenAPI belgesine girmez, çünkü tarayıcı doğrudan HTML ister ve API istemcisi kullanmaz.
/// <para>
/// Yalnız sahte SMS sağlayıcısı etkinken eşlenir (Development, Testing, Staging + Staging:AllowFakeProviders); Production'da
/// yol hiç yoktur (404). <b>Kimlik doğrulaması yoktur:</b> test sunucusunda koruma ters vekildedir (Caddy basic auth,
/// https://sms.test.omvila.com.tr). Mesajlar giriş kodu içerir; bu yol vekilsiz dışarı açılmamalıdır.
/// </para>
/// </summary>
public static class DevSmsLogPage
{
    public const string Path = "/dev/sms";

    // Yalnız < > & " ' ve denetim karakterleri kaçışlanır; Türkçe harfler okunur kalır.
    private static readonly HtmlEncoder Html = HtmlEncoder.Create(UnicodeRanges.All);
    private static readonly LocalDateTimePattern TimePattern = LocalDateTimePattern.CreateWithInvariantCulture("dd.MM.yyyy HH:mm:ss");

    // İç geliştirici sayfası: panel tasarım tokenları burada yüklenmez. Sabit renk yerine tarayıcının sistem renkleri
    // (Canvas, CanvasText, GrayText) ve color-scheme kullanılır; açık ve koyu temaya kendiliğinden uyar.
    private const string Head = """
        <title>Sahte SMS günlüğü · OMVILA</title>
        <style>
        :root { color-scheme: light dark; font-family: system-ui, sans-serif; line-height: 1.45; }
        body { margin: 0 auto; max-width: 72rem; padding: 1rem; background: Canvas; color: CanvasText; }
        h1 { font-size: 1.4rem; margin: 0 0 .25rem; }
        .note { color: GrayText; margin: 0 0 1rem; }
        form { display: flex; flex-wrap: wrap; gap: .5rem; align-items: center; margin-bottom: 1rem; }
        input, button { font: inherit; padding: .4rem .6rem; }
        :focus-visible { outline: 2px solid currentColor; outline-offset: 2px; }
        table { border-collapse: collapse; width: 100%; }
        th, td { text-align: left; vertical-align: top; padding: .5rem; border-bottom: 1px solid GrayText; }
        td.body { white-space: pre-wrap; overflow-wrap: anywhere; }
        td.nowrap { white-space: nowrap; }
        </style>
        </head>
        <body>
        <main>
        <h1>Sahte SMS günlüğü</h1>
        """;

    /// <summary>Sahte SMS sağlayıcısı etkinse sayfayı eşler; değilse hiçbir şey yapmaz.</summary>
    public static IEndpointRouteBuilder MapDevSmsLogPage(this IEndpointRouteBuilder app)
    {
        var services = app.ServiceProvider;
        var environment = services.GetRequiredService<IHostEnvironment>();
        var staging = services.GetRequiredService<IOptions<StagingOptions>>().Value;
        var sms = services.GetRequiredService<IOptions<SmsOptions>>().Value;
        if (!environment.AllowsFakeProviders(staging) || !string.Equals(sms.Provider, SmsOptions.FakeProvider, StringComparison.Ordinal))
        {
            return app;
        }

        // /api/ dışında: yüzey CORS'u ve jeton yok; MapApiGroup gibi tek (salt okunur) transaction'da çalışır.
        var group = app.MapGroup("/dev");
        group.WithMetadata(new TransactionalEndpointMetadata());
        group.AddEndpointFilter<TransactionEndpointFilter>();
        group.ExcludeFromDescription();
        // Anonim: koruma ters vekilde (Caddy basic auth); yol yalnız sahte sağlayıcı ortamlarında var (yukarıda).
        group.MapGet("/sms", RenderAsync).AllowAnonymous();
        return app;
    }

    private static async Task<IResult> RenderAsync(
        HttpContext http, InfraDbContext db, IClock clock, IOptions<DevSmsLogOptions> options, string? telefon, CancellationToken ct)
    {
        var settings = options.Value;
        var filter = string.IsNullOrWhiteSpace(telefon) ? null : telefon.Trim();
        PhoneNumber? phone = null;
        var invalidFilter = filter is not null && !PhoneNumber.TryParse(filter, out phone);

        IReadOnlyList<DevSmsMessage> rows = [];
        if (!invalidFilter)
        {
            var query = db.DevSmsLog.AsNoTracking();
            if (phone is not null)
            {
                var e164 = phone.E164;
                query = query.Where(m => m.PhoneE164 == e164);
            }

            rows = await query.OrderByDescending(m => m.SentUtc).ThenByDescending(m => m.Id).Take(settings.Limit).ToListAsync(ct);
        }

        var headers = http.Response.Headers;
        headers.CacheControl = "no-store";
        headers["X-Robots-Tag"] = "noindex, nofollow";
        headers.XContentTypeOptions = "nosniff";
        headers["Referrer-Policy"] = "no-referrer";
        // Kaçışa ek savunma: betik yok, yalnız satır içi stil ve aynı kökene form.
        headers.ContentSecurityPolicy = "default-src 'none'; style-src 'unsafe-inline'; form-action 'self'; frame-ancestors 'none'; base-uri 'none'";

        var zone = DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var html = Render(rows, filter, phone, invalidFilter, settings, clock.GetCurrentInstant(), zone);
        return Results.Content(html, "text/html; charset=utf-8", Encoding.UTF8);
    }

    private static string Render(
        IReadOnlyList<DevSmsMessage> rows,
        string? filter,
        PhoneNumber? phone,
        bool invalidFilter,
        DevSmsLogOptions settings,
        Instant now,
        DateTimeZone zone)
    {
        string Time(Instant instant) => TimePattern.Format(instant.InZone(zone).LocalDateTime);
        var filterValue = phone?.Display ?? filter ?? string.Empty;
        var sb = new StringBuilder(4096);
        sb.Append("""
            <!doctype html>
            <html lang="tr">
            <head>
            <meta charset="utf-8">
            <meta name="viewport" content="width=device-width, initial-scale=1">
            <meta name="robots" content="noindex, nofollow">
            """).Append('\n');
        if (settings.RefreshSeconds > 0)
        {
            sb.Append(CultureInfo.InvariantCulture, $"<meta http-equiv=\"refresh\" content=\"{settings.RefreshSeconds}\">\n");
        }

        sb.Append(Head).Append('\n');
        sb.Append(CultureInfo.InvariantCulture, $"<p class=\"note\">Sahte sağlayıcının gönderdiği son {settings.Limit} mesaj, en yeni üstte. Saatler Türkiye saatiyle. Son yenileme: {Time(now)}");
        if (settings.RefreshSeconds > 0)
        {
            sb.Append(CultureInfo.InvariantCulture, $"; sayfa {settings.RefreshSeconds} saniyede bir yenilenir");
        }

        sb.Append(".</p>\n");
        sb.Append(CultureInfo.InvariantCulture, $"""
            <form method="get" action="{Path}" role="search">
            <label for="telefon">Telefon</label>
            <input id="telefon" name="telefon" type="tel" inputmode="tel" autocomplete="off" placeholder="0532 123 45 67" value="{Html.Encode(filterValue)}">
            <button type="submit">Süz</button>
            """).Append('\n');
        if (filter is not null)
        {
            sb.Append(CultureInfo.InvariantCulture, $"<a href=\"{Path}\">Süzgeci kaldır</a>\n");
        }

        sb.Append("</form>\n");

        if (invalidFilter)
        {
            sb.Append("<p role=\"alert\">Telefon numarası geçersiz.</p>\n");
        }
        else if (rows.Count == 0)
        {
            sb.Append(phone is null ? "<p>Henüz mesaj yok.</p>\n" : "<p>Bu numaraya gönderilmiş mesaj yok.</p>\n");
        }
        else
        {
            AppendTable(sb, rows, Time);
        }

        sb.Append("</main>\n</body>\n</html>\n");
        return sb.ToString();
    }

    private static void AppendTable(StringBuilder sb, IReadOnlyList<DevSmsMessage> rows, Func<Instant, string> time)
    {
        sb.Append("""
            <table>
            <caption class="note">Gönderilen SMS'ler</caption>
            <thead><tr><th scope="col">Zaman</th><th scope="col">Telefon</th><th scope="col">Gönderen</th><th scope="col">Mesaj</th></tr></thead>
            <tbody>
            """).Append('\n');
        foreach (var row in rows)
        {
            var number = PhoneNumber.TryParse(row.PhoneE164, out var parsed) ? parsed.Display : row.PhoneE164;
            sb.Append(CultureInfo.InvariantCulture, $"<tr><td class=\"nowrap\">{time(row.SentUtc)}</td><td class=\"nowrap\">{Html.Encode(number)}</td><td>{Html.Encode(row.Sender)}</td><td class=\"body\">{Html.Encode(row.Body)}</td></tr>\n");
        }

        sb.Append("</tbody>\n</table>\n");
    }
}
