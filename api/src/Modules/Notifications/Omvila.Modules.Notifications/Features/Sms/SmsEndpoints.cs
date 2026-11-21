using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Features.Sms;

/// <summary>
/// Ayarlar → Bildirimler ve SMS üst kartları (sözleşme manageGetSmsOverview) ve gönderen adı başvurusu
/// (manageRequestSmsSender). Kontör markaya aittir; "bu ay" ana şubenin saat dilimindeki takvim ayıdır. Onay platform
/// panelindedir (Dilim 6); onaylanana kadar SMS'ler OMVILA adıyla gider (dilim-03 kriter 11). İzin: settings.manage.
/// </summary>
internal static partial class SmsEndpoints
{
    private const int RegexTimeoutMilliseconds = 1000;

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/brand", Audiences.Manage);
        group.MapGet("/sms", GetOverviewAsync).WithName("manageGetSmsOverview").RequirePermission(Permissions.Settings.Manage);
        group.MapPost("/sms-sender", RequestSenderAsync).WithName("manageRequestSmsSender")
            .RequirePermission(Permissions.Settings.Manage).RequireIdempotencyKey();
    }

    private static async Task<IResult> GetOverviewAsync(
        IDbSession session,
        NotificationsDbContext db,
        ICreditLedger ledger,
        SmsUsage usage,
        ITenantQueries tenants,
        IClock clock,
        IOptions<SmsOptions> smsOptions,
        CancellationToken ct)
    {
        var brandId = session.RequireBrandId();
        var balance = Math.Max(0, await ledger.GetBalanceAsync(brandId, ct));
        var settings = await tenants.GetNotificationSettingsAsync(brandId, ct);
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(settings?.TimeZoneId ?? PlatformDefaults.DefaultTimeZoneId)
            ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var now = clock.GetCurrentInstant();
        var month = now.InZone(zone).Date.With(DateAdjusters.StartOfMonth);
        var from = zone.AtStartOfDay(month).ToInstant();
        var to = zone.AtStartOfDay(month.PlusMonths(1)).ToInstant();

        // Gönderim anı ayrı tutulmaz: Sent satırın ScheduledUtc'si son deneme anıdır (T-64 (4)). Giriş kodu salonun değildir.
        var sent = db.Outbox.AsNoTracking().Where(m => m.BrandId == brandId && m.Status == NotificationStatus.Sent
            && m.EventCode != NotificationEvents.OtpCode && m.ScheduledUtc >= from && m.ScheduledUtc < to);
        var notificationCount = await sent.CountAsync(ct);
        var smsCount = await sent.CountAsync(m => m.ChannelUsed == NotificationMessage.SmsChannel, ct);
        var creditsUsed = await usage.NetConsumedAsync(brandId, from, to, ct);

        var sender = await db.SmsSenders.AsNoTracking().SingleOrDefaultAsync(s => s.BrandId == brandId, ct);
        return TypedResults.Ok(new SmsOverview(
            balance,
            await usage.EstimateDaysLeftAsync(brandId, balance, ct),
            PlatformDefaults.LowCreditThreshold,
            balance < PlatformDefaults.LowCreditThreshold,
            new SmsMonthUsage(month, smsCount, creditsUsed, notificationCount),
            SmsSenderResponse.From(sender, smsOptions.Value.DefaultSender)));
    }

    private static async Task<IResult> RequestSenderAsync(
        SmsSenderRequest body, IDbSession session, NotificationsDbContext db, IClock clock, IOptions<SmsOptions> smsOptions, CancellationToken ct)
    {
        // Kural: T-62 (6) — 3–11 büyük Latin harf/rakam, en az bir harf.
        if (body.Name is null || !NamePattern().IsMatch(body.Name))
        {
            return Error.Validation("name", body.Name is null ? "required" : "invalid_format").ToProblem();
        }

        var brandId = session.RequireBrandId();
        var now = clock.GetCurrentInstant();
        var sender = await db.SmsSenders.SingleOrDefaultAsync(s => s.BrandId == brandId, ct);
        if (sender is null)
        {
            sender = BrandSmsSender.Submit(brandId, body.Name, now);
            db.SmsSenders.Add(sender);
        }
        else if (sender.Status == SmsSenderStatus.Pending)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition).ToProblem();
        }
        else
        {
            sender.Resubmit(body.Name, now);
        }

        await db.SaveChangesAsync(ct);
        return TypedResults.Created("/api/v1/manage/brand/sms", SmsSenderResponse.From(sender, smsOptions.Value.DefaultSender));
    }

    [GeneratedRegex("^(?=.*[A-Z])[A-Z0-9]{3,11}$", RegexOptions.None, RegexTimeoutMilliseconds)]
    private static partial Regex NamePattern();
}
