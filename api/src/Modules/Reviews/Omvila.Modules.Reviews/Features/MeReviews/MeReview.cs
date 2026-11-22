using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.MeReviews;

/// <summary>Sözleşme MeReview (gönderilen değerlendirme; yorum boşsa null yazılır).</summary>
internal sealed record MeReview(
    Guid Id,
    Guid AppointmentId,
    int Rating,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Comment,
    Instant CreatedUtc);
