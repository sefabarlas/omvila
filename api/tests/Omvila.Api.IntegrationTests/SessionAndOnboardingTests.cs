using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriteri 1 (session: Owner, Trialing, deneme bitişi 14 gün sonra; kurulum 1/5) ve 10 (personelin
// kısıtlı menüsü), § Alan kuralları (kurulum adımları), T-19 (rol ve kapsam veritabanından), sözleşme manage-session.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class SessionAndOnboardingTests(PostgresFixture db) : IAsyncLifetime
{
    private const string OwnerPhone = "+905324187705";
    private const string StaffPhone = "+905382741905";

    private OmvilaApiFactory _factory = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ExecuteAsync("""
            TRUNCATE identity.otp_challenges, identity.refresh_tokens, identity.terms_acceptances, identity.user_memberships, identity.users CASCADE;
            TRUNCATE notifications.notification_outbox, notifications.credit_transactions;
            TRUNCATE platform.subscriptions;
            TRUNCATE tenancy.brands CASCADE;
            """);
        _factory = new OmvilaApiFactory(db);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Session_AfterSignup_ShowsOwnerTrialAndAllBranches()
    {
        var token = await SignupAsync();

        var session = await GetAsync("/api/v1/manage/session", token);

        session.Status.ShouldBe(200);
        var body = session.Body;
        body.GetProperty("user").GetProperty("fullName").GetString().ShouldBe("Elif Kaya");
        body.GetProperty("user").GetProperty("phoneMasked").GetString().ShouldBe("0532 *** ** 05");
        body.GetProperty("brand").GetProperty("slug").GetString().ShouldBe("kuafor-elif");
        body.GetProperty("brand").GetProperty("status").GetString().ShouldBe("Onboarding");
        body.GetProperty("membership").GetProperty("role").GetString().ShouldBe("Owner");
        body.GetProperty("membership").GetProperty("branchIds").GetArrayLength().ShouldBe(0, "boş dizi = tüm şubeler");
        body.GetProperty("membership").GetProperty("staffId").GetGuid().ShouldNotBe(Guid.Empty);
        body.GetProperty("branches").EnumerateArray().Select(b => b.GetProperty("name").GetString()).ShouldBe(["Merkez"]);
        body.GetProperty("subscription").GetProperty("status").GetString().ShouldBe("Trialing");
        body.GetProperty("subscription").GetProperty("trialEndsOn").GetString().ShouldBe("2026-12-09");
        var permissions = body.GetProperty("permissions").EnumerateArray().Select(p => p.GetString()).ToList();
        permissions.ShouldContain(p => string.Equals(p, "settings.manage", StringComparison.Ordinal));
        permissions.ShouldContain(p => string.Equals(p, "subscription.manage", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Session_ForStaff_ShowsOnlyOwnBranchAndRestrictedPermissions()
    {
        var ownerToken = await SignupAsync();
        var staffToken = await AddStaffAndSignInAsync(ownerToken);

        var session = (await GetAsync("/api/v1/manage/session", staffToken)).Body;

        session.GetProperty("membership").GetProperty("role").GetString().ShouldBe("Staff");
        session.GetProperty("branches").EnumerateArray().Select(b => b.GetProperty("name").GetString()).ShouldBe(["Kadıköy"]);
        var permissions = session.GetProperty("permissions").EnumerateArray().Select(p => p.GetString()).ToList();
        permissions.ShouldContain(p => string.Equals(p, "calendar.view", StringComparison.Ordinal));
        permissions.ShouldNotContain(p => string.Equals(p, "settings.manage", StringComparison.Ordinal));
        permissions.ShouldNotContain(p => string.Equals(p, "customer.phone.view", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Session_WithoutToken_Returns401() =>
        (await GetAsync("/api/v1/manage/session", bearer: null)).Status.ShouldBe(401);

    [Fact]
    public async Task Session_AfterMembershipDisabled_Returns401()
    {
        var token = await SignupAsync();
        await ExecuteAsync("UPDATE identity.user_memberships SET status = 'Disabled'");

        (await GetAsync("/api/v1/manage/session", token)).Status.ShouldBe(401, "T-19: üyelik durumu veritabanından");
    }

    [Fact]
    public async Task Onboarding_AfterSignup_IsOneOfFive()
    {
        var token = await SignupAsync();

        var checklist = (await GetAsync("/api/v1/manage/onboarding", token)).Body;

        checklist.GetProperty("completedCount").GetInt32().ShouldBe(1);
        checklist.GetProperty("totalCount").GetInt32().ShouldBe(5);
        Steps(checklist).ShouldBe([("BusinessCreated", true), ("WorkingHours", false), ("Services", false), ("Staff", false), ("ShareBookingLink", false)]);
    }

    [Fact]
    public async Task Onboarding_Events_CompleteShareAndStaffSteps()
    {
        var token = await SignupAsync();

        (await PostAsync("/api/v1/manage/onboarding/events", new { @event = "QrDownloaded" }, token)).Status.ShouldBe(204);
        (await PostAsync("/api/v1/manage/onboarding/events", new { @event = "SoloConfirmed" }, token)).Status.ShouldBe(204);
        (await PostAsync("/api/v1/manage/onboarding/events", new { @event = "BookingLinkCopied" }, token)).Status.ShouldBe(204, "tekrar bildirim zararsız");

        var steps = Steps((await GetAsync("/api/v1/manage/onboarding", token)).Body).ToDictionary(s => s.Key, s => s.Done, StringComparer.Ordinal);
        steps["ShareBookingLink"].ShouldBeTrue();
        steps["Staff"].ShouldBeTrue();
        (await ScalarAsync<string>("SELECT status FROM tenancy.brands")).ShouldBe("Onboarding", "saat ve hizmet adımları açık");
    }

    [Fact]
    public async Task Onboarding_UnknownEvent_Returns400()
    {
        var token = await SignupAsync();

        var result = await PostAsync("/api/v1/manage/onboarding/events", new { @event = "Shared" }, token);

        result.Status.ShouldBe(400);
        result.Body.GetProperty("errors").GetProperty("event")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task Onboarding_EventByStaff_Returns403()
    {
        var staffToken = await AddStaffAndSignInAsync(await SignupAsync());

        (await PostAsync("/api/v1/manage/onboarding/events", new { @event = "SoloConfirmed" }, staffToken)).Status.ShouldBe(403);
    }

    private static List<(string Key, bool Done)> Steps(JsonElement checklist) =>
        [.. checklist.GetProperty("steps").EnumerateArray().Select(s => (s.GetProperty("key").GetString()!, s.GetProperty("done").GetBoolean()))];

    private async Task<string> SignupAsync()
    {
        var signupToken = (await VerifyAsync(OwnerPhone, "Signup")).GetProperty("signupToken").GetString();
        using var client = Client();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/manage/auth/signup")
        {
            Content = JsonContent.Create(new
            {
                signupToken,
                businessName = "Kuaför Elif",
                industryCode = "hair_salon",
                ownerFullName = "Elif Kaya",
                acceptedTerms = true,
                privacyNoticeVersion = "2026-11-01",
            }),
        };
        request.Headers.Add("Idempotency-Key", "0193a8f0-0000-7000-8000-00000000abcd");
        using var response = await client.SendAsync(request, Ct);
        ((int)response.StatusCode).ShouldBe(201);
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync(Ct)).RootElement.GetProperty("accessToken").GetString()!;
    }

    // Personel uçları PR-16'da gelir; burada veri doğrudan yazılır: Kadıköy şubesi, personel kaydı ve davetli üyelik.
    private async Task<string> AddStaffAndSignInAsync(string ownerToken)
    {
        var session = (await GetAsync("/api/v1/manage/session", ownerToken)).Body;
        var brandId = session.GetProperty("brand").GetProperty("id").GetGuid();
        await ExecuteAsync($"""
            INSERT INTO tenancy.branches (id, brand_id, name, slug, address_line, district, city, country_code, time_zone_id, is_active, created_utc)
              VALUES ('0193a8f0-0000-7000-8000-0000000000b1', '{brandId}', 'Kadıköy', 'kadikoy', '', '', '', 'TR', 'Europe/Istanbul', true, now());
            INSERT INTO tenancy.staff (id, brand_id, full_name, calendar_color, service_commission_percent, sale_commission_percent, is_bookable_online, is_active, created_utc)
              VALUES ('0193a8f0-0000-7000-8000-0000000000c1', '{brandId}', 'Deniz Kara', '#2F5D8A', 0, 0, true, true, now());
            INSERT INTO identity.users (id, phone_e164, full_name, language_code, is_active, created_utc)
              VALUES ('0193a8f0-0000-7000-8000-000000000002', '{StaffPhone}', 'Deniz Kara', 'tr', true, now());
            INSERT INTO identity.user_memberships (id, user_id, brand_id, role, branch_ids, staff_id, status, created_utc)
              VALUES ('0193a8f0-0000-7000-8000-000000000012', '0193a8f0-0000-7000-8000-000000000002', '{brandId}', 'Staff',
                      ARRAY['0193a8f0-0000-7000-8000-0000000000b1']::uuid[], '0193a8f0-0000-7000-8000-0000000000c1', 'Invited', now());
            """);
        _factory.Clock.Advance(NodaTime.Duration.FromMinutes(2));
        return (await VerifyAsync(StaffPhone, "Login")).GetProperty("accessToken").GetString()!;
    }

    private async Task<JsonElement> VerifyAsync(string phone, string purpose)
    {
        var otp = await PostAsync("/api/v1/manage/auth/otp/request", new { phone, purpose }, bearer: null);
        otp.Status.ShouldBe(202);
        var code = await ScalarAsync<string>(
            "SELECT payload->>'kod' FROM notifications.notification_outbox ORDER BY created_utc DESC, id DESC LIMIT 1");
        var verified = await PostAsync("/api/v1/manage/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code }, bearer: null);
        verified.Status.ShouldBe(200);
        return verified.Body;
    }

    private HttpClient Client()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Client-Type", "web");
        return client;
    }

    private Task<(int Status, JsonElement Body)> GetAsync(string path, string? bearer) => SendAsync(HttpMethod.Get, path, null, bearer);

    private Task<(int Status, JsonElement Body)> PostAsync(string path, object body, string? bearer) => SendAsync(HttpMethod.Post, path, body, bearer);

    private async Task<(int Status, JsonElement Body)> SendAsync(HttpMethod method, string path, object? body, string? bearer)
    {
        using var client = Client();
        using var request = new HttpRequestMessage(method, path);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        if (bearer is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        using var response = await client.SendAsync(request, Ct);
        var text = await response.Content.ReadAsStringAsync(Ct);
        return ((int)response.StatusCode, text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone());
    }

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }

    private async Task ExecuteAsync(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }
}
