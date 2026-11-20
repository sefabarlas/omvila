namespace Omvila.Modules.Tenancy;

/// <summary>"Tenancy" bölümü.</summary>
public sealed class TenancyOptions
{
    public const string Section = "Tenancy";

    /// <summary>Rezervasyon sayfasının kökü; bağlantı <c>{BookingPageBaseUrl}/{slug}</c> (alan adı S-14 gereği yapılandırmadan).</summary>
    public Uri? BookingPageBaseUrl { get; set; }

    internal bool IsValid() => BookingPageBaseUrl is { IsAbsoluteUri: true };
}
