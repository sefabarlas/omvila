using System.Text.Json;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: sözleşme manageGetNotificationSettings / manageUpdateNotificationSettings (kanonik uç; BrandSettings alanları aynı
// değerin aynası, ETag birlikte değişir; tekrar invalid_format; T-62 (4)), manageGetSmsOverview, manageRequestSmsSender
// (Pending, OMVILA, bekleyen başvuruda 409; T-62 (6), dilim-03 kriter 11), ManageSession.smsCredits (yalnız settings.manage),
// manageListInAppNotifications / manageMarkNotificationsRead (kişiye özel, idempotent), bildirimler.md owner.credits_low
// (Owner; push + panel), alan-modeli.md § CreditTransaction ("salona uyarı üretilir").
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class NotificationCenterTests(PostgresFixture db) : IAsyncLifetime
{
    private const string DispatchNotificationsJob = "Omvila.Modules.Notifications.Jobs.DispatchNotifications, Omvila.Modules.Notifications";
    private const string SettingsPath = "/api/v1/manage/brand/notification-settings";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _owner = null!;
    private string _brandId = null!;
    private string _branchId = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _owner = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _owner)).Body;
        _brandId = session.GetProperty("brand").GetProperty("id").GetString()!;
        _branchId = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task NotificationSettings_DefaultsMirrorBrandSettings_WithTheSameETag()
    {
        var settings = await _api.GetAsync(SettingsPath, _owner);
        var brand = await _api.GetAsync("/api/v1/manage/brand/settings", _owner);

        settings.Status.ShouldBe(200);
        Ints(settings.Body, "reminderOffsetsMinutes").ShouldBe([1440, 120]);
        settings.Body.GetProperty("reviewInviteEnabled").GetBoolean().ShouldBeTrue();
        settings.Body.GetProperty("templates").GetArrayLength().ShouldBe(0);
        settings.ETag.ShouldNotBeNull();
        settings.ETag.ShouldBe(brand.ETag);
    }

    [Fact]
    public async Task UpdatingEitherSource_ChangesBoth()
    {
        var settings = await _api.GetAsync(SettingsPath, _owner);
        var saved = await _api.WriteAsync(HttpMethod.Put, SettingsPath, new { reminderOffsetsMinutes = Of(120, 2880), reviewInviteEnabled = false }, _owner, settings.ETag);

        saved.Status.ShouldBe(200, saved.Body.ToString());
        Ints(saved.Body, "reminderOffsetsMinutes").ShouldBe([2880, 120]);
        var brand = await _api.GetAsync("/api/v1/manage/brand/settings", _owner);
        Ints(brand.Body, "reminderOffsetsMinutes").ShouldBe([2880, 120]);
        brand.Body.GetProperty("reviewInviteEnabled").GetBoolean().ShouldBeFalse();
        brand.ETag.ShouldBe(saved.ETag);

        // Eski alanlar üzerinden yazılan değer kanonik uçta görünür; eski ETag artık geçmez.
        var body = JsonSerializer.Deserialize<Dictionary<string, object>>(brand.Body.GetRawText())!;
        body["reminderOffsetsMinutes"] = Of(60);
        body.Remove("depositAvailable");
        body.Remove("version");
        (await _api.WriteAsync(HttpMethod.Put, "/api/v1/manage/brand/settings", body, _owner, brand.ETag)).Status.ShouldBe(200);
        Ints((await _api.GetAsync(SettingsPath, _owner)).Body, "reminderOffsetsMinutes").ShouldBe([60]);
        (await _api.WriteAsync(HttpMethod.Put, SettingsPath, new { reminderOffsetsMinutes = Of(120), reviewInviteEnabled = true }, _owner, saved.ETag))
            .Status.ShouldBe(412);
    }

    [Fact]
    public async Task InvalidSettings_Are400_WithFieldCodes()
    {
        var etag = (await _api.GetAsync(SettingsPath, _owner)).ETag;

        var duplicate = await _api.WriteAsync(HttpMethod.Put, SettingsPath, new { reminderOffsetsMinutes = Of(120, 120), reviewInviteEnabled = true }, _owner, etag);
        var range = await _api.WriteAsync(HttpMethod.Put, SettingsPath, new { reminderOffsetsMinutes = Of(10) }, _owner, etag);
        var template = await _api.WriteAsync(HttpMethod.Put, SettingsPath, new
        {
            reminderOffsetsMinutes = Of(120),
            reviewInviteEnabled = true,
            templates = new[] { new { eventCode = "appointment.confirmed", variant = (string?)null, customBody = "Merhaba {bağlantı}" } },
        }, _owner, etag);

        Errors(duplicate).ShouldBe(["reminderOffsetsMinutes:invalid_format"]);
        Errors(range).ShouldBe(["reminderOffsetsMinutes:out_of_range", "reviewInviteEnabled:required"]);
        Errors(template).ShouldBe(["templates[0].eventCode:invalid_format"]);
    }

    [Fact]
    public async Task SmsOverview_WithoutCreditsOrSender()
    {
        await ExhaustCreditsAsync();
        var overview = await _api.GetAsync("/api/v1/manage/brand/sms", _owner);

        overview.Status.ShouldBe(200);
        overview.Body.GetProperty("creditBalance").GetInt32().ShouldBe(0);
        overview.Body.GetProperty("estimatedDaysLeft").ValueKind.ShouldBe(JsonValueKind.Null);
        overview.Body.GetProperty("lowCreditThreshold").GetInt32().ShouldBe(200);
        overview.Body.GetProperty("isLow").GetBoolean().ShouldBeTrue();
        overview.Body.GetProperty("sentThisMonth").GetProperty("month").GetString().ShouldBe("2026-11-01");
        overview.Body.GetProperty("sentThisMonth").GetProperty("smsCount").GetInt32().ShouldBe(0);
        var sender = overview.Body.GetProperty("sender");
        sender.GetProperty("status").GetString().ShouldBe("None");
        sender.GetProperty("name").ValueKind.ShouldBe(JsonValueKind.Null);
        sender.GetProperty("effectiveName").GetString().ShouldBe("OMVILA");
    }

    [Fact]
    public async Task SmsOverview_CountsThisMonthsSentMessages_AndEstimatesDays()
    {
        await GrantAsync(300);
        await InviteStaffAsync("+905382741905");
        await _api.RunJobAsync(DispatchNotificationsJob);

        var overview = (await _api.GetAsync("/api/v1/manage/brand/sms", _owner)).Body;

        overview.GetProperty("creditBalance").GetInt32().ShouldBe(299);
        overview.GetProperty("isLow").GetBoolean().ShouldBeFalse();
        overview.GetProperty("estimatedDaysLeft").GetInt32().ShouldBe(299 * 30, "son 30 günde 1 kontör");
        var month = overview.GetProperty("sentThisMonth");
        (month.GetProperty("smsCount").GetInt32(), month.GetProperty("creditsUsed").GetInt32(), month.GetProperty("notificationCount").GetInt32())
            .ShouldBe((1, 1, 1), "giriş kodları salonun mesajı sayılmaz");
    }

    [Fact]
    public async Task SenderRequest_IsPending_UntilApproved_AndSecondRequestConflicts()
    {
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/brand/sms-sender", new { name = "KUAFORELIF" }, _owner, Key());
        var again = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/brand/sms-sender", new { name = "ELIF" }, _owner, Key());
        var invalid = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/brand/sms-sender", new { name = "Kuaför" }, _owner, Key());

        created.Status.ShouldBe(201, created.Body.ToString());
        created.Body.GetProperty("status").GetString().ShouldBe("Pending");
        created.Body.GetProperty("name").GetString().ShouldBe("KUAFORELIF");
        created.Body.GetProperty("effectiveName").GetString().ShouldBe("OMVILA");
        again.Status.ShouldBe(409);
        again.Body.GetProperty("code").GetString().ShouldBe("invalid_state_transition");
        Errors(invalid).ShouldBe(["name:invalid_format"]);
        (await _api.GetAsync("/api/v1/manage/brand/sms", _owner)).Body.GetProperty("sender").GetProperty("status").GetString().ShouldBe("Pending");
    }

    [Fact]
    public async Task Session_ShowsSmsCredits_OnlyToSettingsManagers()
    {
        await GrantAsync(180);
        var staff = await _api.AddMemberAndSignInAsync(_owner, "+905382741905", "Staff", _branchId);

        var owner = (await _api.GetAsync("/api/v1/manage/session", _owner)).Body.GetProperty("smsCredits");
        var staffSession = (await _api.GetAsync("/api/v1/manage/session", staff)).Body;

        owner.GetProperty("balance").GetInt32().ShouldBe(180);
        owner.GetProperty("isLow").GetBoolean().ShouldBeTrue();
        staffSession.TryGetProperty("smsCredits", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task CreditsDropBelowThreshold_OwnerGetsPanelEntryAndPush_Once()
    {
        var staff = await _api.AddMemberAndSignInAsync(_owner, "+905383333333", "Staff", _branchId);
        await GrantAsync(200);
        await InviteStaffAsync("+905382741905");
        await _api.RunJobAsync(DispatchNotificationsJob);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);

        var page = await _api.GetAsync("/api/v1/manage/notifications", _owner);

        page.Status.ShouldBe(200);
        page.Body.GetProperty("unreadCount").GetInt32().ShouldBe(1);
        page.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
        var item = page.Body.GetProperty("items")[0];
        item.GetProperty("event").GetString().ShouldBe("owner.credits_low");
        item.GetProperty("title").GetString().ShouldBe("SMS kontörü azaldı");
        item.GetProperty("body").GetString().ShouldBe("199 SMS kaldı, tahmini 5.970 gün yeter.");
        item.GetProperty("branchId").ValueKind.ShouldBe(JsonValueKind.Null);
        item.GetProperty("readUtc").ValueKind.ShouldBe(JsonValueKind.Null);
        item.GetProperty("target").GetProperty("type").GetString().ShouldBe("SmsCredits");
        (await _api.ScalarAsync<string>("SELECT recipient_type || ':' || status FROM notifications.notification_outbox WHERE event_code = 'owner.credits_low'"))
            .ShouldBe("User:Pending");
        (await _api.GetAsync("/api/v1/manage/notifications", staff)).Body.GetProperty("unreadCount").GetInt32().ShouldBe(0);

        // Okunmamış uyarı varken yeni yetersiz kontör olayı tekrar yazmaz.
        await InviteStaffAsync("+905384444444");
        await ExhaustCreditsAsync();
        await _api.RunJobAsync(DispatchNotificationsJob);
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM notifications.in_app_notifications")).ShouldBe(1);
    }

    [Fact]
    public async Task MarkRead_ByIdOrAll_IsIdempotent_AndIgnoresOtherUsersNotifications()
    {
        var staff = await _api.AddMemberAndSignInAsync(_owner, "+905383333333", "Staff", _branchId);
        var ownerId = (await _api.GetAsync("/api/v1/manage/session", _owner)).Body.GetProperty("user").GetProperty("id").GetString()!;
        var staffId = (await _api.GetAsync("/api/v1/manage/session", staff)).Body.GetProperty("user").GetProperty("id").GetString()!;
        await InsertAsync(ownerId, 3);
        await InsertAsync(staffId, 1);
        var staffNotification = await _api.ScalarAsync<Guid>($"SELECT id FROM notifications.in_app_notifications WHERE user_id = '{staffId}'");

        var first = (await _api.GetAsync("/api/v1/manage/notifications?limit=2", _owner)).Body;
        var cursor = first.GetProperty("nextCursor").GetString()!;
        var second = (await _api.GetAsync($"/api/v1/manage/notifications?limit=2&cursor={cursor}", _owner)).Body;
        var newest = first.GetProperty("items")[0].GetProperty("id").GetString()!;
        first.GetProperty("items").GetArrayLength().ShouldBe(2);
        second.GetProperty("items").GetArrayLength().ShouldBe(1);
        second.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);

        var one = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/notifications/read", new { notificationIds = new[] { Guid.Parse(newest), staffNotification } }, _owner);
        one.Status.ShouldBe(200);
        one.Body.GetProperty("unreadCount").GetInt32().ShouldBe(2);
        var readAt = await _api.ScalarAsync<DateTime>($"SELECT read_utc FROM notifications.in_app_notifications WHERE id = '{newest}'");
        _factory.Clock.Advance(NodaTime.Duration.FromMinutes(1));
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/notifications/read", new { all = true }, _owner)).Body.GetProperty("unreadCount").GetInt32().ShouldBe(0);
        (await _api.ScalarAsync<DateTime>($"SELECT read_utc FROM notifications.in_app_notifications WHERE id = '{newest}'")).ShouldBe(readAt);
        (await _api.GetAsync("/api/v1/manage/notifications?unreadOnly=true", staff)).Body.GetProperty("items").GetArrayLength().ShouldBe(1);

        Errors(await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/notifications/read", new { all = true, notificationIds = new[] { Guid.Parse(newest) } }, _owner))
            .ShouldBe(["all:invalid_format"]);
        Errors(await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/notifications/read", new { }, _owner)).ShouldBe(["notificationIds:required"]);
        Errors(await _api.GetAsync("/api/v1/manage/notifications?cursor=bozuk", _owner)).ShouldBe(["cursor:invalid_format"]);
    }

    // Deneme hediyesi (Platform:TrialCredits) kayıtta yazılır: bakiye düzeltme satırıyla hedefe getirilir.
    private async Task GrantAsync(int balance)
    {
        var current = await _api.ScalarAsync<long>($"SELECT coalesce(sum(amount), 0) FROM notifications.credit_transactions WHERE brand_id = '{_brandId}'");
        await _api.ScalarAsync<int>($"""
            INSERT INTO notifications.credit_transactions (id, brand_id, type, amount, balance_after, reference_type, created_utc)
              VALUES (gen_random_uuid(), '{_brandId}', 'Grant', {balance - current}, {balance}, 'Test', now()) RETURNING amount
            """);
    }

    private async Task ExhaustCreditsAsync()
    {
        var balance = await _api.ScalarAsync<long>($"SELECT sum(amount) FROM notifications.credit_transactions WHERE brand_id = '{_brandId}'");
        await _api.ScalarAsync<int>($"""
            INSERT INTO notifications.credit_transactions (id, brand_id, type, amount, balance_after, reference_type, created_utc)
              VALUES (gen_random_uuid(), '{_brandId}', 'Expire', {-balance}, 0, 'Test', now()) RETURNING amount
            """);
    }

    private Task<int> InsertAsync(string userId, int count) => _api.ScalarAsync<int>($"""
        INSERT INTO notifications.in_app_notifications (id, brand_id, user_id, event_code, title, body, target_type, created_utc)
          SELECT gen_random_uuid(), '{_brandId}', '{userId}', 'staff.appointment_created', 'Yeni randevu', 'Selin Öztürk · 14:30 Saç kesimi · sizinle.',
            'Today', timestamptz '2026-11-24T08:00:00Z' + n * interval '1 minute'
          FROM generate_series(1, {count}) AS n RETURNING 1
        """);

    private async Task InviteStaffAsync(string phone)
    {
        var created = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/staff",
            new { fullName = "Mert Demir", phone, role = "Staff", calendarColor = "#B4532A", branchIds = new[] { _branchId }, sendInvite = true }, _owner, Key());
        created.Status.ShouldBe(201, created.Body.ToString());
    }

    private static int[] Ints(JsonElement body, string name) => [.. body.GetProperty(name).EnumerateArray().Select(e => e.GetInt32())];

    private static string[] Errors(ManageApi.Response response)
    {
        response.Status.ShouldBe(400, response.Body.ToString());
        return [.. response.Body.GetProperty("errors").EnumerateObject()
            .SelectMany(p => p.Value.EnumerateArray().Select(code => $"{p.Name}:{code.GetString()}"))
            .Order(StringComparer.Ordinal)];
    }

    private static int[] Of(params int[] values) => values;

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
