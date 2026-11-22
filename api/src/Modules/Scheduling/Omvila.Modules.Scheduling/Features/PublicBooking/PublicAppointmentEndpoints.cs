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
/// İkisi de telefon doğrulama jetonu (aud=public-booking, ADR-0007) veya müşteri uygulaması jetonu (aud=me; telefon global kayıttan,
/// kod adımı yok; T-101 (3)) ister. Oluşturma Idempotency-Key ister; anahtar şubenin markası kapsamında saklanır. Uygulamadan alınan
/// randevuda marka "Salonlarım"a eklenir (CustomerSavedBrand, Source Booking).
/// </summary>
internal static class PublicAppointmentEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/public/branches");
        group.MapPost("/{branchId:guid}/appointments", CreateAsync).WithName("publicCreateAppointment")
            .RequireAuthenticated(Audiences.PublicBooking, Audiences.Me)
            .AddEndpointFilter<PublicBranchFilter>()
            .RequireIdempotencyKey();

        group.MapGet("/{branchId:guid}/my-packages", ListMyPackagesAsync)
            .WithName("publicListMyPackages")
            .RequireAuthenticated(Audiences.PublicBooking, Audiences.Me)
            .AddEndpointFilter<PublicBranchFilter>();
    }

    /// <summary>
    /// Doğrulanmış telefonun bu şubede kullanabileceği paket kalemleri ("Fön paketinizden düşülsün"). Telefonun markada kartı
    /// yoksa liste boştur; başka markanın paketi görünmez (kiracı bağlamı şubenin markası).
    /// </summary>
    private static async Task<IResult> ListMyPackagesAsync(
        HttpContext http,
        ICustomerDirectory customers,
        ICustomerAccounts accounts,
        IPackageSessions packageSessions,
        ITenantQueries tenants,
        IClock clock,
        CancellationToken ct)
    {
        if (await PublicBookingCaller.ResolveAsync(http, accounts, ct) is not { } caller)
        {
            return Error.Unauthorized().ToProblem();
        }

        var scope = PublicBranchFilter.ScopeOf(http);
        if (await customers.FindBrandCustomerIdAsync(scope.BrandId, caller.Phone, ct) is not { } brandCustomerId)
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
        ICustomerAccounts accounts,
        IPublicBookingQueries booking,
        ITenantQueries tenants,
        IBrandBankAccounts bankAccounts,
        IClock clock,
        CancellationToken ct)
    {
        if (await PublicBookingCaller.ResolveAsync(http, accounts, ct) is not { } caller)
        {
            return Error.Unauthorized().ToProblem();
        }

        var scope = PublicBranchFilter.ScopeOf(http);
        var created = await creator.CreateAsync(caller.Request(http, scope), body with { FullName = caller.NameOr(body.FullName) }, ct);
        if (!created.IsSuccess)
        {
            return created.Error.ToProblem();
        }

        if (caller.AppCustomerId is { } appCustomerId)
        {
            // Kural: alan-modeli.md § CustomerSavedBrand (Source Booking); sözleşme publicCreateAppointment (meBearer)
            await accounts.SaveBookedBrandAsync(appCustomerId, scope.BrandId, ct);
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
