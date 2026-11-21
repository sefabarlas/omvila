using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Npgsql;
using Omvila.Infrastructure.Sms;
using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

// Kural: sözleşme webhookSmsDeliveryReport; dilim-03 kriter 10 (imzasız istek reddedilir, teslim edilen Sent, başarısız
// geri çekilmeyle tekrar, 5. denemede Failed); T-62 (8); T-64 (teslim edilemeyen SMS'in kontörü iade edilir).
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class SmsDeliveryReportTests(PostgresFixture db) : IAsyncLifetime
{
    private const string Path = "/api/v1/webhooks/sms/fake";
    private const string ProviderId = "fake-0193a8f0-0000-7000-8000-0000000000f1";
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-00000000000a");
    private static readonly Guid MessageId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000e1");
    private OmvilaApiFactory _factory = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await ManageApi.ResetAsync(db);
        _factory = new OmvilaApiFactory(db);
    }

    public async ValueTask DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task UnsignedReport_Is401_AndTouchesNothing()
    {
        await SeedSentMessageAsync(attempts: 1);

        var response = await PostAsync(Path, Delivered(), signature: null);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await ProblemCodeAsync(response)).ShouldBe("unauthorized");
        (await StateAsync()).ShouldBe("Sent:1:false");
    }

    [Fact]
    public async Task WrongSignature_Is401()
    {
        await SeedSentMessageAsync(attempts: 1);
        var body = Delivered();

        var response = await PostAsync(Path, body, FakeSmsDeliveryReportVerifier.Sign("wrong_secret", body));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await StateAsync()).ShouldBe("Sent:1:false");
    }

    [Fact]
    public async Task UnknownProvider_Is404()
    {
        var body = Delivered();

        var response = await PostAsync("/api/v1/webhooks/sms/netgsm", body, Sign(body));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SignedButUnreadableBody_Is400()
    {
        var body = Encoding.UTF8.GetBytes("""{"status":"delivered"}""");

        var response = await PostAsync(Path, body, Sign(body));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delivered_RecordsDelivery_AndDuplicateIsIgnored()
    {
        await SeedSentMessageAsync(attempts: 1);
        var body = Delivered();

        (await PostAsync(Path, body, Sign(body))).StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var deliveredAt = await ScalarAsync<DateTime>("SELECT delivered_utc FROM notifications.notification_outbox");
        (await PostAsync(Path, body, Sign(body))).StatusCode.ShouldBe(HttpStatusCode.NoContent);

        (await StateAsync()).ShouldBe("Sent:1:true");
        (await ScalarAsync<DateTime>("SELECT delivered_utc FROM notifications.notification_outbox")).ShouldBe(deliveredAt);
    }

    [Fact]
    public async Task Failed_RefundsCredit_SchedulesResendWithBackoff_AndDuplicateDoesNotRefundTwice()
    {
        await SeedSentMessageAsync(attempts: 1);
        var body = Failed();

        (await PostAsync(Path, body, Sign(body))).StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await PostAsync(Path, body, Sign(body))).StatusCode.ShouldBe(HttpStatusCode.NoContent);

        (await StateAsync()).ShouldBe("Pending:1:false");
        (await ScalarAsync<string>("SELECT failure_reason || ':' || credits_consumed FROM notifications.notification_outbox")).ShouldBe("absent_subscriber:0");
        (await ScalarAsync<bool>($"SELECT scheduled_utc = '{_factory.Clock.GetCurrentInstant()}'::timestamptz + interval '30 seconds' FROM notifications.notification_outbox"))
            .ShouldBeTrue();
        (await ScalarAsync<long>("SELECT sum(amount) FROM notifications.credit_transactions")).ShouldBe(10);
        (await ScalarAsync<long>("SELECT count(*) FROM notifications.credit_transactions WHERE type = 'Refund'")).ShouldBe(1);
    }

    [Fact]
    public async Task FailedReportOnFifthAttempt_MarksFailed()
    {
        await SeedSentMessageAsync(attempts: 5);
        var body = Failed();

        (await PostAsync(Path, body, Sign(body))).StatusCode.ShouldBe(HttpStatusCode.NoContent);

        (await StateAsync()).ShouldBe("Failed:5:false");
    }

    [Fact]
    public async Task UnknownMessageId_Is204()
    {
        var body = Encoding.UTF8.GetBytes("""{"messageId":"fake-unknown","status":"delivered"}""");

        (await PostAsync(Path, body, Sign(body))).StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    private static byte[] Delivered() => Encoding.UTF8.GetBytes($$"""{"messageId":"{{ProviderId}}","status":"delivered"}""");

    private static byte[] Failed() => Encoding.UTF8.GetBytes($$"""{"messageId":"{{ProviderId}}","status":"failed","reason":"absent_subscriber"}""");

    private static string Sign(byte[] body) => FakeSmsDeliveryReportVerifier.Sign(TestSmsWebhook.Secret, body);

    private async Task<HttpResponseMessage> PostAsync(string path, byte[] body, string? signature)
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = new ByteArrayContent(body) };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        if (signature is not null)
        {
            request.Headers.Add(FakeSmsDeliveryReportVerifier.SignatureHeader, signature);
        }

        return await client.SendAsync(request, Ct);
    }

    private static async Task<string?> ProblemCodeAsync(HttpResponseMessage response)
    {
        using var document = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync(Ct));
        return document.RootElement.GetProperty("code").GetString();
    }

    // Gönderilmiş (Sent) mesaj ve defter: 10 hediye − 1 düşüm = 9.
    private async Task SeedSentMessageAsync(int attempts)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("""
            INSERT INTO notifications.notification_outbox
              (id, brand_id, event_code, category, recipient_type, recipient_phone, channel_plan, channel_used, payload, rendered_body,
               scheduled_utc, status, attempt_count, credits_consumed, provider_message_id, created_utc)
            VALUES (@id, @brand, 'appointment.reminder', 'Transactional', 'Phone', '+905550000001', '{Sms}', 'Sms', '{}', 'metin',
               @now, 'Sent', @attempts, 1, @provider, @now);
            INSERT INTO notifications.credit_transactions (id, brand_id, type, amount, balance_after, created_utc)
            VALUES ('0193a8f0-0000-7000-8000-0000000000c1', @brand, 'Grant', 10, 10, @now),
                   ('0193a8f0-0000-7000-8000-0000000000c2', @brand, 'Consume', -1, 9, @now);
            """, connection);
        command.Parameters.AddWithValue("id", MessageId);
        command.Parameters.AddWithValue("brand", Brand);
        command.Parameters.AddWithValue("now", _factory.Clock.GetCurrentInstant().ToDateTimeUtc());
        command.Parameters.AddWithValue("attempts", attempts);
        command.Parameters.AddWithValue("provider", ProviderId);
        await command.ExecuteNonQueryAsync(Ct);
    }

    private Task<string> StateAsync() =>
        ScalarAsync<string>("SELECT status || ':' || attempt_count || ':' || (delivered_utc IS NOT NULL) FROM notifications.notification_outbox");

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
