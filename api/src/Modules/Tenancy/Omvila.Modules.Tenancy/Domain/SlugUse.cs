using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Bir bağlantı adının şu anki kullanımı (güncel ad veya yönlendirmesi süren eski ad).</summary>
internal sealed record SlugUse(Guid BrandId, bool IsCurrent, Instant? RedirectExpiresUtc);
