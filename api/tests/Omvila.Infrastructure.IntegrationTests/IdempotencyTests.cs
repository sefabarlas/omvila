using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

// Kural: api-kurallari.md § Idempotency
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class IdempotencyTests(PostgresFixture db) : IAsyncLifetime
{
    private static int _keySequence;

    private WebApplication _app = null!;
    private HttpClient _client = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await InfraTables.ResetAsync(db, Ct);
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand(
                """
                TRUNCATE infra.idempotency_keys;
                DROP SCHEMA IF EXISTS idem_probe CASCADE;
                CREATE SCHEMA idem_probe;
                GRANT USAGE ON SCHEMA idem_probe TO omvila_app;
                CREATE TABLE idem_probe.orders (id serial PRIMARY KEY, note text NOT NULL);
                GRANT SELECT, INSERT ON idem_probe.orders TO omvila_app;
                GRANT USAGE ON SEQUENCE idem_probe.orders_id_seq TO omvila_app;
                """,
                connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        _app = await TestApiHost.StartAsync(db, MapEndpoints);
        _client = _app.GetTestClient();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }

    [Fact]
    public async Task MissingKey_Returns400Required()
    {
        using var response = await PostAsync(key: null, note: "a");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(Ct)).ShouldContain("\"Idempotency-Key\":[\"required\"]");
    }

    [Fact]
    public async Task NonUuidKey_Returns400InvalidFormat()
    {
        using var response = await PostAsync(key: "abc", note: "a");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync(Ct)).ShouldContain("invalid_format");
    }

    [Fact]
    public async Task SameKeyAndBody_ReplaysFirstResponse_WithoutRepeatingSideEffect()
    {
        var key = NewKey();

        using var first = await PostAsync(key, "Saç kesimi");
        using var second = await PostAsync(key, "Saç kesimi");

        first.StatusCode.ShouldBe(HttpStatusCode.Created);
        second.StatusCode.ShouldBe(HttpStatusCode.Created);
        (await second.Content.ReadAsStringAsync(Ct)).ShouldBe(await first.Content.ReadAsStringAsync(Ct));
        second.Headers.Location.ShouldBe(first.Headers.Location);
        second.Headers.Contains(IdempotencyEndpointFilter.ReplayedHeader).ShouldBeTrue();
        first.Headers.Contains(IdempotencyEndpointFilter.ReplayedHeader).ShouldBeFalse();
        (await OrderCountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task SameKeyDifferentBody_Returns422KeyReused()
    {
        var key = NewKey();
        using var first = await PostAsync(key, "Saç kesimi");

        using var second = await PostAsync(key, "Boya");

        second.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await second.Content.ReadAsStringAsync(Ct)).ShouldContain("\"code\":\"idempotency_key_reused\"");
        (await OrderCountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task FailedFirstAttempt_IsNotStored_AndRetryWithSameKeyRuns()
    {
        var key = NewKey();

        using var failed = await PostAsync(key, "fail");
        failed.StatusCode.ShouldBe(HttpStatusCode.Conflict);

        // Aynı anahtar, aynı gövde: hata saklanmadığı için yeniden çalışır (ve bu kez yine aynı iş kuralıyla döner).
        using var retried = await PostAsync(key, "fail");
        retried.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        retried.Headers.Contains(IdempotencyEndpointFilter.ReplayedHeader).ShouldBeFalse();
        (await OrderCountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task ConcurrentRequestsWithSameKey_RunSideEffectOnce()
    {
        var key = NewKey();

        var responses = await Task.WhenAll(
            Enumerable.Range(0, 4).Select(_ => PostAsync(key, "slow")));

        try
        {
            responses.ShouldAllBe(r => r.StatusCode == HttpStatusCode.Created);
            var bodies = await Task.WhenAll(responses.Select(r => r.Content.ReadAsStringAsync(Ct)));
            bodies.Distinct(StringComparer.Ordinal).Count().ShouldBe(1);
            (await OrderCountAsync()).ShouldBe(1);
        }
        finally
        {
            foreach (var response in responses)
            {
                response.Dispose();
            }
        }
    }

    [Fact]
    public async Task PurgeJob_DeletesRecordsOlderThanRetention()
    {
        using (await PostAsync(NewKey(), "eski")) { }

        var clock = (FakeClock)_app.Services.GetRequiredService<IClock>();
        clock.Advance(Duration.FromHours(25));
        using (await PostAsync(NewKey(), "yeni")) { }

        (await _app.Services.GetRequiredService<JobRunner>().RunOnceAsync(typeof(PurgeIdempotencyKeys), Ct))
            .ShouldBe(JobOutcome.Succeeded);

        (await InfraTables.ScalarAsync<long>(db, "SELECT count(*) FROM infra.idempotency_keys", Ct)).ShouldBe(1);
    }

    private async Task<HttpResponseMessage> PostAsync(string? key, string note)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/test/orders")
        {
            Content = JsonContent.Create(new { note }),
        };
        if (key is not null)
        {
            request.Headers.Add(IdempotencyEndpointFilter.HeaderName, key);
        }

        return await _client.SendAsync(request, Ct);
    }

    // Testlerde sabit, sıralı anahtar (Guid.NewGuid yasak; BannedSymbols.txt).
    private static string NewKey() =>
        $"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _keySequence):D12}";

    private Task<long> OrderCountAsync() => InfraTables.ScalarAsync<long>(db, "SELECT count(*) FROM idem_probe.orders", Ct);

    private sealed record CreateOrder(string Note);

    private static void MapEndpoints(RouteGroupBuilder api) =>
        api.MapPost("/orders", async (CreateOrder body, IDbSession session, CancellationToken ct) =>
        {
            if (string.Equals(body.Note, "fail", StringComparison.Ordinal))
            {
                await Insert(session, body.Note, ct);
                return Error.Conflict(ErrorCodes.SlotTaken).ToProblem();
            }

            if (string.Equals(body.Note, "slow", StringComparison.Ordinal))
            {
                await using var sleep = new NpgsqlCommand("SELECT pg_sleep(0.3)", session.Connection, session.Transaction);
                await sleep.ExecuteNonQueryAsync(ct);
            }

            var id = await Insert(session, body.Note, ct);
            return TypedResults.Created($"/api/test/orders/{id}", new { id, body.Note });
        }).RequireIdempotencyKey();

    private static async Task<int> Insert(IDbSession session, string note, CancellationToken ct)
    {
        await using var command = new NpgsqlCommand(
            "INSERT INTO idem_probe.orders (note) VALUES ($1) RETURNING id", session.Connection, session.Transaction);
        command.Parameters.Add(new NpgsqlParameter { Value = note });
        return (int)(await command.ExecuteScalarAsync(ct))!;
    }
}
