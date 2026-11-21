using System.Text.Json;
using NodaTime;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure;

internal sealed class AuditLog(PlatformDbContext db, IClock clock) : IAuditLog
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public async Task WriteAsync(AuditEntry entry, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var changes = entry.Changes is { Count: > 0 } c ? JsonSerializer.Serialize(c, Json) : null;
        db.AuditLogs.Add(AuditLogEntry.Create(entry, changes, clock.GetCurrentInstant()));
        await db.SaveChangesAsync(ct);
    }
}
