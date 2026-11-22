using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Scheduling.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary><see cref="IPlatformAppointmentStats"/>: platform okuma servisi (ADR-0006); başka bağlamda çağrı programlama hatasıdır.</summary>
internal sealed class PlatformAppointmentStats(SchedulingDbContext db, ITenantContext tenant) : IPlatformAppointmentStats
{
    public async Task<IReadOnlyDictionary<Guid, PlatformAppointmentCount>> CountAsync(
        Instant from, Instant to, IReadOnlyCollection<Guid>? brandIds, CancellationToken ct)
    {
        RequirePlatformScope();
        var query = db.Appointments.AsNoTracking()
            .Where(a => a.StartUtc >= from && a.StartUtc < to && a.Status != AppointmentStatus.Draft && a.Status != AppointmentStatus.Cancelled);
        if (brandIds is not null)
        {
            query = query.Where(a => brandIds.Contains(a.BrandId));
        }

        return await query
            .GroupBy(a => a.BrandId)
            .Select(g => new { BrandId = g.Key, Total = g.Count(), Online = g.Count(a => a.Channel == AppointmentChannel.Online) })
            .ToDictionaryAsync(g => g.BrandId, g => new PlatformAppointmentCount(g.Total, g.Online), ct);
    }

    public async Task<IReadOnlySet<Guid>> GetBrandsWithOnlineAppointmentAsync(IReadOnlyCollection<Guid> brandIds, CancellationToken ct)
    {
        RequirePlatformScope();
        return (await db.Appointments.AsNoTracking()
                .Where(a => brandIds.Contains(a.BrandId) && a.Channel == AppointmentChannel.Online && a.Status != AppointmentStatus.Draft)
                .Select(a => a.BrandId)
                .Distinct()
                .ToListAsync(ct))
            .ToHashSet();
    }

    private void RequirePlatformScope()
    {
        if (!tenant.Scope.IsPlatform)
        {
            throw new DomainException("Platform okuma servisi yalnız platform bağlamında çağrılır (ADR-0006).");
        }
    }
}
