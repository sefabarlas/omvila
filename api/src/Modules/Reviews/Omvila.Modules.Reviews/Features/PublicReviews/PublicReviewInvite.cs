using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.PublicReviews;

/// <summary>Sözleşme PublicReviewInvite ("Kuaför Elif'te nasıldı?").</summary>
internal sealed record PublicReviewInvite(
    PublicReviewBrand Brand,
    string BranchName,
    Instant AppointmentStartUtc,
    string TimeZone,
    IReadOnlyList<string> ServiceNames,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] PublicReviewStaff? Staff,
    string CustomerDisplayName,
    bool AlreadyReviewed,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] PublicSubmittedReview? Review);
