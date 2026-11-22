using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 kabul kriteri 10 (personel yalnız kendi randevuları ve kendi primi), K2 (yalnız kişisel prim özeti); sözleşme
// manageGetToday (yetkiye bağlı kartlar izin yoksa null; telefon yok; iptal edilen yok), manageGetMyCommissionSummary (personel
// kaydı yoksa 404; ters kayıtlar düşülmüş tamsayı toplam), manageRevealCustomerPhone (denetim, no-store; personel yalnız kendi
// randevusundaki müşteri, roller-yetkiler.md customer.phone.view dipnotu); T-105.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul; kayıt saati 2 dk ilerletir). Elif (Owner, prim %24), Zeynep (Staff).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class BusinessAppTests(PostgresFixture db) : IAsyncLifetime
{
    private const string ZeynepPhone = "+905382741905";
    private const string DeryaPhone = "+905331112218";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _owner = null!;
    private string _zeynepToken = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _zeynep = null!;
    private string _kesim = null!;
    private string _selin = null!;
    private string _gizem = null!;
    private string _okan = null!;
    private string _derya = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _owner = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _owner)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;
        _zeynep = (await PostAsync("/api/v1/manage/staff",
            new { fullName = "Zeynep Aksoy", phone = ZeynepPhone, role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } })).Body.GetProperty("id").GetString()!;
        var week = new
        {
            days = Enumerable.Range(1, 6).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
        };
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _owner);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", week, _owner, hours.ETag)).Status.ShouldBe(200);
        foreach (var staff in new[] { _elif, _zeynep })
        {
            var detail = await _api.GetAsync($"/api/v1/manage/staff/{staff}", _owner);
            (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{staff}/branches/{_merkez}/working-hours", week, _owner, detail.ETag)).Status.ShouldBe(200);
        }

        await _api.ScalarAsync<int>($"UPDATE tenancy.staff SET service_commission_percent = 24 WHERE id = '{_elif}' RETURNING 1");
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _kesim = (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Saç kesimi",
            durationMinutes = 45,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif, _zeynep },
        })).Body.GetProperty("id").GetString()!;
        _zeynepToken = (await _api.SignInAsync(ZeynepPhone)).Body.GetProperty("accessToken").GetString()!;

        // Selin: Elif'te geldi ve tamamlandı (prim 450 ₺ × %24 = 108 ₺). Gizem: Zeynep'te geldi. Okan (Elif) ve Derya (Zeynep) onaylı.
        var (selin, selinETag) = await CreateAsync("Selin Öztürk", "0532 123 45 67", _elif, "WalkIn", "2026-11-25T09:00:00Z");
        _selin = selin;
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{selin}/complete",
            new { payments = new[] { new { method = "Cash", amount = new { amountMinor = 45000, currency = "TRY" } } } }, _owner,
            new Dictionary<string, string>(Key(), StringComparer.Ordinal) { ["If-Match"] = selinETag })).Status.ShouldBe(200);
        _gizem = (await CreateAsync("Gizem Arslan", "0532 000 00 11", _zeynep, "WalkIn", "2026-11-25T09:00:00Z")).Id;
        _okan = (await CreateAsync("Okan Er", "0533 000 00 40", _elif, "Phone", "2026-11-25T11:00:00Z")).Id;
        _derya = (await CreateAsync("Derya Şahin", "0533 111 22 18", _zeynep, "Phone", "2026-11-25T13:00:00Z")).Id;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Today_Owner_SeesTheWholeBranch_WithCards()
    {
        var today = await _api.GetAsync("/api/v1/manage/today", _owner);

        today.Status.ShouldBe(200, today.Body.ToString());
        var body = today.Body;
        body.GetProperty("branchId").GetString().ShouldBe(_merkez, "birincil şube");
        body.GetProperty("date").GetString().ShouldBe("2026-11-25");
        body.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        Ids(body.GetProperty("items")).ShouldBe([_selin, _gizem, _okan, _derya]);
        (body.GetProperty("completedCount").GetInt32(), body.GetProperty("totalCount").GetInt32()).ShouldBe((1, 4));
        var next = body.GetProperty("next");
        next.GetProperty("appointmentId").GetString().ShouldBe(_okan);
        next.GetProperty("customerName").GetString().ShouldBe("Okan Er");
        next.GetProperty("serviceSummary").GetString().ShouldBe("Saç kesimi");
        next.GetProperty("isNewCustomer").GetBoolean().ShouldBeTrue();
        body.GetRawText().ShouldNotContain("+90", Case.Sensitive, "telefon dönmez");
        body.GetProperty("pendingApprovalCount").GetInt32().ShouldBe(0);
        body.GetProperty("walkIns").GetProperty("waitingCount").GetInt32().ShouldBe(0);
        var commission = body.GetProperty("commission");
        commission.GetProperty("month").GetString().ShouldBe("2026-11");
        commission.GetProperty("totalMinor").GetInt64().ShouldBe(10800);
        commission.GetProperty("daily")[0].GetProperty("date").GetString().ShouldBe("2026-11-25");
    }

    [Fact]
    public async Task Today_Staff_SeesOnlyOwnAppointments_AndNoApprovalCard_OtherBrand404()
    {
        var today = await _api.GetAsync("/api/v1/manage/today", _zeynepToken);

        today.Status.ShouldBe(200, today.Body.ToString());
        Ids(today.Body.GetProperty("items")).ShouldBe([_gizem, _derya]);
        today.Body.GetProperty("next").GetProperty("appointmentId").GetString().ShouldBe(_derya);
        today.Body.GetProperty("pendingApprovalCount").ValueKind.ShouldBe(JsonValueKind.Null);
        today.Body.GetProperty("walkIns").ValueKind.ShouldBe(JsonValueKind.Object, "personel kapıda sırayı yönetir");
        today.Body.GetProperty("commission").GetProperty("totalMinor").GetInt64().ShouldBe(0);

        var manager = await _api.AddMemberAndSignInAsync(_owner, "+905320000077", "BranchManager", _merkez);
        var managed = (await _api.GetAsync("/api/v1/manage/today", manager)).Body;
        managed.GetProperty("commission").ValueKind.ShouldBe(JsonValueKind.Null, "personel kaydı yok");
        managed.GetProperty("items").GetArrayLength().ShouldBe(4);

        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");
        (await _api.GetAsync($"/api/v1/manage/today?branchId={_merkez}", kaan)).Status.ShouldBe(404);
    }

    [Fact]
    public async Task CommissionSummary_IsOnlyTheUsersOwn()
    {
        var summary = await _api.GetAsync("/api/v1/manage/me/commission-summary", _owner);

        summary.Status.ShouldBe(200, summary.Body.ToString());
        var body = summary.Body;
        body.GetProperty("staffId").GetString().ShouldBe(_elif);
        (body.GetProperty("month").GetString(), body.GetProperty("from").GetString(), body.GetProperty("to").GetString())
            .ShouldBe(("2026-11", "2026-11-01", "2026-11-25"));
        body.GetProperty("totalMinor").GetInt64().ShouldBe(10800);
        body.GetProperty("serviceRevenueMinor").GetInt64().ShouldBe(45000);
        body.GetProperty("appointmentCount").GetInt32().ShouldBe(1);
        body.GetProperty("noShowPercent").GetDouble().ShouldBe(0);
        body.GetProperty("averageRating").ValueKind.ShouldBe(JsonValueKind.Null);
        var entry = body.GetProperty("entries").EnumerateArray().ShouldHaveSingleItem();
        (entry.GetProperty("type").GetString(), entry.GetProperty("status").GetString(), entry.GetProperty("amountMinor").GetInt64())
            .ShouldBe(("Service", "Accrued", 10800L));
        (entry.GetProperty("customerName").GetString(), entry.GetProperty("description").GetString(), entry.GetProperty("appointmentId").GetString())
            .ShouldBe(("Selin Öztürk", "Saç kesimi", _selin));
        body.GetProperty("thisWeek").EnumerateArray().Select(d => d.GetProperty("date").GetString()).ShouldBe(["2026-11-23", "2026-11-24", "2026-11-25"]);
        var wednesday = body.GetProperty("thisWeek")[2];
        (wednesday.GetProperty("amountMinor").GetInt64(), wednesday.GetProperty("appointmentCount").GetInt32()).ShouldBe((10800L, 1));
        body.GetProperty("nextAccrual").ValueKind.ShouldBe(JsonValueKind.Null);

        var zeynep = (await _api.GetAsync("/api/v1/manage/me/commission-summary", _zeynepToken)).Body;
        zeynep.GetProperty("staffId").GetString().ShouldBe(_zeynep);
        zeynep.GetProperty("totalMinor").GetInt64().ShouldBe(0, "başkasının primi görünmez");
        zeynep.GetProperty("entries").GetArrayLength().ShouldBe(0);
        zeynep.GetProperty("noShowPercent").ValueKind.ShouldBe(JsonValueKind.Null);
        zeynep.GetProperty("nextAccrual").GetProperty("customerName").GetString().ShouldBe("Gizem Arslan");

        var october = (await _api.GetAsync("/api/v1/manage/me/commission-summary?month=2026-10", _owner)).Body;
        (october.GetProperty("to").GetString(), october.GetProperty("totalMinor").GetInt64()).ShouldBe(("2026-10-31", 0L));
        (await _api.GetAsync("/api/v1/manage/me/commission-summary?month=2026-13", _owner)).Status.ShouldBe(400);
        var manager = await _api.AddMemberAndSignInAsync(_owner, "+905320000077", "BranchManager", _merkez);
        (await _api.GetAsync("/api/v1/manage/me/commission-summary", manager)).Status.ShouldBe(404, "personel kaydı yok");
    }

    [Fact]
    public async Task RevealPhone_IsAudited_NotCached_StaffOnlyForOwnCustomers()
    {
        var derya = await CustomerOfAsync(_derya);
        var revealed = await RevealAsync(derya, _owner, "Call");

        revealed.Status.ShouldBe(200, revealed.Body.ToString());
        revealed.Body.GetProperty("phone").GetString().ShouldBe(DeryaPhone);
        revealed.Header("Cache-Control").ShouldBe("no-store");
        (await _api.ScalarAsync<string>("SELECT action || '/' || entity_type || '/' || entity_id || '/' || (changes->>'purpose') FROM platform.audit_logs"))
            .ShouldBe($"View/BrandCustomer/{derya}/Call");
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM platform.audit_logs WHERE changes::text LIKE '%{DeryaPhone[3..]}%'")).ShouldBe(0, "numara denetime yazılmaz");

        (await RevealAsync(derya, _zeynepToken, "Sms")).Status.ShouldBe(200, "kendi randevusundaki müşteri");
        (await RevealAsync(await CustomerOfAsync(_okan), _zeynepToken, "Call")).Status.ShouldBe(404, "başka personelin müşterisi");
        var invalid = await RevealAsync(derya, _owner, "Whatsapp");
        invalid.Status.ShouldBe(400);
        invalid.Body.GetProperty("errors").GetProperty("purpose")[0].GetString().ShouldBe("invalid_format");
        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");
        (await RevealAsync(derya, kaan, "Call")).Status.ShouldBe(404);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs")).ShouldBe(2);
    }

    private Task<ManageApi.Response> RevealAsync(string brandCustomerId, string token, string purpose) =>
        _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/customers/{brandCustomerId}/reveal-phone", new { purpose }, token);

    private async Task<string> CustomerOfAsync(string appointmentId) =>
        (await _api.GetAsync($"/api/v1/manage/appointments/{appointmentId}", _owner)).Body
            .GetProperty("customer").GetProperty("brandCustomerId").GetString()!;

    private async Task<(string Id, string ETag)> CreateAsync(string fullName, string phone, string staffId, string channel, string startUtc)
    {
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            newCustomer = new { fullName, phone },
            channel,
            startUtc,
            items = new[] { new { serviceId = _kesim, staffId } },
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        return (created.Body.GetProperty("appointment").GetProperty("id").GetString()!, created.ETag!);
    }

    private static List<string?> Ids(JsonElement items) => [.. items.EnumerateArray().Select(i => i.GetProperty("appointmentId").GetString())];

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _owner, Key());

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
