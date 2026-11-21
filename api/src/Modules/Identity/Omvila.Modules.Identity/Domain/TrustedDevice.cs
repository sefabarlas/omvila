using NodaTime;

namespace Omvila.Modules.Identity.Domain;

/// <summary>
/// Rezervasyon sayfasında güvenilir cihaz (alan-modeli.md § TrustedDevice, ADR-0007): telefon bir kez SMS koduyla
/// doğrulandıktan sonra aynı tarayıcıdan aynı telefonla gelindiğinde kod istenmez. Tarayıcıdaki çerez 256 bit rastgele
/// değerdir; yalnız SHA-256 özeti saklanır. brand_id yoktur: cihaz telefona aittir, markadan bağımsızdır (OtpChallenge gibi).
/// Süre sabittir (TrustedDeviceDays), kullanıldıkça uzamaz.
/// </summary>
internal sealed class TrustedDevice
{
    private TrustedDevice()
    {
    }

    public Guid Id { get; private set; }

    public string PhoneE164 { get; private set; } = string.Empty;

    public string TokenHash { get; private set; } = string.Empty;

    /// <summary>Cihazı güvenilir yapan doğrulama (rezervasyon jetonunun otp_id iddiası).</summary>
    public Guid VerifiedOtpId { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant ExpiresUtc { get; private set; }

    public Instant? LastUsedUtc { get; private set; }

    public static TrustedDevice Create(Guid id, string phoneE164, string tokenHash, Guid verifiedOtpId, Instant now, Duration lifetime) => new()
    {
        Id = id,
        PhoneE164 = phoneE164,
        TokenHash = tokenHash,
        VerifiedOtpId = verifiedOtpId,
        CreatedUtc = now,
        ExpiresUtc = now + lifetime,
    };

    public bool IsUsableFor(string phoneE164, Instant now) =>
        now < ExpiresUtc && string.Equals(PhoneE164, phoneE164, StringComparison.Ordinal);

    public void MarkUsed(Instant now) => LastUsedUtc = now;
}
