using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriteri 10 (personel telefonla eklenir, davet SMS'i, "Davet gönderildi", ilk girişte Active ve
// kısıtlı menü), 13–14 (yetki ve izolasyon), ADR-0007 § Personel daveti, sözleşme manage-staff, T-32.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class StaffTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Deniz = "+905382741905";

    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _ownerStaffId = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _ownerStaffId = session.GetProperty("membership").GetProperty("staffId").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task AddStaffWithPhone_SendsInvite_FirstLoginActivates_RestrictedMenu()
    {
        var kadikoy = await BranchAsync("Kadıköy");

        var created = await CreateAsync(new { fullName = "Deniz Kara", phone = "0538 274 19 05", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { kadikoy } });

        created.Status.ShouldBe(201);
        created.Body.GetProperty("membershipStatus").GetString().ShouldBe("Invited", "listede \"Davet gönderildi\"");
        created.Body.GetProperty("role").GetString().ShouldBe("Staff");
        created.Body.GetProperty("phoneMasked").GetString().ShouldBe("0538 *** ** 05");
        (await _api.ScalarAsync<string>($"SELECT payload->>'salon' FROM notifications.notification_outbox WHERE event_code = 'staff.invite' AND recipient_phone = '{Deniz}'"))
            .ShouldBe("Kuaför Elif");
        var list = (await _api.GetAsync("/api/v1/manage/staff", _token)).Body.GetProperty("items").EnumerateArray().ToList();
        list.Select(s => s.GetProperty("fullName").GetString()).ShouldBe(["Elif Kaya", "Deniz Kara"]);
        list[0].GetProperty("role").GetString().ShouldBe("Owner");

        var login = await _api.SignInAsync(Deniz);
        login.Body.GetProperty("result").GetString().ShouldBe("SignedIn");
        var staffToken = login.Body.GetProperty("accessToken").GetString()!;
        var permissions = (await _api.GetAsync("/api/v1/manage/session", staffToken)).Body.GetProperty("permissions")
            .EnumerateArray().Select(p => p.GetString()!).ToList();
        permissions.ShouldBe(["calendar.view", "appointment.checkin", "appointment.create", "appointment.update", "commission.view",
            "customer.edit", "customer.view", "timeoff.request", "walkin.manage"], ignoreOrder: true);
        (await _api.GetAsync($"/api/v1/manage/staff/{created.Body.GetProperty("id").GetString()}", _token)).Body
            .GetProperty("membershipStatus").GetString().ShouldBe("Active");
    }

    [Fact]
    public async Task AddStaff_CopiesPrimaryBranchHours_AndAssignsServices()
    {
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours",
            new { days = new[] { new { dayOfWeek = 2, intervals = new[] { new { start = "10:00", end = "13:00" }, new { start = "13:45", end = "19:00" } } } } },
            _token, hours.ETag);
        var category = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/service-categories", new { name = "Bakım" }, _token)).Body.GetProperty("id").GetString();
        var keratin = (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Keratin",
            durationMinutes = 60,
            price = new { amountMinor = 150000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _ownerStaffId },
        }, _token, Key())).Body.GetProperty("id").GetString();

        var created = await CreateAsync(new
        {
            fullName = "Mert Demir",
            role = "Staff",
            calendarColor = "#B4532A",
            branchIds = new[] { _merkez },
            sendInvite = false,
            services = new object[] { new { serviceId = keratin, durationOverrideMinutes = 75, surcharge = new { amountMinor = 30000, currency = "TRY" } } },
        });

        created.Status.ShouldBe(201);
        (created.Body.TryGetProperty("membershipStatus", out var status) && status.ValueKind != JsonValueKind.Null)
            .ShouldBeFalse("telefonsuz personelin üyeliği yok");
        var day = created.Body.GetProperty("workingHours")[0].GetProperty("hours").GetProperty("days")[0];
        day.GetProperty("intervals").GetArrayLength().ShouldBe(2);
        var service = created.Body.GetProperty("services")[0];
        service.GetProperty("durationOverrideMinutes").GetInt32().ShouldBe(75);
        service.GetProperty("surcharge").GetProperty("amountMinor").GetInt64().ShouldBe(30000);
    }

    [Fact]
    public async Task AddStaff_PhoneAlreadyMember_Returns409_AndValidation()
    {
        var taken = await CreateAsync(new { fullName = "Elif Kaya", phone = "0532 418 77 05", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } });
        taken.Status.ShouldBe(409);
        taken.Body.GetProperty("code").GetString().ShouldBe("phone_already_exists");

        var invalid = await CreateAsync(new { fullName = "D", role = "Owner", calendarColor = "mavi", branchIds = Array.Empty<string>(), sendInvite = true });
        invalid.Status.ShouldBe(400);
        var errors = invalid.Body.GetProperty("errors");
        foreach (var field in new[] { "fullName", "role", "calendarColor", "branchIds", "phone" })
        {
            errors.TryGetProperty(field, out _).ShouldBeTrue(field);
        }
    }

    [Fact]
    public async Task Patch_RoleAndDeactivate_UpdateMembership_OwnerCannotBeDeactivated()
    {
        var created = await CreateAsync(new { fullName = "Deniz Kara", phone = Deniz, role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _merkez } });
        var id = created.Body.GetProperty("id").GetString();

        var promoted = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/staff/{id}", new { role = "BranchManager", title = "Müdür" }, _token, created.ETag);
        promoted.Status.ShouldBe(200);
        promoted.Body.GetProperty("role").GetString().ShouldBe("BranchManager");
        promoted.Body.GetProperty("title").GetString().ShouldBe("Müdür");

        var disabled = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/staff/{id}", new { isActive = false }, _token, promoted.ETag);
        disabled.Status.ShouldBe(200);
        disabled.Body.GetProperty("membershipStatus").GetString().ShouldBe("Disabled");
        (await _api.SignInAsync(Deniz)).Body.GetProperty("result").GetString().ShouldBe("SignupRequired", "pasif personel giriş yapamaz (T-23)");

        var owner = await _api.GetAsync($"/api/v1/manage/staff/{_ownerStaffId}", _token);
        (await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/staff/{_ownerStaffId}", new { isActive = false }, _token, owner.ETag)).Status.ShouldBe(409);
    }

    [Fact]
    public async Task BranchManager_AddsStaffOnlyInOwnBranch_AsStaffRole()
    {
        var kadikoy = await BranchAsync("Kadıköy");
        var nisantasi = await BranchAsync("Nişantaşı");
        var manager = await _api.AddMemberAndSignInAsync(_token, "+905383333333", "BranchManager", kadikoy);

        (await CreateAsync(new { fullName = "Can Yılmaz", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { kadikoy }, sendInvite = false }, manager)).Status.ShouldBe(201);
        (await CreateAsync(new { fullName = "Can Yılmaz 2", role = "BranchManager", calendarColor = "#2F5D8A", branchIds = new[] { kadikoy }, sendInvite = false }, manager)).Status.ShouldBe(403);
        (await CreateAsync(new { fullName = "Can Yılmaz 3", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { nisantasi }, sendInvite = false }, manager)).Status.ShouldBe(403);

        var other = await CreateAsync(new { fullName = "Selin Öztürk", role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { nisantasi }, sendInvite = false });
        (await _api.GetAsync($"/api/v1/manage/staff/{other.Body.GetProperty("id").GetString()}", manager)).Status.ShouldBe(404, "kapsam dışı personel");
        (await _api.GetAsync("/api/v1/manage/staff", manager)).Body.GetProperty("items").EnumerateArray()
            .Select(s => s.GetProperty("fullName").GetString()).ShouldBe(["Can Yılmaz"]);
    }

    [Fact]
    public async Task OtherBrand_CannotSeeStaff()
    {
        var other = await _api.SignupAsync("+905330000001", "Berber Kaan");

        (await _api.GetAsync($"/api/v1/manage/staff/{_ownerStaffId}", other)).Status.ShouldBe(404);
        (await _api.GetAsync("/api/v1/manage/staff", other)).Body.GetProperty("items").EnumerateArray()
            .Select(s => s.GetProperty("id").GetString()).ShouldNotContain(id => string.Equals(id, _ownerStaffId, StringComparison.Ordinal));
    }

    private Task<ManageApi.Response> CreateAsync(object body, string? token = null) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/manage/staff", body, token ?? _token, Key());

    private async Task<string> BranchAsync(string name) =>
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/branches",
            new { name, addressLine = "Cad. 1", district = name, city = "İstanbul" }, _token, Key())).Body.GetProperty("id").GetString()!;

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
