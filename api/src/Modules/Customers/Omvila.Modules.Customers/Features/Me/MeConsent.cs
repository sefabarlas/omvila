using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>Sözleşme MeConsent (İzinlerim satırı).</summary>
internal sealed record MeConsent(
    MeConsent.BrandRef Brand,
    bool MarketingGranted,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? UpdatedUtc)
{
    /// <summary>Sözleşme MeBrandRef.</summary>
    internal sealed record BrandRef(Guid Id, string Name, string Slug, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? LogoUrl);
}
