using NodaTime;

namespace Omvila.Infrastructure.Idempotency;

/// <summary>
/// infra.idempotency_keys: (kapsam, anahtar) için ilk başarılı yanıt (docs/sozlesme/api-kurallari.md § Idempotency).
/// Kapsam kiracıdan türer (brand:…, customer:…, platform, anonymous); bir kiracının yanıtı başkasına dönmez.
/// Gövde kişisel veri içerebilir: saklama süresi dolunca PurgeIdempotencyKeys siler.
/// </summary>
internal sealed class IdempotencyRecord
{
    public required string Scope { get; init; }

    public Guid Key { get; init; }

    /// <summary>Yöntem + yol + gövdenin SHA-256 özeti (hex).</summary>
    public required string RequestHash { get; init; }

    public int StatusCode { get; init; }

    public string? ResponseBody { get; init; }

    public string? Location { get; init; }

    public Instant CreatedUtc { get; init; }
}
