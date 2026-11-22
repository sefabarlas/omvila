using System.Text.Json;
using NodaTime;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-05 KK1 (QR/bağlantıyla eklenen salon BrandCustomer açmaz, panelde görünmez); sözleşme me-auth, me-profile,
// me-brands, me-notifications; ADR-0007 (aud=me jetonu başka yüzeyde geçmez; yenileme jetonu döner, yeniden kullanımda zincir
// iptal); bildirimler.md § Kanal planı (müşteri cihazı varsa push, yoksa SMS; kapora her zaman SMS); T-66 (5), T-103.
// Saat: 2026-11-25 09:00Z (Çarşamba 12:00 İstanbul).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class MeAppTests(PostgresFixture db) : IAsyncLifetime
{
    private const string MervePhone = "+905551112233";
    private const string NurPhone = "+905552223344";
    private const string OwnerPhone = "+905324187705";
    private const string PushToken = "fcm-token-me-0123456789-abcdefghij";
    private const string DispatchJob = "Omvila.Modules.Notifications.Jobs.DispatchNotifications, Omvila.Modules.Notifications";
    private OmvilaApiFactory _factory = null!;
    private ManageApi _api = null!;
    private string _owner = null!;
    private string _merkez = null!;
    private string _elif = null!;
    private string _fon = null!;
    private int _requests;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["Auth:AccessTokenMinutes"] = "600",
            ["Push:Provider"] = "Fake",
        });
        _api = new ManageApi(_factory, db);
        await _api.ScalarAsync<long>(
            $"WITH d AS (DELETE FROM infra.dev_sms_log WHERE phone_e164 IN ('{MervePhone}', '{NurPhone}') RETURNING 1) SELECT count(*) FROM d");
        _owner = await _api.SignupAsync(OwnerPhone, "Kuaför Elif");
        var session = (await _api.GetAsync("/api/v1/manage/session", _owner)).Body;
        _merkez = session.GetProperty("branches")[0].GetProperty("id").GetString()!;
        _elif = session.GetProperty("membership").GetProperty("staffId").GetString()!;
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    // ───────── Giriş ─────────

    [Fact]
    public async Task Otp_SignsIn_CreatesGlobalCustomerOnly_AndSecondSignInIsNotNew()
    {
        var first = await SignInAsync(MervePhone);

        first.Status.ShouldBe(200, first.Body.ToString());
        first.Body.GetProperty("isNewAccount").GetBoolean().ShouldBeTrue();
        first.Body.GetProperty("refreshToken").GetString().ShouldNotBeNullOrEmpty();
        first.Body.GetProperty("expiresInSeconds").GetInt32().ShouldBe(36000);
        (await _api.ScalarAsync<string>($"SELECT is_verified || ':' || has_app_account || ':' || full_name FROM customers.customers WHERE phone_e164 = '{MervePhone}'"))
            .ShouldBe("true:true:");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.brand_customers")).ShouldBe(0, "giriş hiçbir markada kart açmaz");
        (await _api.ScalarAsync<string>("SELECT purpose FROM identity.otp_challenges ORDER BY created_utc DESC LIMIT 1")).ShouldBe("CustomerLogin");
        (await _api.ScalarAsync<string>("SELECT audience || ':' || client_type FROM identity.refresh_tokens WHERE audience = 'me'")).ShouldBe("me:Mobile");

        var second = await SignInAsync(MervePhone);
        second.Body.GetProperty("isNewAccount").GetBoolean().ShouldBeFalse();
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM customers.customers WHERE phone_e164 = '{MervePhone}'")).ShouldBe(1);
    }

    [Fact]
    public async Task Otp_InvalidPhoneOrCode_AndManageCodeIsNotAcceptedHere()
    {
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/otp/request", new { phone = "0212 444 55 66" })).Status.ShouldBe(400);
        var challenge = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/otp/request", new { phone = MervePhone });
        challenge.Status.ShouldBe(202);
        challenge.Body.GetProperty("phoneMasked").GetString().ShouldBe("0555 *** ** 33");
        var otpId = challenge.Body.GetProperty("otpId").GetGuid();

        var wrong = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/otp/verify", new { otpId, code = "000000" });
        wrong.Status.ShouldBe(422);
        wrong.Body.GetProperty("code").GetString().ShouldBe("otp_invalid");

        // Salon giriş kodu müşteri uygulamasında geçmez (amaç koda bağlı).
        _factory.Clock.Advance(Duration.FromMinutes(2));
        var manage = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/request", new { phone = MervePhone, purpose = "Login" });
        var manageCode = await LatestCodeAsync();
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/otp/verify", new { otpId = manage.Body.GetProperty("otpId").GetGuid(), code = manageCode }))
            .Status.ShouldBe(422);
    }

    [Fact]
    public async Task MeToken_IsRejectedOnOtherSurfaces_AndManageTokenIsRejectedOnMe()
    {
        var me = await SignInTokenAsync(MervePhone);

        (await _api.GetAsync("/api/v1/manage/session", me)).Status.ShouldBe(401);
        (await _api.GetAsync("/api/v1/manage/customers", me)).Status.ShouldBe(401);
        (await _api.GetAsync("/api/v1/me/profile", _owner)).Status.ShouldBe(401);
        (await _api.GetAsync("/api/v1/me/brands", _owner)).Status.ShouldBe(401);
        (await _api.SendAsync(HttpMethod.Get, "/api/v1/me/profile", null)).Status.ShouldBe(401);
    }

    [Fact]
    public async Task Refresh_RotatesToken_ReuseRevokesTheChain_AndMeTokenIsNotAManageToken()
    {
        var signedIn = (await SignInAsync(MervePhone)).Body;
        var first = signedIn.GetProperty("refreshToken").GetString()!;

        var rotated = await RefreshAsync(first);
        rotated.Status.ShouldBe(200, rotated.Body.ToString());
        var second = rotated.Body.GetProperty("refreshToken").GetString()!;
        string.Equals(second, first, StringComparison.Ordinal).ShouldBeFalse();
        (await _api.GetAsync("/api/v1/me/profile", rotated.Body.GetProperty("accessToken").GetString()!)).Status.ShouldBe(200);

        // Salon yenileme ucunda müşteri jetonu geçmez ve kullanılmış sayılmaz.
        var manage = await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/auth/refresh", new { refreshToken = second },
            headers: new Dictionary<string, string>(StringComparer.Ordinal) { ["X-Client-Type"] = "mobile" });
        manage.Status.ShouldBe(401);

        (await RefreshAsync(first)).Status.ShouldBe(401, "kullanılmış jeton");
        (await RefreshAsync(second)).Status.ShouldBe(401, "yeniden kullanımda zincirin geçerli jetonu da iptal edilir");
    }

    [Fact]
    public async Task Logout_RevokesTheChain_AndDeactivatesThePushDevice()
    {
        var signedIn = (await SignInAsync(MervePhone)).Body;
        var token = signedIn.GetProperty("accessToken").GetString()!;
        (await RegisterDeviceAsync(token, PushToken)).Status.ShouldBe(200);

        var logout = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/logout",
            new { refreshToken = signedIn.GetProperty("refreshToken").GetString(), pushToken = PushToken }, token);

        logout.Status.ShouldBe(204);
        (await RefreshAsync(signedIn.GetProperty("refreshToken").GetString()!)).Status.ShouldBe(401);
        (await _api.ScalarAsync<bool>("SELECT is_active FROM notifications.devices")).ShouldBeFalse();
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/logout", null, token)).Status.ShouldBe(204, "kendiliğinden idempotent");
    }

    // ───────── Profil ─────────

    [Fact]
    public async Task Profile_GetAndUpdate_WithIfMatch_PhoneReadOnly_EmailNullClears()
    {
        var token = await SignInTokenAsync(MervePhone);
        var profile = await _api.GetAsync("/api/v1/me/profile", token);
        profile.Status.ShouldBe(200);
        profile.ETag.ShouldNotBeNull();
        profile.Body.GetProperty("fullName").ValueKind.ShouldBe(JsonValueKind.Null);
        profile.Body.GetProperty("reviewerDisplayName").ValueKind.ShouldBe(JsonValueKind.Null);
        profile.Body.GetProperty("phone").GetString().ShouldBe(MervePhone);
        profile.Body.GetProperty("language").GetString().ShouldBe("tr");
        profile.Body.GetProperty("version").GetString().ShouldBe(profile.ETag);

        var updated = await _api.WriteAsync(HttpMethod.Patch, "/api/v1/me/profile",
            new { fullName = "Merve Yıldız", email = "merve.yildiz@example.com", phone = "+905559999999" }, token, profile.ETag);
        updated.Status.ShouldBe(200, updated.Body.ToString());
        updated.Body.GetProperty("fullName").GetString().ShouldBe("Merve Yıldız");
        updated.Body.GetProperty("reviewerDisplayName").GetString().ShouldBe("Merve Y.");
        updated.Body.GetProperty("email").GetString().ShouldBe("merve.yildiz@example.com");
        updated.Body.GetProperty("phone").GetString().ShouldBe(MervePhone, "telefon bu uçla değişmez");

        (await _api.WriteAsync(HttpMethod.Patch, "/api/v1/me/profile", new { email = (string?)null }, token, profile.ETag)).Status.ShouldBe(412);
        var cleared = await _api.WriteAsync(HttpMethod.Patch, "/api/v1/me/profile", new { email = (string?)null }, token, updated.ETag);
        cleared.Status.ShouldBe(200);
        cleared.Body.GetProperty("email").ValueKind.ShouldBe(JsonValueKind.Null);
        cleared.Body.GetProperty("fullName").GetString().ShouldBe("Merve Yıldız", "gönderilmeyen alan değişmez");

        var invalid = await _api.WriteAsync(HttpMethod.Patch, "/api/v1/me/profile", new { fullName = "M", email = "merve@", language = "en" }, token, cleared.ETag);
        invalid.Status.ShouldBe(400);
        var errors = invalid.Body.GetProperty("errors");
        errors.GetProperty("fullName")[0].GetString().ShouldBe("too_short");
        errors.GetProperty("email")[0].GetString().ShouldBe("invalid_format");
        errors.GetProperty("language")[0].GetString().ShouldBe("invalid_format");
        (await _api.SendAsync(HttpMethod.Patch, "/api/v1/me/profile", new { fullName = "Merve" }, token)).Status.ShouldBe(428);
    }

    // ───────── Salonlarım ─────────

    [Fact]
    public async Task Criterion1_SaveBrand_DoesNotCreateBrandCustomer_AndSalonDoesNotSeeTheCustomer()
    {
        var token = await SignInTokenAsync(MervePhone);

        var saved = await SaveBrandAsync(token, "kuafor-elif", "Qr");

        saved.Status.ShouldBe(200, saved.Body.ToString());
        saved.Body.GetProperty("name").GetString().ShouldBe("Kuaför Elif");
        saved.Body.GetProperty("slug").GetString().ShouldBe("kuafor-elif");
        saved.Body.GetProperty("hasBooked").GetBoolean().ShouldBeFalse();
        saved.Body.GetProperty("savedUtc").GetString().ShouldBe("2026-11-25T09:04:00Z");
        saved.Body.GetProperty("lastVisitOn").ValueKind.ShouldBe(JsonValueKind.Null);
        saved.Body.GetProperty("bookingEnabled").GetBoolean().ShouldBeTrue();
        saved.Body.GetProperty("coverColor").GetString().ShouldBe("petrol");
        saved.Body.GetProperty("branches")[0].GetProperty("id").GetString().ShouldBe(_merkez);
        saved.Body.GetProperty("activePackages").GetArrayLength().ShouldBe(0);
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.brand_customers")).ShouldBe(0);
        (await _api.GetAsync("/api/v1/manage/customers", _owner)).Body.GetProperty("items").GetArrayLength().ShouldBe(0, "salon randevu almamış kişiyi görmez");

        _factory.Clock.Advance(Duration.FromMinutes(5));
        var again = await SaveBrandAsync(token, "kuafor-elif", "Link");
        again.Status.ShouldBe(200);
        again.Body.GetProperty("savedUtc").GetString().ShouldBe("2026-11-25T09:04:00Z", "kendiliğinden idempotent: aynı satır");
        (await _api.ScalarAsync<long>("SELECT count(*) FROM customers.customer_saved_brands")).ShouldBe(1);

        var list = await _api.GetAsync("/api/v1/me/brands", token);
        list.Status.ShouldBe(200);
        list.Body.GetProperty("items").EnumerateArray().Select(b => b.GetProperty("slug").GetString()).ShouldBe(["kuafor-elif"]);
    }

    [Fact]
    public async Task SaveBrand_UnknownSlug404_InvalidBody400()
    {
        var token = await SignInTokenAsync(MervePhone);

        var unknown = await SaveBrandAsync(token, "olmayan-salon", "Qr");
        unknown.Status.ShouldBe(404);
        unknown.Body.GetProperty("code").GetString().ShouldBe("not_found");

        var invalid = await SaveBrandAsync(token, "Büyük Harf", "Booking");
        invalid.Status.ShouldBe(400);
        invalid.Body.GetProperty("errors").GetProperty("brandSlug")[0].GetString().ShouldBe("invalid_format");
        invalid.Body.GetProperty("errors").GetProperty("source")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task ListBrands_BookedBrand_HasBookedAndPackageBadge_OtherCustomersBrandsAreNotListed()
    {
        await SetUpFonAsync();
        var customer = await PostAsync("/api/v1/manage/customers", new { fullName = "Merve Yıldız", phone = MervePhone });
        var package = await PostAsync("/api/v1/manage/packages", new
        {
            name = "Fön paketi",
            price = new { amountMinor = 150000, currency = "TRY" },
            items = new[] { new { serviceId = _fon, sessionCount = 5 } },
            validityDays = 90,
            isAvailableAllBranches = true,
            isActive = true,
        });
        package.Status.ShouldBe(201, package.Body.ToString());
        var sold = await PostAsync($"/api/v1/manage/customers/{customer.Body.GetProperty("id").GetString()}/packages",
            new { packageId = package.Body.GetProperty("id").GetString(), branchId = _merkez, soldByStaffId = _elif, paymentMethod = "Cash" });
        sold.Status.ShouldBe(201, sold.Body.ToString());
        var merve = await SignInTokenAsync(MervePhone);
        var nur = await SignInTokenAsync(NurPhone);

        var list = (await _api.GetAsync("/api/v1/me/brands", merve)).Body.GetProperty("items");

        var row = list.EnumerateArray().ShouldHaveSingleItem();
        row.GetProperty("hasBooked").GetBoolean().ShouldBeTrue();
        row.GetProperty("savedUtc").ValueKind.ShouldBe(JsonValueKind.Null);
        var badge = row.GetProperty("activePackages").EnumerateArray().ShouldHaveSingleItem();
        badge.GetProperty("packageName").GetString().ShouldBe("Fön paketi");
        badge.GetProperty("remainingSessions").GetInt32().ShouldBe(5);
        badge.GetProperty("customerPackageId").GetString().ShouldBe(sold.Body.GetProperty("id").GetString());
        (await _api.GetAsync("/api/v1/me/brands", nur)).Body.GetProperty("items").GetArrayLength().ShouldBe(0, "başka müşterinin salonları görünmez");
    }

    // ───────── Cihaz ─────────

    [Fact]
    public async Task RegisterDevice_IsBrandless_SameTokenUpdates_AndTokenMovingToStaffDeactivatesIt()
    {
        var token = await SignInTokenAsync(MervePhone);

        var first = await RegisterDeviceAsync(token, PushToken);
        first.Status.ShouldBe(200, first.Body.ToString());
        var second = await RegisterDeviceAsync(token, PushToken, "Android");
        second.Body.GetProperty("deviceId").GetString().ShouldBe(first.Body.GetProperty("deviceId").GetString());
        second.Body.GetProperty("platform").GetString().ShouldBe("Android");
        (await _api.ScalarAsync<string>("SELECT owner_type || ':' || app || ':' || (brand_id IS NULL) || ':' || is_active FROM notifications.devices"))
            .ShouldBe("Customer:Customer:true:true");

        // Aynı telefonda salon hesabına geçiş: jeton işletme cihazı olur, müşteri kaydı pasifleşir (aktif kayıtlar arasında jeton tek).
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/manage/devices", new { platform = "iOS", pushToken = PushToken, appVersion = "1.0.3" }, _owner))
            .Status.ShouldBe(200);
        (await _api.ScalarAsync<string>("SELECT string_agg(owner_type || ':' || is_active, ',' ORDER BY owner_type) FROM notifications.devices"))
            .ShouldBe("Customer:false,User:true");

        (await RegisterDeviceAsync(token, PushToken)).Status.ShouldBe(200);
        (await _api.ScalarAsync<string>("SELECT string_agg(owner_type || ':' || is_active, ',' ORDER BY owner_type) FROM notifications.devices"))
            .ShouldBe("Customer:true,User:false");

        var invalid = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/devices", new { platform = "Windows", pushToken = "short", appVersion = "1" }, token);
        invalid.Status.ShouldBe(400);
    }

    // ───────── Müşteri push planı ve bildirimler ─────────

    [Fact]
    public async Task CustomerWithDevice_ConfirmationGoesByPush_AndAppearsInNotifications()
    {
        await SetUpFonAsync();
        var merve = await SignInTokenAsync(MervePhone);
        (await RegisterDeviceAsync(merve, PushToken)).Status.ShouldBe(200);
        var appointment = await BookByPanelAsync(MervePhone, "2026-12-02T11:30:00Z");

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        await _api.RunJobAsync(DispatchJob);

        (await _api.ScalarAsync<string>(
            "SELECT recipient_type || '|' || array_to_string(channel_plan, ',') || '|' || status || '|' || channel_used || '|' || credits_consumed FROM notifications.notification_outbox WHERE event_code = 'appointment.confirmed'"))
            .ShouldBe("Customer|Push,Sms|Sent|Push|0");
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM infra.dev_sms_log WHERE phone_e164 = '{MervePhone}' AND body LIKE '%onaylandı%'")).ShouldBe(0);
        (await _api.ScalarAsync<string>(
            "SELECT recipient_type || '|' || array_to_string(channel_plan, ',') FROM notifications.notification_outbox WHERE event_code = 'appointment.reminder' LIMIT 1"))
            .ShouldBe("Customer|Push,Sms", "hatırlatmalar da müşteri hesabıyla yazılır");

        var page = await _api.GetAsync("/api/v1/me/notifications", merve);
        page.Status.ShouldBe(200, page.Body.ToString());
        page.Body.GetProperty("unreadCount").GetInt32().ShouldBe(1, "zamanı gelmemiş hatırlatma listede yok");
        page.Body.GetProperty("nextCursor").ValueKind.ShouldBe(JsonValueKind.Null);
        var item = page.Body.GetProperty("items").EnumerateArray().ShouldHaveSingleItem();
        item.GetProperty("event").GetString().ShouldBe("appointment.confirmed");
        item.GetProperty("title").GetString().ShouldBe("Randevunuz onaylandı");
        item.GetProperty("body").GetString().ShouldBe("Kuaför Elif · Çar 2 Ara 14:30. Görüşmek üzere.");
        item.GetProperty("readUtc").ValueKind.ShouldBe(JsonValueKind.Null);
        item.GetProperty("brand").GetProperty("slug").GetString().ShouldBe("kuafor-elif");
        item.GetProperty("target").GetProperty("type").GetString().ShouldBe("Appointment");
        item.GetProperty("target").GetProperty("appointmentId").GetString().ShouldBe(appointment);
        item.GetProperty("target").GetProperty("brandSlug").GetString().ShouldBe("kuafor-elif");

        var read = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/notifications/read", new { all = true }, merve);
        read.Status.ShouldBe(200);
        read.Body.GetProperty("unreadCount").GetInt32().ShouldBe(0);
        (await _api.GetAsync("/api/v1/me/notifications?unreadOnly=true", merve)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
    }

    [Fact]
    public async Task CustomerWithoutDevice_ConfirmationFallsBackToSms_AndIsStillListed()
    {
        await SetUpFonAsync();
        var merve = await SignInTokenAsync(MervePhone);
        await BookByPanelAsync(MervePhone, "2026-12-02T11:30:00Z");

        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        await _api.RunJobAsync(DispatchJob);

        (await _api.ScalarAsync<string>(
            "SELECT array_to_string(channel_plan, ',') || '|' || status || '|' || channel_used FROM notifications.notification_outbox WHERE event_code = 'appointment.confirmed'"))
            .ShouldBe("Push,Sms|Sent|Sms");
        (await _api.ScalarAsync<long>($"SELECT count(*) FROM infra.dev_sms_log WHERE phone_e164 = '{MervePhone}' AND body LIKE '%onaylandı%'")).ShouldBe(1);
        (await _api.GetAsync("/api/v1/me/notifications", merve)).Body.GetProperty("unreadCount").GetInt32().ShouldBe(1, "kayıt teslimden bağımsız");
    }

    [Fact]
    public async Task Notifications_AreIsolatedPerCustomer_AndMarkReadValidates()
    {
        await SetUpFonAsync();
        var merve = await SignInTokenAsync(MervePhone);
        var nur = await SignInTokenAsync(NurPhone);
        await BookByPanelAsync(MervePhone, "2026-12-02T11:30:00Z");
        await _api.RunJobAsync(ManageApi.OutboxDispatcherJob);
        await _api.RunJobAsync(DispatchJob);
        var merveNotification = (await _api.GetAsync("/api/v1/me/notifications", merve)).Body.GetProperty("items")[0].GetProperty("id").GetString();

        (await _api.GetAsync("/api/v1/me/notifications", nur)).Body.GetProperty("items").GetArrayLength().ShouldBe(0);
        var foreign = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/notifications/read", new { notificationIds = new[] { merveNotification } }, nur);
        foreign.Status.ShouldBe(200);
        foreign.Body.GetProperty("unreadCount").GetInt32().ShouldBe(0);
        (await _api.GetAsync("/api/v1/me/notifications", merve)).Body.GetProperty("unreadCount").GetInt32().ShouldBe(1, "başka müşterinin bildirimi sessizce yok sayılır");

        (await _api.SendAsync(HttpMethod.Post, "/api/v1/me/notifications/read", new { }, merve)).Status.ShouldBe(400);
        (await _api.SendAsync(HttpMethod.Post, "/api/v1/me/notifications/read", new { all = true, notificationIds = new[] { merveNotification } }, merve))
            .Status.ShouldBe(400);
        (await _api.GetAsync("/api/v1/me/notifications?limit=0", merve)).Status.ShouldBe(400);
        var mine = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/notifications/read", new { notificationIds = new[] { merveNotification } }, merve);
        mine.Body.GetProperty("unreadCount").GetInt32().ShouldBe(0);
    }

    // ───────── Yardımcılar ─────────

    private async Task<ManageApi.Response> SignInAsync(string phone)
    {
        _factory.Clock.Advance(Duration.FromMinutes(2));
        var otp = await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/otp/request", new { phone });
        otp.Status.ShouldBe(202, otp.Body.ToString());
        return await _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code = await LatestCodeAsync() });
    }

    private async Task<string> SignInTokenAsync(string phone)
    {
        var signedIn = await SignInAsync(phone);
        signedIn.Status.ShouldBe(200, signedIn.Body.ToString());
        return signedIn.Body.GetProperty("accessToken").GetString()!;
    }

    private Task<string> LatestCodeAsync() =>
        _api.ScalarAsync<string>("SELECT payload->>'kod' FROM notifications.notification_outbox WHERE event_code = 'otp.code' ORDER BY created_utc DESC, id DESC LIMIT 1");

    private Task<ManageApi.Response> RefreshAsync(string refreshToken) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/me/auth/refresh", new { refreshToken });

    private Task<ManageApi.Response> SaveBrandAsync(string token, string brandSlug, string source) =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/me/brands", new { brandSlug, source }, token);

    private Task<ManageApi.Response> RegisterDeviceAsync(string token, string pushToken, string platform = "iOS") =>
        _api.SendAsync(HttpMethod.Post, "/api/v1/me/devices", new { platform, pushToken, appVersion = "1.0.0" }, token);

    private async Task SetUpFonAsync()
    {
        var week = new
        {
            days = Enumerable.Range(1, 7).Select(d => new { dayOfWeek = d, intervals = new[] { new { start = "09:00", end = "19:00" } } }).ToArray(),
        };
        var hours = await _api.GetAsync($"/api/v1/manage/branches/{_merkez}/working-hours", _owner);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/branches/{_merkez}/working-hours", week, _owner, hours.ETag)).Status.ShouldBe(200);
        var elif = await _api.GetAsync($"/api/v1/manage/staff/{_elif}", _owner);
        (await _api.WriteAsync(HttpMethod.Put, $"/api/v1/manage/staff/{_elif}/branches/{_merkez}/working-hours", week, _owner, elif.ETag)).Status.ShouldBe(200);
        var category = (await PostAsync("/api/v1/manage/service-categories", new { name = "Saç" })).Body.GetProperty("id").GetString()!;
        _fon = (await PostAsync("/api/v1/manage/services", new
        {
            categoryId = category,
            name = "Fön",
            durationMinutes = 45,
            price = new { amountMinor = 45000, currency = "TRY" },
            priceDisplay = "Fixed",
            deposit = new { mode = "Inherit" },
            isOnlineBookable = true,
            staffIds = new[] { _elif },
        })).Body.GetProperty("id").GetString()!;
    }

    private async Task<string> BookByPanelAsync(string phone, string startUtc)
    {
        var customer = await PostAsync("/api/v1/manage/customers", new { fullName = "Merve Yıldız", phone });
        var created = await PostAsync("/api/v1/manage/appointments", new
        {
            branchId = _merkez,
            brandCustomerId = customer.Body.GetProperty("id").GetString(),
            channel = "Phone",
            startUtc,
            items = new[] { new { serviceId = _fon, staffId = _elif } },
            notifyCustomer = true,
        });
        created.Status.ShouldBe(201, created.Body.ToString());
        return created.Body.GetProperty("appointment").GetProperty("id").GetString()!;
    }

    private Task<ManageApi.Response> PostAsync(string path, object body) =>
        _api.SendAsync(HttpMethod.Post, path, body, _owner,
            new Dictionary<string, string>(StringComparer.Ordinal) { ["Idempotency-Key"] = $"0193a8f0-0000-7000-8000-{++_requests:D12}" });
}
