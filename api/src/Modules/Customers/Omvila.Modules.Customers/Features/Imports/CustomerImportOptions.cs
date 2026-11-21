namespace Omvila.Modules.Customers.Features.Imports;

/// <summary>
/// İçe aktarma sınırları (appsettings "Customers:Import"; sözleşme managePreviewImport: "sınırlar yapılandırmadadır"). Teknik
/// ayardır, ürün süresi değildir.
/// </summary>
internal sealed class CustomerImportOptions
{
    public const string Section = "Customers:Import";

    /// <summary>En büyük dosya (bayt).</summary>
    public int MaxFileBytes { get; set; } = 2 * 1024 * 1024;

    /// <summary>Başlık hariç en çok satır.</summary>
    public int MaxRows { get; set; } = 5000;

    /// <summary>Önizlemenin uygulanabilir kaldığı süre (dakika).</summary>
    public int PreviewTtlMinutes { get; set; } = 60;

    public bool IsValid() => MaxFileBytes > 0 && MaxRows > 0 && PreviewTtlMinutes > 0;
}
