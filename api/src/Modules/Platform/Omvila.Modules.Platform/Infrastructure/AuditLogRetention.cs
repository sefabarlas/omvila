using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Platform.Contracts;

namespace Omvila.Modules.Platform.Infrastructure;

/// <summary>
/// Uygulama rolünün denetim kaydında yalnız ip_address ve user_agent sütunlarını güncelleme izni vardır (migration
/// AuditLogRetentionGrant); silme ve diğer sütunlar kapalı kalır.
/// </summary>
internal sealed class AuditLogRetention(PlatformDbContext db) : IAuditLogRetention
{
    public Task<int> ClearPersonalDetailsBeforeAsync(Instant before, CancellationToken ct) =>
        db.AuditLogs
            .Where(a => a.OccurredUtc < before && (a.IpAddress != null || a.UserAgent != null))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.IpAddress, (string?)null).SetProperty(a => a.UserAgent, (string?)null), ct);
}
