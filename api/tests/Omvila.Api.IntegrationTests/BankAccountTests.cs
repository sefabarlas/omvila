using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-04 KK1, sözleşme manageGetBankAccount / manageUpdateBankAccount (hesap yokken 200 configured=false ve W/"0";
// ilk kayıt If-Match W/"0"; IBAN yanıtta maskeli; geçersiz IBAN 400 invalid_format; settings.payment yalnız Owner),
// alan-modeli.md § BrandBankAccount (aktif hesap yoksa kapora konamaz: depositAvailable, deposit_unavailable_no_iban),
// publicGetBranchCatalog hasDeposit/depositHint, guvenlik-kvkk.md (IBAN şifreli saklanır).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class BankAccountTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Path = "/api/v1/manage/brand/bank-account";
    private const string ValidIban = "tr29 0006 4000 0011 2345 6789 01";

    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Get_WithoutAccount_ReturnsDefaultsWithVersionZero()
    {
        var result = await _api.GetAsync(Path, _token);

        result.Status.ShouldBe(200);
        result.ETag.ShouldBe("W/\"0\"");
        result.Body.GetProperty("configured").GetBoolean().ShouldBeFalse();
        result.Body.GetProperty("isActive").GetBoolean().ShouldBeFalse();
        result.Body.GetProperty("transferCodePrefix").GetString().ShouldBe("OMV");
        result.Body.GetProperty("transferWindowMinutes").GetInt32().ShouldBe(120);
        result.Body.TryGetProperty("ibanMasked", out _).ShouldBeFalse();
        result.Body.TryGetProperty("accountHolderName", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task Put_FirstSave_CreatesActiveAccount_IbanMaskedAndEncrypted()
    {
        var saved = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: ValidIban), _token, "W/\"0\"");

        saved.Status.ShouldBe(200, saved.Body.ToString());
        saved.Body.GetProperty("configured").GetBoolean().ShouldBeTrue();
        saved.Body.GetProperty("isActive").GetBoolean().ShouldBeTrue();
        saved.Body.GetProperty("accountHolderName").GetString().ShouldBe("Elif Kaya");
        saved.Body.GetProperty("ibanMasked").GetString().ShouldBe("TR29 •••• 6789 01");
        saved.Body.GetProperty("ibanLast4").GetString().ShouldBe("8901");
        saved.Body.GetProperty("updatedUtc").GetString().ShouldNotBeNull();
        saved.Body.TryGetProperty("iban", out _).ShouldBeFalse();
        saved.Body.ToString().ShouldNotContain("0006400000112345");
        saved.ETag.ShouldBe($"W/\"{saved.Body.GetProperty("version").GetString()}\"");

        var stored = await _api.ScalarAsync<string>("SELECT encode(iban_encrypted, 'escape') FROM tenancy.brand_bank_accounts");
        stored.ShouldNotContain("0006400000112345", Case.Insensitive, "IBAN şifreli saklanır");

        var read = await _api.GetAsync(Path, _token);
        read.Body.GetProperty("ibanMasked").GetString().ShouldBe("TR29 •••• 6789 01");
        read.ETag.ShouldBe(saved.ETag);
    }

    [Fact]
    public async Task Put_UpdateWithoutIban_KeepsIban_StaleVersionIs412()
    {
        var first = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: ValidIban), _token, "W/\"0\"");

        var updated = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: null, prefix: "ELIF", window: 240), _token, first.ETag);

        updated.Status.ShouldBe(200, updated.Body.ToString());
        updated.Body.GetProperty("ibanMasked").GetString().ShouldBe("TR29 •••• 6789 01");
        updated.Body.GetProperty("transferCodePrefix").GetString().ShouldBe("ELIF");
        updated.Body.GetProperty("transferWindowMinutes").GetInt32().ShouldBe(240);

        var stale = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: null), _token, first.ETag);
        stale.Status.ShouldBe(412);
        stale.Body.GetProperty("code").GetString().ShouldBe("concurrency_conflict");
        (await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: ValidIban), _token, "W/\"0\"")).Status.ShouldBe(412, "hesap varken W/\"0\"");
    }

    [Fact]
    public async Task Put_FirstSaveWithStaleVersion_Is412()
    {
        var result = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: ValidIban), _token, "W/\"5521\"");

        result.Status.ShouldBe(412);
    }

    [Fact]
    public async Task Put_InvalidOrMissingIban_Is400()
    {
        var invalid = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: "TR12 0006 4000 0011 2345 6789 01"), _token, "W/\"0\"");
        invalid.Status.ShouldBe(400);
        invalid.Body.GetProperty("code").GetString().ShouldBe("validation_failed");
        invalid.Body.GetProperty("errors").GetProperty("iban")[0].GetString().ShouldBe("invalid_format");

        var missing = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: null), _token, "W/\"0\"");
        missing.Status.ShouldBe(400);
        missing.Body.GetProperty("errors").GetProperty("iban")[0].GetString().ShouldBe("required");

        var window = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: ValidIban, window: 90), _token, "W/\"0\"");
        window.Body.GetProperty("errors").GetProperty("transferWindowMinutes")[0].GetString().ShouldBe("out_of_range");
        (await _api.GetAsync(Path, _token)).Body.GetProperty("configured").GetBoolean().ShouldBeFalse();
    }

    [Theory]
    [InlineData("BranchManager")]
    [InlineData("Staff")]
    public async Task OtherRoles_AreForbidden(string role)
    {
        var token = await _api.AddMemberAndSignInAsync(_token, "+905382741905", role, _merkez);

        (await _api.GetAsync(Path, token)).Status.ShouldBe(403);
        var put = await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: ValidIban), token, "W/\"0\"");
        put.Status.ShouldBe(403);
        put.Body.GetProperty("code").GetString().ShouldBe("forbidden");
    }

    [Fact]
    public async Task DepositAvailable_FollowsAccount_AndAllowsDepositPolicy()
    {
        var before = await _api.GetAsync("/api/v1/manage/brand/settings", _token);
        before.Body.GetProperty("depositAvailable").GetBoolean().ShouldBeFalse();

        await SaveAccountAsync();

        var after = await _api.GetAsync("/api/v1/manage/brand/settings", _token);
        after.Body.GetProperty("depositAvailable").GetBoolean().ShouldBeTrue();
        var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(after.Body.GetRawText())!;
        body.Remove("version");
        body.Remove("depositAvailable");
        body["depositPolicy"] = JsonSerializer.SerializeToElement("Always");
        var always = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/settings", body, _token, after.ETag);
        always.Status.ShouldBe(200, always.Body.ToString());
        always.Body.GetProperty("depositPolicy").GetString().ShouldBe("Always");
    }

    private async Task SaveAccountAsync() =>
        (await _api.WriteAsync(HttpMethod.Put, Path, Body(iban: ValidIban), _token, "W/\"0\"")).Status.ShouldBe(200);

    private static object Body(string? iban, string prefix = "OMV", int window = 120) => iban is null
        ? new { accountHolderName = "Elif Kaya", bankName = "Ziraat Bankası", transferCodePrefix = prefix, transferWindowMinutes = window }
        : new { accountHolderName = "Elif Kaya", bankName = "Ziraat Bankası", iban, transferCodePrefix = prefix, transferWindowMinutes = window };
}
