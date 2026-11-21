using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Reviews.Domain;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Infrastructure;

/// <summary>
/// <see cref="IReviewInvitations"/>: tamamlanan randevu için daveti kaydeder, /d/ kodunu Scheduling'den alır ve
/// <see cref="ReviewInviteScheduled"/> yayımlar (metin, kanal planı ve müşterinin SMS tercihi Notifications'ta).
/// </summary>
internal sealed class ReviewInvitations(
    IDbSession session,
    ReviewsDbContext db,
    IAppointmentReviewLinks appointments,
    ITenantQueries tenants,
    IOutbox outbox,
    IIdGenerator ids,
    IClock clock) : IReviewInvitations
{
    // Kural: durum-makinesi.md A11 (değerlendirme daveti planlanır); sureler-ve-ayarlar.md ReviewInviteEnabled, ReviewInviteDelayMinutes
    public async Task<bool> ScheduleAsync(Guid brandId, Guid appointmentId, Instant completedUtc, CancellationToken ct)
    {
        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var settings = await tenants.GetNotificationSettingsAsync(brandId, ct);
        if (settings is not { ReviewInviteEnabled: true })
        {
            return false;
        }

        var subject = await appointments.GetSubjectAsync(brandId, appointmentId, ct);
        if (subject is not { IsCompleted: true }
            || await db.Invitations.AnyAsync(i => i.BrandId == brandId && i.AppointmentId == appointmentId, ct)
            || await db.Reviews.AnyAsync(r => r.BrandId == brandId && r.AppointmentId == appointmentId, ct))
        {
            return false;
        }

        var code = await appointments.IssueReviewCodeAsync(brandId, appointmentId, ct);
        if (code is null)
        {
            return false;
        }

        var sendAt = completedUtc + Duration.FromMinutes(PlatformDefaults.ReviewInviteDelayMinutes);
        db.Invitations.Add(ReviewInvitation.Plan(ids.New(), brandId, subject.BranchId, appointmentId, sendAt, clock.GetCurrentInstant()));
        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(new ReviewInviteScheduled(brandId, appointmentId, code, sendAt), ct);
        return true;
    }
}
