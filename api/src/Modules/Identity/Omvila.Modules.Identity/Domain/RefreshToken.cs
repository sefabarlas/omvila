using NodaTime;

namespace Omvila.Modules.Identity.Domain;

/// <summary>
/// Yenileme jetonu (alan-modeli.md § RefreshToken, ADR-0007). Yalnız SHA-256 özeti saklanır; her kullanımda
/// döndürülür. Kullanılmış jeton yeniden gelirse aynı ailedeki (FamilyId) bütün jetonlar iptal edilir.
/// brand_id yoktur: kullanıcıya aittir ve yenileme ucu kiracı bağlamı olmadan çalışır.
/// </summary>
internal sealed class RefreshToken
{
    public const string ManageAudience = "manage";

    public const string MeAudience = "me";

    /// <summary>Platform paneli (aud=admin): jeton gövdede taşınır, üyelik yoktur (T-116 (1)).</summary>
    public const string AdminAudience = "admin";

    private RefreshToken()
    {
    }

    public Guid Id { get; private set; }

    /// <summary>Oturum sahibi: salon/platform kullanıcısı veya (yüzey me ise) global müşteri kaydı (alan-modeli.md § CustomerSession).</summary>
    public Guid UserId { get; private set; }

    /// <summary>Jetonun yüzeyi (manage, me veya admin): bir yüzeyin yenileme jetonu diğerinde geçmez (ADR-0007).</summary>
    public string Audience { get; private set; } = ManageAudience;

    /// <summary>manage jetonunun üyeliği; platform kullanıcısında boş.</summary>
    public Guid? MembershipId { get; private set; }

    public Guid FamilyId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public RefreshClientType ClientType { get; private set; }

    public bool RememberDevice { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant ExpiresUtc { get; private set; }

    public Instant? UsedUtc { get; private set; }

    public Instant? RevokedUtc { get; private set; }

    public string? RevokedReason { get; private set; }

    public static RefreshToken Create(
        Guid id, Guid userId, Guid? membershipId, Guid familyId, string tokenHash,
        RefreshClientType clientType, bool rememberDevice, Instant now, Duration lifetime, string audience = ManageAudience) => new()
        {
            Id = id,
            UserId = userId,
            Audience = audience,
            MembershipId = membershipId,
            FamilyId = familyId,
            TokenHash = tokenHash,
            ClientType = clientType,
            RememberDevice = rememberDevice,
            CreatedUtc = now,
            ExpiresUtc = now + lifetime,
        };

    public bool IsUsable(Instant now) => UsedUtc is null && RevokedUtc is null && now < ExpiresUtc;

    public void MarkUsed(Instant now) => UsedUtc = now;

    public void Revoke(Instant now, string reason)
    {
        if (RevokedUtc is null)
        {
            RevokedUtc = now;
            RevokedReason = reason;
        }
    }
}
