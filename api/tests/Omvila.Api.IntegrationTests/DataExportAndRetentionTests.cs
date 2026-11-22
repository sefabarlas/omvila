using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: sözleşme manageRequestDataExport (202, açık istek varsa o döner, customer.export yalnız Owner, denetim Export),
// guvenlik-kvkk.md § Haklar (erişim: tüm veriler CSV) ve § Saklama (ApplyRetention), sureler-ve-ayarlar.md
// TransactionRetentionYears, AuditLogRetentionYears, DataExportLinkValidityDays; T-121.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DataExportAndRetentionTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Path = "/api/v1/manage/brand/data-exports";
    private const string ProcessDataExportsJob = "Omvila.Modules.Customers.Jobs.ProcessDataExports, Omvila.Modules.Customers";
    private const string ApplyRetentionJob = "Omvila.Modules.Customers.Jobs.ApplyRetention, Omvila.Modules.Customers";

    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        _merkez = (await _api.GetAsync("/api/v1/manage/session", _token)).Body.GetProperty("branches")[0].GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Request_QueuesOnce_RequiresEmail_OwnerOnly()
    {
        var missing = await _api.SendAsync(HttpMethod.Post, Path, new { }, _token, Key());
        missing.Status.ShouldBe(400);
        missing.Body.GetProperty("errors").GetProperty("email")[0].GetString().ShouldBe("required");

        var first = await _api.SendAsync(HttpMethod.Post, Path, new { email = "elif@ornek.com" }, _token, Key());
        first.Status.ShouldBe(202, first.Body.ToString());
        first.Body.GetProperty("status").GetString().ShouldBe("Queued");
        first.Body.GetProperty("emailMasked").GetString().ShouldBe("el***@ornek.com");
        var second = await _api.SendAsync(HttpMethod.Post, Path, new { email = "baska@ornek.com" }, _token, Key());
        second.Status.ShouldBe(202);
        second.Body.GetProperty("exportId").GetString().ShouldBe(first.Body.GetProperty("exportId").GetString(), "açık istek varken yenisi açılmaz");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.data_exports")).ShouldBe(1L);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE action = 'Export' AND entity_type = 'DataExport'")).ShouldBe(2L);

        var manager = await _api.AddMemberAndSignInAsync(_token, "+905383333333", "BranchManager", _merkez);
        (await _api.SendAsync(HttpMethod.Post, Path, new { email = "deniz@ornek.com" }, manager, Key())).Status.ShouldBe(403);
    }

    [Fact]
    public async Task Job_BuildsArchiveOfOnlyThisBrand_LinkDownloadsUntilExpiry()
    {
        await CreateCustomerAsync(_token, "Selin Öztürk", "+905550004418", "Kısa saç sever");
        var other = await _api.SignupAsync("+905321112233", "Berber Ali");
        await CreateCustomerAsync(other, "Okan Yıldız", "+905550009999", null);
        var requested = await _api.SendAsync(HttpMethod.Post, Path, new { email = "elif@ornek.com" }, _token, Key());
        var exportId = Guid.Parse(requested.Body.GetProperty("exportId").GetString()!);

        await _api.RunJobAsync(ProcessDataExportsJob);

        (await _api.ScalarAsync<string>($"SELECT status FROM customers.data_exports WHERE id = '{exportId}'")).ShouldBe("Ready");
        var brandId = await _api.ScalarAsync<Guid>($"SELECT brand_id FROM customers.data_exports WHERE id = '{exportId}'");
        var token = CreateToken(brandId, exportId, _factory.Clock.GetCurrentInstant() + Duration.FromDays(7));

        var download = await DownloadAsync(token);
        download.Status.ShouldBe(200);
        download.ContentType.ShouldBe("application/zip");
        var files = Unzip(download.Body);
        files.Keys.Order(StringComparer.Ordinal).ShouldBe(["musteriler.csv", "odemeler.csv", "randevular.csv"]);
        files["musteriler.csv"].ShouldContain("Selin Öztürk");
        files["musteriler.csv"].ShouldContain("0555 000 44 18");
        files["musteriler.csv"].ShouldNotContain("Okan Yıldız", Case.Sensitive, "başka markanın verisi");
        files["randevular.csv"].ShouldStartWith("Randevu kimliği;");
        files["odemeler.csv"].ShouldStartWith("Ödeme kimliği;");

        // Yeni istek açılabilir (hazır istek açık sayılmaz); bozuk ve süresi dolan bağlantı 404.
        (await _api.SendAsync(HttpMethod.Post, Path, new { email = "elif@ornek.com" }, _token, Key())).Body.GetProperty("exportId").GetString()
            .ShouldNotBe(exportId.ToString(), StringComparer.Ordinal);
        (await DownloadAsync(token[..^2] + "AA")).Status.ShouldBe(404);
        _factory.Clock.Advance(Duration.FromDays(8));
        (await DownloadAsync(token)).Status.ShouldBe(404);
        await _api.RunJobAsync(ProcessDataExportsJob);
        (await _api.ScalarAsync<bool>($"SELECT content IS NULL FROM customers.data_exports WHERE id = '{exportId}'")).ShouldBeTrue("süresi dolan arşiv silinir");
    }

    [Fact]
    public async Task ApplyRetention_AnonymizesExpiredCards_KeepsRecentOnes_ClearsOldAuditDetails()
    {
        var old = await CreateCustomerAsync(_token, "Derya Şahin", "+905550002118", "Uzun saç");
        var recent = await CreateCustomerAsync(_token, "Selin Öztürk", "+905550004418", null);
        var withinFive = await CreateCustomerAsync(_token, "Ece Koç", "+905550003318", null);
        // Saat 2026-11-25: 3 yıl öncesi 2023-11-25.
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET last_visit_on = '2023-01-10' WHERE id = '{old}' RETURNING 1");
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET last_visit_on = '2026-10-01' WHERE id = '{recent}' RETURNING 1");
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET last_visit_on = '2022-06-01' WHERE id = '{withinFive}' RETURNING 1");
        await _api.ScalarAsync<int>(
            "INSERT INTO platform.audit_logs (actor_type, action, entity_type, ip_address, user_agent, occurred_utc) "
            + "VALUES ('User', 'View', 'BrandCustomer', '10.0.0.1', 'test', '2024-01-01T00:00:00Z') RETURNING 1");

        await _api.RunJobAsync(ApplyRetentionJob);

        (await _api.ScalarAsync<string>($"SELECT display_name FROM customers.brand_customers WHERE id = '{old}'")).ShouldBe("Silinmiş müşteri");
        (await _api.ScalarAsync<bool>($"SELECT notes IS NULL FROM customers.brand_customers WHERE id = '{old}'")).ShouldBeTrue();
        (await _api.ScalarAsync<bool>($"SELECT anonymized_utc IS NULL FROM customers.brand_customers WHERE id = '{recent}'")).ShouldBeTrue();
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE action = 'Anonymize' AND actor_type = 'System'")).ShouldBe(2L, "Derya ve Ece (3 yıl)");
        (await _api.ScalarAsync<bool>("SELECT ip_address IS NULL AND user_agent IS NULL FROM platform.audit_logs WHERE occurred_utc = '2024-01-01T00:00:00Z'")).ShouldBeTrue();
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE occurred_utc = '2024-01-01T00:00:00Z'")).ShouldBe(1L, "kayıt silinmez");
    }

    [Fact]
    public async Task ApplyRetention_UsesBrandRetentionYears()
    {
        var card = await CreateCustomerAsync(_token, "Ece Koç", "+905550003318", null);
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers SET last_visit_on = '2022-06-01' WHERE id = '{card}' RETURNING 1");
        var privacy = await _api.GetAsync("/api/v1/manage/brand/privacy", _token);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/privacy", new { noticeMode = "Standard", transactionRetentionYears = 5 }, _token, privacy.ETag))
            .Status.ShouldBe(200);

        await _api.RunJobAsync(ApplyRetentionJob);

        (await _api.ScalarAsync<bool>($"SELECT anonymized_utc IS NULL FROM customers.brand_customers WHERE id = '{card}'")).ShouldBeTrue("5 yıl dolmadı");
    }

    private static Dictionary<string, string> Unzip(byte[] archive)
    {
        using var zip = new ZipArchive(new MemoryStream(archive), ZipArchiveMode.Read);
        return zip.Entries.ToDictionary(
            e => e.FullName,
            e =>
            {
                using var reader = new StreamReader(e.Open(), Encoding.UTF8);
                return reader.ReadToEnd();
            },
            StringComparer.Ordinal);
    }

    // İndirme bağlantısı e-postayla gider (T-121); testte jeton uygulamanın imzalayıcısıyla üretilir.
    private string CreateToken(Guid brandId, Guid exportId, Instant expires)
    {
        var type = Type.GetType("Omvila.Modules.Customers.Features.DataExports.DataExportLinks, Omvila.Modules.Customers", throwOnError: true)!;
        var links = _factory.Services.GetRequiredService(type);
        return (string)type.GetMethod("CreateToken")!.Invoke(links, [brandId, exportId, expires])!;
    }

    private async Task<(int Status, string? ContentType, byte[] Body)> DownloadAsync(string token)
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync(new Uri($"/api/v1/public/data-exports/{token}", UriKind.Relative), TestContext.Current.CancellationToken);
        return ((int)response.StatusCode, response.Content.Headers.ContentType?.MediaType, await response.Content.ReadAsByteArrayAsync(TestContext.Current.CancellationToken));
    }

    private async Task<string> CreateCustomerAsync(string token, string fullName, string phone, string? notes)
    {
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/customers", new { fullName, phone, notes }, token, Key());
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("id").GetString()!;
    }

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
