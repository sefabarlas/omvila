using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriterleri 7 (hizmet, aşamalı süre, kapora IBAN yok), 9 (şube fiyatı), 13 (yetki), 14 (izolasyon),
// 15 (412), § Alan kuralları (Services kurulum adımı), roller-yetkiler.md *** (şube yöneticisi yalnız kendi şubesinin fiyatı).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class CatalogTests(PostgresFixture db) : IAsyncLifetime
{
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _ownerStaffId = null!;
    private string _merkez = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _ownerStaffId = session.GetProperty("membership").GetProperty("staffId").GetString()!;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Category_CreateListAndDuplicateName()
    {
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/service-categories", new { name = "Saç" }, _token);
        created.Status.ShouldBe(201);
        created.Body.GetProperty("sortOrder").GetInt32().ShouldBe(1);

        var duplicate = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/service-categories", new { name = "Saç" }, _token);
        duplicate.Status.ShouldBe(400);
        duplicate.Body.GetProperty("errors").GetProperty("name")[0].GetString().ShouldBe("already_exists");

        var renamed = await _api.SendAsync(HttpMethod.Patch, $"/api/v1/manage/service-categories/{created.Body.GetProperty("id").GetString()}",
            new { name = "Saç bakımı", sortOrder = 5 }, _token);
        renamed.Status.ShouldBe(200);
        (await _api.GetAsync("/api/v1/manage/service-categories", _token)).Body.GetProperty("items")[0].GetProperty("name").GetString().ShouldBe("Saç bakımı");
    }

    [Fact]
    public async Task Service_SimpleAndStaged_AndOnboardingStep()
    {
        var category = await CategoryAsync("Saç");

        var haircut = await CreateServiceAsync(Service(category, "Saç kesimi", 45, 45000));
        haircut.Status.ShouldBe(201);
        haircut.ETag.ShouldNotBeNull();
        haircut.Body.GetProperty("price").GetProperty("amountMinor").GetInt64().ShouldBe(45000);
        haircut.Body.GetProperty("staffIds")[0].GetString().ShouldBe(_ownerStaffId);

        var stages = new[]
        {
            new { name = "Uygulama", durationMinutes = 45, staffBusy = true },
            new { name = "Bekleme", durationMinutes = 40, staffBusy = false },
            new { name = "Yıkama ve fön", durationMinutes = 35, staffBusy = true },
        };
        var dye = await CreateServiceAsync(Service(category, "Boya", 120, 180000, stages: stages));
        dye.Status.ShouldBe(201);
        dye.Body.GetProperty("stages").GetArrayLength().ShouldBe(3);
        dye.Body.GetProperty("durationMinutes").GetInt32().ShouldBe(120);

        var mismatch = await CreateServiceAsync(Service(category, "Balyaj", 100, 250000, stages: stages));
        mismatch.Status.ShouldBe(400);
        mismatch.Body.GetProperty("errors").GetProperty("durationMinutes")[0].GetString().ShouldBe("stage_sum_mismatch");

        var steps = (await _api.GetAsync("/api/v1/manage/onboarding", _token)).Body.GetProperty("steps").EnumerateArray()
            .ToDictionary(s => s.GetProperty("key").GetString()!, s => s.GetProperty("done").GetBoolean(), StringComparer.Ordinal);
        steps["Services"].ShouldBeTrue();
    }

    [Fact]
    public async Task Service_DepositAlwaysWithoutIban_Returns409()
    {
        var result = await CreateServiceAsync(Service(await CategoryAsync("Saç"), "Saç kesimi", 45, 45000, depositMode: "Always", depositPercent: 20));

        result.Status.ShouldBe(409);
        result.Body.GetProperty("code").GetString().ShouldBe("deposit_unavailable_no_iban");
    }

    [Fact]
    public async Task Service_UnknownStaffOrCategory_AndDuplicateName_Return400()
    {
        var category = await CategoryAsync("Saç");
        (await CreateServiceAsync(Service(category, "Saç kesimi", 45, 45000))).Status.ShouldBe(201);

        var result = await CreateServiceAsync(Service(category, "Saç kesimi", 45, 45000, staffIds: ["0193a8f0-0000-7000-8000-0000000000ff"]));

        result.Status.ShouldBe(400);
        result.Body.GetProperty("errors").GetProperty("name")[0].GetString().ShouldBe("already_exists");
        result.Body.GetProperty("errors").GetProperty("staffIds")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task Service_Update_WithStaleVersion_Returns412()
    {
        var category = await CategoryAsync("Saç");
        var created = await CreateServiceAsync(Service(category, "Saç kesimi", 45, 45000));
        var id = created.Body.GetProperty("id").GetString();

        var first = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/services/{id}", Service(category, "Saç kesimi", 50, 50000), _token, created.ETag);
        var second = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/services/{id}", Service(category, "Saç kesimi", 40, 40000), _token, created.ETag);

        first.Status.ShouldBe(200);
        first.Body.GetProperty("durationMinutes").GetInt32().ShouldBe(50);
        second.Status.ShouldBe(412, "kabul kriteri 15");
        second.Body.GetProperty("code").GetString().ShouldBe("concurrency_conflict");
    }

    [Fact]
    public async Task BranchPrice_OverridesPriceForThatBranchOnly()
    {
        var service = (await CreateServiceAsync(Service(await CategoryAsync("Saç"), "Saç kesimi", 45, 45000))).Body.GetProperty("id").GetString();
        var nisantasi = await BranchAsync("Nişantaşı");

        var set = await _api.SendAsync(HttpMethod.Put, $"/api/v1/manage/services/{service}/branch-prices/{nisantasi}",
            new { price = new { amountMinor = 55000, currency = "TRY" }, isAvailable = true }, _token);
        set.Status.ShouldBe(200);

        var atNisantasi = (await _api.GetAsync($"/api/v1/manage/services?branchId={nisantasi}", _token)).Body.GetProperty("items")[0];
        atNisantasi.GetProperty("price").GetProperty("amountMinor").GetInt64().ShouldBe(55000);
        atNisantasi.GetProperty("isBranchPrice").GetBoolean().ShouldBeTrue();
        var atMerkez = (await _api.GetAsync($"/api/v1/manage/services?branchId={_merkez}", _token)).Body.GetProperty("items")[0];
        atMerkez.GetProperty("price").GetProperty("amountMinor").GetInt64().ShouldBe(45000);
        atMerkez.GetProperty("isBranchPrice").GetBoolean().ShouldBeFalse();

        await _api.SendAsync(HttpMethod.Put, $"/api/v1/manage/services/{service}/branch-prices/{nisantasi}",
            new { price = new { amountMinor = 55000, currency = "TRY" }, isAvailable = false }, _token);
        (await _api.GetAsync($"/api/v1/manage/services?branchId={nisantasi}", _token)).Body.GetProperty("items").GetArrayLength()
            .ShouldBe(0, "şubede verilmeyen hizmet listelenmez");

        (await _api.SendAsync(HttpMethod.Delete, $"/api/v1/manage/services/{service}/branch-prices/{nisantasi}", null, _token)).Status.ShouldBe(204);
        (await _api.GetAsync($"/api/v1/manage/services?branchId={nisantasi}", _token)).Body.GetProperty("items")[0]
            .GetProperty("price").GetProperty("amountMinor").GetInt64().ShouldBe(45000);
    }

    [Fact]
    public async Task OtherBrand_CannotSeeOrChangeServices()
    {
        var service = (await CreateServiceAsync(Service(await CategoryAsync("Saç"), "Saç kesimi", 45, 45000))).Body.GetProperty("id").GetString();
        var other = await _api.SignupAsync("+905330000001", "Berber Kaan");

        (await _api.GetAsync($"/api/v1/manage/services/{service}", other)).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/manage/services", other)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.GetAsync("/api/v1/manage/service-categories", other)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        (await _api.SendAsync(HttpMethod.Put, $"/api/v1/manage/services/{service}/branch-prices/{_merkez}",
            new { price = new { amountMinor = 1, currency = "TRY" }, isAvailable = true }, other)).Status.ShouldBe(404);
    }

    [Fact]
    public async Task BranchManager_SetsOwnBranchPriceOnly_CannotCreateService()
    {
        var category = await CategoryAsync("Saç");
        var service = (await CreateServiceAsync(Service(category, "Saç kesimi", 45, 45000))).Body.GetProperty("id").GetString();
        var kadikoy = await BranchAsync("Kadıköy");
        var nisantasi = await BranchAsync("Nişantaşı");
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "BranchManager", kadikoy);

        (await _api.SendAsync(HttpMethod.Put, $"/api/v1/manage/services/{service}/branch-prices/{kadikoy}",
            new { price = new { amountMinor = 48000, currency = "TRY" }, isAvailable = true }, manager)).Status.ShouldBe(200);
        (await _api.SendAsync(HttpMethod.Put, $"/api/v1/manage/services/{service}/branch-prices/{nisantasi}",
            new { price = new { amountMinor = 48000, currency = "TRY" }, isAvailable = true }, manager)).Status.ShouldBe(404);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/services", Service(category, "Fön", 30, 30000), manager, Key())).Status.ShouldBe(403);
    }

    private object Service(
        string categoryId, string name, int duration, long priceMinor, object[]? stages = null, string depositMode = "Inherit",
        int? depositPercent = null, string[]? staffIds = null) => new
        {
            categoryId,
            name,
            durationMinutes = duration,
            price = new { amountMinor = priceMinor, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = depositMode, percent = depositPercent },
            isOnlineBookable = true,
            stages = stages ?? [],
            staffIds = staffIds ?? [_ownerStaffId],
        };

    private Task<ManageApi.Response> CreateServiceAsync(object body) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/manage/services", body, _token, Key());

    private async Task<string> CategoryAsync(string name) =>
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/service-categories", new { name }, _token)).Body.GetProperty("id").GetString()!;

    private async Task<string> BranchAsync(string name) =>
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/branches",
            new { name, addressLine = "Cad. 1", district = name, city = "İstanbul" }, _token, Key())).Body.GetProperty("id").GetString()!;

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
