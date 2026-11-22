using NodaTime;
using Omvila.SharedKernel;

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

    /// <summary>İşletme bilgileri (AyarlarIsletme). Değerler uçta doğrulanmıştır; telefon E.164.</summary>
    public void UpdateProfile(BrandProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        Name = profile.Name.Trim();
        IndustryCode = profile.IndustryCode;
        ShortDescription = profile.ShortDescription;
        Phone = profile.Phone;
        Email = profile.Email;
        Instagram = profile.Instagram;
        Website = profile.Website;
        LegalName = profile.LegalName;
        TaxOffice = profile.TaxOffice;
        TaxNumber = profile.TaxNumber;
    }

    /// <summary>Bağlantı adı değişir; eski ad yönlendirme için geçmişe yazılır (aynı adsa null).</summary>
    public BrandSlugHistory? ChangeSlug(Slug slug, Guid historyId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(slug);
        if (string.Equals(Slug, slug.Value, StringComparison.Ordinal))
        {
            return null;
        }

        var history = BrandSlugHistory.Record(historyId, Id, Slug, now);
        Slug = slug.Value;
        return history;
    }

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

    /// <summary>
    /// Platformun askıya alması veya yeniden açması (durum-makinesi.md § 10: Active ↔ Suspended; sözleşme adminChangeBrandStatus).
    /// Onboarding veya Archived markada ya da durum zaten istenen değerdeyse 409 invalid_state_transition.
    /// </summary>
    public Result ChangeStatusByPlatform(BrandStatus target)
    {
        var allowed = (Status, target) is (BrandStatus.Active, BrandStatus.Suspended) or (BrandStatus.Suspended, BrandStatus.Active);
        if (!allowed)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        Status = target;
        return Result.Success();
    }

    /// <summary>Eşzamanlı düzenleme sürümü (PostgreSQL xmin; If-Match/ETag).</summary>
    public uint Version { get; private set; }

    /// <summary>Kayıtta: durum Onboarding; kurulum adımları bitince Active (dilim-01).</summary>
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
