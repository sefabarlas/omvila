using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Omvila.Infrastructure.Security;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriterleri 13 (Staff ile ayar 403; şube yöneticisi başka şubeye 404) ve 14 (otomatik izolasyon:
// başka markanın kimlikleri her uçta 404), roller-yetkiler.md § Uygulama kuralları (izin yoksa 403, başka şube 404),
// ADR-0006. Uç listesi uygulamadan okunur: yeni uç eklenince bu testler onu da kapsar.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed partial class IsolationAndPermissionTests(PostgresFixture db) : IAsyncLifetime
{
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _elif = null!;
    private Dictionary<string, string> _kaanIds = null!;
    private Dictionary<string, string> _elifIds = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _elif = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        _elifIds = await SeedBrandAsync(_elif);
        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");
        _kaanIds = await SeedBrandAsync(kaan);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task EveryManageEndpointWithAnId_Returns404_ForAnotherBrandsIds()
    {
        var failures = new List<string>();
        var endpoints = ManageEndpoints().Where(e => RouteParameters(e.Pattern).Any(p => !PlatformParameters.ContainsKey(p))).ToList();
        endpoints.Count.ShouldBeGreaterThanOrEqualTo(19, "kimlikli manage uçları bulunamadı");
        foreach (var (method, pattern) in endpoints)
        {
            var path = Fill(pattern, _kaanIds);
            var result = await SendAsync(method, path, _elif);
            if (result.Status != 404)
            {
                failures.Add($"{method} {pattern} → {result.Status}");
            }
        }

        foreach (var path in new[]
                 {
                     $"/api/v1/manage/services?branchId={_kaanIds["branchId"]}", $"/api/v1/manage/staff?branchId={_kaanIds["branchId"]}",
                     $"/api/v1/manage/approval-queue?branchId={_kaanIds["branchId"]}",
                 })
        {
            var result = await SendAsync("GET", path, _elif);
            if (result.Status != 404)
            {
                failures.Add($"GET {path} → {result.Status}");
            }
        }

        failures.ShouldBeEmpty();
    }

    [Fact]
    public async Task ListEndpoints_NeverReturnAnotherBrandsIds()
    {
        var foreign = _kaanIds.Values.ToHashSet(StringComparer.Ordinal);
        foreach (var (method, pattern) in ManageEndpoints().Where(e => string.Equals(e.Method, "GET", StringComparison.Ordinal)))
        {
            var result = await SendAsync(method, Fill(pattern, _elifIds), _elif);
            var leaked = foreign.Where(id => result.Text.Contains(id, StringComparison.OrdinalIgnoreCase)).ToList();
            leaked.ShouldBeEmpty($"{pattern} başka markanın kimliğini döndü");
        }
    }

    [Fact]
    public async Task StaffRole_GetsForbidden_OnEveryEndpointItsRoleLacks()
    {
        var staffToken = await _api.AddMemberAndSignInAsync(_elif, "+905382741905", "Staff", _elifIds["branchId"]);
        var granted = (await _api.GetAsync("/api/v1/manage/session", staffToken)).Body.GetProperty("permissions")
            .EnumerateArray().Select(p => p.GetString()!).ToHashSet(StringComparer.Ordinal);
        var failures = new List<string>();

        var denied = ManageEndpointsWithPermission().Where(e => !granted.Contains(e.Permission)).ToList();
        denied.Count.ShouldBeGreaterThanOrEqualTo(15, "personelin izni olmayan uçlar bulunamadı");
        foreach (var (method, pattern, permission) in denied)
        {
            var result = await SendAsync(method, Fill(pattern, _elifIds), staffToken);
            if (result.Status != 403)
            {
                failures.Add($"{method} {pattern} ({permission}) → {result.Status}");
            }
        }

        failures.ShouldBeEmpty();
        (await SendAsync("PUT", "/api/v1/manage/brand/settings", staffToken)).Status.ShouldBe(403, "kabul kriteri 13");
    }

    [Fact]
    public async Task BranchManager_OtherBranchIsNotFound_OnEveryBranchScopedEndpoint()
    {
        var kadikoy = await PostIdAsync(_elif, "/api/v1/manage/branches", new { name = "Kadıköy", addressLine = "Cad. 1", district = "Kadıköy", city = "İstanbul" });
        var manager = await _api.AddMemberAndSignInAsync(_elif, "+905383333333", "BranchManager", kadikoy);
        var failures = new List<string>();

        // Yönetici Merkez'i (elifIds.branchId) görmez: şubeye bağlı her uçta 404 (varlık sızdırılmaz).
        foreach (var (method, pattern) in ManageEndpoints().Where(e => RouteParameters(e.Pattern).Contains("branchId", StringComparer.Ordinal)))
        {
            var result = await SendAsync(method, Fill(pattern, _elifIds), manager);
            if (result.Status is not (404 or 403))
            {
                failures.Add($"{method} {pattern} → {result.Status}");
            }
            else if (string.Equals(method, "GET", StringComparison.Ordinal) && result.Status != 404)
            {
                failures.Add($"{method} {pattern} → {result.Status} (okuma 404 olmalı)");
            }
        }

        failures.ShouldBeEmpty();
        (await SendAsync("GET", $"/api/v1/manage/branches/{_elifIds["branchId"]}", manager)).Status.ShouldBe(404, "kabul kriteri 13");
    }

    // Markaya ait olmayan rota parametreleri (platform verisi; her marka aynı değeri kullanır).
    private static readonly Dictionary<string, string> PlatformParameters = new(StringComparer.Ordinal) { ["industryCode"] = "hair_salon" };

    private IEnumerable<(string Method, string Pattern)> ManageEndpoints() =>
        ManageEndpointsWithPermission().Select(e => (e.Method, e.Pattern)).Concat(RouteEndpoints()
            .Where(e => e.Metadata.GetMetadata<PermissionMetadata>() is null)
            .SelectMany(e => Methods(e).Select(m => (m, e.RoutePattern.RawText!))))
            .Distinct();

    private IEnumerable<(string Method, string Pattern, string Permission)> ManageEndpointsWithPermission() =>
        RouteEndpoints()
            .Where(e => e.Metadata.GetMetadata<PermissionMetadata>() is not null)
            .SelectMany(e => Methods(e).Select(m => (m, e.RoutePattern.RawText!, e.Metadata.GetMetadata<PermissionMetadata>()!.Permission)));

    // Kimlik doğrulama uçları (/manage/auth) kapsam dışıdır: marka verisi taşımazlar.
    private IEnumerable<RouteEndpoint> RouteEndpoints()
    {
        using var _ = _factory.CreateClient();
        return _factory.Services.GetServices<EndpointDataSource>().SelectMany(s => s.Endpoints).OfType<RouteEndpoint>()
            .Where(e => e.RoutePattern.RawText?.StartsWith("/api/v1/manage/", StringComparison.Ordinal) == true
                && !e.RoutePattern.RawText.StartsWith("/api/v1/manage/auth/", StringComparison.Ordinal));
    }

    private static IEnumerable<string> Methods(RouteEndpoint e) =>
        e.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()?.HttpMethods ?? [];

    private static IEnumerable<string> RouteParameters(string pattern) => RouteParameter().Matches(pattern).Select(m => m.Groups["name"].Value);

    private static string Fill(string pattern, Dictionary<string, string> ids) =>
        RouteParameter().Replace(pattern, m => ids.TryGetValue(m.Groups["name"].Value, out var id) || PlatformParameters.TryGetValue(m.Groups["name"].Value, out id)
            ? id
            : throw new InvalidOperationException($"Test verisinde '{m.Groups["name"].Value}' yok: {pattern} için SeedBrandAsync'e ekleyin."));

    private async Task<(int Status, string Text)> SendAsync(string method, string path, string token)
    {
        var headers = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["If-Match"] = "W/\"1\"",
            ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}",
        };
        var body = method is "GET" or "DELETE" ? null : new { };
        var result = await _api.SendAsync(new HttpMethod(method), path, body, token, headers);
        return (result.Status, result.Body.ValueKind == System.Text.Json.JsonValueKind.Undefined ? string.Empty : result.Body.GetRawText());
    }

    // Her markaya ait rota parametresi için bir kimlik: şube, kategori, hizmet, personel, özel gün, izin, müşteri kartı, randevu.
    // İki marka aynı numarayı ekler: global kişi ortaktır, kartlar ayrıdır.
    private async Task<Dictionary<string, string>> SeedBrandAsync(string token)
    {
        var session = (await _api.GetAsync("/api/v1/manage/session", token)).Body;
        var branchId = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        var ownerStaffId = session.GetProperty("membership").GetProperty("staffId").GetString()!;
        var categoryId = await PostIdAsync(token, "/api/v1/manage/service-categories", new { name = "Saç" });
        var serviceId = await PostIdAsync(token, "/api/v1/manage/services", new
        {
            categoryId,
            name = "Saç kesimi",
            durationMinutes = 45,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { ownerStaffId },
        });
        var staffId = await PostIdAsync(token, "/api/v1/manage/staff",
            new { fullName = "Mert Demir", role = "Staff", calendarColor = "#B4532A", branchIds = new[] { branchId }, sendInvite = false });
        var overrideId = await PostIdAsync(token, $"/api/v1/manage/branches/{branchId}/overrides", new { fromDate = "2027-01-01", kind = "Holiday", isClosed = true });
        var timeOff = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/staff/{staffId}/time-off",
            new { type = "Leave", isAllDay = true, fromDate = "2026-12-02" }, token);
        var brandCustomerId = await PostIdAsync(token, "/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = "+905550004418" });
        var appointment = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/appointments", new
        {
            branchId,
            brandCustomerId,
            channel = "Phone",
            startUtc = "2026-12-03T10:00:00Z",
            items = new[] { new { serviceId, staffId = ownerStaffId } },
            notifyCustomer = false,
            allowOutsideWorkingHours = true,
        }, token, new Dictionary<string, string>(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" });
        appointment.Status.ShouldBe(201, appointment.Body.ToString());
        var appointmentId = appointment.Body.GetProperty("appointment").GetProperty("id").GetString()!;

        // Değerlendirme: tamamla ucu (T-88) ve /d/ gönderimi bu testin konusu değil; satır doğrudan yazılır.
        var brandId = session.GetProperty("brand").GetProperty("id").GetString()!;
        var reviewId = await _api.ScalarAsync<string>($"""
            INSERT INTO reviews.reviews (id, brand_id, branch_id, appointment_id, brand_customer_id, rating, service_names, is_published, created_utc)
            VALUES (gen_random_uuid(), '{brandId}', '{branchId}', '{appointmentId}', '{brandCustomerId}', 4, ARRAY['Saç kesimi'], true, now())
            RETURNING id::text
            """);
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["branchId"] = branchId,
            ["brandCustomerId"] = brandCustomerId,
            ["appointmentId"] = appointmentId,
            ["reviewId"] = reviewId,
            ["categoryId"] = categoryId,
            ["serviceId"] = serviceId,
            ["staffId"] = staffId,
            ["overrideId"] = overrideId,
            ["timeOffId"] = timeOff.Body.GetProperty("timeOff").GetProperty("id").GetString()!,
        };
    }

    private async Task<string> PostIdAsync(string token, string path, object body)
    {
        var result = await _api.SendAsync(HttpMethod.Post, path, body, token,
            new Dictionary<string, string>(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" });
        result.Status.ShouldBe(201, $"{path}: {result.Body}");
        return result.Body.GetProperty("id").GetString()!;
    }

    [GeneratedRegex(@"\{(?<name>\w+)(?::[^}]*)?\}", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex RouteParameter();
}
