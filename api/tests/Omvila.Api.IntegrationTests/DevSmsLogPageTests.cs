using System.Net;
using Npgsql;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: T-76 — sahte SMS günlüğü sayfası (/dev/sms): yalnız sahte sağlayıcı etkinken; en yeni üstte, son N kayıt (ayar),
// telefonla süzme (?telefon=), zaman Europe/Istanbul'da, içerik HTML kaçışlı, noindex ve no-store.
// Production'da yolun hiç olmadığı Omvila.Host.Tests/DevSmsLogMappingTests'te.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DevSmsLogPageTests(PostgresFixture db) : IAsyncLifetime
{
    private const string AyşePhone = "+905321112233";
    private const string MertPhone = "+905449998877";
    private OmvilaApiFactory _factory = null!;
    private int _seq;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("TRUNCATE infra.dev_sms_log", connection);
        await command.ExecuteNonQueryAsync(Ct);
        _factory = new OmvilaApiFactory(db);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Page_ShowsRecordsNewestFirst_InIstanbulTime_WithNoIndexAndNoStore()
    {
        await SeedAsync(AyşePhone, "OMVILA giriş kodunuz: 482913", Instant.FromUtc(2026, 11, 21, 9, 0, 5));
        await SeedAsync(MertPhone, "Randevunuz onaylandı.", Instant.FromUtc(2026, 11, 21, 10, 30, 0));

        using var response = await GetAsync("/dev/sms");
        var html = await response.Content.ReadAsStringAsync(Ct);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.ShouldBe("text/html");
        response.Content.Headers.ContentType.CharSet.ShouldBe("utf-8");
        response.Headers.GetValues("X-Robots-Tag").ShouldContain(v => v.Contains("noindex", StringComparison.Ordinal));
        response.Headers.CacheControl!.NoStore.ShouldBeTrue();
        html.ShouldContain("<html lang=\"tr\">");
        html.ShouldContain("Sahte SMS günlüğü");
        html.ShouldContain("21.11.2026 12:00:05"); // 09:00:05 UTC → İstanbul UTC+3
        html.ShouldContain("21.11.2026 13:30:00");
        html.ShouldContain("0532 111 22 33");
        html.ShouldContain("482913");
        html.IndexOf("Randevunuz onaylandı.", StringComparison.Ordinal)
            .ShouldBeLessThan(html.IndexOf("482913", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Page_EscapesMessageContent()
    {
        await SeedAsync(AyşePhone, "<script>alert('x')</script> & \"tırnak\"", Instant.FromUtc(2026, 11, 21, 9, 0, 0), sender: "<b>SAL</b>");

        using var response = await GetAsync("/dev/sms");
        var html = await response.Content.ReadAsStringAsync(Ct);

        html.ShouldNotContain("<script>");
        html.ShouldNotContain("<b>SAL</b>");
        html.ShouldContain("&lt;script&gt;");
        html.ShouldContain("&amp;");
        html.ShouldContain("&lt;b&gt;SAL&lt;/b&gt;");
    }

    [Fact]
    public async Task Filter_ByPhone_ShowsOnlyThatNumber()
    {
        await SeedAsync(AyşePhone, "Ayşe mesajı", Instant.FromUtc(2026, 11, 21, 9, 0, 0));
        await SeedAsync(MertPhone, "Mert mesajı", Instant.FromUtc(2026, 11, 21, 9, 1, 0));

        using var response = await GetAsync("/dev/sms?telefon=" + Uri.EscapeDataString("0532 111 22 33"));
        var html = await response.Content.ReadAsStringAsync(Ct);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        html.ShouldContain("Ayşe mesajı");
        html.ShouldNotContain("Mert mesajı");
        html.ShouldContain("value=\"0532 111 22 33\"");
    }

    [Fact]
    public async Task Filter_WithInvalidPhone_ShowsMessageAndNoRecords()
    {
        await SeedAsync(AyşePhone, "Ayşe mesajı", Instant.FromUtc(2026, 11, 21, 9, 0, 0));

        using var response = await GetAsync("/dev/sms?telefon=" + Uri.EscapeDataString("<abc>"));
        var html = await response.Content.ReadAsStringAsync(Ct);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        html.ShouldContain("Telefon numarası geçersiz.");
        html.ShouldNotContain("Ayşe mesajı");
        html.ShouldNotContain("<abc>");
    }

    [Fact]
    public async Task Page_ShowsOnlyConfiguredNumberOfRecords()
    {
        await _factory.DisposeAsync();
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal) { ["Sms:DevLog:Limit"] = "2" });
        await SeedAsync(AyşePhone, "birinci", Instant.FromUtc(2026, 11, 21, 9, 0, 0));
        await SeedAsync(AyşePhone, "ikinci", Instant.FromUtc(2026, 11, 21, 9, 1, 0));
        await SeedAsync(AyşePhone, "üçüncü", Instant.FromUtc(2026, 11, 21, 9, 2, 0));

        using var response = await GetAsync("/dev/sms");
        var html = await response.Content.ReadAsStringAsync(Ct);

        html.ShouldContain("üçüncü");
        html.ShouldContain("ikinci");
        html.ShouldNotContain("birinci");
    }

    [Fact]
    public async Task EmptyLog_ShowsEmptyState()
    {
        using var response = await GetAsync("/dev/sms");
        var html = await response.Content.ReadAsStringAsync(Ct);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        html.ShouldContain("Henüz mesaj yok.");
        html.ShouldNotContain("<table");
    }

    private async Task<HttpResponseMessage> GetAsync(string path)
    {
        using var client = _factory.CreateClient();
        return await client.GetAsync(new Uri(path, UriKind.Relative), Ct);
    }

    private async Task SeedAsync(string phone, string body, Instant sentUtc, string sender = "OMVILA")
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(
            "INSERT INTO infra.dev_sms_log (id, phone_e164, sender, body, sent_utc) VALUES ($1, $2, $3, $4, $5)", connection);
        command.Parameters.Add(new NpgsqlParameter { Value = Guid.Parse($"0193a8f0-0000-7000-8000-{++_seq:D12}") });
        command.Parameters.Add(new NpgsqlParameter { Value = phone });
        command.Parameters.Add(new NpgsqlParameter { Value = sender });
        command.Parameters.Add(new NpgsqlParameter { Value = body });
        command.Parameters.Add(new NpgsqlParameter { Value = sentUtc.ToDateTimeUtc() });
        await command.ExecuteNonQueryAsync(Ct);
    }
}
