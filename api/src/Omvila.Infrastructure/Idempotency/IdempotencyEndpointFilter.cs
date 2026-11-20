using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Idempotency;

/// <summary>
/// Kaynak oluşturan ve para kaydeden POST'lar (api-kurallari.md § Idempotency):
/// aynı anahtar + aynı istek → ilk yanıt aynen döner; aynı anahtar + farklı istek → 422 idempotency_key_reused.
/// Uç transaction'ı içinde çalışır: kayıt iş verisiyle birlikte commit olur. Aynı anahtarlı eşzamanlı istekler
/// transaction'a bağlı advisory lock ile sıralanır; ikincisi birincinin commit'ini bekler ve yanıtını alır.
/// Hata sonucu transaction'ı geri aldığı için saklanmaz; aynı anahtarla tekrar denenebilir.
/// </summary>
internal sealed class IdempotencyEndpointFilter(
    IDbSession session, InfraDbContext db, IClock clock, IOptions<JsonOptions> json) : IEndpointFilter
{
    public const string HeaderName = "Idempotency-Key";
    public const string ReplayedHeader = "Idempotent-Replayed";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var http = context.HttpContext;
        var ct = http.RequestAborted;
        var header = http.Request.Headers[HeaderName].ToString();
        if (string.IsNullOrWhiteSpace(header))
        {
            return Error.Validation(HeaderName, "required").ToProblem();
        }

        if (!Guid.TryParse(header, out var key))
        {
            return Error.Validation(HeaderName, "invalid_format").ToProblem();
        }

        var scope = ScopeOf(session.Scope);
        var hash = await HashRequestAsync(http.Request, ct);
        await LockAsync(scope, key, ct);

        var existing = await db.IdempotencyKeys.AsNoTracking()
            .SingleOrDefaultAsync(r => r.Scope == scope && r.Key == key, ct);
        if (existing is not null)
        {
            return string.Equals(existing.RequestHash, hash, StringComparison.Ordinal)
                ? Replay(http, existing)
                : Error.Unprocessable(ErrorCodes.IdempotencyKeyReused).ToProblem();
        }

        var result = await next(context);
        var status = StatusOf(result);
        if (status < 400)
        {
            db.IdempotencyKeys.Add(new IdempotencyRecord
            {
                Scope = scope,
                Key = key,
                RequestHash = hash,
                StatusCode = status,
                ResponseBody = BodyOf(result),
                Location = LocationOf(result),
                CreatedUtc = clock.GetCurrentInstant(),
            });
            await db.SaveChangesAsync(ct);
        }

        return result;
    }

    private static string ScopeOf(TenantScope tenant) => tenant switch
    {
        { BrandId: { } brand } => $"brand:{brand}",
        { CustomerId: { } customer } => $"customer:{customer}",
        { IsPlatform: true } => "platform",
        _ => "anonymous",
    };

    private static async Task<string> HashRequestAsync(HttpRequest request, CancellationToken ct)
    {
        using var sha = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        sha.AppendData(Encoding.UTF8.GetBytes($"{request.Method}\n{request.Path}{request.QueryString}\n"));
        if (request.Body.CanSeek)
        {
            request.Body.Position = 0;
            var buffer = new byte[8192];
            int read;
            while ((read = await request.Body.ReadAsync(buffer, ct)) > 0)
            {
                sha.AppendData(buffer, 0, read);
            }

            request.Body.Position = 0;
        }

        return Convert.ToHexStringLower(sha.GetHashAndReset());
    }

    private async Task LockAsync(string scope, Guid key, CancellationToken ct)
    {
        await using var command = new NpgsqlCommand(
            "SELECT pg_advisory_xact_lock(hashtextextended($1, 0))", session.Connection, session.Transaction);
        command.Parameters.Add(new NpgsqlParameter { Value = $"idempotency:{scope}:{key}" });
        await command.ExecuteNonQueryAsync(ct);
    }

    private static IResult Replay(HttpContext http, IdempotencyRecord record)
    {
        http.Response.Headers[ReplayedHeader] = "true";
        if (record.Location is not null)
        {
            http.Response.Headers.Location = record.Location;
        }

        return record.ResponseBody is null
            ? TypedResults.StatusCode(record.StatusCode)
            : TypedResults.Content(record.ResponseBody, "application/json", Encoding.UTF8, record.StatusCode);
    }

    private static int StatusOf(object? result) =>
        result is IStatusCodeHttpResult { StatusCode: { } status } ? status : StatusCodes.Status200OK;

    private string? BodyOf(object? result) => result switch
    {
        IValueHttpResult { Value: { } value } => JsonSerializer.Serialize(value, value.GetType(), json.Value.SerializerOptions),
        IResult or null => null,
        _ => JsonSerializer.Serialize(result, result.GetType(), json.Value.SerializerOptions),
    };

    // Created / Accepted sonuçlarının Location'ı (ortak arayüz yok; ad sabit).
    private static string? LocationOf(object? result) =>
        result?.GetType().GetProperty("Location")?.GetValue(result) as string;
}
