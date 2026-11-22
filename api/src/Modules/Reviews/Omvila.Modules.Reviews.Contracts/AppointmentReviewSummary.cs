using NodaTime;

namespace Omvila.Modules.Reviews.Contracts;

/// <summary>Gönderilmiş değerlendirmenin özeti (yorum taşımaz).</summary>
/// <param name="AppointmentId">Randevu.</param>
/// <param name="Rating">Puan (1–5).</param>
/// <param name="CreatedUtc">Gönderim anı.</param>
public sealed record AppointmentReviewSummary(Guid AppointmentId, int Rating, Instant CreatedUtc);
