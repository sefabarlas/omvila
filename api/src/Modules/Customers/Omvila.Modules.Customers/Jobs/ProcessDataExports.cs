using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Email;
using Omvila.Infrastructure.Jobs;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Features.DataExports;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Jobs;

/// <summary>
/// "Tüm verileri dışa aktar" kuyruğu (T-121): bekleyen her istek markanın kiracı bağlamında bütün katılımcıların tablolarıyla
/// ZIP arşivine dönüşür (Ready; bağlantı DataExportLinkValidityDays geçerli), sonra imzalı indirme bağlantısı isteğin adresine
/// e-postayla gider (owner.data_export_ready, Sent). E-posta kapalıysa, <c>DownloadBaseUrl</c> yoksa veya gönderim geçici hatayla
/// düşerse istek Ready kalır ve sonraki turda yeniden denenir. Süresi dolan arşivler silinir. Arşiv hazırlanırken hata olursa yalnız
/// o istek Failed olur; ayrıntı günlükte (kişisel veri yok).
/// </summary>
internal sealed partial class ProcessDataExports(
    CustomersDbContext db,
    IDbSession session,
    IEnumerable<IBrandDataExportParticipant> participants,
    DataExportLinks links,
    IEmailSender email,
    ITenantQueries tenants,
    IOptions<DataExportOptions> options,
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
            .Where(e => e.Status == DataExportStatus.Queued || e.Status == DataExportStatus.Processing
                || (e.Status == DataExportStatus.Ready && e.Content != null && e.ExpiresUtc > now))
            .OrderBy(e => e.RequestedUtc)
            .Select(e => new { e.Id, e.BrandId })
            .ToListAsync(ct);
        foreach (var item in queued)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(item.BrandId), ct);
            var export = await db.DataExports.SingleAsync(e => e.Id == item.Id, ct);
            if (export.Status != DataExportStatus.Ready)
            {
                export = await BuildAsync(export, ct);
                await db.SaveChangesAsync(ct);
            }

            if (export.Status == DataExportStatus.Ready && await SendAsync(export, ct))
            {
                export.MarkSent();
                await db.SaveChangesAsync(ct);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
    }

    private async Task<DataExport> BuildAsync(DataExport export, CancellationToken ct)
    {
        var (exportId, brandId) = (export.Id, export.BrandId);
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

        return export;
    }

    // Arşiv kaydedildikten sonra gönderilir: e-postadaki bağlantı her zaman var olan bir arşive gider.
    private async Task<bool> SendAsync(DataExport export, CancellationToken ct)
    {
        if (options.Value.DownloadBaseUrl is not { } baseUrl)
        {
            return false;
        }

        var token = links.CreateToken(export.BrandId, export.Id, export.ExpiresUtc!.Value);
        var link = new Uri(baseUrl, $"api/v1/public/data-exports/{token}");
        var brand = await tenants.GetSummaryAsync(export.BrandId, null, ct);
        try
        {
            var status = await email.SendAsync(
                DataExportEmail.Build(export.Email, brand?.Name ?? "İşletmeniz", link, PlatformDefaults.DataExportLinkValidityDays), ct);
            return status == EmailSendStatus.Sent;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            LogSendFailed(logger, ex, export.Id, export.BrandId);
            return false;
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Data export {ExportId} for brand {BrandId} is ready with {FileCount} files")]
    private static partial void LogReady(ILogger logger, Guid exportId, Guid brandId, int fileCount);

    [LoggerMessage(Level = LogLevel.Error, Message = "Data export {ExportId} for brand {BrandId} failed")]
    private static partial void LogFailed(ILogger logger, Exception exception, Guid exportId, Guid brandId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Data export {ExportId} for brand {BrandId}: link e-mail not sent, retried next run")]
    private static partial void LogSendFailed(ILogger logger, Exception exception, Guid exportId, Guid brandId);
}
