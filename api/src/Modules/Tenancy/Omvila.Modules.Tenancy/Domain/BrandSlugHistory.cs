using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Eski bağlantı adı ve yönlendirme bitişi (alan-modeli.md § Brand: 30 gün; SlugAvailability).</summary>
internal sealed class BrandSlugHistory
{
    private BrandSlugHistory()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public string Slug { get; private set; } = string.Empty;

    public Instant ChangedUtc { get; private set; }

    public Instant RedirectExpiresUtc { get; private set; }

    public static BrandSlugHistory Record(Guid id, Guid brandId, string oldSlug, Instant changedUtc) => new()
    {
        Id = id,
        BrandId = brandId,
        Slug = oldSlug,
        ChangedUtc = changedUtc,
        RedirectExpiresUtc = SlugAvailability.RedirectExpiry(changedUtc),
    };
}
