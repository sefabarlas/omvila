using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.PublicReviews;

internal sealed record PublicReviewBrand(string Name, string Slug, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? LogoUrl);
