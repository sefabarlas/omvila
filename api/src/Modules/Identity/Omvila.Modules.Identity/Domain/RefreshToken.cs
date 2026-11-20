using NodaTime;

namespace Omvila.Modules.Identity.Domain;

/// <summary>
/// Yenileme jetonu (alan-modeli.md § RefreshToken, ADR-0007). Yalnız SHA-256 özeti saklanır; her kullanımda
/// döndürülür. Kullanılmış jeton yeniden gelirse aynı ailedeki (FamilyId) bütün jetonlar iptal edilir.
/// brand_id yoktur: kullanıcıya aittir ve yenileme ucu kiracı bağlamı olmadan çalışır.
/// </summary>
internal sealed class RefreshToken
{
    private RefreshToken()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

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
        RefreshClientType clientType, bool rememberDevice, Instant now, Duration lifetime) => new()
        {
            Id = id,
            UserId = userId,
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
