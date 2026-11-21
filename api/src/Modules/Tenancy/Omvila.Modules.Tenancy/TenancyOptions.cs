namespace Omvila.Modules.Tenancy;

/// <summary>"Tenancy" bölümü.</summary>
public sealed class TenancyOptions
{
    public const string Section = "Tenancy";

    /// <summary>Rezervasyon sayfasının kökü; bağlantı <c>{BookingPageBaseUrl}/{slug}</c> (alan adı S-14 gereği yapılandırmadan).</summary>
    public Uri? BookingPageBaseUrl { get; set; }

    /// <summary>
    /// IBAN şifreleme anahtarı (AES-256-GCM; base64, 32 bayt). Gizli değer: kasadan/ortam değişkeninden
    /// (<c>Tenancy__IbanEncryptionKey</c>). Development ve Testing'de boşsa geçici anahtar; Staging/Production'da zorunlu.
    /// </summary>
    public string? IbanEncryptionKey { get; set; }

    internal bool IsValid() => BookingPageBaseUrl is { IsAbsoluteUri: true };
}
