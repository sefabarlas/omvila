using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Modules.Integrations.Calendar;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-06 K7 (Google Takvim sahte sağlayıcıyla, tek yönlü), alan-modeli.md § CalendarLink / AppointmentCalendarSync,
// durum-makinesi.md § CalendarLink (Active → TokenExpired / Revoked), sözleşme manage-integrations (state 10 dk, kullanıcıya bağlı,
// tek kullanımlık; geçersiz state/code 400; bağlantı yoksa NotConnected; kesme idempotent 204), guvenlik-kvkk.md (jetonlar şifreli).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class GoogleCalendarTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Path = "/api/v1/manage/integrations/google";
    private const string SyncJob = "Omvila.Modules.Integrations.Jobs.SyncCalendarEvents, Omvila.Modules.Integrations";
    private const string RefreshJob = "Omvila.Modules.Integrations.Jobs.RefreshCalendarTokens, Omvila.Modules.Integrations";

    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _token = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _kesim = null!;
    private string _selin = null!;
    private int _requests;

    private FakeCalendarStore Store => _factory.Services.GetRequiredService<FakeCalendarStore>();

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
        _api = new ManageApi(_factory, db);
        _token = await _api.SignupAsync("+905324187705", "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _token)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", Week(), _token, hours.ETag)).Status.ShouldBe(200);
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", Week(), _token, elif.ETag)).Status.ShouldBe(200);
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _kesim = (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Saç kesimi",
            durationMinutes = 45,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;
        _selin = (await PostAsync("/api/v1/manage/customers", new { fullName = "Selin Öztürk", phone = "+905550004418" })).Body.GetProperty("id").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Connect_WithFakeProvider_StateIsSingleUse_TokensEncrypted()
    {
        var initial = await _api.GetAsync(Path, _token);
        initial.Status.ShouldBe(200);
        initial.Body.GetProperty("status").GetString().ShouldBe("NotConnected");

        var (code, state) = await ConnectUrlAsync();
        var connected = await _api.SendAsync(HttpMethod.Post, $"{Path}/callback", new { code, state }, _token);

        connected.Status.ShouldBe(200, connected.Body.ToString());
        connected.Body.GetProperty("status").GetString().ShouldBe("Active");
        connected.Body.GetProperty("accountEmailMasked").GetString().ShouldBe("pe***@ornek.com");
        (await _api.GetAsync(Path, _token)).Body.GetProperty("status").GetString().ShouldBe("Active");
        var stored = await _api.ScalarAsync<string>("SELECT encode(access_token_encrypted, 'escape') || encode(refresh_token_encrypted, 'escape') FROM integrations.calendar_links");
        stored.ShouldNotContain("fake-", Case.Sensitive, "jetonlar şifreli");

        var reused = await _api.SendAsync(HttpMethod.Post, $"{Path}/callback", new { code, state }, _token);
        reused.Status.ShouldBe(400);
        reused.Body.GetProperty("errors").GetProperty("state")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task Callback_InvalidStateOrRejectedCode_Is400_ExpiredStateToo()
    {
        var (_, state) = await ConnectUrlAsync();
        (await _api.SendAsync(HttpMethod.Post, $"{Path}/callback", new { code = "x", state = state[..^3] + "AAA" }, _token))
            .Body.GetProperty("errors").GetProperty("state")[0].GetString().ShouldBe("invalid_format");
        (await _api.SendAsync(HttpMethod.Post, $"{Path}/callback", new { code = FakeCalendarProvider.RejectedCode, state }, _token))
            .Body.GetProperty("errors").GetProperty("code")[0].GetString().ShouldBe("invalid_format");

        // Başka kullanıcının state'i geçmez.
        var other = await _api.SignupAsync("+905321112233", "Berber Ali");
        (await _api.SendAsync(HttpMethod.Post, $"{Path}/callback", new { code = "x", state }, other)).Status.ShouldBe(400);

        _factory.Clock.Advance(Duration.FromMinutes(11));
        (await _api.SendAsync(HttpMethod.Post, $"{Path}/callback", new { code = "x", state }, _token)).Status.ShouldBe(400, "state 10 dk");
    }

    [Fact]
    public async Task Sync_WritesUpdatesAndDeletesEvents_OneWay()
    {
        await ConnectAsync();
        var created = await CreateAppointmentAsync("2026-11-26T08:00:00Z");
        var id = created.Body.GetProperty("appointment").GetProperty("id").GetString()!;

        await _api.RunJobAsync(SyncJob);

        var written = Store.Events.Values.Single();
        written.Title.ShouldBe("Saç kesimi · Selin Ö.");
        written.StartUtc.ShouldBe(Instant.FromUtc(2026, 11, 26, 8, 0));
        written.EndUtc.ShouldBe(Instant.FromUtc(2026, 11, 26, 8, 45));
        (await _api.ScalarAsync<string>($"SELECT sync_status FROM integrations.appointment_calendar_syncs WHERE appointment_id = '{id}'")).ShouldBe("Synced");
        (await _api.GetAsync(Path, _token)).Body.GetProperty("lastSyncedUtc").GetString().ShouldNotBeNull();

        // Değişmeyen randevu yeniden yazılmaz; saat değişince aynı olay güncellenir.
        var eventId = Store.Events.Keys.Single();
        await _api.RunJobAsync(SyncJob);
        Store.Events.Count.ShouldBe(1);
        var current = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        var itemId = current.Body.GetProperty("items")[0].GetProperty("id").GetString();
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/reschedule",
            new { startUtc = "2026-11-26T10:00:00Z", items = new[] { new { itemId, staffId = _elif } } }, _token, current.ETag)).Status.ShouldBe(200);
        await _api.RunJobAsync(SyncJob);
        Store.Events.Keys.Single().ShouldBe(eventId);
        Store.Events[eventId].StartUtc.ShouldBe(Instant.FromUtc(2026, 11, 26, 10, 0));

        // İptal → olay silinir.
        var moved = await _api.GetAsync($"/api/v1/manage/appointments/{id}", _token);
        (await _api.WriteAsync(HttpMethod.Post, $"/api/v1/manage/appointments/{id}/cancel",
            new { reason = "CustomerRequested", scope = "ThisOnly", notifyCustomer = false }, _token, moved.ETag)).Status.ShouldBe(200);
        await _api.RunJobAsync(SyncJob);
        Store.Events.ShouldBeEmpty();
        (await _api.ScalarAsync<string>($"SELECT sync_status FROM integrations.appointment_calendar_syncs WHERE appointment_id = '{id}'")).ShouldBe("Deleted");
    }

    [Fact]
    public async Task RevokedAccess_LinkBecomesTokenExpired_ReconnectRestoresAndDisconnectRevokes()
    {
        await ConnectAsync();
        await CreateAppointmentAsync("2026-11-26T08:00:00Z");
        Store.RevokeAll();

        await _api.RunJobAsync(SyncJob);

        var broken = await _api.GetAsync(Path, _token);
        broken.Body.GetProperty("status").GetString().ShouldBe("TokenExpired", "Google Takvim bağlantısı koptu");
        (await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _token)).Body.GetProperty("googleCalendarStatus").GetString().ShouldBe("TokenExpired");

        // Yenileme de reddedilirse bağlantı kopuk kalır; yeniden bağlanınca Active ve randevular yeniden yazılır.
        Store.Reset();
        await ConnectAsync();
        await _api.RunJobAsync(SyncJob);
        Store.Events.Count.ShouldBe(1);

        (await _api.SendAsync(HttpMethod.Delete, Path, null, _token)).Status.ShouldBe(204);
        (await _api.GetAsync(Path, _token)).Body.GetProperty("status").GetString().ShouldBe("Revoked");
        (await _api.ScalarAsync<bool>("SELECT access_token_encrypted IS NULL AND refresh_token_encrypted IS NULL FROM integrations.calendar_links")).ShouldBeTrue();
        Store.Events.Count.ShouldBe(1, "aktarılmış olaylar silinmez");
        (await _api.SendAsync(HttpMethod.Delete, Path, null, _token)).Status.ShouldBe(204, "idempotent");
    }

    [Fact]
    public async Task RefreshJob_RenewsExpiringToken_OrMarksTokenExpired()
    {
        await ConnectAsync();
        var before = await _api.ScalarAsync<DateTime>("SELECT token_expires_utc FROM integrations.calendar_links");
        _factory.Clock.Advance(Duration.FromMinutes(55));

        await _api.RunJobAsync(RefreshJob);

        (await _api.ScalarAsync<DateTime>("SELECT token_expires_utc FROM integrations.calendar_links")).ShouldBeGreaterThan(before);
        (await _api.ScalarAsync<string>("SELECT status FROM integrations.calendar_links")).ShouldBe("Active");

        Store.RevokeAll();
        _factory.Clock.Advance(Duration.FromMinutes(55));
        await _api.RunJobAsync(RefreshJob);
        (await _api.ScalarAsync<string>("SELECT status FROM integrations.calendar_links")).ShouldBe("TokenExpired");
    }

    private async Task ConnectAsync()
    {
        var (code, state) = await ConnectUrlAsync();
        (await _api.SendAsync(HttpMethod.Post, $"{Path}/callback", new { code, state }, _token)).Status.ShouldBe(200);
    }

    private async Task<(string Code, string State)> ConnectUrlAsync()
    {
        var url = await _api.GetAsync($"{Path}/connect-url", _token);
        url.Status.ShouldBe(200, url.Body.ToString());
        var query = QueryHelpers.ParseQuery(new Uri(url.Body.GetProperty("url").GetString()!).Query);
        url.Body.GetProperty("url").GetString()!.ShouldStartWith("https://salon.omvila.com/ayarlar/google-takvim?");
        query["state"].ToString().ShouldBe(url.Body.GetProperty("state").GetString());
        url.Body.GetProperty("expiresUtc").GetString().ShouldBe(NodaTime.Text.InstantPattern.General.Format(_factory.Clock.GetCurrentInstant() + Duration.FromMinutes(10)));
        return (query["code"].ToString(), url.Body.GetProperty("state").GetString()!);
    }

    private Task<ManageApi.Response> CreateAppointmentAsync(string startUtc) => PostAsync("/api/v1/manage/appointments", new
    {
        branchId = _merkez,
        brandCustomerId = _selin,
        channel = "Panel",
        startUtc,
        items = new[] { new { serviceId = _kesim, staffId = _elif } },
        notifyCustomer = false,
    });

    private Task<ManageApi.Response> PostAsync(string path, object body) => _api.SendAsync(HttpMethod.Post, path, body, _token, Key());

    private static object Week() => new
    {
        days = Enumerable.Range(1, 6).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
    };

    private Dictionary<string, string> Key() =>
        new(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" };
}
