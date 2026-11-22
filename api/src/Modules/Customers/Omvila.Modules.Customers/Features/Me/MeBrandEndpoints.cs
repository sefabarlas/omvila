using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Me;

/// <summary>"Salonlarım" ve salon ekleme (sözleşme meListBrands, meSaveBrand; YRSalon, MainBos, derin bağlantı K3).</summary>
internal static partial class MeBrandEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/me/brands", Audiences.Me);
        group.MapGet(string.Empty, ListAsync).WithName("meListBrands").RequireAuthenticated(Audiences.Me);

        // Kendiliğinden idempotent (aynı satır 200 ile döner); sözleşmede Idempotency-Key yok.
        group.MapPost(string.Empty, SaveAsync).WithName("meSaveBrand").RequireAuthenticated(Audiences.Me);
    }

    private static async Task<IResult> ListAsync(ITenantContext tenant, MeBrandReader reader, CancellationToken ct) =>
        TypedResults.Ok(await reader.ListAsync(MeScope.CustomerId(tenant), ct));

    private static async Task<IResult> SaveAsync(MeSaveBrandRequest body, ITenantContext tenant, MeBrandReader reader, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (string.IsNullOrEmpty(body.BrandSlug))
        {
            errors["brandSlug"] = ["required"];
        }
        else if (!SlugPattern().IsMatch(body.BrandSlug))
        {
            errors["brandSlug"] = ["invalid_format"];
        }

        // Booking kaynağını yalnız sunucu yazar (sözleşme MeSaveBrandRequest.source).
        if (string.IsNullOrEmpty(body.Source))
        {
            errors["source"] = ["required"];
        }
        else if (body.Source is not (CustomerSavedBrand.QrSource or CustomerSavedBrand.LinkSource))
        {
            errors["source"] = ["invalid_format"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var saved = await reader.SaveAsync(MeScope.CustomerId(tenant), body.BrandSlug!, body.Source!, ct);
        return saved is null ? Error.NotFound().ToProblem() : TypedResults.Ok(saved);
    }

    [GeneratedRegex("^[a-z0-9-]{3,60}$", RegexOptions.CultureInvariant, matchTimeoutMilliseconds: 100)]
    private static partial Regex SlugPattern();
}
