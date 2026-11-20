using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriterleri 1–4 ve 10 (giriş kısmı), sözleşme manage-auth, ADR-0007
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class ManageAuthTests(PostgresFixture db) : IAsyncLifetime
{
    private const string OwnerPhone = "+905324187705";
    private const string StaffPhone = "+905382741905";
    private const string UnknownPhone = "+905550000099";
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid Kadikoy = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly Guid StaffRecord = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1");

    private OmvilaApiFactory _factory = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ExecuteAsync($"""
            TRUNCATE identity.otp_challenges, identity.refresh_tokens, identity.user_memberships, identity.users CASCADE;
            TRUNCATE notifications.notification_outbox;
            INSERT INTO identity.users (id, phone_e164, full_name, language_code, is_active, created_utc)
              VALUES ('0193a8f0-0000-7000-8000-000000000001', '{OwnerPhone}', 'Elif Kaya', 'tr', true, now()),
                     ('0193a8f0-0000-7000-8000-000000000002', '{StaffPhone}', 'Deniz Kara', 'tr', true, now());
            INSERT INTO identity.user_memberships (id, user_id, brand_id, role, branch_ids, staff_id, status, created_utc)
              VALUES ('0193a8f0-0000-7000-8000-000000000011', '0193a8f0-0000-7000-8000-000000000001', '{Brand}', 'Owner', ARRAY[]::uuid[], null, 'Active', now()),
                     ('0193a8f0-0000-7000-8000-000000000012', '0193a8f0-0000-7000-8000-000000000002', '{Brand}', 'Staff', ARRAY['{Kadikoy}']::uuid[], '{StaffRecord}', 'Invited', now());
            """);
        _factory = new OmvilaApiFactory(db);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task OtpRequest_Returns202_WithMaskedPhone_ForAnyNumber()
    {
        using var client = Client("web");

        var known = await PostAsync(client, "/api/v1/manage/auth/otp/request", new { phone = "0532 418 77 05", purpose = "Login" });
        var unknown = await PostAsync(client, "/api/v1/manage/auth/otp/request", new { phone = "0555 000 00 99", purpose = "Login" });

        known.Status.ShouldBe(202);
        unknown.Status.ShouldBe(202, "numaranın kayıtlı olup olmadığı açık edilmez (kabul kriteri 3)");
        known.Body.GetProperty("phoneMasked").GetString().ShouldBe("0532 *** ** 05");
        known.Body.GetProperty("expiresInSeconds").GetInt32().ShouldBe(180);
    }

    [Fact]
    public async Task UnknownNumber_VerifiesAs_SignupRequired_WithSignupToken()
    {
        using var client = Client("web");

        var result = await LoginAsync(client, UnknownPhone);

        result.Body.GetProperty("result").GetString().ShouldBe("SignupRequired");
        result.Body.GetProperty("signupToken").GetString().ShouldNotBeNullOrEmpty();
        result.Body.TryGetProperty("accessToken", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task OwnerOnWeb_SignsIn_WithHttpOnlyCookie_AndNoRefreshTokenInBody()
    {
        using var client = Client("web");

        var result = await LoginAsync(client, OwnerPhone, remember: true);

        result.Body.GetProperty("result").GetString().ShouldBe("SignedIn");
        result.Body.GetProperty("expiresInSeconds").GetInt32().ShouldBe(900);
        result.Body.TryGetProperty("refreshToken", out _).ShouldBeFalse();
        var cookie = result.SetCookie.ShouldNotBeNull();
        cookie.ShouldStartWith("omvila_rt=");
        cookie.ShouldContain("httponly", Case.Insensitive);
        cookie.ShouldContain("expires=", Case.Insensitive, "30 gün hatırla (kabul kriteri 4)");
    }

    [Fact]
    public async Task WithoutRemember_CookieIsSessionCookie() =>
        (await LoginAsync(Client("web"), OwnerPhone, remember: false)).SetCookie!.ShouldNotContain("expires=", Case.Insensitive);

    [Fact]
    public async Task OwnerOnMobile_GetsRefreshTokenInBody_AndNoCookie()
    {
        var result = await LoginAsync(Client("mobile"), OwnerPhone);

        result.Body.GetProperty("refreshToken").GetString().ShouldNotBeNullOrEmpty();
        result.SetCookie.ShouldBeNull();
    }

    [Fact]
    public async Task InvitedStaff_FirstLogin_ActivatesMembership()
    {
        (await LoginAsync(Client("web"), StaffPhone)).Body.GetProperty("result").GetString().ShouldBe("SignedIn");

        (await ScalarAsync<string>("SELECT status FROM identity.user_memberships WHERE id = '0193a8f0-0000-7000-8000-000000000012'"))
            .ShouldBe("Active", "kabul kriteri 10");
    }

    [Fact]
    public async Task WrongCode_Returns422_WithAttemptsLeft()
    {
        using var client = Client("web");
        var otpId = await RequestOtpAsync(client, OwnerPhone);
        var wrong = string.Equals(await LatestCodeAsync(), "000000", StringComparison.Ordinal) ? "111111" : "000000";

        var result = await PostAsync(client, "/api/v1/manage/auth/otp/verify", new { otpId, code = wrong });

        result.Status.ShouldBe(422);
        result.Body.GetProperty("code").GetString().ShouldBe("otp_invalid");
        result.Body.GetProperty("meta").GetProperty("attemptsLeft").GetInt32().ShouldBe(4);
    }

    [Fact]
    public async Task ResendTooSoon_Returns429_WithRetryAfter()
    {
        using var client = Client("web");
        await RequestOtpAsync(client, OwnerPhone);
        _factory.Clock.Advance(NodaTime.Duration.FromSeconds(18));

        var result = await PostAsync(client, "/api/v1/manage/auth/otp/request", new { phone = OwnerPhone, purpose = "Login" });

        result.Status.ShouldBe(429);
        result.Body.GetProperty("code").GetString().ShouldBe("otp_resend_too_soon");
        result.Body.GetProperty("meta").GetProperty("retryAfterSeconds").GetInt32().ShouldBe(42);
    }

    [Fact]
    public async Task MissingClientTypeHeader_Returns400()
    {
        using var client = _factory.CreateClient();

        (await PostAsync(client, "/api/v1/manage/auth/otp/request", new { phone = OwnerPhone, purpose = "Login" })).Status.ShouldBe(400);
    }

    [Fact]
    public async Task WebRefresh_RotatesCookie_AndRequiresXRequestedWith()
    {
        using var client = Client("web");
        var login = await LoginAsync(client, OwnerPhone, remember: true);
        var cookie = login.SetCookie!.Split(';')[0];

        var withoutHeader = await PostAsync(client, "/api/v1/manage/auth/refresh", new { }, cookie: cookie);
        var refreshed = await PostAsync(client, "/api/v1/manage/auth/refresh", new { }, cookie: cookie, xRequestedWith: true);

        withoutHeader.Status.ShouldBe(401, "CSRF: çerezle yenileme X-Requested-With ister");
        refreshed.Status.ShouldBe(200);
        refreshed.Body.GetProperty("accessToken").GetString().ShouldNotBeNullOrEmpty();
        string.Equals(refreshed.SetCookie!.Split(';')[0], cookie, StringComparison.Ordinal).ShouldBeFalse("jeton her kullanımda döndürülür");
    }

    [Fact]
    public async Task MobileRefresh_WithBodyToken_ReturnsNewPair_AndOldTokenIsRejected()
    {
        using var client = Client("mobile");
        var first = (await LoginAsync(client, OwnerPhone)).Body.GetProperty("refreshToken").GetString();

        var refreshed = await PostAsync(client, "/api/v1/manage/auth/refresh", new { refreshToken = first });
        var reuse = await PostAsync(client, "/api/v1/manage/auth/refresh", new { refreshToken = first });

        refreshed.Status.ShouldBe(200);
        string.Equals(refreshed.Body.GetProperty("refreshToken").GetString(), first, StringComparison.Ordinal).ShouldBeFalse();
        reuse.Status.ShouldBe(401);
    }

    [Fact]
    public async Task Logout_RevokesSession_AndDeletesCookie()
    {
        using var client = Client("web");
        var login = await LoginAsync(client, OwnerPhone, remember: true);
        var cookie = login.SetCookie!.Split(';')[0];
        var access = login.Body.GetProperty("accessToken").GetString()!;

        var logout = await PostAsync(client, "/api/v1/manage/auth/logout", new { }, cookie: cookie, bearer: access);
        var refresh = await PostAsync(client, "/api/v1/manage/auth/refresh", new { }, cookie: cookie, xRequestedWith: true);

        logout.Status.ShouldBe(204);
        logout.SetCookie!.ShouldContain("omvila_rt=;");
        refresh.Status.ShouldBe(401);
    }

    [Fact]
    public async Task Logout_WithoutAccessToken_Returns401() =>
        (await PostAsync(Client("web"), "/api/v1/manage/auth/logout", new { })).Status.ShouldBe(401);

    [Fact]
    public async Task OtpRequests_AboveIpLimit_Return429RateLimited()
    {
        await using var limited = new OmvilaApiFactory(db, new Dictionary<string, string?>(StringComparer.Ordinal) { ["Auth:Otp:MaxPerIpPerHour"] = "2" });
        using var client = limited.CreateClient();
        client.DefaultRequestHeaders.Add("X-Client-Type", "web");

        for (var i = 0; i < 2; i++)
        {
            (await PostAsync(client, "/api/v1/manage/auth/otp/request", new { phone = $"+90555000010{i}", purpose = "Login" })).Status.ShouldBe(202);
        }

        var third = await PostAsync(client, "/api/v1/manage/auth/otp/request", new { phone = "+905550000109", purpose = "Login" });
        third.Status.ShouldBe(429);
        third.Body.GetProperty("code").GetString().ShouldBe("rate_limited");
    }

    private HttpClient Client(string clientType)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Client-Type", clientType);
        return client;
    }

    private async Task<(int Status, JsonElement Body, string? SetCookie)> LoginAsync(HttpClient client, string phone, bool remember = false)
    {
        var otpId = await RequestOtpAsync(client, phone);
        var result = await PostAsync(client, "/api/v1/manage/auth/otp/verify", new { otpId, code = await LatestCodeAsync(), rememberDevice = remember });
        result.Status.ShouldBe(200);
        return result;
    }

    private static async Task<Guid> RequestOtpAsync(HttpClient client, string phone)
    {
        var result = await PostAsync(client, "/api/v1/manage/auth/otp/request", new { phone, purpose = "Login" });
        result.Status.ShouldBe(202);
        return result.Body.GetProperty("otpId").GetGuid();
    }

    private static async Task<(int Status, JsonElement Body, string? SetCookie)> PostAsync(
        HttpClient client, string path, object body, string? cookie = null, bool xRequestedWith = false, string? bearer = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = JsonContent.Create(body) };
        if (cookie is not null)
        {
            request.Headers.Add("Cookie", cookie);
        }

        if (xRequestedWith)
        {
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        }

        if (bearer is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
        }

        using var response = await client.SendAsync(request, Ct);
        var text = await response.Content.ReadAsStringAsync(Ct);
        var setCookie = response.Headers.TryGetValues("Set-Cookie", out var values) ? values.FirstOrDefault() : null;
        return ((int)response.StatusCode, text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone(), setCookie);
    }

    private Task<string> LatestCodeAsync() =>
        ScalarAsync<string>("SELECT payload->>'kod' FROM notifications.notification_outbox ORDER BY created_utc DESC, id DESC LIMIT 1");

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
