using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

/// <summary>Sözleşme CustomerReviewPage.</summary>
internal sealed record CustomerReviewPage(
    IReadOnlyList<ReviewView> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);
