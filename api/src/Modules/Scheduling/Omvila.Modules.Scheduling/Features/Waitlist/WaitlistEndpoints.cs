using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.Modules.Scheduling.Features.AppointmentLink;
using Omvila.Modules.Scheduling.Features.PublicBooking;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>
/// Bekleme listesi uçları (sözleşme manage-waitlist ve public-waitlist; dilim-05 kabul kriteri 3). Panel uçları
/// waitlist.manage ister (Owner, BranchManager kendi şubesinde; personel 403). "Saat öner", "Teklifi geri al", kabul ve ret
/// If-Match alır (panelde kaydın, bağlantıda teklifin sürümü). Bağlantı uçları jetonsuzdur: yetki 10 karakterlik
/// kriptografik rastgele koddur (62^10) ve yalnız o teklife veya o kayda yeter.
/// </summary>
internal static class WaitlistEndpoints
{
    private const string CustomerAppClient = "customer-app";

    public static void Map(IEndpointRouteBuilder app)
    {
        var manage = app.MapApiGroup("/api/v1/manage/waitlist", Audiences.Manage);
        manage.MapGet(string.Empty, ListAsync).WithName("manageListWaitlist").RequirePermission(Permissions.Waitlist.Manage);
        manage.MapPost(string.Empty, AddAsync).WithName("manageAddToWaitlist").RequirePermission(Permissions.Waitlist.Manage).RequireIdempotencyKey();
        manage.MapPost("/{entryId:guid}/offer", OfferAsync).WithName("manageOfferSlot").RequirePermission(Permissions.Waitlist.Manage).RequireIfMatch();
        manage.MapPost("/{entryId:guid}/offer/withdraw", WithdrawAsync).WithName("manageWithdrawWaitlistOffer")
            .RequirePermission(Permissions.Waitlist.Manage).RequireIfMatch();

        var join = app.MapApiGroup("/api/v1/public/branches");
        join.MapPost("/{branchId:guid}/waitlist", JoinAsync).WithName("publicJoinWaitlist")
            .RequireAuthenticated(Audiences.PublicBooking)
            .AddEndpointFilter<PublicBranchFilter>()
            .RequireIdempotencyKey();

        var offers = app.MapApiGroup("/api/v1/public/waitlist-offers");

        // Üyeliksiz (ürün kuralı 7): teklif kodu yalnız bu teklifin okunmasına yetki verir; yan etkisiz.
        offers.MapGet("/{accessCode}", GetOfferAsync).WithName("publicGetWaitlistOffer").AllowAnonymous();

        // Üyeliksiz: teklif kodu yalnız bu teklifin kabulüne yetki verir; sürüm If-Match ile denetlenir, tekrar aynı randevuyu döner.
        offers.MapPost("/{accessCode}/accept", AcceptAsync).WithName("publicAcceptWaitlistOffer").AllowAnonymous().RequireIfMatch();

        // Üyeliksiz: teklif kodu yalnız bu teklifin reddine yetki verir; sürüm If-Match ile denetlenir.
        offers.MapPost("/{accessCode}/decline", DeclineAsync).WithName("publicDeclineWaitlistOffer").AllowAnonymous().RequireIfMatch();

        // Üyeliksiz: kayıt kodu yalnız bu kaydın listeden çıkışına yetki verir; kendiliğinden idempotent.
        app.MapApiGroup("/api/v1/public/waitlist")
            .MapPost("/{accessCode}/leave", LeaveAsync).WithName("publicLeaveWaitlist").AllowAnonymous();
    }

