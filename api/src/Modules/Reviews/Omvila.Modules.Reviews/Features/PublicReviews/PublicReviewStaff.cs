using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.PublicReviews;

internal sealed record PublicReviewStaff(string Name, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? PhotoUrl);
