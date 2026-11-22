namespace Omvila.Modules.Reviews.Features.MeReviews;

/// <summary>Sözleşme MeReviewCreate (Degerlendir gövdesi).</summary>
internal sealed record MeReviewCreate(Guid? AppointmentId, int? Rating, string? Comment);
