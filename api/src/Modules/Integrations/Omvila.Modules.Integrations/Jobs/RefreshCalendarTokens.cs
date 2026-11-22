using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Jobs;
using Omvila.Modules.Integrations.Calendar;
using Omvila.Modules.Integrations.Domain;
using Omvila.Modules.Integrations.Infrastructure;

namespace Omvila.Modules.Integrations.Jobs;

/// <summary>
/// RefreshCalendarTokens (mimari.md: Integrations, 5 dk; durum-makinesi.md § CalendarLink). Erişim jetonu yakında dolacak etkin
/// bağlantılar yenilenir; sağlayıcı yenileme jetonunu reddederse bağlantı TokenExpired olur ("Google Takvim bağlantısı koptu").
/// Geçici hatada bir sonraki turda yeniden denenir. Platform bağlamında çalışır (yalnız bu modülün tablosu).
/// </summary>
internal sealed partial class RefreshCalendarTokens(
    IntegrationsDbContext db,
    ICalendarProvider provider,
    CalendarTokenProtector protector,
    IOptions<CalendarOptions> options,
    IClock clock,
    ILogger<RefreshCalendarTokens> logger) : IPeriodicJob
{
    public string Key => "RefreshCalendarTokens";

    public async Task RunAsync(CancellationToken ct)
    {
        if (!provider.IsEnabled)
        {
            return;
        }

        var now = clock.GetCurrentInstant();
        var due = now + Duration.FromMinutes(options.Value.RefreshAheadMinutes);
        var links = await db.CalendarLinks.Where(l => l.Status == CalendarLinkStatus.Active && l.TokenExpiresUtc <= due).ToListAsync(ct);
        foreach (var link in links)
        {
            var refreshed = await provider.RefreshAsync(protector.Unprotect(link.Id, link.RefreshTokenEncrypted!), ct);
            switch (refreshed.Status)
            {
                case CalendarCallStatus.Ok:
                    link.RefreshAccess(protector.Protect(link.Id, refreshed.AccessToken!), refreshed.ExpiresUtc!.Value);
                    break;
                case CalendarCallStatus.Revoked:
                    link.MarkTokenExpired(now);
                    LogDisconnected(logger, link.Id, link.BrandId);
                    break;
                default:
                    break;
            }
        }

        await db.SaveChangesAsync(ct);
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Calendar link {LinkId} of brand {BrandId} lost access (token expired)")]
    private static partial void LogDisconnected(ILogger logger, Guid linkId, Guid brandId);
}
