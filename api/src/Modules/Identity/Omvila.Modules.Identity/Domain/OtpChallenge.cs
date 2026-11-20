using NodaTime;

namespace Omvila.Modules.Identity.Domain;

/// <summary>
/// SMS doğrulama kodu (alan-modeli.md § OtpChallenge, ADR-0007). Kod düz metin saklanmaz: HMAC-SHA256
/// (anahtar + amaç + telefon + kod). Tek kullanımlık; deneme sınırı aşılınca telefon kilitlenir.
/// brand_id yoktur: kod telefona aittir, istek kiracı bağlamı olmadan gelir.
/// </summary>
internal sealed class OtpChallenge
{
    private OtpChallenge()
    {
    }

    public Guid Id { get; private set; }

    public string PhoneE164 { get; private set; } = string.Empty;

    public OtpPurpose Purpose { get; private set; }

    public string CodeHash { get; private set; } = string.Empty;

    public Instant CreatedUtc { get; private set; }

    public Instant ExpiresUtc { get; private set; }

    public int AttemptCount { get; private set; }

    public Instant? ConsumedUtc { get; private set; }

    /// <summary>Deneme sınırı aşıldıysa bu zamana kadar aynı telefona yeni kod verilmez.</summary>
    public Instant? LockedUntilUtc { get; private set; }

    public static OtpChallenge Create(Guid id, string phoneE164, OtpPurpose purpose, string codeHash, Instant now, Duration ttl) => new()
    {
        Id = id,
        PhoneE164 = phoneE164,
        Purpose = purpose,
        CodeHash = codeHash,
        CreatedUtc = now,
        ExpiresUtc = now + ttl,
    };
}
