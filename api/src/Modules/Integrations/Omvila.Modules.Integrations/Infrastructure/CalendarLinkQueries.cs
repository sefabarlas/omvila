using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Integrations.Contracts;

namespace Omvila.Modules.Integrations.Infrastructure;

internal sealed class CalendarLinkQueries(IntegrationsDbContext db) : ICalendarLinkQueries
{
    public async Task<IReadOnlyDictionary<Guid, string>> GetStatusesAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct) =>
        await db.CalendarLinks.AsNoTracking()
            .Where(l => l.BrandId == brandId && staffIds.Contains(l.StaffId))
            .ToDictionaryAsync(l => l.StaffId, l => l.Status.ToString(), ct);
}
