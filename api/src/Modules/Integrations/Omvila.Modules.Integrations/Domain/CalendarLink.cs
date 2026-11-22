using NodaTime;

namespace Omvila.Modules.Integrations.Domain;

/// <summary>
/// Personelin Google Takvim bağlantısı (alan-modeli.md § CalendarLink; durum-makinesi.md: Active → TokenExpired / Revoked).
/// Personel başına tek satır; yeniden bağlanınca aynı satır yeni jetonlarla Active olur. Jetonlar şifreli saklanır
/// (CalendarTokenProtector, ek veri = bağlantı kimliği); Revoked olunca silinir. <see cref="ConnectNonce"/> son kullanılan
/// state'in tek kullanımlık değeridir (aynı state ikinci kez bağlayamaz).
/// </summary>
internal sealed class CalendarLink
{
    public const string GoogleProvider = "Google";

    private CalendarLink()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid StaffId { get; private set; }

    public Guid UserId { get; private set; }

    public string Provider { get; private set; } = GoogleProvider;

    public string ExternalCalendarId { get; private set; } = string.Empty;

    /// <summary>Bağlı hesabın maskeli e-postası (tam adres saklanmaz).</summary>
    public string? AccountEmailMasked { get; private set; }

    public byte[]? AccessTokenEncrypted { get; private set; }

    public byte[]? RefreshTokenEncrypted { get; private set; }

    public Instant? TokenExpiresUtc { get; private set; }

    public CalendarLinkStatus Status { get; private set; }

    public Instant ConnectedUtc { get; private set; }

    public Instant? LastSyncedUtc { get; private set; }

    public Instant? StatusChangedUtc { get; private set; }

    public Guid ConnectNonce { get; private set; }

    public static CalendarLink Create(Guid id, Guid brandId, Guid staffId, Guid userId) => new()
    {
        Id = id,
        BrandId = brandId,
        StaffId = staffId,
        UserId = userId,
        Status = CalendarLinkStatus.Revoked,
    };

    /// <summary>Bağlantı kuruldu veya yenilendi (callback). Eski aktarımlar yeni takvime yeniden yazılır.</summary>
    public void Connect(Guid userId, Guid nonce, string calendarId, string? accountEmailMasked, byte[] accessToken, byte[] refreshToken, Instant expiresUtc, Instant now)
    {
        UserId = userId;
        ConnectNonce = nonce;
        ExternalCalendarId = calendarId;
        AccountEmailMasked = accountEmailMasked;
        AccessTokenEncrypted = accessToken;
        RefreshTokenEncrypted = refreshToken;
        TokenExpiresUtc = expiresUtc;
        Status = CalendarLinkStatus.Active;
        ConnectedUtc = now;
        StatusChangedUtc = now;
        LastSyncedUtc = null;
    }

    public void RefreshAccess(byte[] accessToken, Instant expiresUtc)
    {
        AccessTokenEncrypted = accessToken;
        TokenExpiresUtc = expiresUtc;
    }

    // Kural: durum-makinesi.md § CalendarLink (Active → TokenExpired: yenilenemedi, "Google Takvim bağlantısı koptu")
    public void MarkTokenExpired(Instant now)
    {
        if (Status != CalendarLinkStatus.Active)
        {
            return;
        }

        Status = CalendarLinkStatus.TokenExpired;
        StatusChangedUtc = now;
    }

    // Kural: durum-makinesi.md § CalendarLink (→ Revoked: kullanıcı bağlantıyı kesti; jetonlar silinir)
    public void Revoke(Instant now)
    {
        Status = CalendarLinkStatus.Revoked;
        AccessTokenEncrypted = null;
        RefreshTokenEncrypted = null;
        TokenExpiresUtc = null;
        StatusChangedUtc = now;
    }

    public void MarkSynced(Instant now) => LastSyncedUtc = now;
}
