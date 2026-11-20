using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriterleri 5 (şube saatleri), 6 (özel gün), 13 (BranchManager başka şubeye 404), 14 (kiracı
// izolasyonu: başka markanın şubesi 404), § Alan kuralları (WorkingHours kurulum adımı), sözleşme manage-branches.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class BranchTests(PostgresFixture db) : IAsyncLifetime
{
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
        var list = await _api.GetAsync("/api/v1/manage/branches", _token);
        _merkez = list.Body.GetProperty("items")[0].GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task WorkingHours_SaveAndRead_WithBreak()
    {
        var current = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        current.Body.GetProperty("days").GetArrayLength().ShouldBe(0, "yeni şubenin saati boş");

        var saved = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", new
        {
            days = new object[]
            {
                new { dayOfWeek = 2, intervals = new[] { new { start = "13:45", end = "19:00" }, new { start = "10:00", end = "13:00" } } },
                new { dayOfWeek = 6, intervals = new[] { new { start = "09:00", end = "18:00" } } },
            },
        }, _token, current.ETag);

        saved.Status.ShouldBe(200);
        var read = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        var tuesday = read.Body.GetProperty("days")[0];
        tuesday.GetProperty("dayOfWeek").GetInt32().ShouldBe(2);
        tuesday.GetProperty("intervals").EnumerateArray().Select(i => $"{i.GetProperty("start").GetString()}-{i.GetProperty("end").GetString()}")
            .ShouldBe(["10:00-13:00", "13:45-19:00"]);
        read.ETag.ShouldBe(saved.ETag);

        var steps = (await _api.GetAsync("/api/v1/manage/onboarding", _token)).Body.GetProperty("steps").EnumerateArray()
            .ToDictionary(s => s.GetProperty("key").GetString()!, s => s.GetProperty("done").GetBoolean(), StringComparer.Ordinal);
        steps["WorkingHours"].ShouldBeTrue();
    }

    [Theory]
    [InlineData("10:00", "13:00", "12:00", "15:00", "overlapping")]
    [InlineData("22:00", "02:00", null, null, "invalid_range")]
    [InlineData("24:00", "25:00", null, null, "invalid_format")]
    public async Task WorkingHours_InvalidIntervals_Return400(string s1, string e1, string? s2, string? e2, string code)
    {
        var current = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        var intervals = s2 is null ? new[] { new { start = s1, end = e1 } } : [new { start = s1, end = e1 }, new { start = s2, end = e2! }];

        var result = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours",
            new { days = new[] { new { dayOfWeek = 2, intervals } } }, _token, current.ETag);

        result.Status.ShouldBe(400);
        result.Body.GetProperty("code").GetString().ShouldBe("validation_failed");
        result.Body.GetProperty("errors").EnumerateObject().SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString())).ShouldContain(c => string.Equals(c, code, StringComparison.Ordinal));
    }

    [Fact]
    public async Task Overrides_SingleDayAndThreeDayHoliday_AreListedPerDate()
    {
        var newYear = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/branches/{_merkez}/overrides",
            new { fromDate = "2027-01-01", kind = "Holiday", isClosed = true, note = "Yılbaşı" }, _token);
        var bayram = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/branches/{_merkez}/overrides",
            new { fromDate = "2027-03-09", toDate = "2027-03-11", kind = "Holiday", isClosed = true, note = "Ramazan Bayramı" }, _token);

        newYear.Status.ShouldBe(201);
        bayram.Status.ShouldBe(201);
        var list = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/overrides?from=2027-01-01&to=2027-12-31", _token);
        list.Body.GetProperty("items").EnumerateArray().Select(i => i.GetProperty("date").GetString())
            .ShouldBe(["2027-01-01", "2027-03-09", "2027-03-10", "2027-03-11"]);

        var deleted = await _api.SendAsync(HttpMethod.Delete, $"/api/v1/manage/branches/{_merkez}/overrides/{newYear.Body.GetProperty("id").GetString()}", null, _token);
        deleted.Status.ShouldBe(204);
    }

    [Fact]
    public async Task Overrides_ExtraOpeningNeedsHours_ClosureCannotHaveHours()
    {
        var open = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/branches/{_merkez}/overrides",
            new { fromDate = "2027-01-03", kind = "ExtraOpening", isClosed = false, intervals = new[] { new { start = "10:00", end = "14:00" } } }, _token);
        open.Status.ShouldBe(201);
        open.Body.GetProperty("intervals")[0].GetProperty("start").GetString().ShouldBe("10:00");

        var closure = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/branches/{_merkez}/overrides",
            new { fromDate = "2027-01-04", kind = "Closure", isClosed = false }, _token);
        closure.Status.ShouldBe(400);
    }

    [Fact]
    public async Task CreateBranch_CopiesHours_AndListsBoth()
    {
        var current = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours",
            new { days = new[] { new { dayOfWeek = 1, intervals = new[] { new { start = "09:00", end = "18:00" } } } } }, _token, current.ETag);

        var created = await CreateBranchAsync("Kadıköy", copyHoursFrom: _merkez);

        created.Status.ShouldBe(201);
        created.Body.GetProperty("slug").GetString().ShouldBe("kadikoy");
        created.Body.GetProperty("timeZone").GetString().ShouldBe("Europe/Istanbul");
        var id = created.Body.GetProperty("id").GetString();
        (await _api.GetAsync($"/api/v1/manage/branches/{id}/working-hours", _token)).Body.GetProperty("days").GetArrayLength().ShouldBe(1);
        (await _api.GetAsync("/api/v1/manage/branches", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(2);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM infra.outbox_events WHERE type LIKE '%BranchCreated%'")).ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task CreateBranch_Validation_And_UnknownCopySource()
    {
        var result = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/branches",
            new { name = "K", addressLine = "", district = "Kadıköy", city = "İstanbul", copyWorkingHoursFromBranchId = "0193a8f0-0000-7000-8000-0000000000ff" },
            _token, Headers());

        result.Status.ShouldBe(400);
        var errors = result.Body.GetProperty("errors");
        errors.GetProperty("name")[0].GetString().ShouldBe("too_short");
        errors.GetProperty("addressLine")[0].GetString().ShouldBe("required");
        errors.GetProperty("copyWorkingHoursFromBranchId")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task PatchBranch_UpdatesAddress_AndLastActiveBranchCannotBeClosed()
    {
        var current = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}", _token);
        var updated = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/branches/{_merkez}",
            new { addressLine = "Moda Cad. 42", district = "Kadıköy", city = "İstanbul", slotIntervalMinutes = 10 }, _token, current.ETag);
        updated.Status.ShouldBe(200);
        updated.Body.GetProperty("addressLine").GetString().ShouldBe("Moda Cad. 42");
        updated.Body.GetProperty("slotIntervalMinutes").GetInt32().ShouldBe(10);

        var close = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/branches/{_merkez}", new { isActive = false }, _token, updated.ETag);
        close.Status.ShouldBe(409);
        close.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
    }

    [Fact]
    public async Task OtherBrandsBranch_Returns404_OnEveryEndpoint()
    {
        var other = await _api.SignupAsync("+905330000001", "Berber Kaan");

        (await _api.GetAsync($"/api/v1/manage/branches/{_merkez}", other)).Status.ShouldBe(404);
        (await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", other)).Status.ShouldBe(404);
        (await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/overrides", other)).Status.ShouldBe(404);
        (await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/branches/{_merkez}", new { name = "Ele geçir" }, other, "W/\"1\"")).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/manage/branches", other)).Body.GetProperty("items").EnumerateArray()
            .Select(b => b.GetProperty("id").GetString()).ShouldNotContain(id => string.Equals(id, _merkez, StringComparison.Ordinal));
    }

    [Fact]
    public async Task BranchManager_SeesOnlyOwnBranch_OtherBranchIs404()
    {
        var kadikoy = (await CreateBranchAsync("Kadıköy")).Body.GetProperty("id").GetString()!;
        var nisantasi = (await CreateBranchAsync("Nişantaşı")).Body.GetProperty("id").GetString()!;
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905382741905", "BranchManager", kadikoy);

        (await _api.GetAsync($"/api/v1/manage/branches/{kadikoy}", manager)).Status.ShouldBe(200);
        (await _api.GetAsync($"/api/v1/manage/branches/{nisantasi}", manager)).Status.ShouldBe(404, "kabul kriteri 13");
        (await _api.GetAsync("/api/v1/manage/branches", manager)).Body.GetProperty("items").EnumerateArray()
            .Select(b => b.GetProperty("name").GetString()).ShouldBe(["Kadıköy"]);
        var current = await _api.GetAsync($"/api/v1/manage/branches/{kadikoy}/working-hours", manager);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{kadikoy}/working-hours", new { days = Array.Empty<object>() }, manager, current.ETag))
            .Status.ShouldBe(403);
    }

    private Task<ManageApi.Response> CreateBranchAsync(string name, string? copyHoursFrom = null) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/manage/branches",
            new { name, addressLine = "Moda Cad. 1", district = name, city = "İstanbul", copyWorkingHoursFromBranchId = copyHoursFrom }, _token, Headers());

    private int _requests;

    private Dictionary<string, string> Headers() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
