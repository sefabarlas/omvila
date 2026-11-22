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
using Omvila.Modules.Notifications.Domain;
using Omvila.Modules.Notifications.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Features.Preferences;

/// <summary>
/// İşletme uygulaması "Bildirim ayarları" (sözleşme manageGetMyNotificationPreferences / manageUpdateMyNotificationPreferences).
/// Oturumdaki kullanıcının bu markadaki tercihleri; oturumlu her kullanıcıya açıktır (izin gerekmez, yalnız kendi satırı).
/// Kaydı olmayan kullanıcıya varsayılanlar sürüm "0" ile döner; ilk PUT (If-Match 0) satırı açar.
/// </summary>
internal static class NotificationPreferenceEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/me/notification-preferences", Audiences.Manage);
        group.MapGet(string.Empty, GetAsync).WithName("manageGetMyNotificationPreferences").RequireAuthenticated(Audiences.Manage);
        group.MapPut(string.Empty, UpdateAsync).WithName("manageUpdateMyNotificationPreferences")
            .RequireAuthenticated(Audiences.Manage).RequireIfMatch();
    }

    private static async Task<IResult> GetAsync(
        HttpContext http, ClaimsPrincipal principal, IDbSession session, NotificationsDbContext db, CancellationToken ct)
    {
        if (!Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var userId))
        {
            return Error.Unauthorized().ToProblem();
        }

        var brandId = session.RequireBrandId();
        var preference = await db.NotificationPreferences.AsNoTracking().SingleOrDefaultAsync(p => p.BrandId == brandId && p.UserId == userId, ct)
            ?? NotificationPreference.Defaults(brandId, userId);
        return VersionedResults.Versioned(http, preference.Version, MyNotificationPreferences.From(preference));
    }

    private static async Task<IResult> UpdateAsync(
        MyNotificationPreferences body,
        HttpContext http,
        ClaimsPrincipal principal,
        IDbSession session,
        NotificationsDbContext db,
        IClock clock,
        CancellationToken ct)
    {
        if (!Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var userId))
        {
            return Error.Unauthorized().ToProblem();
        }

        var brandId = session.RequireBrandId();
        var expected = http.GetIfMatchVersion();
        var preference = await db.NotificationPreferences.SingleOrDefaultAsync(p => p.BrandId == brandId && p.UserId == userId, ct);
        if (preference is null ? expected != 0 : !db.MatchesVersion(preference, preference.Version, expected))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var errors = body.Validate();
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var now = clock.GetCurrentInstant();
        if (preference is null)
        {
            preference = NotificationPreference.Create(brandId, userId, body.ToValues(), now);
            db.NotificationPreferences.Add(preference);
        }
        else
        {
            preference.Apply(body.ToValues(), now);
        }

        await db.SaveChangesAsync(ct);
        return VersionedResults.Versioned(http, preference.Version, MyNotificationPreferences.From(preference));
    }
}
