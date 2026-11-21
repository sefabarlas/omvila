using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Reviews.Features.PublicReviews;

/// <summary>Sözleşme PublicReviewCreate.</summary>
internal sealed record PublicReviewCreate(int? Rating, string? Comment);
