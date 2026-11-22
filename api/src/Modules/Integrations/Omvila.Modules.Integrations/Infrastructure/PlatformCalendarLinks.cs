using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Integrations.Contracts;
using Omvila.Modules.Integrations.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Integrations.Infrastructure;

internal sealed class PlatformCalendarLinks(IntegrationsDbContext db, ITenantContext tenant) : IPlatformCalendarLinks
{
    public Task<int> CountConnectedStaffAsync(Guid brandId, CancellationToken ct)
    {
        if (!tenant.Scope.IsPlatform)
        {
            throw new DomainException("Platform servisi yalnız platform bağlamında çağrılır (ADR-0006).");
        }

        return db.CalendarLinks.CountAsync(l => l.BrandId == brandId && l.Status == CalendarLinkStatus.Active, ct);
    }
}