    private static async Task<IResult> ListAsync(
        Guid? branchId, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, WaitlistManagement waitlist, CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.Waitlist.Manage);
        var result = await waitlist.ListAsync(tenant.RequireBrandId(), branchId, access, ct);
        return result.ToHttp(r => TypedResults.Ok(r));
    }

    private static async Task<IResult> AddAsync(
        ManageWaitlistCreate body, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, WaitlistManagement waitlist, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var access = ManageAccess.For(http.User, permissions, Permissions.Waitlist.Manage);
        var result = await waitlist.AddAsync(brandId, body, access, UserId(http.User), ct);
        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        http.Response.Headers.ETag = ETagVersion.Format(result.Value.Version);
        return TypedResults.Created($"/api/v1/manage/waitlist/{result.Value.Id}", await waitlist.MapAsync(brandId, result.Value, access, ct));
    }

    private static async Task<IResult> OfferAsync(
        Guid entryId, WaitlistOfferRequest body, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, WaitlistManagement waitlist, CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.Waitlist.Manage);
        var result = await waitlist.OfferAsync(entryId, body, access, UserId(http.User), http.GetIfMatchVersion(), ct);
        return await RespondAsync(result, tenant.RequireBrandId(), access, http, waitlist, ct);
    }

    private static async Task<IResult> WithdrawAsync(
        Guid entryId, HttpContext http, ITenantContext tenant, IPermissionResolver permissions, WaitlistManagement waitlist, CancellationToken ct)
    {
        var access = ManageAccess.For(http.User, permissions, Permissions.Waitlist.Manage);
        var result = await waitlist.WithdrawAsync(entryId, access, http.GetIfMatchVersion(), ct);
        return await RespondAsync(result, tenant.RequireBrandId(), access, http, waitlist, ct);
    }

    private static async Task<IResult> JoinAsync(PublicWaitlistJoin body, HttpContext http, PublicWaitlist waitlist, CancellationToken ct)
    {
        if (!PhoneNumber.TryParse(http.User.FindFirst(OmvilaClaims.Phone)?.Value, out var phone))
        {
            return Error.Unauthorized().ToProblem();
        }

        var customerApp = IsCustomerApp(http);
        var context = new PublicBookingRequest(
            PublicBranchFilter.ScopeOf(http),
            phone,
            customerApp ? ClientApp.CustomerApp : ClientApp.BookingPage,
            customerApp ? ConsentChannel.CustomerApp : ConsentChannel.Web,
            http.Connection.RemoteIpAddress?.ToString());
        var joined = await waitlist.JoinAsync(context, body, ct);
        if (!joined.IsSuccess)
        {
            return joined.Error.ToProblem();
        }

        var view = await waitlist.MapEntryAsync(joined.Value.Entry, ct);
        return joined.Value.Created
            ? TypedResults.Created($"/api/v1/public/waitlist/{joined.Value.Entry.AccessCode}", view)
            : TypedResults.Ok(view);
    }

    private static async Task<IResult> GetOfferAsync(string accessCode, HttpContext http, PublicWaitlist waitlist, CancellationToken ct)
    {
        var found = await waitlist.EnterOfferAsync(accessCode, ct);
        return found is { } f ? await RespondOfferAsync(f.Entry, f.Offer, http, waitlist, ct) : InvalidCode();
    }

    private static async Task<IResult> AcceptAsync(
        string accessCode, HttpContext http, PublicWaitlist waitlist, AppointmentLinkResolver links, CancellationToken ct)
    {
        if (await waitlist.EnterOfferAsync(accessCode, ct) is not { } found)
        {
            return InvalidCode();
        }

        var clientApp = IsCustomerApp(http) ? ClientApp.CustomerApp : ClientApp.BookingPage;
        var accepted = await waitlist.AcceptAsync(found.Entry, found.Offer, http.GetIfMatchVersion(), clientApp, ct);
        if (!accepted.IsSuccess)
        {
            return accepted.Error.ToProblem();
        }

        var (appointment, code) = accepted.Value;
        var view = await links.MapAsync(appointment, code, ct);
        return view is null ? Error.NotFound().ToProblem() : VersionedResults.Versioned(http, appointment.Version, view);
    }

    private static async Task<IResult> DeclineAsync(string accessCode, HttpContext http, PublicWaitlist waitlist, CancellationToken ct)
    {
        if (await waitlist.EnterOfferAsync(accessCode, ct) is not { } found)
        {
            return InvalidCode();
        }

        var declined = await waitlist.DeclineAsync(found.Entry, found.Offer, http.GetIfMatchVersion(), ct);
        return declined.IsSuccess ? await RespondOfferAsync(found.Entry, found.Offer, http, waitlist, ct) : declined.Error.ToProblem();
    }

    private static async Task<IResult> LeaveAsync(string accessCode, PublicWaitlist waitlist, CancellationToken ct)
    {
        if (await waitlist.EnterEntryAsync(accessCode, ct) is not { } entry)
        {
            return InvalidCode();
        }

        var left = await waitlist.LeaveAsync(entry, ct);
        if (!left.IsSuccess)
        {
            return left.Error.ToProblem();
        }

        var view = await waitlist.MapEntryAsync(entry, ct);
        return view is null ? InvalidCode() : TypedResults.Ok(view);
    }

    private static async Task<IResult> RespondAsync(
        Result<WaitlistEntry> result, Guid brandId, ManageAccess access, HttpContext http, WaitlistManagement waitlist, CancellationToken ct) =>
        result.IsSuccess
            ? VersionedResults.Versioned(http, result.Value.Version, await waitlist.MapAsync(brandId, result.Value, access, ct))
            : result.Error.ToProblem();

    private static async Task<IResult> RespondOfferAsync(WaitlistEntry entry, WaitlistOffer offer, HttpContext http, PublicWaitlist waitlist, CancellationToken ct)
    {
        var view = await waitlist.MapOfferAsync(entry, offer, ct);
        return view is null ? InvalidCode() : VersionedResults.Versioned(http, offer.Version, view);
    }

    /// <summary>Bilinmeyen veya biçimi bozuk kod: hangi kodun var olduğu dışarı sızmaz.</summary>
    private static IResult InvalidCode() => Error.NotFound(ErrorCodes.AccessCodeInvalid).ToProblem();

    private static bool IsCustomerApp(HttpContext http) =>
        string.Equals(http.Request.Headers["X-Client"].ToString(), CustomerAppClient, StringComparison.Ordinal);

    private static Guid? UserId(ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : null;
}
