using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriterleri 11 (personel hizmetleri), 12 (izin; affectedAppointments boş), 15 (412),
// alan-modeli.md § StaffWorkingHours (şubeler arası örtüşme yok), sözleşme manage-staff, roller-yetkiler.md (timeoff K).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class StaffScheduleTests(PostgresFixture db) : IAsyncLifetime
{
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task StaffServices_SpecialDurationAndSurcharge_SavedAndRead_StaleIs412()
    {
        var category = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/service-categories", new { name = "Renk" }, _token)).Body.GetProperty("id").GetString();
        var keratin = await ServiceAsync(category!, "Keratin", 60);
        var balyaj = await ServiceAsync(category!, "Balyaj", 120);
        var current = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);

        var saved = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/services", new
        {
            items = new object[]
            {
                new { serviceId = keratin, durationOverrideMinutes = 75 },
                new { serviceId = balyaj, surcharge = new { amountMinor = 30000, currency = "TRY" } },
            },
        }, _token, current.ETag);

        saved.Status.ShouldBe(200);
        var services = saved.Body.GetProperty("services").EnumerateArray().ToDictionary(s => s.GetProperty("serviceId").GetString()!, StringComparer.Ordinal);
        services[keratin].GetProperty("durationOverrideMinutes").GetInt32().ShouldBe(75);
        services[balyaj].GetProperty("surcharge").GetProperty("amountMinor").GetInt64().ShouldBe(30000);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/services", new { items = Array.Empty<object>() }, _token, current.ETag))
            .Status.ShouldBe(412);

        var invalid = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/services",
            new { items = new[] { new { serviceId = "0193a8f0-0000-7000-8000-0000000000ff" } } }, _token, saved.ETag);
        invalid.Status.ShouldBe(400);
        invalid.Body.GetProperty("errors").GetProperty("items[0].serviceId")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task StaffHours_OverlapWithOtherBranch_Is400_PerContract()
    {
        var kadikoy = await BranchAsync("Kadıköy");
        var staff = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/staff",
            new { fullName = "Mert Demir", role = "Staff", calendarColor = "#B4532A", branchIds = new[] { _merkez, kadikoy }, sendInvite = false, copyBranchWorkingHours = false },
            _token, Key());
        var id = staff.Body.GetProperty("id").GetString();

        var first = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{id}/branches/{_merkez}/working-hours",
            Week((2, "10:00", "14:00")), _token, staff.ETag);
        first.Status.ShouldBe(200);

        var overlap = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{id}/branches/{kadikoy}/working-hours",
            Week((1, "10:00", "18:00"), (2, "13:00", "19:00")), _token, first.ETag);
        overlap.Status.ShouldBe(400);
        overlap.Body.GetProperty("errors").GetProperty("days[1].intervals")[0].GetString().ShouldBe("overlaps_other_branch");

        var backToBack = await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{id}/branches/{kadikoy}/working-hours",
            Week((2, "15:00", "19:00")), _token, first.ETag);
        backToBack.Status.ShouldBe(200);
        var detail = await _api.GetAsync($"/api/v1/manage/staff/{id}", _token);
        detail.Body.GetProperty("workingHours").GetArrayLength().ShouldBe(2);

        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{id}/branches/{await BranchAsync("Moda")}/working-hours",
            Week((2, "09:00", "10:00")), _token, detail.ETag)).Status.ShouldBe(404, "personel o şubede çalışmıyor");
    }

    [Fact]
    public async Task TimeOff_AllDayLeave_ListedAndDeleted_AffectedAppointmentsEmpty()
    {
        var created = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/staff/{_elif}/time-off",
            new { type = "Leave", isAllDay = true, fromDate = "2026-12-02", toDate = "2026-12-04", note = "Yıllık izin" }, _token);

        created.Status.ShouldBe(201);
        created.Body.GetProperty("affectedAppointments").GetArrayLength().ShouldBe(0);
        var timeOff = created.Body.GetProperty("timeOff");
        timeOff.GetProperty("startUtc").GetString().ShouldBe("2026-12-01T21:00:00Z", "İstanbul 2 Aralık 00:00");
        timeOff.GetProperty("endUtc").GetString().ShouldBe("2026-12-04T21:00:00Z", "4 Aralık dahil");

        var list = await _api.GetAsync($"/api/v1/manage/staff/{_elif}/time-off?from=2026-12-01&to=2026-12-31", _token);
        list.Body.GetProperty("items").GetArrayLength().ShouldBe(1);
        (await _api.GetAsync($"/api/v1/manage/staff/{_elif}/time-off?from=2027-01-01", _token)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);

        (await _api.SendAsync(HttpMethod.Delete, $"/api/v1/manage/staff/{_elif}/time-off/{timeOff.GetProperty("id").GetString()}", null, _token)).Status.ShouldBe(204);
    }

    [Fact]
    public async Task TimeOff_Validation()
    {
        var timed = await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/staff/{_elif}/time-off",
            new { type = "Break", isAllDay = false, startUtc = "2026-12-02T12:00:00Z", endUtc = "2026-12-02T11:00:00Z" }, _token);
        timed.Status.ShouldBe(400);
        timed.Body.GetProperty("errors").GetProperty("endUtc")[0].GetString().ShouldBe("invalid_range");

        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/staff/{_elif}/time-off", new { type = "Vacation", isAllDay = true, fromDate = "2026-12-02" }, _token))
            .Status.ShouldBe(400);
    }

    [Fact]
    public async Task StaffRole_ManagesOnlyOwnTimeOff()
    {
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/staff",
            new { fullName = "Deniz Kara", phone = "+905382741905", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } }, _token, Key());
        var deniz = created.Body.GetProperty("id").GetString();
        var token = (await _api.SignInAsync("+905382741905")).Body.GetProperty("accessToken").GetString()!;

        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/staff/{deniz}/time-off",
            new { type = "Leave", isAllDay = true, fromDate = "2026-12-10" }, token)).Status.ShouldBe(201);
        (await _api.SendAsync(HttpMethod.Post, $"/api/v1/manage/staff/{_elif}/time-off",
            new { type = "Leave", isAllDay = true, fromDate = "2026-12-10" }, token)).Status.ShouldBe(404, "başkasının izni (K)");
        (await _api.GetAsync($"/api/v1/manage/staff/{_elif}/time-off", token)).Status.ShouldBe(404);
        var current = await _api.GetAsync($"/api/v1/manage/staff/{deniz}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{deniz}/services", new { items = Array.Empty<object>() }, token, current.ETag))
            .Status.ShouldBe(403, "staff.manage yok");
    }

    private static object Week(params (int Day, string Start, string End)[] days) => new
    {
        days = days.Select(d => new { dayOfWeek = d.Day, intervals = new[] { new { start = d.Start, end = d.End } } }).ToArray(),
    };

    private async Task<string> ServiceAsync(string category, string name, int minutes) =>
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/services", new
        {
            categoryId = category,
            name,
            durationMinutes = minutes,
            price = new { amountMinor = 100000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        }, _token, Key())).Body.GetProperty("id").GetString()!;

    private async Task<string> BranchAsync(string name) =>
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/branches",
            new { name, addressLine = "Cad. 1", district = name, city = "İstanbul" }, _token, Key())).Body.GetProperty("id").GetString()!;

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
