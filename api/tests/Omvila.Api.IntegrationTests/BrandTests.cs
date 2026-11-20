using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: sözleşme manage-brand, api-kurallari.md § Eşzamanlı düzenleme (ETag/If-Match, 412/428) ve PATCH'te null,
// T-11 (bağlantı adı; eski ad 30 gün yönlendirilir), S-22 (IBAN yokken kapora açılamaz), sureler-ve-ayarlar.md.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class BrandTests(PostgresFixture db) : IAsyncLifetime
{
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task GetBrand_ReturnsETagMatchingVersion()
    {
        var brand = await _api.GetAsync("/api/v1/manage/brand", _token);

        brand.Status.ShouldBe(200);
        brand.Body.GetProperty("name").GetString().ShouldBe("Kuaför Elif");
        brand.Body.GetProperty("industryCode").GetString().ShouldBe("hair_salon");
        brand.ETag.ShouldBe($"W/\"{brand.Body.GetProperty("version").GetString()}\"");
    }

    [Fact]
    public async Task PatchBrand_UpdatesOnlySentFields_NullClears_PhoneNormalized()
    {
        var current = await _api.GetAsync("/api/v1/manage/brand", _token);
        var first = await _api.WriteAsync(HttpMethod.Patch, "/api/v1/manage/brand",
            new { shortDescription = "Saç ve bakım", phone = "0216 555 12 34", instagram = "kuaforelif" }, _token, current.ETag);
        first.Status.ShouldBe(200);
        string.Equals(first.ETag, current.ETag, StringComparison.Ordinal).ShouldBeFalse();

        var second = await _api.WriteAsync(HttpMethod.Patch, "/api/v1/manage/brand", new { instagram = (string?)null }, _token, first.ETag);

        second.Status.ShouldBe(200);
        second.Body.GetProperty("phone").GetString().ShouldBe("+902165551234");
        second.Body.GetProperty("shortDescription").GetString().ShouldBe("Saç ve bakım");
        (second.Body.TryGetProperty("instagram", out var instagram) && instagram.ValueKind != System.Text.Json.JsonValueKind.Null)
            .ShouldBeFalse("null değeri temizler (yanıtta boş alan yazılmaz)");
        second.Body.GetProperty("name").GetString().ShouldBe("Kuaför Elif");
    }

    [Fact]
    public async Task PatchBrand_WithStaleVersion_Returns412_WithoutIfMatch_Returns428()
    {
        var current = await _api.GetAsync("/api/v1/manage/brand", _token);
        (await _api.WriteAsync(HttpMethod.Patch, "/api/v1/manage/brand", new { name = "Kuaför Elif Kaya" }, _token, current.ETag)).Status.ShouldBe(200);

        var stale = await _api.WriteAsync(HttpMethod.Patch, "/api/v1/manage/brand", new { name = "Başka" }, _token, current.ETag);
        stale.Status.ShouldBe(412);
        stale.Body.GetProperty("code").GetString().ShouldBe("concurrency_conflict");
        (await _api.WriteAsync(HttpMethod.Patch, "/api/v1/manage/brand", new { name = "Başka" }, _token, ifMatch: null)).Status.ShouldBe(428);
    }

    [Fact]
    public async Task PatchBrand_InvalidFields_Return400()
    {
        var current = await _api.GetAsync("/api/v1/manage/brand", _token);

        var result = await _api.WriteAsync(HttpMethod.Patch, "/api/v1/manage/brand",
            new { name = "K", industryCode = "kuafor", email = "elif@", phone = "12" }, _token, current.ETag);

        result.Status.ShouldBe(400);
        var errors = result.Body.GetProperty("errors");
        errors.GetProperty("name")[0].GetString().ShouldBe("too_short");
        errors.GetProperty("industryCode")[0].GetString().ShouldBe("invalid_format");
        errors.GetProperty("email")[0].GetString().ShouldBe("invalid_format");
        errors.GetProperty("phone")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task Settings_DefaultsAndUpdate_DepositNeedsIban()
    {
        var current = await _api.GetAsync("/api/v1/manage/brand/settings", _token);
        current.Body.GetProperty("slotIntervalMinutes").GetInt32().ShouldBe(15);
        current.Body.GetProperty("depositPolicy").GetString().ShouldBe("RiskyCustomersOnly");
        current.Body.GetProperty("depositAvailable").GetBoolean().ShouldBeFalse();

        var always = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/settings", Settings("Always", 10), _token, current.ETag);
        always.Status.ShouldBe(409);
        always.Body.GetProperty("code").GetString().ShouldBe("deposit_unavailable_no_iban");

        var saved = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/settings", Settings("RiskyCustomersOnly", 10), _token, current.ETag);
        saved.Status.ShouldBe(200, "var olan kapora değeri korunabilir");
        saved.Body.GetProperty("slotIntervalMinutes").GetInt32().ShouldBe(10);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/settings", Settings("None", 10), _token, saved.ETag)).Status.ShouldBe(200);
    }

    [Fact]
    public async Task Settings_MissingField_Returns400()
    {
        var current = await _api.GetAsync("/api/v1/manage/brand/settings", _token);

        var result = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/settings", new { requiresApproval = true }, _token, current.ETag);

        result.Status.ShouldBe(400);
        result.Body.GetProperty("errors").GetProperty("slotIntervalMinutes")[0].GetString().ShouldBe("required");
    }

    [Fact]
    public async Task BookingPage_SlugChange_KeepsOldSlugRedirecting_AndBlocksOthers()
    {
        var page = await _api.GetAsync("/api/v1/manage/brand/booking-page", _token);
        page.Body.GetProperty("url").GetString().ShouldBe("https://omvila.com/kuafor-elif");

        var changed = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/booking-page", Page("elif-salon"), _token, page.ETag);

        changed.Status.ShouldBe(200);
        changed.Body.GetProperty("url").GetString().ShouldBe("https://omvila.com/elif-salon");
        changed.Body.GetProperty("coverColor").GetString().ShouldBe("mercan");
        (await _api.ScalarAsync<string>("SELECT slug || '/' || (redirect_expires_utc - changed_utc)::text FROM tenancy.brand_slug_history"))
            .ShouldBe("kuafor-elif/30 days");

        var other = await _api.SignupAsync("+905330000001", "Berber Kaan");
        var otherPage = await _api.GetAsync("/api/v1/manage/brand/booking-page", other);
        var taken = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/booking-page", Page("kuafor-elif"), other, otherPage.ETag);
        taken.Status.ShouldBe(409);
        taken.Body.GetProperty("code").GetString().ShouldBe("slug_unavailable");
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/booking-page", Page("elif-salon"), other, otherPage.ETag)).Status.ShouldBe(409);

        var back = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/booking-page", Page("kuafor-elif"), _token, changed.ETag);
        back.Status.ShouldBe(200, "işletme kendi eski adına dönebilir (T-11)");
    }

    [Theory]
    [InlineData("admin", 409, "slug_unavailable")]
    [InlineData("Kuafor_Elif", 400, "validation_failed")]
    public async Task BookingPage_ReservedOrInvalidSlug_IsRejected(string slug, int status, string code)
    {
        var page = await _api.GetAsync("/api/v1/manage/brand/booking-page", _token);

        var result = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/booking-page", Page(slug), _token, page.ETag);

        result.Status.ShouldBe(status);
        result.Body.GetProperty("code").GetString().ShouldBe(code);
    }

    private static object Page(string slug) => new
    {
        slug,
        coverColor = "mercan",
        showReviews = true,
        showPrices = false,
        preAppointmentNote = "Randevunuzdan 10 dakika önce gelmenizi rica ederiz.",
    };

    private static object Settings(string depositPolicy, int slotInterval) => new
    {
        requiresApproval = false,
        approvalTimeoutMinutes = 120,
        allowStaffSelection = true,
        allowAnyStaffOption = true,
        cancellationCutoffHours = 24,
        rescheduleCutoffHours = 12,
        noShowAfterMinutes = 15,
        minNoticeMinutes = 120,
        maxAdvanceDays = 60,
        slotIntervalMinutes = slotInterval,
        defaultBranchTransferMinutes = 30,
        depositPolicy,
        riskyCustomerNoShowThreshold = 2,
        maskCustomerPhoneFromStaff = true,
        reminderOffsetsMinutes = new[] { 1440, 120 },
        reviewInviteEnabled = true,
    };
}
