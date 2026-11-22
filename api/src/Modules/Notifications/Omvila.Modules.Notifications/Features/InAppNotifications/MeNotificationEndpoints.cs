using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Features.InAppNotifications;

/// <summary>
/// Müşteri uygulaması bildirimleri (sözleşme meListNotifications, meMarkNotificationsRead; Bildirimler, BildirimlerBos). Müşteri
/// yalnız kendi satırlarını görür (RLS + sorgu); başka müşterinin bildirimi okundu sayılmaz, sessizce yok sayılır. Kurallar
/// manage karşılığıyla aynı (sıra, imleç, okundu).
/// </summary>
internal static class MeNotificationEndpoints
{
    private const int DefaultLimit = 50;
    private const int MaxLimit = 100;
    private const int MaxIdsPerRequest = 100;

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/me/notifications", Audiences.Me);
        group.MapGet(string.Empty, ListAsync).WithName("meListNotifications").RequireAuthenticated(Audiences.Me);
        group.MapPost("/read", MarkReadAsync).WithName("meMarkNotificationsRead").RequireAuthenticated(Audiences.Me);
    }

    private static async Task<IResult> ListAsync(
        int? limit, string? cursor, bool? unreadOnly, IDbSession session, NotificationsDbContext db, ICustomerBrandDirectory brands, CancellationToken ct)
    {
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

        var customerId = CustomerOf(session);
        var mine = db.CustomerNotifications.AsNoTracking().Where(n => n.CustomerId == customerId);
        var unreadCount = await mine.CountAsync(n => n.ReadUtc == null, ct);
        var page = unreadOnly == true ? mine.Where(n => n.ReadUtc == null) : mine;
        if (after is { } c)
        {
            page = page.Where(n => n.CreatedUtc < c.CreatedUtc || (n.CreatedUtc == c.CreatedUtc && n.Id.CompareTo(c.Id) < 0));
        }

        var rows = await page.OrderByDescending(n => n.CreatedUtc).ThenByDescending(n => n.Id).Take(take + 1).ToListAsync(ct);
        var next = rows.Count > take ? new NotificationCursor(rows[take - 1].CreatedUtc, rows[take - 1].Id).Encode() : null;
        var items = rows.Take(take).ToList();
        var cards = await brands.GetCardsAsync([.. items.Where(n => n.BrandId is not null).Select(n => n.BrandId!.Value).Distinct()], ct);
        return TypedResults.Ok(new MeNotificationPage([.. items.Select(n => MeNotificationItem.From(n, n.BrandId is { } b ? cards.GetValueOrDefault(b) : null))], next, unreadCount));
    }

    private static async Task<IResult> MarkReadAsync(
        MarkNotificationsReadRequest body, IDbSession session, NotificationsDbContext db, IClock clock, CancellationToken ct)
    {
        // Sözleşme: notificationIds (1–100) veya all = true, tam biri (manageMarkNotificationsRead ile aynı).
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

        var customerId = CustomerOf(session);
        var now = clock.GetCurrentInstant();
        var unread = db.CustomerNotifications.Where(n => n.CustomerId == customerId && n.ReadUtc == null);
        if (!all)
        {
            unread = unread.Where(n => ids!.Contains(n.Id));
        }

        await unread.Where(n => n.CreatedUtc <= now).ExecuteUpdateAsync(set => set.SetProperty(n => n.ReadUtc, (Instant?)now), ct);
        var count = await db.CustomerNotifications.CountAsync(n => n.CustomerId == customerId && n.ReadUtc == null, ct);
        return TypedResults.Ok(new UnreadNotificationCount(count));
    }

    private static Guid CustomerOf(IDbSession session) =>
        session.Scope.CustomerId ?? throw new DomainException("me isteğinde müşteri bağlamı yok.");
}
