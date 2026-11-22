using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-06 K6 (kullanıcılar, rol ve şube düzenleme, salt okunur rol matrisi), roller-yetkiler.md § Salon rolleri
// ("Owner en az bir tane"), sözleşme manageListUsers / manageUpdateUser / manageGetRoleMatrix (409 last_owner_required,
// settings.roles yalnız Owner), manageGetPrivacySettings / manageUpdatePrivacySettings (T-86 (7), sureler-ve-ayarlar.md § KVKK).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class UsersAndPrivacyTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Owner = "+905324187705";
    private const string Deniz = "+905383333333";
    private static readonly string[] UnknownBranch = ["0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f5a99"];

    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync(Owner, "Kuaför Elif");
        _merkez = (await _api.GetAsync("/api/v1/manage/session", _token)).Body.GetProperty("branches")[0].GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task ListUsers_ShowsMembershipsSortedByName_StaffIsForbidden()
    {
        var staffToken = await _api.AddMemberAndSignInAsync(_token, Deniz, "Staff", _merkez);

        var list = await _api.GetAsync("/api/v1/manage/users", _token);

        list.Status.ShouldBe(200);
        var items = list.Body.GetProperty("items").EnumerateArray().ToList();
        items.Count.ShouldBe(2);
        var owner = items.Single(i => string.Equals(i.GetProperty("role").GetString(), "Owner", StringComparison.Ordinal));
        owner.GetProperty("isCurrentUser").GetBoolean().ShouldBeTrue();
        owner.GetProperty("branchIds").GetArrayLength().ShouldBe(0);
        owner.GetProperty("status").GetString().ShouldBe("Active");
        owner.GetProperty("phoneMasked").GetString().ShouldBe("0532 *** ** 05");
        var deniz = items.Single(i => string.Equals(i.GetProperty("role").GetString(), "Staff", StringComparison.Ordinal));
        deniz.GetProperty("fullName").GetString().ShouldBe("Deniz Kara");
        deniz.GetProperty("isCurrentUser").GetBoolean().ShouldBeFalse();
        deniz.GetProperty("branchIds")[0].GetString().ShouldBe(_merkez);
        deniz.GetProperty("version").GetString().ShouldStartWith("W/\"");
        items[0].GetProperty("fullName").GetString().ShouldBe("Deniz Kara", "ada göre sıralı");

        (await _api.GetAsync("/api/v1/manage/users", staffToken)).Status.ShouldBe(403);
        (await _api.GetAsync("/api/v1/manage/roles", staffToken)).Status.ShouldBe(403);
    }

    [Fact]
    public async Task UpdateUser_ChangesRoleAndBranches_LastOwnerCannotBeDemoted()
    {
        await _api.AddMemberAndSignInAsync(_token, Deniz, "Staff", _merkez);
        var (denizId, denizVersion) = await UserAsync("Staff");
        var (ownerId, _) = await UserAsync("Owner");

        var invalid = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/users/{denizId}", new { role = "BranchManager", branchIds = Array.Empty<string>() }, _token, denizVersion);
        invalid.Status.ShouldBe(400);
        invalid.Body.GetProperty("errors").GetProperty("branchIds")[0].GetString().ShouldBe("required");
        (await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/users/{denizId}", new { role = "BranchManager", branchIds = UnknownBranch }, _token, denizVersion))
            .Status.ShouldBe(400, "başka markanın veya olmayan şube");

        var manager = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/users/{denizId}", new { role = "BranchManager", branchIds = new[] { _merkez } }, _token, denizVersion);
        manager.Status.ShouldBe(200, manager.Body.ToString());
        manager.Body.GetProperty("role").GetString().ShouldBe("BranchManager");
        manager.ETag.ShouldBe(manager.Body.GetProperty("version").GetString());
        (await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/users/{denizId}", new { role = "Staff", branchIds = new[] { _merkez } }, _token, denizVersion))
            .Status.ShouldBe(412, "eski sürüm");

        // Tek Owner başka role alınamaz.
        var (_, ownerVersion) = await UserAsync("Owner");
        var lastOwner = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/users/{ownerId}", new { role = "BranchManager", branchIds = new[] { _merkez } }, _token, ownerVersion);
        lastOwner.Status.ShouldBe(409);
        lastOwner.Body.GetProperty("code").GetString().ShouldBe("last_owner_required");

        // İkinci Owner olunca ilki başka role alınabilir; Owner'ın şubeleri boş (tümü).
        var promoted = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/users/{denizId}", new { role = "Owner", branchIds = new[] { _merkez } }, _token, manager.ETag);
        promoted.Status.ShouldBe(200);
        promoted.Body.GetProperty("branchIds").GetArrayLength().ShouldBe(0);
        var demoted = await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/users/{ownerId}", new { role = "BranchManager", branchIds = new[] { _merkez } }, _token, ownerVersion);
        demoted.Status.ShouldBe(200, demoted.Body.ToString());

        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE entity_type = 'UserMembership' AND action = 'Update'")).ShouldBe(3L);
        var stored = await _api.ScalarAsync<string>($"SELECT role FROM identity.user_memberships WHERE user_id = '{ownerId}'");
        stored.ShouldBe("BranchManager");
    }

    [Fact]
    public async Task UpdateUser_OtherBrandsUserIsNotFound()
    {
        var other = await _api.SignupAsync("+905321112233", "Berber Ali");
        var (otherOwnerId, version) = await UserAsync("Owner", other);

        (await _api.WriteAsync(HttpMethod.Patch, $"/api/v1/manage/users/{otherOwnerId}", new { role = "Owner", branchIds = Array.Empty<string>() }, _token, version))
            .Status.ShouldBe(404);
    }

    [Fact]
    public async Task RoleMatrix_ReflectsRolePermissions()
    {
        var matrix = await _api.GetAsync("/api/v1/manage/roles", _token);

        matrix.Status.ShouldBe(200);
        var rows = matrix.Body.GetProperty("rows").EnumerateArray().ToDictionary(r => r.GetProperty("group").GetString()!, StringComparer.Ordinal);
        rows.Keys.ShouldBe(["CalendarAndAppointments", "CustomerPhone", "CustomerCardAndNotes", "CashAndPayments", "ReportsAndCommission", "ServicesAndPrices", "Settings"]);
        Levels(rows["CalendarAndAppointments"]).ShouldBe(("All", "Branch", "Own"));
        Levels(rows["CustomerPhone"]).ShouldBe(("All", "All", "None"));
        Levels(rows["CustomerCardAndNotes"]).ShouldBe(("All", "All", "All"));
        Levels(rows["CashAndPayments"]).ShouldBe(("All", "Branch", "None"));
        Levels(rows["ReportsAndCommission"]).ShouldBe(("All", "Branch", "Own"));
        Levels(rows["Settings"]).ShouldBe(("All", "None", "None"));
        rows["Settings"].GetProperty("permissions").EnumerateArray().Select(p => p.GetString()).ShouldBe(["settings.manage", "settings.payment", "settings.roles"]);
    }

    [Fact]
    public async Task PrivacySettings_DefaultsValidationAndSave()
    {
        var read = await _api.GetAsync("/api/v1/manage/brand/privacy", _token);
        read.Status.ShouldBe(200);
        read.Body.GetProperty("noticeMode").GetString().ShouldBe("Standard");
        read.Body.GetProperty("transactionRetentionYears").GetInt32().ShouldBe(3);
        read.Body.GetProperty("allowedRetentionYears").EnumerateArray().Select(y => y.GetInt32()).ShouldBe([3, 5, 10]);
        read.Body.GetProperty("standardNotice").GetProperty("version").GetString().ShouldBe("2026-11-01");
        read.Body.GetProperty("standardNotice").GetProperty("updatedOn").GetString().ShouldBe("2026-11-01");
        read.Body.GetProperty("standardNotice").GetProperty("text").GetString()!.ShouldStartWith("Kuaför Elif olarak");
        read.Body.TryGetProperty("customNoticeText", out _).ShouldBeFalse("boş alan yazılmaz");

        var missing = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/privacy", new { noticeMode = "Custom", transactionRetentionYears = 7 }, _token, read.ETag);
        missing.Status.ShouldBe(400);
        missing.Body.GetProperty("errors").GetProperty("customNoticeText")[0].GetString().ShouldBe("required");
        missing.Body.GetProperty("errors").GetProperty("transactionRetentionYears")[0].GetString().ShouldBe("out_of_range");

        var saved = await _api.WriteAsync(
            HttpMethod.Put, "/api/v1/manage/brand/privacy", new { noticeMode = "Custom", customNoticeText = "Salonumuzun aydınlatma metni.", transactionRetentionYears = 5 }, _token, read.ETag);
        saved.Status.ShouldBe(200, saved.Body.ToString());
        saved.Body.GetProperty("noticeMode").GetString().ShouldBe("Custom");
        saved.Body.GetProperty("customNoticeText").GetString().ShouldBe("Salonumuzun aydınlatma metni.");
        saved.Body.GetProperty("customNoticeUpdatedUtc").GetString().ShouldNotBeNull();
        saved.Body.GetProperty("transactionRetentionYears").GetInt32().ShouldBe(5);
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/privacy", new { noticeMode = "Standard", transactionRetentionYears = 3 }, _token, read.ETag))
            .Status.ShouldBe(412);

        // Standard'a dönünce salonun metni saklanır.
        var standard = await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/privacy", new { noticeMode = "Standard", transactionRetentionYears = 10 }, _token, saved.ETag);
        standard.Status.ShouldBe(200);
        standard.Body.GetProperty("customNoticeText").GetString().ShouldBe("Salonumuzun aydınlatma metni.");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM platform.audit_logs WHERE entity_type = 'PrivacySettings'")).ShouldBe(2L);
        (await _api.ScalarAsync<string>("SELECT changes::text FROM platform.audit_logs WHERE entity_type = 'PrivacySettings' LIMIT 1"))
            .ShouldNotContain("aydınlatma", Case.Insensitive, "metin denetime yazılmaz");

        var staffToken = await _api.AddMemberAndSignInAsync(_token, Deniz, "BranchManager", _merkez);
        (await _api.GetAsync("/api/v1/manage/brand/privacy", staffToken)).Status.ShouldBe(403);
    }

    private static (string, string, string) Levels(JsonElement row) =>
        (row.GetProperty("owner").GetString()!, row.GetProperty("branchManager").GetString()!, row.GetProperty("staff").GetString()!);

    private async Task<(string UserId, string Version)> UserAsync(string role, string? token = null)
    {
        var user = (await _api.GetAsync("/api/v1/manage/users", token ?? _token)).Body.GetProperty("items").EnumerateArray()
            .First(i => string.Equals(i.GetProperty("role").GetString(), role, StringComparison.Ordinal));
        return (user.GetProperty("userId").GetString()!, user.GetProperty("version").GetString()!);
    }
}
