using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

/// <summary>Sözleşme ReviewPage.</summary>
internal sealed record ReviewPage(
    IReadOnlyList<ReviewView> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor,
    ReviewOverview Overview);
