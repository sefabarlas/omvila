using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Omvila.Infrastructure.Jobs;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

/// <summary>Panel API'si için test istemcisi: kayıt akışı, kimlikli istek, ETag/If-Match.</summary>
internal sealed class ManageApi(OmvilaApiFactory factory, PostgresFixture db)
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public sealed record Response(int Status, JsonElement Body, string? ETag, IReadOnlyDictionary<string, string>? Headers = null)
    {
        public string? Header(string name) => Headers is not null && Headers.TryGetValue(name, out var value) ? value : null;
    }

    /// <summary>Tabloları boşaltır (kayıt akışının dokunduğu her şey).</summary>
    public static async Task ResetAsync(PostgresFixture db)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("""
            TRUNCATE identity.otp_challenges, identity.trusted_devices, identity.refresh_tokens, identity.terms_acceptances, identity.user_memberships, identity.users CASCADE;
            TRUNCATE notifications.notification_outbox, notifications.credit_transactions, notifications.brand_sms_senders, notifications.devices, notifications.in_app_notifications;
            TRUNCATE platform.subscriptions;
            TRUNCATE tenancy.brands CASCADE;
            TRUNCATE catalog.service_categories CASCADE;
            TRUNCATE customers.customers CASCADE;
            TRUNCATE scheduling.appointment_access_codes, scheduling.appointment_item_segments, scheduling.appointment_items, scheduling.appointments, scheduling.appointment_series;
            TRUNCATE sales.payment_allocations, sales.payments, sales.commission_entries, sales.cash_day_closes;
            TRUNCATE infra.outbox_events CASCADE;
            """, connection);
        await command.ExecuteNonQueryAsync(Ct);
    }

    /// <summary>OTP → kayıt; erişim jetonunu döner. Her çağrıdan önce saat ilerletilir (OTP tekrar bekleme süresi).</summary>
    public async Task<string> SignupAsync(string phone, string businessName)
    {
        factory.Clock.Advance(NodaTime.Duration.FromMinutes(2));
        var otp = await SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/request", new { phone, purpose = "Signup" });
        var code = await ScalarAsync<string>(
            "SELECT payload->>'kod' FROM notifications.notification_outbox WHERE event_code = 'otp.code' ORDER BY created_utc DESC, id DESC LIMIT 1");
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

    /// <summary>Markaya üye ekler (personel uçları PR-16'da) ve o numarayla giriş yapar; erişim jetonunu döner.</summary>
    public async Task<string> AddMemberAndSignInAsync(string ownerToken, string phone, string role, string branchId)
    {
        var brandId = (await GetAsync("/api/v1/manage/session", ownerToken)).Body.GetProperty("brand").GetProperty("id").GetGuid();
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand($"""
                INSERT INTO identity.users (id, phone_e164, full_name, language_code, is_active, created_utc)
                  VALUES (gen_random_uuid(), '{phone}', 'Deniz Kara', 'tr', true, now());
                INSERT INTO identity.user_memberships (id, user_id, brand_id, role, branch_ids, staff_id, status, created_utc)
                  SELECT gen_random_uuid(), id, '{brandId}', '{role}', ARRAY['{branchId}']::uuid[], null, 'Invited', now()
                  FROM identity.users WHERE phone_e164 = '{phone}';
                """, connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        factory.Clock.Advance(NodaTime.Duration.FromMinutes(2));
        var otp = await SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/request", new { phone, purpose = "Login" });
        var code = await ScalarAsync<string>(
            "SELECT payload->>'kod' FROM notifications.notification_outbox WHERE event_code = 'otp.code' ORDER BY created_utc DESC, id DESC LIMIT 1");
        var verified = await SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code });
        return verified.Body.GetProperty("accessToken").GetString()!;
    }

    /// <summary>OTP ile panele giriş; erişim jetonunu (veya SignupRequired gövdesini) döner.</summary>
    public async Task<Response> SignInAsync(string phone)
    {
        factory.Clock.Advance(NodaTime.Duration.FromMinutes(2));
        var otp = await SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/request", new { phone, purpose = "Login" });
        var code = await ScalarAsync<string>(
            "SELECT payload->>'kod' FROM notifications.notification_outbox WHERE event_code = 'otp.code' ORDER BY created_utc DESC, id DESC LIMIT 1");
        return await SendAsync(HttpMethod.Post, "/api/v1/manage/auth/otp/verify", new { otpId = otp.Body.GetProperty("otpId").GetGuid(), code });
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
            response.Headers.ETag?.ToString(),
            response.Headers.ToDictionary(h => h.Key, h => string.Join(',', h.Value), StringComparer.OrdinalIgnoreCase));
    }

    /// <summary>Entegrasyon olaylarını işleyicilere iletir (DispatchIntegrationEvents).</summary>
    public const string OutboxDispatcherJob = "Omvila.Infrastructure.Messaging.OutboxDispatcher, Omvila.Infrastructure";

    public const string MarkNoShowsJob = "Omvila.Modules.Scheduling.Jobs.MarkNoShows, Omvila.Modules.Scheduling";

    public const string ExtendRecurringSeriesJob = "Omvila.Modules.Scheduling.Jobs.ExtendRecurringSeries, Omvila.Modules.Scheduling";

    /// <summary>
    /// Periyodik işi bir kez çalıştırır (api/AGENTS.md: testte JobRunner.RunOnceAsync, saat FakeClock). İş sınıfları modül içi
    /// (internal) olduğundan türe adıyla erişilir.
    /// </summary>
    public async Task RunJobAsync(string jobTypeName)
    {
        var type = Type.GetType(jobTypeName, throwOnError: true)!;
        (await factory.Services.GetRequiredService<JobRunner>().RunOnceAsync(type, Ct)).ShouldBe(JobOutcome.Succeeded, jobTypeName);
    }

    public async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
