namespace Omvila.Modules.Identity.Security;

/// <summary>appsettings "Auth" bölümü (ADR-0007). İmza anahtarları kasadan/ortam değişkeninden gelir, depoda yoktur.</summary>
public sealed class AuthOptions
{
    public const string Section = "Auth";

    public string Issuer { get; set; } = string.Empty;

    /// <summary>Erişim jetonu ömrü (ADR-0007: 15 dk).</summary>
    public int AccessTokenMinutes { get; set; }

    /// <summary>"Hatırla" seçili panel ve mobil için yenileme jetonu ömrü, gün (sureler-ve-ayarlar.md RefreshTokenLifetimeDays: 30).</summary>
    public int RefreshTokenDaysRemembered { get; set; }

    /// <summary>"Hatırla" seçilmemiş panel oturumu için ömür, gün (RefreshTokenLifetimeDays: 1).</summary>
    public int RefreshTokenDaysSession { get; set; }

    /// <summary>ES256 imza anahtarları. İlki imzalar; hepsi doğrular (döndürme sırasında eski jetonlar geçer).</summary>
    public IList<SigningKeyOptions> SigningKeys { get; } = [];

    internal bool IsValid() =>
        !string.IsNullOrWhiteSpace(Issuer) && AccessTokenMinutes > 0 && RefreshTokenDaysRemembered > 0 && RefreshTokenDaysSession > 0;
}
