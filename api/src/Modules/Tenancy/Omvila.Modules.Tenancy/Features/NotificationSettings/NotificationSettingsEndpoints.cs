using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;
using static Omvila.Modules.Tenancy.Features.ManageEndpointSupport;

namespace Omvila.Modules.Tenancy.Features.NotificationSettings;

/// <summary>
/// Ayarlar → Bildirimler ve SMS "Hatırlatmalar" (sözleşme manageGetNotificationSettings / manageUpdateNotificationSettings).
/// Hatırlatma zamanları ve değerlendirme daveti için kanonik uç; değerler marka ayarları satırında saklanır, bu yüzden
/// BrandSettings'teki deprecated alanlar aynı değeri gösterir ve iki kaynağın ETag'i (xmin) birlikte değişir (T-62 (4)).
/// Salon şablon ezmesi henüz yoktur: <c>templates</c> boş döner, gönderilen her şablon ezilemez sayılır (T-71).
/// </summary>
internal static class NotificationSettingsEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/brand/notification-settings", Audiences.Manage);
        group.MapGet(string.Empty, GetAsync).WithName("manageGetNotificationSettings").RequirePermission(Permissions.Settings.Manage);
        group.MapPut(string.Empty, UpdateAsync).WithName("manageUpdateNotificationSettings")
            .RequirePermission(Permissions.Settings.Manage).RequireIfMatch();
    }

    private static async Task<IResult> GetAsync(HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var settings = await db.BrandSettings.AsNoTracking().SingleAsync(s => s.BrandId == BrandOf(tenant), ct);
        return Versioned(http, settings.Version, NotificationSettingsResponse.From(settings));
    }

    private static async Task<IResult> UpdateAsync(
        NotificationSettingsUpdate body, HttpContext http, ITenantContext tenant, TenancyDbContext db, CancellationToken ct)
    {
        var settings = await db.BrandSettings.SingleAsync(s => s.BrandId == BrandOf(tenant), ct);
        if (!Matches(db, settings, settings.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var errors = Validate(body);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        settings.ApplyNotificationSettings(body.ReminderOffsetsMinutes!, body.ReviewInviteEnabled!.Value);
        db.Entry(settings).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return Versioned(http, settings.Version, NotificationSettingsResponse.From(settings));
    }

    private static Dictionary<string, string[]> Validate(NotificationSettingsUpdate body)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.ReminderOffsetsMinutes is null)
        {
            errors["reminderOffsetsMinutes"] = ["required"];
        }
        else if (BrandSettingsRules.ValidateNotificationOffsets(body.ReminderOffsetsMinutes) is { } code)
        {
            errors["reminderOffsetsMinutes"] = [code];
        }

        if (body.ReviewInviteEnabled is null)
        {
            errors["reviewInviteEnabled"] = ["required"];
        }

        // Ezilebilen şablon yok (GET templates boş): her satır "ezilemeyen olay kodu" (T-71).
        for (var i = 0; i < (body.Templates?.Count ?? 0); i++)
        {
            errors[$"templates[{i}].eventCode"] = ["invalid_format"];
        }

        return errors;
    }
}
