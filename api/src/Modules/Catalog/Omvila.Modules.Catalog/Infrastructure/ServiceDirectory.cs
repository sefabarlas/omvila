using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Catalog.Contracts;

namespace Omvila.Modules.Catalog.Infrastructure;

internal sealed class ServiceDirectory(CatalogDbContext db) : IServiceDirectory
{
    public async Task<IReadOnlyDictionary<Guid, ServiceSummary>> GetSummariesAsync(
        Guid brandId, IReadOnlyCollection<Guid> serviceIds, CancellationToken ct)
    {
        if (serviceIds.Count == 0)
        {
            return new Dictionary<Guid, ServiceSummary>();
        }

        return await db.Services.AsNoTracking()
            .Where(s => s.BrandId == brandId && serviceIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => new ServiceSummary(s.Id, s.Name, s.IsActive), ct);
    }
}
