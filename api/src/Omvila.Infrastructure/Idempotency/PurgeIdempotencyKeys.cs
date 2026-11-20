using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;

namespace Omvila.Infrastructure.Idempotency;

/// <summary>Saklama süresi (24 saat) dolan idempotency kayıtlarını siler; yanıt gövdeleri kişisel veri içerebilir.</summary>
internal sealed class PurgeIdempotencyKeys(
    InfraDbContext db, IClock clock, IOptions<IdempotencyOptions> options) : IPeriodicJob
{
    public string Key => "PurgeIdempotencyKeys";

    public Duration Interval => Duration.FromMinutes(options.Value.PurgeIntervalMinutes);

    public async Task RunAsync(CancellationToken ct)
    {
        var cutoff = clock.GetCurrentInstant() - Duration.FromHours(options.Value.RetentionHours);
        await db.IdempotencyKeys.Where(r => r.CreatedUtc < cutoff).ExecuteDeleteAsync(ct);
    }
}
