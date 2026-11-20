namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Rezervasyon sayfası görünümü (alan-modeli.md § BookingPageSettings; tek satır, BrandId PK).</summary>
internal sealed class BookingPageSettings
{
    private BookingPageSettings()
    {
    }

    public Guid BrandId { get; private set; }

    /// <summary>petrol / mercan / kum (sözleşme coverColor).</summary>
    public string CoverColorToken { get; private set; } = string.Empty;

    public bool ShowReviews { get; private set; }

    public bool ShowPrices { get; private set; }

    public string? PreAppointmentNote { get; private set; }

    /// <summary>Kayıtta: petrol kapak (tasarımda seçili ilk renk), değerlendirmeler ve fiyatlar açık (AyarlarRezervasyon).</summary>
    /// <summary>Eşzamanlı düzenleme sürümü (PostgreSQL xmin; If-Match/ETag).</summary>
    public uint Version { get; private set; }

    public static BookingPageSettings CreateDefault(Guid brandId) => new()
    {
        BrandId = brandId,
        CoverColorToken = "petrol",
        ShowReviews = true,
        ShowPrices = true,
    };
}
