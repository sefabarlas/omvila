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

namespace Omvila.Modules.Notifications.Features.Devices;

/// <summary>
/// İşletme uygulaması push jetonu kaydı (sözleşme manageRegisterDevice; T-62 (7)). Jetona göre upsert: aynı markada aynı
/// jeton varsa kayıt oturumdaki kullanıcıya geçer ve etkinleşir, aynı deviceId döner. Jeton başka markada aktifse o kayıt
/// pasifleşir (alan modeli: aktif kayıtlar arasında jeton tek; aynı telefonda hesap değişimi). İstek doğası gereği
/// idempotent olduğundan Idempotency-Key istenmez. Jeton günlüğe yazılmaz.
/// </summary>
internal static class DeviceEndpoints
{
    private const int MinTokenLength = 20;

    private const int MaxTokenLength = 4096;

    private const int MaxAppVersionLength = 20;

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapApiGroup("/api/v1/manage/devices", Audiences.Manage)
            .MapPost(string.Empty, RegisterAsync)
            .WithName("manageRegisterDevice")
            .RequireAuthenticated(Audiences.Manage);

    private static async Task<IResult> RegisterAsync(
        DeviceRegistration body,
        ClaimsPrincipal principal,
        IDbSession session,
        NotificationsDbContext db,
        IClock clock,
        IIdGenerator ids,
        CancellationToken ct)
    {
        if (!Guid.TryParse(principal.FindFirst(OmvilaClaims.Subject)?.Value, out var userId))
        {
            return Error.Unauthorized().ToProblem();
        }

        var errors = Validate(body, out var platform);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var brandId = session.RequireBrandId();
        var token = body.PushToken!;
        var now = clock.GetCurrentInstant();

        // Başka markadaki aktif kayıt kiracı bağlamında görünmez: platform bağlamında yalnız bu jetonun kaydı pasifleşir.
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            await db.Devices.Where(d => d.PushToken == token && d.IsActive && d.BrandId != brandId)
                .ExecuteUpdateAsync(set => set.SetProperty(d => d.IsActive, false), ct);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }

        var device = await db.Devices.SingleOrDefaultAsync(d => d.PushToken == token, ct);
        if (device is null)
        {
            device = Device.RegisterForUser(ids.New(), brandId, userId, platform, token, body.AppVersion!, now);
            db.Devices.Add(device);
        }
        else
        {
            device.Refresh(userId, platform, body.AppVersion!, now);
        }

        await db.SaveChangesAsync(ct);
        return TypedResults.Ok(new RegisteredDevice(device.Id, device.Platform, device.LastSeenUtc));
    }

    internal static Dictionary<string, string[]> Validate(DeviceRegistration body, out DevicePlatform platform)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        platform = default;
        if (string.IsNullOrEmpty(body.Platform))
        {
            errors["platform"] = ["required"];
        }
        else if (!Enum.GetNames<DevicePlatform>().Contains(body.Platform, StringComparer.Ordinal) || !Enum.TryParse(body.Platform, ignoreCase: false, out platform))
        {
            errors["platform"] = ["invalid_format"];
        }

        if (string.IsNullOrWhiteSpace(body.PushToken))
        {
            errors["pushToken"] = ["required"];
        }
        else if (body.PushToken.Length is < MinTokenLength or > MaxTokenLength)
        {
            errors["pushToken"] = ["invalid_format"];
        }

        if (string.IsNullOrWhiteSpace(body.AppVersion))
        {
            errors["appVersion"] = ["required"];
        }
        else if (body.AppVersion.Length > MaxAppVersionLength)
        {
            errors["appVersion"] = ["too_long"];
        }

        return errors;
    }
}
