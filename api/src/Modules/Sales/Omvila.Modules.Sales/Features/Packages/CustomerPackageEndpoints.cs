using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Idempotency;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>
/// Müşteri paketi uçları: müşteri kartı Paketler sekmesi ve "Müşteriye paket sat" (manageListCustomerPackages, manageSellPackage),
/// Paketler · Müşteri paketleri (manageListAllCustomerPackages) ve "İade et" (manageRefundCustomerPackage).
/// </summary>
internal static class CustomerPackageEndpoints
{
    private enum ListFilter
    {
        Active,
        ExpiringSoon,
        Expired,
        All,
    }

    public static void Map(IEndpointRouteBuilder app)
    {
        var customers = app.MapApiGroup("/api/v1/manage/customers", Audiences.Manage);
        customers.MapGet("/{brandCustomerId:guid}/packages", ListForCustomerAsync).WithName("manageListCustomerPackages")
            .RequirePermission(Permissions.Customer.View);
        customers.MapPost("/{brandCustomerId:guid}/packages", SellAsync).WithName("manageSellPackage")
            .RequirePermission(Permissions.Package.Sell).RequireIdempotencyKey();

        var packages = app.MapApiGroup("/api/v1/manage/customer-packages", Audiences.Manage);
        packages.MapGet(string.Empty, ListAllAsync).WithName("manageListAllCustomerPackages").RequirePermission(Permissions.Package.Sell);
        packages.MapPost("/{customerPackageId:guid}/refund", RefundAsync).WithName("manageRefundCustomerPackage")
            .RequirePermission(Permissions.Package.Sell).RequireIfMatch().RequireIdempotencyKey();
    }

    // Kural: sözleşme manageListCustomerPackages (Active önce, sonra yeniden eskiye; kullanımlar dahil)
    private static async Task<IResult> ListForCustomerAsync(
        Guid brandCustomerId, int? limit, string? cursor, ITenantContext tenant, SalesDbContext db, ICustomerDirectory customers,
        PackageReader reader, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if ((await customers.GetDisplayInfoAsync(brandId, [brandCustomerId], ct)).Count == 0)
        {
            return Error.NotFound().ToProblem();
        }

        var query = db.CustomerPackages.AsNoTracking().Where(p => p.BrandId == brandId && p.BrandCustomerId == brandCustomerId)
            .OrderBy(p => p.Status != CustomerPackageStatus.Active).ThenByDescending(p => p.PurchasedUtc).ThenByDescending(p => p.Id);
        return await PageAsync(brandId, query, limit, cursor, withUsages: true, reader, ct);
    }

    // Kural: sözleşme manageListAllCustomerPackages
    private static async Task<IResult> ListAllAsync(
        Guid? branchId, string? filter, string? q, int? limit, string? cursor, HttpContext http, ITenantContext tenant, SalesDbContext db,
        ITenantQueries tenants, PackageReader reader, IClock clock, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if (!Enum.TryParse<ListFilter>(filter ?? nameof(ListFilter.Active), ignoreCase: false, out var parsed) || !Enum.IsDefined(parsed))
        {
            return Error.Validation("filter", "invalid_format").ToProblem();
        }

        if (q is { Length: > 80 })
        {
            return Error.Validation("q", "too_long").ToProblem();
        }

        var scope = new PackageScope(BranchScope.From(http.User), branchId);
        if (!scope.IsValid || (branchId is { } branch && !await tenants.BranchExistsAsync(brandId, branch, ct)))
        {
            return Error.NotFound().ToProblem();
        }

        var query = scope.Apply(db.CustomerPackages.AsNoTracking().Where(p => p.BrandId == brandId));
        if (!string.IsNullOrWhiteSpace(q))
        {
            var pattern = $"%{q.Trim()}%";
            query = query.Where(p => EF.Functions.ILike(p.PackageNameSnapshot, pattern));
        }

        var today = clock.GetCurrentInstant().InZone(await BranchCalendar.ZoneAsync(tenants, brandId, branchId, ct)).Date;
        var soon = today.PlusDays(PlatformDefaults.PackageExpiringSoonDays);
        IOrderedQueryable<CustomerPackage> ordered = parsed switch
        {
            ListFilter.ExpiringSoon => query.Where(p => p.Status == CustomerPackageStatus.Active && p.ExpiresOn >= today && p.ExpiresOn <= soon)
                .OrderBy(p => p.ExpiresOn).ThenBy(p => p.Id),
            ListFilter.Expired => query.Where(p => p.Status == CustomerPackageStatus.Expired)
                .OrderByDescending(p => p.ExpiresOn).ThenByDescending(p => p.Id),
            ListFilter.All => query.OrderBy(p => p.Status != CustomerPackageStatus.Active).ThenByDescending(p => p.PurchasedUtc).ThenByDescending(p => p.Id),
            _ => query.Where(p => p.Status == CustomerPackageStatus.Active).OrderByDescending(p => p.PurchasedUtc).ThenByDescending(p => p.Id),
        };
        return await PageAsync(brandId, ordered, limit, cursor, withUsages: false, reader, ct);
    }

    private static async Task<IResult> SellAsync(
        Guid brandCustomerId, PackageSaleRequest body, HttpContext http, ITenantContext tenant, PackageSales sales, PackageReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var sold = await sales.SellAsync(brandId, brandCustomerId, body, new PackageScope(BranchScope.From(http.User), null), UserId(http.User), ct);
        if (!sold.IsSuccess)
        {
            return sold.Error.ToProblem();
        }

        var package = sold.Value;
        http.Response.Headers.ETag = ETagVersion.Format(package.Version);
        var view = (await reader.MapAsync(brandId, [package], withUsages: false, ct))[0];
        return TypedResults.Created($"/api/v1/manage/customers/{brandCustomerId}/packages", view);
    }

    private static async Task<IResult> RefundAsync(
        Guid customerPackageId, PackageRefundRequest body, HttpContext http, ITenantContext tenant, PackageSales sales, PackageReader reader,
        CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var refunded = await sales.RefundAsync(
            brandId, customerPackageId, body, new PackageScope(BranchScope.From(http.User), null), http.GetIfMatchVersion(), UserId(http.User), ct);
        if (!refunded.IsSuccess)
        {
            return refunded.Error.ToProblem();
        }

        var package = refunded.Value;
        return VersionedResults.Versioned(http, package.Version, (await reader.MapAsync(brandId, [package], withUsages: false, ct))[0]);
    }

    private static async Task<IResult> PageAsync(
        Guid brandId, IOrderedQueryable<CustomerPackage> query, int? limit, string? cursor, bool withUsages, PackageReader reader, CancellationToken ct)
    {
        if (!PageCursor.TryLimit(limit, out var take))
        {
            return Error.Validation("limit", "out_of_range").ToProblem();
        }

        if (!PageCursor.TryDecode(cursor, out var offset))
        {
            return Error.Validation("cursor", "invalid_format").ToProblem();
        }

        var rows = await query.Include(p => p.Items).Skip(offset).Take(take + 1).ToListAsync(ct);
        var next = rows.Count > take ? PageCursor.Encode(offset + take) : null;
        var items = await reader.MapAsync(brandId, [.. rows.Take(take)], withUsages, ct);
        return TypedResults.Ok(new CustomerPackagePageView(items, next));
    }

    private static Guid? UserId(ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirst(OmvilaClaims.Subject)?.Value, out var id) ? id : null;
}
