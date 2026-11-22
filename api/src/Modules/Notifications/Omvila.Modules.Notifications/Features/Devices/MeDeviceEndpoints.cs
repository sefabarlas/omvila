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
/// Müşteri uygulaması push jetonu kaydı (sözleşme meRegisterDevice; T-103). manageRegisterDevice ile aynı kurallar: jetona göre
/// upsert, aynı jeton aynı deviceId'yi döner; jeton başka hesapta (işletme cihazı veya başka müşteri) aktifse o kayıt pasifleşir.
/// Müşteri cihazı markasızdır (brand_id boş) ve müşterinin kendi bağlamında görünür. Jeton günlüğe yazılmaz.
/// </summary>
internal static class MeDeviceEndpoints
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapApiGroup("/api/v1/me/devices", Audiences.Me)
            .MapPost(string.Empty, RegisterAsync)
            .WithName("meRegisterDevice")
            .RequireAuthenticated(Audiences.Me);

    private static async Task<IResult> RegisterAsync(
        DeviceRegistration body, IDbSession session, NotificationsDbContext db, IClock clock, IIdGenerator ids, CancellationToken ct)
    {
        var errors = DeviceEndpoints.Validate(body, out var platform);
        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var customerId = session.Scope.CustomerId ?? throw new DomainException("me isteğinde müşteri bağlamı yok.");
        var token = body.PushToken!;
        var now = clock.GetCurrentInstant();

        // Jetonun başka sahipteki aktif kaydı müşteri bağlamında görünmez: platform bağlamında yalnız bu jetonun kaydı pasifleşir.
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            await db.Devices
                .Where(d => d.PushToken == token && d.IsActive && d.BrandId != null)
                .ExecuteUpdateAsync(set => set.SetProperty(d => d.IsActive, false), ct);
            // Markasız satır başka müşterideyse (aynı telefonda hesap değişimi) bu müşteriye geçer, aynı deviceId döner.
            var device = await db.Devices.SingleOrDefaultAsync(d => d.PushToken == token && d.BrandId == null, ct);
            if (device is null)
            {
                device = Device.RegisterForCustomer(ids.New(), customerId, platform, token, body.AppVersion!, now);
                db.Devices.Add(device);
            }
            else
            {
                device.RefreshForCustomer(customerId, platform, body.AppVersion!, now);
            }

            await db.SaveChangesAsync(ct);
            return TypedResults.Ok(new RegisteredDevice(device.Id, device.Platform, device.LastSeenUtc));
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
