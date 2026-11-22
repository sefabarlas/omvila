using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>
/// "İzinlerim (KVKK)" (sözleşme meListConsents, meUpdateMarketingConsent). Liste müşterinin kartı olan markalardır (yalnız kaydedilen
/// salon yoktur); kartlar ve rıza kayıtları marka RLS'inde olduğundan platform bağlamında yalnız müşterinin kimliğiyle süzülür.
/// Randevu mesajları işlem mesajıdır, listede satırı yoktur. İYS'ye bildirim v1'de yapılmaz (T-09).
/// </summary>
internal static class MeConsentEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/me/consents", Audiences.Me);
        group.MapGet(string.Empty, ListAsync).WithName("meListConsents").RequireAuthenticated(Audiences.Me);

        // Rıza defteri yalnız eklenir ve aynı değer yeni satır yazmaz: istek kendiliğinden idempotenttir, If-Match almaz (T-101 (7)).
        group.MapPut(string.Empty, UpdateAsync).WithName("meUpdateMarketingConsent").RequireAuthenticated(Audiences.Me);
    }

    private static async Task<IResult> ListAsync(
        ITenantContext tenant, IDbSession session, CustomersDbContext db, MarketingConsentLedger ledger, ICustomerBrandDirectory brands, CancellationToken ct)
    {
        var customerId = MeScope.CustomerId(tenant);
        return TypedResults.Ok(await InPlatformAsync(session, () => BuildAsync(customerId, db, ledger, brands, ct), ct));
    }

    private static async Task<IResult> UpdateAsync(
        MeMarketingConsentUpdate body,
        HttpContext http,
        ITenantContext tenant,
        IDbSession session,
        CustomersDbContext db,
        MarketingConsentLedger ledger,
        ICustomerBrandDirectory brands,
        IClock clock,
        CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (body.BrandId is null)
        {
            errors["brandId"] = ["required"];
        }

        if (body.Granted is null)
        {
            errors["granted"] = ["required"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var customerId = MeScope.CustomerId(tenant);
        var list = await InPlatformAsync(session, async () =>
        {
            if (!await ActiveCards(db, customerId).AnyAsync(b => b.BrandId == body.BrandId, ct))
            {
                return null;
            }

            await ledger.SetAsync(customerId, body.BrandId!.Value, body.Granted!.Value, http.Connection.RemoteIpAddress?.ToString(), clock.GetCurrentInstant(), ct);
            return await BuildAsync(customerId, db, ledger, brands, ct);
        }, ct);
        return list is null ? Error.NotFound().ToProblem() : TypedResults.Ok(list);
    }

    // Kural: sözleşme meListConsents (marketingGranted = son Marketing* kaydı; kayıt yoksa false, updatedUtc null)
    private static async Task<MeConsentList> BuildAsync(
        Guid customerId, CustomersDbContext db, MarketingConsentLedger ledger, ICustomerBrandDirectory brands, CancellationToken ct)
    {
        var brandIds = await ActiveCards(db, customerId).Select(b => b.BrandId).Distinct().ToListAsync(ct);
        var cards = await brands.GetCardsAsync(brandIds, ct);
        var latest = await ledger.LatestAsync(customerId, brandIds, ct);
        return new MeConsentList([.. cards.Values
            .OrderBy(c => c.Name, StringComparer.Ordinal)
            .Select(c => new MeConsent(
                new MeConsent.BrandRef(c.Id, c.Name, c.Slug, c.LogoUrl),
                latest.TryGetValue(c.Id, out var l) && l.Granted,
                latest.TryGetValue(c.Id, out var u) ? u.UpdatedUtc : null))]);
    }

    private static IQueryable<Domain.BrandCustomer> ActiveCards(CustomersDbContext db, Guid customerId) =>
        db.BrandCustomers.AsNoTracking().Where(b => b.CustomerId == customerId && b.MergedIntoId == null && b.AnonymizedUtc == null);

    private static async Task<T> InPlatformAsync<T>(IDbSession session, Func<Task<T>> work, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            return await work();
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
