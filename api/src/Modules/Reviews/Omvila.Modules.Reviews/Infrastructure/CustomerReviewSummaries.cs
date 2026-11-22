using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Reviews.Contracts;

namespace Omvila.Modules.Reviews.Infrastructure;

/// <summary><see cref="ICustomerReviewSummaries"/>: değerlendirmeler marka RLS'indedir; platform bağlamında randevu kimlikleriyle süzülür.</summary>
internal sealed class CustomerReviewSummaries(ReviewsDbContext db, IDbSession session) : ICustomerReviewSummaries
{
    public async Task<IReadOnlyDictionary<Guid, AppointmentReviewSummary>> GetByAppointmentsAsync(
        IReadOnlyCollection<Guid> appointmentIds, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(appointmentIds);
        if (appointmentIds.Count == 0)
        {
            return new Dictionary<Guid, AppointmentReviewSummary>();
        }

        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await db.Reviews.AsNoTracking()
                .Where(r => appointmentIds.Contains(r.AppointmentId))
                .Select(r => new AppointmentReviewSummary(r.AppointmentId, r.Rating, r.CreatedUtc))
                .ToDictionaryAsync(r => r.AppointmentId, ct);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
