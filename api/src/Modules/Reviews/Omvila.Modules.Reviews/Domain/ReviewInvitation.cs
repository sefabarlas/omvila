using NodaTime;

namespace Omvila.Modules.Reviews.Domain;

/// <summary>
/// Planlanan değerlendirme daveti (review.invite). Randevu başına bir kez (AppointmentId benzersiz); "Bu ay · davet edilenlerin
/// %38'i" (ReviewOverview.thisMonthInviteResponsePercent) bu kayıtlardan hesaplanır. Gönderimin kendisi bildirim kuyruğundadır
/// (docs/acik-sorular.md T-89).
/// </summary>
internal sealed class ReviewInvitation
{
    private ReviewInvitation()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public Guid AppointmentId { get; private set; }

    /// <summary>Gönderim zamanı (tamamlanma + ReviewInviteDelayMinutes).</summary>
    public Instant SendAtUtc { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public static ReviewInvitation Plan(Guid id, Guid brandId, Guid branchId, Guid appointmentId, Instant sendAtUtc, Instant now) =>
        new()
        {
            Id = id,
            BrandId = brandId,
            BranchId = branchId,
            AppointmentId = appointmentId,
            SendAtUtc = sendAtUtc,
            CreatedUtc = now,
        };
}
