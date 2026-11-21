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
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.Modules.Sales.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Packages;

/// <summary>
/// Paket tanımı uçları (sözleşme manage-packages: manageListPackages, manageCreatePackage, manageGetPackage, manageUpdatePackage;
/// Paketler, PaketlerBos, PaketOlustur). Okuma "package.manage veya package.sell" ister: package.manage yalnız Owner'da ve Owner
/// package.sell'e de sahip olduğundan okuma package.sell ile korunur. Yazma package.manage (Owner).
/// </summary>
internal static class PackageEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/packages", Audiences.Manage);
        group.MapGet(string.Empty, ListAsync).WithName("manageListPackages").RequirePermission(Permissions.Package.Sell);
        group.MapPost(string.Empty, CreateAsync).WithName("manageCreatePackage")
            .RequirePermission(Permissions.Package.Manage).RequireIdempotencyKey();
        group.MapGet("/{packageId:guid}", GetAsync).WithName("manageGetPackage").RequirePermission(Permissions.Package.Sell);
        group.MapPut("/{packageId:guid}", UpdateAsync).WithName("manageUpdatePackage")
            .RequirePermission(Permissions.Package.Manage).RequireIfMatch();
    }

    private static async Task<IResult> ListAsync(
        Guid? branchId, HttpContext http, ITenantContext tenant, PackageReader reader, ITenantQueries tenants, IClock clock, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var scope = new PackageScope(BranchScope.From(http.User), branchId);
        if (!scope.IsValid || (branchId is { } branch && !await tenants.BranchExistsAsync(brandId, branch, ct)))
        {
            return Error.NotFound().ToProblem();
        }

        var zone = await BranchCalendar.ZoneAsync(tenants, brandId, branchId, ct);
        var currency = await CurrencyAsync(tenants, brandId, ct);
        return TypedResults.Ok(await reader.ListDefinitionsAsync(brandId, scope, clock.GetCurrentInstant().InZone(zone).Date, zone, currency, ct));
    }

    private static async Task<IResult> GetAsync(Guid packageId, HttpContext http, ITenantContext tenant, SalesDbContext db, PackageReader reader, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var package = await db.Packages.AsNoTracking().Include(p => p.Items).SingleOrDefaultAsync(p => p.Id == packageId && p.BrandId == brandId, ct);
        return package is null
            ? Error.NotFound().ToProblem()
            : VersionedResults.Versioned(http, package.Version, await reader.MapDefinitionAsync(package, ct));
    }

    // Kural: sözleşme manageCreatePackage; alan-modeli.md § Package
    private static async Task<IResult> CreateAsync(
        PackageWriteRequest body, HttpContext http, ITenantContext tenant, SalesDbContext db, PackageReader reader, ITenantQueries tenants,
        IServiceDirectory services, IIdGenerator ids, IClock clock, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var draft = await DraftAsync(body, brandId, tenants, services, ct);
        if (!draft.IsSuccess)
        {
            return draft.Error.ToProblem();
        }

        var package = Package.Create(ids, brandId, draft.Value, clock.GetCurrentInstant());
        db.Packages.Add(package);
        await db.SaveChangesAsync(ct);
        http.Response.Headers.ETag = ETagVersion.Format(package.Version);
        return TypedResults.Created($"/api/v1/manage/packages/{package.Id}", await reader.MapDefinitionAsync(package, ct));
    }

    // Kural: sözleşme manageUpdatePackage (yalnız sonraki satışları etkiler; paketler silinmez)
    private static async Task<IResult> UpdateAsync(
        Guid packageId, PackageWriteRequest body, HttpContext http, ITenantContext tenant, SalesDbContext db, PackageReader reader,
        ITenantQueries tenants, IServiceDirectory services, IIdGenerator ids, IClock clock, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var package = await db.Packages.Include(p => p.Items).SingleOrDefaultAsync(p => p.Id == packageId && p.BrandId == brandId, ct);
        if (package is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (!db.MatchesVersion(package, package.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var draft = await DraftAsync(body, brandId, tenants, services, ct);
        if (!draft.IsSuccess)
        {
            return draft.Error.ToProblem();
        }

        db.PackageItems.RemoveRange(package.Update(ids, draft.Value, clock.GetCurrentInstant()));
        db.Entry(package).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        return VersionedResults.Versioned(http, package.Version, await reader.MapDefinitionAsync(package, ct));
    }

    private static async Task<Result<PackageDraft>> DraftAsync(
        PackageWriteRequest body, Guid brandId, ITenantQueries tenants, IServiceDirectory services, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        var currency = await CurrencyAsync(tenants, brandId, ct);
        var summaries = await services.GetSummariesAsync(brandId, body.ServiceIds, ct);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var draft = body.ToDraft(currency, summaries.Values.Where(s => s.IsActive).Select(s => s.ServiceId).ToHashSet(), errors);
        return draft is null ? Error.Validation(errors) : draft;
    }

    internal static async Task<Currency> CurrencyAsync(ITenantQueries tenants, Guid brandId, CancellationToken ct) =>
        Enum.TryParse<Currency>((await tenants.GetCustomerSettingsAsync(brandId, ct))?.CurrencyCode, out var currency)
            ? currency
            : PlatformDefaults.DefaultCurrency;
}
