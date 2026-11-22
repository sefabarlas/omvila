using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Features.DataExports;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Jobs;

/// <summary>
/// "Tüm verileri dışa aktar" kuyruğu (T-121): bekleyen her istek markanın kiracı bağlamında bütün katılımcıların tablolarıyla
/// ZIP arşivine dönüşür (Ready; bağlantı DataExportLinkValidityDays geçerli). Süresi dolan arşivler silinir. Bağlantıyı
/// e-postayla gönderme (Sent) e-posta altyapısı bağlanınca bu işe eklenir (docs/acik-sorular.md T-121).
/// Bir istekte hata olursa yalnız o istek Failed olur; ayrıntı günlükte (kişisel veri yok).
/// </summary>
internal sealed partial class ProcessDataExports(
    CustomersDbContext db,
    IDbSession session,
    IEnumerable<IBrandDataExportParticipant> participants,
    IClock clock,
    ILogger<ProcessDataExports> logger) : IPeriodicJob
{
    public string Key => "ProcessDataExports";

    public async Task RunAsync(CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        await db.DataExports.Where(e => e.Content != null && e.ExpiresUtc <= now)
            .ExecuteUpdateAsync(s => s.SetProperty(e => e.Content, (byte[]?)null), ct);

        var queued = await db.DataExports.AsNoTracking()
            .Where(e => e.Status == DataExportStatus.Queued || e.Status == DataExportStatus.Processing)
            .OrderBy(e => e.RequestedUtc)
            .Select(e => new { e.Id, e.BrandId })
            .ToListAsync(ct);
        foreach (var item in queued)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(item.BrandId), ct);
            await ProcessAsync(item.Id, item.BrandId, ct);
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    private async Task ProcessAsync(Guid exportId, Guid brandId, CancellationToken ct)
    {
        var export = await db.DataExports.SingleAsync(e => e.Id == exportId, ct);
        export.Start();
        var now = clock.GetCurrentInstant();
        try
        {
            var files = new List<BrandDataExportFile>();
            foreach (var participant in participants)
            {
                files.AddRange(await participant.ExportAsync(brandId, ct));
            }

            var name = $"omvila-veriler-{now.InUtc().Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}.zip";
            export.Complete(name, DataExportArchive.Build(files), now, Duration.FromDays(PlatformDefaults.DataExportLinkValidityDays));
            LogReady(logger, exportId, brandId, files.Count);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Katılımcı hatası yalnız bu isteği düşürür; kuyruktaki diğer markalar işlenir.
            LogFailed(logger, ex, exportId, brandId);
            db.ChangeTracker.Clear();
            export = await db.DataExports.SingleAsync(e => e.Id == exportId, ct);
            export.Fail(ex.GetType().Name, now);
        }

        await db.SaveChangesAsync(ct);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Data export {ExportId} for brand {BrandId} is ready with {FileCount} files")]
    private static partial void LogReady(ILogger logger, Guid exportId, Guid brandId, int fileCount);

    [LoggerMessage(Level = LogLevel.Error, Message = "Data export {ExportId} for brand {BrandId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, Guid exportId, Guid brandId);
}
