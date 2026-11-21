using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>
/// Bildirim paneli (zil): sözleşme manageListInAppNotifications ve manageMarkNotificationsRead. Kullanıcı yalnız kendi
/// satırlarını görür (alıcıyı olay işleyicisi rol ve şube kapsamına göre seçmiştir, roller-yetkiler.md); başka kullanıcının
/// bildirimi okundu sayılmaz, sessizce yok sayılır. Oturumlu her kullanıcıya açıktır (izin gerekmez).
/// </summary>
internal static class InAppNotificationEndpoints
{
    private const int DefaultLimit = 50;
    private const int MaxLimit = 100;
    private const int MaxIdsPerRequest = 100;

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/notifications", Audiences.Manage);
        group.MapGet(string.Empty, ListAsync).WithName("manageListInAppNotifications").RequireAuthenticated(Audiences.Manage);
        group.MapPost("/read", MarkReadAsync).WithName("manageMarkNotificationsRead").RequireAuthenticated(Audiences.Manage);
    }

    private static async Task<IResult> ListAsync(
        int? limit, string? cursor, bool? unreadOnly, ClaimsPrincipal principal, IDbSession session, NotificationsDbContext db, CancellationToken ct)
    {
        if (!Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var userId))
        {
            return Error.Unauthorized().ToProblem();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var take = limit ?? DefaultLimit;
        if (take is < 1 or > MaxLimit)
        {
            errors["limit"] = ["out_of_range"];
        }

        NotificationCursor? after = null;
        if (cursor is not null)
        {
            if (NotificationCursor.TryDecode(cursor, out var decoded))
            {
                after = decoded;
            }
            else
            {
                errors["cursor"] = ["invalid_format"];
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var brandId = session.RequireBrandId();
        var mine = db.InAppNotifications.AsNoTracking().Where(n => n.BrandId == brandId && n.UserId == userId);
        var unreadCount = await mine.CountAsync(n => n.ReadUtc == null, ct);
        var page = unreadOnly == true ? mine.Where(n => n.ReadUtc == null) : mine;
        if (after is { } c)
        {
            page = page.Where(n => n.CreatedUtc < c.CreatedUtc || (n.CreatedUtc == c.CreatedUtc && n.Id.CompareTo(c.Id) < 0));
        }

        var rows = await page.OrderByDescending(n => n.CreatedUtc).ThenByDescending(n => n.Id).Take(take + 1).ToListAsync(ct);
        var next = rows.Count > take ? new NotificationCursor(rows[take - 1].CreatedUtc, rows[take - 1].Id).Encode() : null;
        return TypedResults.Ok(new InAppNotificationPage([.. rows.Take(take).Select(InAppNotificationItem.From)], next, unreadCount));
    }

    private static async Task<IResult> MarkReadAsync(
        MarkNotificationsReadRequest body, ClaimsPrincipal principal, IDbSession session, NotificationsDbContext db, IClock clock, CancellationToken ct)
    {
        if (!Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var userId))
        {
            return Error.Unauthorized().ToProblem();
        }

        // Sözleşme: notificationIds (1–100) veya all = true, tam biri.
        var all = body.All == true;
        var ids = body.NotificationIds;
        var error = (all, ids) switch
        {
            (true, not null) => ("all", "invalid_format"),
            (false, null) => ("notificationIds", "required"),
            (_, { Count: 0 }) => ("notificationIds", "too_short"),
            (_, { Count: > MaxIdsPerRequest }) => ("notificationIds", "too_long"),
            _ => default((string Field, string Code)?),
        };
        if (error is { } e)
        {
            return Error.Validation(e.Field, e.Code).ToProblem();
        }

        var brandId = session.RequireBrandId();
        var now = clock.GetCurrentInstant();
        var unread = db.InAppNotifications.Where(n => n.BrandId == brandId && n.UserId == userId && n.ReadUtc == null);
        if (!all)
        {
            unread = unread.Where(n => ids!.Contains(n.Id));
        }

        // "Tümünü okundu say" isteğin geldiği ana kadar oluşmuş bildirimleri kapsar.
        await unread.Where(n => n.CreatedUtc <= now).ExecuteUpdateAsync(set => set.SetProperty(n => n.ReadUtc, (Instant?)now), ct);
        var count = await db.InAppNotifications.CountAsync(n => n.BrandId == brandId && n.UserId == userId && n.ReadUtc == null, ct);
        return TypedResults.Ok(new UnreadNotificationCount(count));
    }
}
