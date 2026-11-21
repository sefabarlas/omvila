using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

internal sealed record ReviewReplyView(
    string Text,
    Instant RepliedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? RepliedByName);
