namespace Omvila.Modules.Identity.Security;

/// <summary>
/// appsettings "Auth:Otp" (sureler-ve-ayarlar.md: OtpLength 6, OtpTtlSeconds 180, OtpResendCooldownSeconds 60,
/// OtpMaxAttempts 5, OtpLockoutMinutes 5, OtpMaxPerPhonePerHour 5). HMAC anahtarı kasadan; depoda yok.
/// </summary>
public sealed class OtpOptions
{
    public const string Section = "Auth:Otp";

    public int Length { get; set; }

    public int TtlSeconds { get; set; }

    public int ResendCooldownSeconds { get; set; }

    public int MaxAttempts { get; set; }

    public int LockoutMinutes { get; set; }

    public int MaxPerPhonePerHour { get; set; }

    /// <summary>IP başına saatlik OTP isteği sınırı (ADR-0007: "ayrıca IP başına sınır"; değer belgede yok, A-01).</summary>
    public int MaxPerIpPerHour { get; set; }

    /// <summary>Base64, en az 32 bayt. Development'ta yoksa geçici anahtar üretilir.</summary>
    public string HmacKey { get; set; } = string.Empty;

    internal bool IsValid() =>
        Length is >= 4 and <= 8 && TtlSeconds > 0 && ResendCooldownSeconds > 0 && MaxAttempts > 0 && LockoutMinutes > 0 && MaxPerPhonePerHour > 0 && MaxPerIpPerHour > 0;
}
