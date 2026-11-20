using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

// Kural: kodlama-dotnet.md § Hata eşleme, api-kurallari.md § Hatalar ve § Eşzamanlı düzenleme
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class HttpPipelineTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly string[] Alternatives = ["2026-11-25T11:45:00Z"];

    private WebApplication _app = null!;
    private HttpClient _client = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand(
                """
                DROP SCHEMA IF EXISTS http_probe CASCADE;
                CREATE SCHEMA http_probe;
                GRANT USAGE ON SCHEMA http_probe TO omvila_app;
                CREATE TABLE http_probe.writes (id int PRIMARY KEY);
                CREATE TABLE http_probe.slots (during tstzrange NOT NULL, EXCLUDE USING gist (during WITH &&));
                CREATE TABLE http_probe.phones (phone text CONSTRAINT ix_probe_phone UNIQUE);
                CREATE TABLE http_probe.others (value text CONSTRAINT ix_probe_other UNIQUE);
                GRANT SELECT, INSERT ON ALL TABLES IN SCHEMA http_probe TO omvila_app;
                """,
                connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(db.Settings());
        builder.Services.AddSingleton<IClock>(TestClock.Create());
        builder.Services.AddSingleton<IIdGenerator, UuidV7IdGenerator>();
        builder.Services.AddOmvilaProblemDetails();
        builder.Services.AddOmvilaInfrastructure(builder.Configuration);
        builder.Services.AddUniqueConstraintError("ix_probe_phone", ErrorCodes.PhoneAlreadyExists);

        _app = builder.Build();
        _app.UseExceptionHandler();
        _app.UseStatusCodePages();
        MapProbeEndpoints(_app.MapApiGroup("/api/test"));
        await _app.StartAsync(Ct);
        _client = _app.GetTestClient();
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }

    [Theory]
    [InlineData("validation", 400, "validation_failed")]
    [InlineData("forbidden", 403, "forbidden")]
    [InlineData("not-found", 404, "not_found")]
    [InlineData("conflict", 409, "slot_taken")]
    [InlineData("precondition", 412, "concurrency_conflict")]
    [InlineData("unprocessable", 422, "otp_invalid")]
    [InlineData("too-many", 429, "otp_resend_too_soon")]
    public async Task ErrorResult_MapsToStatusAndContractCode(string kind, int status, string code)
    {
        var (actualStatus, body) = await SendAsync(HttpMethod.Get, $"/api/test/errors/{kind}");

        actualStatus.ShouldBe(status);
        body.GetProperty("code").GetString().ShouldBe(code);
        body.GetProperty("type").GetString().ShouldBe($"https://docs.omvila.com/errors/{code}");
        body.GetProperty("traceId").GetString().ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task ValidationError_CarriesFieldErrors_AndConflictCarriesMeta()
    {
        var (_, validation) = await SendAsync(HttpMethod.Get, "/api/test/errors/validation");
        var (_, conflict) = await SendAsync(HttpMethod.Get, "/api/test/errors/conflict");

        validation.GetProperty("errors").GetProperty("businessName")[0].GetString().ShouldBe("required");
        conflict.GetProperty("meta").GetProperty("alternatives")[0].GetString().ShouldBe("2026-11-25T11:45:00Z");
    }

    [Fact]
    public async Task IfMatch_Missing_Returns428()
    {
        var (status, body) = await SendAsync(HttpMethod.Put, "/api/test/items");

        status.ShouldBe(428);
        body.GetProperty("code").GetString().ShouldBe("precondition_required");
    }

    [Fact]
    public async Task IfMatch_Malformed_Returns400()
    {
        var (status, body) = await SendAsync(HttpMethod.Put, "/api/test/items", ifMatch: "W/\"abc\"");

        status.ShouldBe(400);
        body.GetProperty("errors").GetProperty("If-Match")[0].GetString().ShouldBe("invalid_format");
    }

    [Fact]
    public async Task IfMatch_WeakETag_ReachesHandlerAsVersion()
    {
        var (status, body) = await SendAsync(HttpMethod.Put, "/api/test/items", ifMatch: "W/\"8812\"");

        status.ShouldBe(200);
        body.GetProperty("version").GetUInt32().ShouldBe(8812u);
    }

    [Fact]
    public async Task SuccessfulWrite_IsCommitted_AndErrorResultRollsBack()
    {
        (await SendAsync(HttpMethod.Post, "/api/test/writes/1/ok")).Status.ShouldBe(201);
        (await SendAsync(HttpMethod.Post, "/api/test/writes/2/conflict")).Status.ShouldBe(409);

        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand("SELECT array_agg(id ORDER BY id) FROM http_probe.writes", connection);
        ((int[])(await command.ExecuteScalarAsync(Ct))!).ShouldBe([1]);
    }

    [Theory]
    [InlineData("exclusion", 409, "slot_taken")]
    [InlineData("unique-registered", 409, "phone_already_exists")]
    [InlineData("concurrency", 412, "concurrency_conflict")]
    [InlineData("unique-unregistered", 500, "try_again")]
    public async Task DatabaseException_MapsToContractCode(string kind, int status, string code)
    {
        var (actualStatus, body) = await SendAsync(HttpMethod.Post, $"/api/test/db-errors/{kind}");

        actualStatus.ShouldBe(status);
        body.GetProperty("code").GetString().ShouldBe(code);
        body.TryGetProperty("detail", out _).ShouldBeFalse("iç hata ayrıntısı istemciye gitmez");
    }

    private async Task<(int Status, JsonElement Body)> SendAsync(HttpMethod method, string path, string? ifMatch = null)
    {
        using var request = new HttpRequestMessage(method, path);
        if (ifMatch is not null)
        {
            request.Headers.TryAddWithoutValidation("If-Match", ifMatch);
        }

        using var response = await _client.SendAsync(request, Ct);
        var text = await response.Content.ReadAsStringAsync(Ct);
        var body = text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone();
        return ((int)response.StatusCode, body);
    }

    private static void MapProbeEndpoints(RouteGroupBuilder api)
    {
        api.MapGet("/errors/{kind}", (string kind) => (kind switch
        {
            "validation" => Error.Validation("businessName", "required"),
            "forbidden" => Error.Forbidden(),
            "not-found" => Error.NotFound(),
            "conflict" => Error.Conflict(ErrorCodes.SlotTaken, Meta("alternatives", Alternatives)),
            "precondition" => Error.PreconditionFailed(),
            "unprocessable" => Error.Unprocessable(ErrorCodes.OtpInvalid, Meta("attemptsLeft", 3)),
            _ => Error.TooManyRequests(ErrorCodes.OtpResendTooSoon, Meta("retryAfterSeconds", 42)),
        }).ToProblem());

        api.MapPut("/items", (HttpContext context) => TypedResults.Ok(new { version = context.GetIfMatchVersion() }))
            .RequireIfMatch();

        api.MapPost("/writes/{id:int}/{outcome}", async (int id, string outcome, IDbSession session, CancellationToken ct) =>
        {
            await ExecuteAsync(session, $"INSERT INTO http_probe.writes VALUES ({id})", ct);
            Result<int> result = string.Equals(outcome, "ok", StringComparison.Ordinal) ? id : Error.Conflict(ErrorCodes.SlotTaken);
            return result.ToHttp(value => TypedResults.Created($"/api/test/writes/{value}", new { id = value }));
        });

        api.MapPost("/db-errors/{kind}", async (string kind, IDbSession session, CancellationToken ct) =>
        {
            switch (kind)
            {
                case "exclusion":
                    await ExecuteAsync(session, "INSERT INTO http_probe.slots VALUES (tstzrange('2026-11-25 11:00Z', '2026-11-25 12:00Z')), (tstzrange('2026-11-25 11:30Z', '2026-11-25 12:30Z'))", ct);
                    break;
                case "unique-registered":
                    await ExecuteAsync(session, "INSERT INTO http_probe.phones VALUES ('+905550000001'), ('+905550000001')", ct);
                    break;
                case "unique-unregistered":
                    await ExecuteAsync(session, "INSERT INTO http_probe.others VALUES ('x'), ('x')", ct);
                    break;
                default:
                    throw new DbUpdateConcurrencyException("deneme");
            }

            return TypedResults.Ok();
        });
    }

    private static Dictionary<string, object?> Meta(string key, object value) =>
        new(StringComparer.Ordinal) { [key] = value };

    private static async Task ExecuteAsync(IDbSession session, string sql, CancellationToken ct)
    {
        await using var command = new NpgsqlCommand(sql, session.Connection, session.Transaction);
        await command.ExecuteNonQueryAsync(ct);
    }
}
