using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: sözleşme manageRegisterDevice (jetona göre upsert, aynı deviceId; jeton başka kullanıcıya geçerse eski kayıt
// pasifleşir), alan-modeli.md § Device (aktif kayıtlar arasında jeton tek), T-62 (7).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class DeviceTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Path = "/api/v1/manage/devices";
    private const string Token = "fcm-token-0123456789-abcdefghij";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _elif = null!;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _elif = await _api.SignupAsync("+905324187705", "Kuaför Elif");
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Register_ReturnsDevice_AndSameTokenAgainUpdatesTheSameRecord()
    {
        var first = await _api.SendAsync(HttpMethod.Post, Path, new { platform = "iOS", pushToken = Token, appVersion = "1.0.3" }, _elif);
        first.Status.ShouldBe(200, first.Body.ToString());
        first.Body.GetProperty("platform").GetString().ShouldBe("iOS");

        _factory.Clock.Advance(NodaTime.Duration.FromMinutes(5));
        var second = await _api.SendAsync(HttpMethod.Post, Path, new { platform = "Android", pushToken = Token, appVersion = "1.0.4" }, _elif);

        second.Status.ShouldBe(200);
        second.Body.GetProperty("deviceId").GetString().ShouldBe(first.Body.GetProperty("deviceId").GetString());
        second.Body.GetProperty("platform").GetString().ShouldBe("Android");
        (second.Body.GetProperty("lastSeenUtc").GetDateTimeOffset() - first.Body.GetProperty("lastSeenUtc").GetDateTimeOffset())
            .ShouldBe(TimeSpan.FromMinutes(5));
        (await _api.ScalarAsync<string>("SELECT app_version || ':' || is_active || ':' || owner_type || ':' || app FROM notifications.devices"))
            .ShouldBe("1.0.4:true:User:Business");
    }

    [Fact]
    public async Task TokenMovesToAnotherUser_InAnotherBrand_OldRecordIsDeactivated()
    {
        await _api.SendAsync(HttpMethod.Post, Path, new { platform = "iOS", pushToken = Token, appVersion = "1.0.3" }, _elif);
        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");

        var moved = await _api.SendAsync(HttpMethod.Post, Path, new { platform = "iOS", pushToken = Token, appVersion = "1.0.3" }, kaan);

        moved.Status.ShouldBe(200);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.devices WHERE is_active")).ShouldBe(1);
        (await _api.ScalarAsync<string>("SELECT string_agg(is_active::text, ',' ORDER BY created_utc) FROM notifications.devices")).ShouldBe("false,true");
    }

    [Fact]
    public async Task TokenMovesToAnotherUser_InTheSameBrand_RecordChangesOwner()
    {
        var first = await _api.SendAsync(HttpMethod.Post, Path, new { platform = "iOS", pushToken = Token, appVersion = "1.0.3" }, _elif);
        var branchId = (await _api.GetAsync("/api/v1/manage/session", _elif)).Body.GetProperty("branches")[0].GetProperty("id").GetString()!;
        var mert = await _api.AddMemberAndSignInAsync(_elif, "+905382741905", "Staff", branchId);

        var moved = await _api.SendAsync(HttpMethod.Post, Path, new { platform = "iOS", pushToken = Token, appVersion = "1.0.3" }, mert);

        moved.Body.GetProperty("deviceId").GetString().ShouldBe(first.Body.GetProperty("deviceId").GetString());
        (await _api.ScalarAsync<long>("SELECT count(DISTINCT owner_id) FROM notifications.devices")).ShouldBe(1);
        (await _api.ScalarAsync<bool>("SELECT d.owner_id = u.id FROM notifications.devices d JOIN identity.users u ON u.phone_e164 = '+905382741905'"))
            .ShouldBeTrue();
    }

    [Fact]
    public async Task InvalidBody_Is400_WithFieldCodes()
    {
        var result = await _api.SendAsync(HttpMethod.Post, Path, new { platform = "Windows", pushToken = "short", appVersion = new string('1', 21) }, _elif);

        result.Status.ShouldBe(400);
        var errors = result.Body.GetProperty("errors");
        errors.GetProperty("platform")[0].GetString().ShouldBe("invalid_format");
        errors.GetProperty("pushToken")[0].GetString().ShouldBe("invalid_format");
        errors.GetProperty("appVersion")[0].GetString().ShouldBe("too_long");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.devices")).ShouldBe(0);
    }

    [Fact]
    public async Task WithoutToken_Is401()
    {
        var result = await _api.SendAsync(HttpMethod.Post, Path, new { platform = "iOS", pushToken = Token, appVersion = "1.0.3" });

        result.Status.ShouldBe(401);
    }
}
