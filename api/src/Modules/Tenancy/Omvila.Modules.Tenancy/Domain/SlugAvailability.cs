using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>
/// Bağlantı adı değişikliği kuralı (api-kurallari.md slug_unavailable, acik-sorular.md T-11): başka işletmenin güncel
/// adı veya yönlendirmesi süren eski adı alınamaz; işletme kendi eski adına dönebilir; ayrılmış ad alınamaz.
/// </summary>
internal static class SlugAvailability
{
    public static Result Check(Slug candidate, Guid brandId, IEnumerable<SlugUse> uses, Instant now)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(uses);
        var blockedByAnother = uses.Any(use =>
            use.BrandId != brandId && (use.IsCurrent || use.RedirectExpiresUtc > now));
        return blockedByAnother ? Error.Conflict(ErrorCodes.SlugUnavailable) : Result.Success();
    }

    /// <summary>Eski adın yönlendirme bitişi (sureler-ve-ayarlar.md SlugRedirectDays).</summary>
    public static Instant RedirectExpiry(Instant changedUtc) => changedUtc + Duration.FromDays(PlatformDefaults.SlugRedirectDays);
}
