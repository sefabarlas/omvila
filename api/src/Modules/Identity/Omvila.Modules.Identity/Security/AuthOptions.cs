namespace Omvila.Modules.Identity.Security;

/// <summary>appsettings "Auth" bölümü (ADR-0007). İmza anahtarları kasadan/ortam değişkeninden gelir, depoda yoktur.</summary>
public sealed class AuthOptions
{
    public const string Section = "Auth";

    public string Issuer { get; set; } = string.Empty;

    /// <summary>Erişim jetonu ömrü (ADR-0007: 15 dk).</summary>
    public int AccessTokenMinutes { get; set; }

    /// <summary>ES256 imza anahtarları. İlki imzalar; hepsi doğrular (döndürme sırasında eski jetonlar geçer).</summary>
    public IList<SigningKeyOptions> SigningKeys { get; } = [];

    internal bool IsValid() => !string.IsNullOrWhiteSpace(Issuer) && AccessTokenMinutes > 0;
}
