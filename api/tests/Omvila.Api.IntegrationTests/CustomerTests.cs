using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-02 kabul kriteri 15 (kayıtlı numarayla ekleme → phone_already_exists + "Mevcut kartı aç"; "4418" son 4
// haneyle bulur; personel rolünde liste maskeli telefon döner), sözleşme manage-customers, roller-yetkiler.md
// customer.view / customer.phone.view / customer.edit ve ** dipnotu, alan-modeli.md § Customer (markalar global kaydın
// tamamını görmez), ADR-0006 (başka markanın kartı 404).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class CustomerTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Selin = "+905550004418";
    private const string Derya = "+905550002118";

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
    public async Task Create_ReturnsSummary_AndCardIsReadable()
    {
        var created = await CreateAsync(new { fullName = "Selin Öztürk", phone = "0555 000 44 18", birthYear = 1992, notes = "Kısa saç sever", sendAppointmentSms = false });

        created.Status.ShouldBe(201);
        var body = created.Body;
        body.GetProperty("fullName").GetString().ShouldBe("Selin Öztürk");
        body.GetProperty("phone").GetString().ShouldBe(Selin, "Owner customer.phone.view taşır");
        body.GetProperty("phoneMasked").GetString().ShouldBe("0555 *** ** 18");
        body.GetProperty("birthYear").GetInt32().ShouldBe(1992);
        body.GetProperty("visitCount").GetInt32().ShouldBe(0);
        body.GetProperty("noShowCount").GetInt32().ShouldBe(0);
        body.GetProperty("totalSpent").GetProperty("amountMinor").GetInt64().ShouldBe(0);
        body.GetProperty("totalSpent").GetProperty("currency").GetString().ShouldBe("TRY");
        body.GetProperty("isNew").GetBoolean().ShouldBeTrue();
        body.GetProperty("isNoShowRisk").GetBoolean().ShouldBeFalse();
        body.GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ShouldBe(["Yeni"]);
        body.GetProperty("notes").GetString().ShouldBe("Kısa saç sever");
        (await _api.ScalarAsync<string>("SELECT source || '/' || appointment_sms_enabled FROM customers.brand_customers")).ShouldBe("Panel/false");
        (await _api.ScalarAsync<bool>("SELECT is_verified FROM customers.customers")).ShouldBeFalse("panelden eklenen numara doğrulanmamıştır");

        var id = body.GetProperty("id").GetString();
        var card = await _api.GetAsync($"/api/v1/manage/customers/{id}", _token);
        card.Status.ShouldBe(200);
        card.Body.GetProperty("fullName").GetString().ShouldBe("Selin Öztürk");
        var appointments = await _api.GetAsync($"/api/v1/manage/customers/{id}/appointments", _token);
        appointments.Status.ShouldBe(200);
        appointments.Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        appointments.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
    }

    [Fact]
    public async Task Create_WithRegisteredPhone_Returns409_WithExistingCardId()
    {
        var first = await CreateAsync(new { fullName = "Selin Öztürk", phone = Selin });

        var again = await CreateAsync(new { fullName = "Selin Ö.", phone = "0555 000 4418" });

        again.Status.ShouldBe(409);
        again.Body.GetProperty("code").GetString().ShouldBe("phone_already_exists");
        again.Body.GetProperty("meta").GetProperty("brandCustomerId").GetString().ShouldBe(first.Body.GetProperty("id").GetString());
    }

    [Fact]
    public async Task Create_Validation()
    {
        var invalid = await CreateAsync(new { fullName = "S", phone = "12345", birthYear = 2027, notes = new string('n', 2001) });

        invalid.Status.ShouldBe(400);
        invalid.Body.GetProperty("code").GetString().ShouldBe("validation_failed");
        var errors = invalid.Body.GetProperty("errors");
        errors.GetProperty("phone")[0].GetString().ShouldBe("invalid_format");
        foreach (var field in new[] { "fullName", "birthYear", "notes" })
        {
            errors.TryGetProperty(field, out _).ShouldBeTrue(field);
        }
    }

    [Fact]
    public async Task Search_ByNameAndByLastFourDigits()
    {
        await CreateAsync(new { fullName = "Selin Öztürk", phone = Selin });
        await CreateAsync(new { fullName = "Derya Şahin", phone = Derya });

        Names(await ListAsync("?q=4418")).ShouldBe(["Selin Öztürk"], "kabul kriteri 15");
        Names(await ListAsync("?q=derya")).ShouldBe(["Derya Şahin"]);
        Names(await ListAsync("?q=0555%20000%2021%2018")).ShouldBe(["Derya Şahin"]);
        Names(await ListAsync("?q=%25")).ShouldBeEmpty("LIKE joker karakteri kaçırılır");
        Names(await ListAsync(string.Empty)).ShouldBe(["Derya Şahin", "Selin Öztürk"], ignoreOrder: true);
        Names(await ListAsync("?filter=New")).Count.ShouldBe(2);
        Names(await ListAsync("?filter=NoShowRisk")).ShouldBeEmpty();
        Names(await ListAsync("?filter=WithPackage")).ShouldBeEmpty();
        (await _api.GetAsync("/api/v1/manage/customers?filter=Vip", _token)).Status.ShouldBe(400);
        (await _api.GetAsync("/api/v1/manage/customers?limit=101", _token)).Status.ShouldBe(400);
    }

    [Fact]
    public async Task List_PagesWithCursor_LastVisitFirst()
    {
        await CreateAsync(new { fullName = "Selin Öztürk", phone = Selin });
        await CreateAsync(new { fullName = "Derya Şahin", phone = Derya });
        await CreateAsync(new { fullName = "Pınar Yalçın", phone = "+905550000941" });
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers b SET visit_count = 12, first_visit_on = '2025-01-10', last_visit_on = '2026-11-25' FROM customers.customers c WHERE c.id = b.customer_id AND c.phone_e164 = '{Selin}' RETURNING 1");
        await _api.ScalarAsync<int>($"UPDATE customers.brand_customers b SET visit_count = 2, no_show_count = 2, first_visit_on = '2026-03-01', last_visit_on = '2026-09-14' FROM customers.customers c WHERE c.id = b.customer_id AND c.phone_e164 = '{Derya}' RETURNING 1");

        var page1 = await ListAsync("?limit=2");
        Names(page1).ShouldBe(["Selin Öztürk", "Derya Şahin"]);
        var cursor = page1.GetProperty("nextCursor").GetString()!;
        var page2 = await ListAsync($"?limit=2&cursor={Uri.EscapeDataString(cursor)}");
        Names(page2).ShouldBe(["Pınar Yalçın"]);
        page2.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);

        var items = page1.GetProperty("items");
        items[0].GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ShouldBe(["Düzenli"]);
        items[1].GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ShouldBe(["2 kez gelmedi"]);
        items[1].GetProperty("isNoShowRisk").GetBoolean().ShouldBeTrue();
        Names(await ListAsync("?filter=Regular")).ShouldBe(["Selin Öztürk"]);
        Names(await ListAsync("?filter=NoShowRisk")).ShouldBe(["Derya Şahin"]);
        Names(await ListAsync("?filter=Lapsed60")).ShouldBe(["Derya Şahin"]);
        Names(await ListAsync("?filter=New")).ShouldBe(["Pınar Yalçın"]);
        (await _api.GetAsync("/api/v1/manage/customers?cursor=bozuk", _token)).Status.ShouldBe(400);
    }

    [Fact]
    public async Task StaffRole_SeesMaskedPhoneOnly_UnlessBrandUnmasks()
    {
        await CreateAsync(new { fullName = "Selin Öztürk", phone = Selin });
        var staff = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "Staff", _merkez);

        var masked = (await _api.GetAsync("/api/v1/manage/customers?q=4418", staff)).Body.GetProperty("items")[0];
        masked.TryGetProperty("phone", out _).ShouldBeFalse("kabul kriteri 15: personel tam numarayı görmez");
        masked.GetProperty("phoneMasked").GetString().ShouldBe("0555 *** ** 18");
        var card = await _api.GetAsync($"/api/v1/manage/customers/{masked.GetProperty("id").GetString()}", staff);
        card.Body.TryGetProperty("phone", out _).ShouldBeFalse();
        (await _api.SendAsync(HttpMethod.Get, "/api/v1/manage/customers", null, staff)).Body.GetRawText().ShouldNotContain(Selin);

        await _api.ScalarAsync<int>("UPDATE tenancy.brand_settings SET mask_customer_phone_from_staff = false RETURNING 1");
        (await _api.GetAsync("/api/v1/manage/customers", staff)).Body.GetProperty("items")[0].GetProperty("phone").GetString()
            .ShouldBe(Selin, "ayar kapalıysa personel de tam görür");
    }

    [Fact]
    public async Task OtherBrand_SharesGlobalCustomer_ButNotTheCard()
    {
        var elifCard = (await CreateAsync(new { fullName = "Selin Öztürk", phone = Selin, birthYear = 1992, notes = "Elif'in notu" })).Body.GetProperty("id").GetString();
        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");

        var kaanCard = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/customers", new { fullName = "Selin", phone = Selin }, kaan, Key());

        kaanCard.Status.ShouldBe(201, "başka markadaki kayıt bu markada çakışma değildir");
        kaanCard.Body.GetProperty("fullName").GetString().ShouldBe("Selin");
        kaanCard.Body.TryGetProperty("birthYear", out _).ShouldBeFalse("Elif'in girdiği doğum yılı Kaan'a görünmez");
        kaanCard.Body.TryGetProperty("notes", out _).ShouldBeFalse("Elif'in notu Kaan'a görünmez");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.customers")).ShouldBe(1);
        (await _api.GetAsync($"/api/v1/manage/customers/{elifCard}", kaan)).Status.ShouldBe(404);
        (await _api.GetAsync($"/api/v1/manage/customers/{elifCard}/appointments", kaan)).Status.ShouldBe(404);
        Names((await _api.GetAsync("/api/v1/manage/customers", kaan)).Body).ShouldBe(["Selin"]);
    }

    private Task<ManageApi.Response> CreateAsync(object body) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/manage/customers", body, _token, Key());

    private async Task<JsonElement> ListAsync(string query)
    {
        var result = await _api.GetAsync($"/api/v1/manage/customers{query}", _token);
        result.Status.ShouldBe(200, result.Body.ToString());
        return result.Body;
    }

    private static List<string> Names(JsonElement list) =>
        [.. list.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("fullName").GetString()!)];

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
