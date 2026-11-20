using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

/// <summary>Panel API'si için test istemcisi: kayıt akışı, kimlikli istek, ETag/If-Match.</summary>
internal sealed class ManageApi(OmvilaApiFactory factory, PostgresFixture db)
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public sealed record Response(int Status, JsonElement Body, string? ETag);

    /// <summary>Tabloları boşaltır (kayıt akışının dokunduğu her şey).</summary>
    public static async Task ResetAsync(PostgresFixture db)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("""
            TRUNCATE identity.otp_challenges, identity.refresh_tokens, identity.terms_acceptances, identity.user_memberships, identity.users CASCADE;
            TRUNCATE notifications.notification_outbox, notifications.credit_transactions;
            TRUNCATE platform.subscriptions;
            TRUNCATE tenancy.brands CASCADE;
            """, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    /// <summary>OTP → kayıt; erişim jetonunu döner. Her çağrıdan önce saat ilerletilir (OTP tekrar bekleme süresi).</summary>
    public async Task<string> SignupAsync(string phone, string businessName)
    {
        factory.Clock.Advance(NodaTime.Duration.FromMinutes(2));
        var otp = await SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/request", new { phone, purpose = "Signup" });
        var code = await ScalarAsync<string>(
            "SELECT payload->>'kod' FROM notifications.notification_outbox ORDER BY created_utc DESC, id DESC LIMIT 1");
        var verified = await SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code });
        var signup = await SendAsync(
            HttpMethod.Post,
            "/api/v1/manage/auth/signup",
            new
            {
                signupToken = verified.Body.GetProperty("signupToken").GetString(),
                businessName,
                industryCode = "hair_salon",
                acceptedTerms = true,
                privacyNoticeVersion = "2026-11-01",
            },
            headers: new Dictionary<string, string>(StringComparer.Ordinal) { ["Idempotency-Key"] = "0193a8f0-0000-7000-8000-00000000abcd" });
        signup.Status.ShouldBe(201);
        return signup.Body.GetProperty("accessToken").GetString()!;
    }

    public Task<Response> GetAsync(string path, string token) => SendAsync(HttpMethod.Get, path, null, token);

    public Task<Response> WriteAsync(HttpMethod method, string path, object body, string token, string? ifMatch) =>
        SendAsync(method, path, body, token, ifMatch is null ? null : new Dictionary<string, string>(StringComparer.Ordinal) { ["If-Match"] = ifMatch });

    public async Task<Response> SendAsync(
        HttpMethod method, string path, object? body, string? token = null, IReadOnlyDictionary<string, string>? headers = null)
    {
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Client-Type", "web");
        using var request = new HttpRequestMessage(method, path);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        foreach (var (name, value) in headers ?? new Dictionary<string, string>(StringComparer.Ordinal))
        {
            request.Headers.TryAddWithoutValidation(name, value);
        }

        using var response = await client.SendAsync(request, Ct);
        var text = await response.Content.ReadAsStringAsync(Ct);
        return new Response(
            (int)response.StatusCode,
            text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone(),
            response.Headers.ETag?.ToString());
    }

    public async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
