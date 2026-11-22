using System.Text.Json.Serialization;

namespace Omvila.Modules.Sales.Features.Me;

/// <summary>Sözleşme MeBrandRef.</summary>
internal sealed record MeBrandRef(Guid Id, string Name, string Slug, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? LogoUrl);
