using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Npgsql;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: dilim-01 kabul kriteri 1 ve § Alan kuralları (kayıt tek transaction: marka, ayarlar, Merkez şubesi, kullanıcı,
// Owner üyeliği, sahip personeli, Trialing abonelik bugün + 14, deneme kontörü, koşul onayı), sözleşme manageSignup.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class SignupTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Phone = "+905324187705";

    private OmvilaApiFactory _factory = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ExecuteAsync("""
            TRUNCATE identity.otp_challenges, identity.refresh_tokens, identity.terms_acceptances, identity.user_memberships, identity.users CASCADE;
            TRUNCATE notifications.notification_outbox, notifications.credit_transactions;
            TRUNCATE platform.payment_notices, platform.subscription_invoices, platform.subscriptions;
            TRUNCATE tenancy.brands CASCADE;
            """);
        _factory = new OmvilaApiFactory(db);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task Signup_CreatesBrandOwnerAndTrial_AndSignsIn()
    {
        using var client = Client("web");
        var token = await SignupTokenAsync(client);

        var result = await SignupAsync(client, Body(token));

        result.Status.ShouldBe(201);
        result.Body.GetProperty("result").GetString().ShouldBe("SignedIn");
        result.Body.TryGetProperty("refreshToken", out _).ShouldBeFalse("web: yenileme jetonu çerezde");
        var claims = JwtPayload(result.Body.GetProperty("accessToken").GetString()!);
        claims.GetProperty("role").GetString().ShouldBe("Owner");

        (await ScalarAsync<string>("SELECT slug || '/' || status FROM tenancy.brands")).ShouldBe("kuafor-elif/Onboarding");
        (await ScalarAsync<string>("SELECT name || '/' || time_zone_id FROM tenancy.branches")).ShouldBe("Merkez/Europe/Istanbul");
        (await ScalarAsync<string>("SELECT role || '/' || status FROM identity.user_memberships")).ShouldBe("Owner/Active");
        (await ScalarAsync<bool>("SELECT m.staff_id = s.id AND s.user_id = m.user_id FROM identity.user_memberships m, tenancy.staff s")).ShouldBeTrue();
        (await ScalarAsync<string>("SELECT full_name FROM tenancy.staff")).ShouldBe("Elif Kaya");

        // TestClock: 25 Kasım 2026 12:00 İstanbul → deneme bitişi 9 Aralık 2026.
        (await ScalarAsync<string>("SELECT status || '/' || trial_ends_on::text FROM platform.subscriptions")).ShouldBe("Trialing/2026-12-09");
        (await ScalarAsync<string>("SELECT type || '/' || amount || '/' || balance_after FROM notifications.credit_transactions")).ShouldBe("Grant/100/100");
        (await ScalarAsync<string>("SELECT string_agg(document_type || '@' || document_version, ',' ORDER BY document_type) FROM identity.terms_acceptances"))
            .ShouldBe("PrivacyNotice@2026-11-01,TermsOfUse@2026-11-01");
    }

    [Fact]
    public async Task Signup_OnMobile_ReturnsRefreshTokenInBody()
    {
        using var client = Client("mobile");

        var result = await SignupAsync(client, Body(await SignupTokenAsync(client)));

        result.Status.ShouldBe(201);
        result.Body.GetProperty("refreshToken").GetString().ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Signup_Repeated_SignsIntoExistingBrand_WithoutSecondBrand()
    {
        using var client = Client("web");
        var token = await SignupTokenAsync(client);

        (await SignupAsync(client, Body(token))).Status.ShouldBe(201);
        var again = await SignupAsync(client, Body(token, businessName: "Başka Ad"));

        again.Status.ShouldBe(201);
        again.Body.GetProperty("result").GetString().ShouldBe("SignedIn");
        (await ScalarAsync<long>("SELECT count(*) FROM tenancy.brands")).ShouldBe(1);
        (await ScalarAsync<long>("SELECT count(*) FROM platform.subscriptions")).ShouldBe(1);
    }

    [Fact]
    public async Task Signup_WithInvalidToken_Returns401_AndCreatesNothing()
    {
        using var client = Client("web");

        (await SignupAsync(client, Body("gecersiz.jeton.degeri"))).Status.ShouldBe(401);

        (await ScalarAsync<long>("SELECT count(*) FROM tenancy.brands")).ShouldBe(0);
    }

    [Fact]
    public async Task Signup_WithExpiredToken_Returns401()
    {
        using var client = Client("web");
        var token = await SignupTokenAsync(client);
        _factory.Clock.Advance(NodaTime.Duration.FromMinutes(16));

        (await SignupAsync(client, Body(token))).Status.ShouldBe(401);
    }

    [Fact]
    public async Task Signup_ValidatesFields()
    {
        using var client = Client("web");
        var token = await SignupTokenAsync(client);

        var result = await SignupAsync(client, new
        {
            signupToken = token,
            businessName = "K",
            industryCode = "berber",
            ownerFullName = new string('a', 121),
            acceptedTerms = false,
            privacyNoticeVersion = "2025-01-01",
        });

        result.Status.ShouldBe(400);
        result.Body.GetProperty("code").GetString().ShouldBe("validation_failed");
        var errors = result.Body.GetProperty("errors");
        errors.GetProperty("businessName")[0].GetString().ShouldBe("too_short");
        errors.GetProperty("industryCode")[0].GetString().ShouldBe("invalid_format");
        errors.GetProperty("ownerFullName")[0].GetString().ShouldBe("too_long");
        errors.GetProperty("acceptedTerms")[0].GetString().ShouldBe("required");
        errors.GetProperty("privacyNoticeVersion")[0].GetString().ShouldBe("out_of_range");
        (await ScalarAsync<long>("SELECT count(*) FROM identity.users")).ShouldBe(0);
    }

    [Fact]
    public async Task Signup_WithoutIdempotencyKey_Returns400()
    {
        using var client = Client("web");

        (await SignupAsync(client, Body(await SignupTokenAsync(client)), idempotencyKey: null)).Status.ShouldBe(400);
    }

    private static object Body(string token, string businessName = "Kuaför Elif") => new
    {
        signupToken = token,
        businessName,
        industryCode = "hair_salon",
        ownerFullName = "Elif Kaya",
        acceptedTerms = true,
        privacyNoticeVersion = "2026-11-01",
    };

    private HttpClient Client(string clientType)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Client-Type", clientType);
        return client;
    }

    private async Task<string> SignupTokenAsync(HttpClient client)
    {
        var otp = await PostAsync(client, "/api/v1/manage/auth/otp/request", new { phone = Phone, purpose = "Signup" });
        otp.Status.ShouldBe(202);
        var code = await ScalarAsync<string>(
            "SELECT payload->>'kod' FROM notifications.notification_outbox ORDER BY created_utc DESC, id DESC LIMIT 1");
        var verified = await PostAsync(
            client, "/api/v1/manage/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code });
        verified.Body.GetProperty("result").GetString().ShouldBe("SignupRequired");
        return verified.Body.GetProperty("signupToken").GetString()!;
    }

    private static Task<(int Status, JsonElement Body)> SignupAsync(HttpClient client, object body, string? idempotencyKey = "0193a8f0-0000-7000-8000-00000000abcd") =>
        PostAsync(client, "/api/v1/manage/auth/signup", body, idempotencyKey);

    private static async Task<(int Status, JsonElement Body)> PostAsync(HttpClient client, string path, object body, string? idempotencyKey = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = JsonContent.Create(body) };
        if (idempotencyKey is not null)
        {
            request.Headers.Add("Idempotency-Key", idempotencyKey);
        }

        using var response = await client.SendAsync(request, Ct);
        var text = await response.Content.ReadAsStringAsync(Ct);
        return ((int)response.StatusCode, text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone());
    }

    private static JsonElement JwtPayload(string jwt)
    {
        var part = jwt.Split('.')[1].Replace('-', '+').Replace('_', '/');
        part = part.PadRight(part.Length + ((4 - (part.Length % 4)) % 4), '=');
        return JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(part))).RootElement.Clone();
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
