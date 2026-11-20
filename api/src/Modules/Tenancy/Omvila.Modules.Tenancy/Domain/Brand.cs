using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Kiracı (alan-modeli.md § Brand). Salon tarafında "İşletme". Sektör sözleşmedeki industryCode ile tutulur.
/// </summary>
internal sealed class Brand
{
    private Brand()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string IndustryCode { get; private set; } = string.Empty;

    public BrandStatus Status { get; private set; }

    public string? ShortDescription { get; private set; }

    public string? LogoUrl { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string? Instagram { get; private set; }

    public string? Website { get; private set; }

    public string? LegalName { get; private set; }

    public string? TaxOffice { get; private set; }

    public string? TaxNumber { get; private set; }

    public string DefaultCurrencyCode { get; private set; } = string.Empty;

    public Instant CreatedUtc { get; private set; }

    /// <summary>Kurulum adımlarının hepsi tamamlanınca (dilim-01): Onboarding → Active. Diğer durumlarda değişmez.</summary>
    public bool CompleteOnboarding()
    {
        if (Status != BrandStatus.Onboarding)
        {
            return false;
        }

        Status = BrandStatus.Active;
        return true;
    }

    /// <summary>Kayıtta: durum Onboarding; kurulum adımları bitince Active (dilim-01).</summary>
    /// <summary>Eşzamanlı düzenleme sürümü (PostgreSQL xmin; If-Match/ETag).</summary>
    public uint Version { get; private set; }

    public static Brand Register(Guid id, string name, Slug slug, string industryCode, string currencyCode, Instant now) => new()
    {
        Id = id,
        Name = name.Trim(),
        Slug = slug.Value,
        IndustryCode = industryCode,
        Status = BrandStatus.Onboarding,
        DefaultCurrencyCode = currencyCode,
        CreatedUtc = now,
    };
}
