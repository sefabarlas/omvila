using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.PublicReviews;

internal sealed record PublicSubmittedReview(int Rating, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Comment, Instant CreatedUtc);
