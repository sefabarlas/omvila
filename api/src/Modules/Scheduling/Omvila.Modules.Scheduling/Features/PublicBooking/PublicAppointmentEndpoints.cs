using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Rezervasyon sayfasında randevu oluşturma ve paket listesi (sözleşme publicCreateAppointment, publicListMyPackages).
/// İkisi de telefon doğrulama jetonu (aud=public-booking, ADR-0007) ister; müşteri uygulaması jetonu (aud=me) /me yüzeyiyle
/// eklenir. Oluşturma Idempotency-Key ister; anahtar şubenin markası kapsamında saklanır.
/// </summary>
internal static class PublicAppointmentEndpoints
{
    private const string CustomerAppClient = "customer-app";

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/public/branches");
        group.MapPost("/{branchId:guid}/appointments", CreateAsync).WithName("publicCreateAppointment")
            .RequireAuthenticated(Audiences.PublicBooking)
            .AddEndpointFilter<PublicBranchFilter>()
            .RequireIdempotencyKey();

        group.MapGet("/{branchId:guid}/my-packages", ListMyPackagesAsync)
            .WithName("publicListMyPackages")
            .RequireAuthenticated(Audiences.PublicBooking)
            .AddEndpointFilter<PublicBranchFilter>();
    }

    /// <summary>
    /// Doğrulanmış telefonun bu şubede kullanabileceği paket kalemleri ("Fön paketinizden düşülsün"). Telefonun markada kartı
    /// yoksa liste boştur; başka markanın paketi görünmez (kiracı bağlamı şubenin markası).
    /// </summary>
    private static async Task<IResult> ListMyPackagesAsync(
        HttpContext http, ICustomerDirectory customers, IPackageSessions packageSessions, ITenantQueries tenants, IClock clock, CancellationToken ct)
    {
        if (!PhoneNumber.TryParse(http.User.FindFirst(OmvilaClaims.Phone)?.Value, out var phone))
        {
            return Error.Unauthorized().ToProblem();
        }

        var scope = PublicBranchFilter.ScopeOf(http);
        if (await customers.FindBrandCustomerIdAsync(scope.BrandId, phone, ct) is not { } brandCustomerId)
        {
            return TypedResults.Ok(new PackageList([]));
        }

        var zoneId = await tenants.GetBranchTimeZoneAsync(scope.BrandId, scope.BranchId, ct) ?? PlatformDefaults.DefaultTimeZoneId;
        var zone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var today = clock.GetCurrentInstant().InZone(zone).Date;
        return TypedResults.Ok(new PackageList(await packageSessions.ListUsableAsync(scope.BrandId, scope.BranchId, brandCustomerId, today, ct)));
    }

    private static async Task<IResult> CreateAsync(
        PublicAppointmentCreate body,
        HttpContext http,
        PublicAppointmentCreator creator,
        IPublicBookingQueries booking,
        ITenantQueries tenants,
        IBrandBankAccounts bankAccounts,
        IClock clock,
        CancellationToken ct)
    {
        if (!PhoneNumber.TryParse(http.User.FindFirst(OmvilaClaims.Phone)?.Value, out var phone))
        {
            return Error.Unauthorized().ToProblem();
        }

        var scope = PublicBranchFilter.ScopeOf(http);
        var customerApp = string.Equals(http.Request.Headers["X-Client"].ToString(), CustomerAppClient, StringComparison.Ordinal);
        var context = new PublicBookingRequest(
            scope,
            phone,
            customerApp ? ClientApp.CustomerApp : ClientApp.BookingPage,
            customerApp ? ConsentChannel.CustomerApp : ConsentChannel.Web,
            http.Connection.RemoteIpAddress?.ToString());
        var created = await creator.CreateAsync(context, body, ct);
        if (!created.IsSuccess)
        {
            return created.Error.ToProblem();
        }

        var (appointment, accessCode, _) = created.Value;
        var card = await booking.GetBranchCardAsync(scope.BrandId, scope.BranchId, ct);
        var settings = await tenants.GetAppointmentSettingsAsync(scope.BrandId, ct);
        var staffIds = appointment.Items.Where(i => i.StaffId is not null).Select(i => i.StaffId!.Value).Distinct().ToList();
        var staff = await PublicAvailabilityEndpoints.PublicNamesAsync(tenants, scope.BrandId, staffIds, ct);
        var account = appointment.DisplayStatus == DisplayStatus.DepositPending ? await bankAccounts.GetActiveAsync(scope.BrandId, ct) : null;
        var view = PublicAppointmentMapper.Map(
            appointment, accessCode!.Code, booking.AppointmentPageUrl(accessCode.Code), card!, settings!, staff, account, clock.GetCurrentInstant());
        http.Response.Headers.ETag = ETagVersion.Format(appointment.Version);
        return TypedResults.Created($"/api/v1/public/appointments/{accessCode.Code}", view);
    }
}
