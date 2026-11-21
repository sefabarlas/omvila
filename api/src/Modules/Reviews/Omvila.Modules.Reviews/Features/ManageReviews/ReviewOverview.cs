using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

/// <summary>Sözleşme ReviewOverview.</summary>
internal sealed record ReviewOverview(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? AverageRating,
    int RatingCount,
    int TotalCount,
    int AwaitingReplyCount,
    int ThisMonthCount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? ThisMonthInviteResponsePercent,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] ReviewTopStaff? TopStaff);
