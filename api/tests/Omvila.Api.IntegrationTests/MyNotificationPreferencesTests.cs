using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: sözleşme manageGetMyNotificationPreferences / manageUpdateMyNotificationPreferences (kayıt yoksa varsayılanlar: gün sonu
// özeti kapalı, diğerleri ve sessiz saat açık; If-Match; kişiye ve markaya özel), bildirimler.md staff.walkin_queue ("Tercihe
// göre"; T-91 (6), T-105): yeni bilette sırayı yönetenlere panel kaydı + tercihe göre push, ekleyen almaz.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class MyNotificationPreferencesTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Path = "/api/v1/manage/me/notification-preferences";
    private const string ZeynepPhone = "+905382741905";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _owner = null!;
    private string _ownerUserId = null!;
    private string _branchId = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _owner = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _owner)).Body;
        _branchId = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _ownerUserId = session.GetProperty("user").GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Get_WithoutRecord_ReturnsDefaults()
    {
        var response = await _api.GetAsync(Path, _owner);

        response.Status.ShouldBe(200, response.Body.ToString());
        response.ETag.ShouldBe("W/\"0\"");
        var body = response.Body;
        foreach (var key in new[] { "newOnlineAppointment", "cancellationAndReschedule", "customerCheckedIn", "walkInQueue", "newReview", "quietHoursEnabled" })
        {
            body.GetProperty(key).GetBoolean().ShouldBeTrue(key);
        }

        body.GetProperty("dailySummary").GetBoolean().ShouldBeFalse();
        body.GetProperty("dailySummaryLocalTime").GetString().ShouldBe("20:00");
        body.GetProperty("quietHoursStartLocal").GetString().ShouldBe("22:00");
        body.GetProperty("quietHoursEndLocal").GetString().ShouldBe("08:00");
    }

    [Fact]
    public async Task Put_SavesForTheUserOnly_StaleVersion412_MissingKey400()
    {
        var first = await _api.WriteAsync(HttpMethod.Put, Path, Body(walkInQueue: false), _owner, "W/\"0\"");

        first.Status.ShouldBe(200, first.Body.ToString());
        first.Body.GetProperty("walkInQueue").GetBoolean().ShouldBeFalse();
        string.Equals(first.ETag, "W/\"0\"", StringComparison.Ordinal).ShouldBeFalse();
        var read = await _api.GetAsync(Path, _owner);
        read.Body.GetProperty("walkInQueue").GetBoolean().ShouldBeFalse();
        read.ETag.ShouldBe(first.ETag);

        (await _api.WriteAsync(HttpMethod.Put, Path, Body(), _owner, "W/\"0\"")).Status.ShouldBe(412);
        var missing = await _api.WriteAsync(HttpMethod.Put, Path, new { newOnlineAppointment = true }, _owner, first.ETag);
        missing.Status.ShouldBe(400);
        missing.Body.GetProperty("errors").GetProperty("walkInQueue")[0].GetString().ShouldBe("required");

        var zeynep = await AddStaffAsync();
        (await _api.GetAsync(Path, zeynep)).Body.GetProperty("walkInQueue").GetBoolean().ShouldBeTrue("tercih kişiye özel");
        var kaan = await _api.SignupAsync("+905330000001", "Berber Kaan");
        (await _api.GetAsync(Path, kaan)).ETag.ShouldBe("W/\"0\"", "başka markanın kaydı görünmez");
    }

    [Fact]
    public async Task NewWalkIn_NotifiesQueueManagers_ExceptTheCreator_PushFollowsPreference()
    {
        var zeynep = await AddStaffAsync();
        var ticket = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/walk-in-tickets", new { branchId = _branchId, fullName = "Okan Er" }, zeynep, Key());
        ticket.Status.ShouldBe(201, ticket.Body.ToString());
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await _api.ScalarAsync<string>("SELECT string_agg(user_id::text, ',') FROM notifications.in_app_notifications WHERE event_code = 'staff.walkin_queue'"))
            .ShouldBe(_ownerUserId, "ekleyen personel bildirim almaz");
        (await _api.ScalarAsync<string>("SELECT recipient_id::text || '|' || (payload->>'n') || '|' || coalesce(template_variant, '') FROM notifications.notification_outbox WHERE event_code = 'staff.walkin_queue'"))
            .ShouldBe($"{_ownerUserId}|1|without_estimate", "şubede çalışma saati tanımlı personel yok");

        (await _api.WriteAsync(HttpMethod.Put, Path, Body(walkInQueue: false), _owner, "W/\"0\"")).Status.ShouldBe(200);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/walk-in-tickets", new { branchId = _branchId, fullName = "Burak" }, zeynep, Key())).Status.ShouldBe(201);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.in_app_notifications WHERE event_code = 'staff.walkin_queue'")).ShouldBe(2, "panel kaydı tercihten bağımsız");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.notification_outbox WHERE event_code = 'staff.walkin_queue'")).ShouldBe(1, "kapalı tercihte push yok");
    }

    private static object Body(bool walkInQueue = true) => new
    {
        newOnlineAppointment = true,
        cancellationAndReschedule = true,
        customerCheckedIn = false,
        walkInQueue,
        newReview = true,
        dailySummary = false,
        quietHoursEnabled = true,
    };

    private async Task<string> AddStaffAsync()
    {
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/staff",
            new { fullName = "Zeynep Aksoy", phone = ZeynepPhone, role = "Staff", calendarColor = "#2F5D8A", branchIds = new[] { _branchId } }, _owner, Key()))
            .Status.ShouldBe(201);
        return (await _api.SignInAsync(ZeynepPhone)).Body.GetProperty("accessToken").GetString()!;
    }

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
