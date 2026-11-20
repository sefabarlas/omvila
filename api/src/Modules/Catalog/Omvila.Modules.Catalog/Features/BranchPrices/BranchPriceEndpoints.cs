using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Catalog.Domain;
using Omvila.Modules.Catalog.Features.Services;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.BranchPrices;

/// <summary>
/// Şubeye özel fiyat ve süre (kabul kriteri 9). service.manage; şube yöneticisi yalnız kendi şubesi (kapsam dışı 404).
/// </summary>
internal static class BranchPriceEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/services/{serviceId:guid}/branch-prices", Audiences.Manage);
        group.MapPut("/{branchId:guid}", SetAsync).WithName("manageSetBranchServicePrice").RequirePermission(Permissions.Service.Manage);
        group.MapDelete("/{branchId:guid}", DeleteAsync).WithName("manageDeleteBranchServicePrice").RequirePermission(Permissions.Service.Manage);
    }

    private static async Task<IResult> SetAsync(
        Guid serviceId, Guid branchId, BranchPriceRequest body, ClaimsPrincipal user, ITenantContext tenant, CatalogDbContext db,
        ITenantQueries tenants, IIdGenerator ids, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var service = await FindAsync(serviceId, branchId, brandId, user, db, tenants, ct);
        if (service is null)
        {
            return Error.NotFound().ToProblem();
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var price = body.Price is null ? null : body.Price.ToMoney("price", errors);
        if (body.Price is null)
        {
            errors["price"] = ["required"];
        }

        if (body.IsAvailable is null)
        {
            errors["isAvailable"] = ["required"];
        }

        if (price is { } p && p.Currency != service.BasePrice.Currency)
        {
            errors["price.currency"] = ["invalid_format"];
        }

        if (body.DurationOverrideMinutes is { } minutes)
        {
            var stages = await db.Stages.AsNoTracking().Where(st => st.ServiceId == serviceId).OrderBy(st => st.SeqNo).Select(st => st.ToStage()).ToListAsync(ct);
            if (minutes > ServiceRules.MaxDurationMinutes || !OfferResolver.FitsStages(stages, service.DurationMinutes, minutes))
            {
                errors["durationOverrideMinutes"] = ["out_of_range"];
            }
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors).ToProblem();
        }

        var value = new BranchPrice(price!.Value, body.DurationOverrideMinutes, body.IsAvailable!.Value);
        var row = await db.BranchPrices.SingleOrDefaultAsync(x => x.ServiceId == serviceId && x.BranchId == branchId, ct);
        if (row is null)
        {
            db.BranchPrices.Add(BranchServicePrice.Create(ids.New(), brandId, branchId, serviceId, value));
        }
        else
        {
            row.Set(value);
        }

        await db.SaveChangesAsync(ct);
        return TypedResults.Ok(new BranchPriceRequest(new MoneyDto(value.Price.AmountMinor, value.Price.Currency.ToString()), value.DurationOverrideMinutes, value.IsAvailable));
    }

    private static async Task<IResult> DeleteAsync(
        Guid serviceId, Guid branchId, ClaimsPrincipal user, ITenantContext tenant, CatalogDbContext db, ITenantQueries tenants, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if (await FindAsync(serviceId, branchId, brandId, user, db, tenants, ct) is null)
        {
            return Error.NotFound().ToProblem();
        }

        var deleted = await db.BranchPrices.Where(x => x.ServiceId == serviceId && x.BranchId == branchId).ExecuteDeleteAsync(ct);
        return deleted == 0 ? Error.NotFound().ToProblem() : TypedResults.NoContent();
    }

    private static async Task<Service?> FindAsync(
        Guid serviceId, Guid branchId, Guid brandId, ClaimsPrincipal user, CatalogDbContext db, ITenantQueries tenants, CancellationToken ct)
    {
        if (!BranchScope.From(user).Includes(branchId) || !await tenants.BranchExistsAsync(brandId, branchId, ct))
        {
            return null;
        }

        return await db.Services.AsNoTracking().SingleOrDefaultAsync(s => s.Id == serviceId && s.BrandId == brandId, ct);
    }
}
