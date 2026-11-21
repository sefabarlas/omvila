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
using Omvila.Modules.Catalog.Domain;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.Services;

/// <summary>
/// Hizmet uçları (sözleşme manage-catalog; Hizmetler, HizmetEkle; kabul kriterleri 7, 9). Yeni hizmet ve düzenleme
/// marka genelinde yetkili (Owner) içindir; şube yöneticisi yalnız kendi şubesinin fiyatını düzenler.
/// </summary>
internal static class ServiceEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/manage/services", Audiences.Manage);
        group.MapGet(string.Empty, ListAsync).WithName("manageListServices").RequireAuthenticated(Audiences.Manage);
        group.MapPost(string.Empty, CreateAsync).WithName("manageCreateService")
            .RequirePermission(Permissions.Service.Manage).RequireIdempotencyKey();
        group.MapGet("/{serviceId:guid}", GetAsync).WithName("manageGetService").RequireAuthenticated(Audiences.Manage);
        group.MapPut("/{serviceId:guid}", UpdateAsync).WithName("manageUpdateService")
            .RequirePermission(Permissions.Service.Manage).RequireIfMatch();
    }

    private static async Task<IResult> ListAsync(
        Guid? branchId, bool? includeInactive, ClaimsPrincipal user, ITenantContext tenant, CatalogDbContext db, ITenantQueries tenants, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        if (branchId is { } branch && (!BranchScope.From(user).Includes(branch) || !await tenants.BranchExistsAsync(brandId, branch, ct)))
        {
            return Error.NotFound().ToProblem();
        }

        var query = db.Services.AsNoTracking().Where(s => s.BrandId == brandId);
        if (includeInactive is not true)
        {
            query = query.Where(s => s.IsActive);
        }

        var services = await query.OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToListAsync(ct);
        var ids = services.Select(s => s.Id).ToList();
        var stages = (await db.Stages.AsNoTracking().Where(st => ids.Contains(st.ServiceId)).ToListAsync(ct))
            .ToLookup(st => st.ServiceId);
        var staff = (await db.ServiceStaff.AsNoTracking().Where(x => ids.Contains(x.ServiceId) && x.IsActive).ToListAsync(ct))
            .ToLookup(x => x.ServiceId);
        var prices = branchId is { } b
            ? await db.BranchPrices.AsNoTracking().Where(p => p.BranchId == b && ids.Contains(p.ServiceId)).ToDictionaryAsync(p => p.ServiceId, ct)
            : [];

        // Şubede verilmeyen hizmet ("isAvailable: false") o şubenin listesinde yer almaz.
        var items = services
            .Where(s => !prices.TryGetValue(s.Id, out var p) || p.IsAvailable)
            .Select(s => ServiceResponse.From(
                s,
                [.. stages[s.Id].OrderBy(st => st.SeqNo).Select(st => st.ToStage())],
                [.. staff[s.Id].Select(x => x.StaffId)],
                prices.GetValueOrDefault(s.Id),
                branchRequested: branchId is not null))
            .ToList();
        return TypedResults.Ok(new ServiceList(items));
    }

    private static async Task<IResult> GetAsync(Guid serviceId, HttpContext http, ITenantContext tenant, CatalogDbContext db, CancellationToken ct)
    {
        var service = await db.Services.AsNoTracking().SingleOrDefaultAsync(s => s.Id == serviceId && s.BrandId == tenant.RequireBrandId(), ct);
        return service is null
            ? Error.NotFound().ToProblem()
            : VersionedResults.Versioned(http, service.Version, await ToResponseAsync(db, service, ct));
    }

    private static async Task<IResult> CreateAsync(
        ServiceWriteRequest body, HttpContext http, ITenantContext tenant, CatalogDbContext db, ITenantQueries tenants,
        IBrandBankAccounts bankAccounts, IIdGenerator ids, IClock clock, CancellationToken ct)
    {
        if (!BranchScope.From(http.User).All)
        {
            return Error.Forbidden().ToProblem();
        }

        var brandId = tenant.RequireBrandId();
        var defaultSort = (await db.Services.Where(s => s.BrandId == brandId).MaxAsync(s => (int?)s.SortOrder, ct) ?? 0) + 1;
        var validated = await ValidateAsync(body, brandId, exceptId: null, defaultSort, db, tenants, bankAccounts, ct);
        if (!validated.IsSuccess)
        {
            return validated.Error.ToProblem();
        }

        var service = Service.Create(ids.New(), brandId, validated.Value, clock.GetCurrentInstant());
        db.Services.Add(service);
        await db.SaveChangesAsync(ct);
        await ReplaceChildrenAsync(db, ids, service, validated.Value.Draft.Stages, body.StaffIds!, ct);

        http.Response.Headers.ETag = ETagVersion.Format(service.Version);
        return TypedResults.Created($"/api/v1/manage/services/{service.Id}", await ToResponseAsync(db, service, ct));
    }

    private static async Task<IResult> UpdateAsync(
        Guid serviceId, ServiceWriteRequest body, HttpContext http, ITenantContext tenant, CatalogDbContext db, ITenantQueries tenants,
        IBrandBankAccounts bankAccounts, IIdGenerator ids, CancellationToken ct)
    {
        var brandId = tenant.RequireBrandId();
        var service = await db.Services.SingleOrDefaultAsync(s => s.Id == serviceId && s.BrandId == brandId, ct);
        if (service is null)
        {
            return Error.NotFound().ToProblem();
        }

        if (!BranchScope.From(http.User).All)
        {
            return Error.Forbidden().ToProblem();
        }

        if (!db.MatchesVersion(service, service.Version, http.GetIfMatchVersion()))
        {
            return Error.PreconditionFailed().ToProblem();
        }

        var validated = await ValidateAsync(body, brandId, serviceId, service.SortOrder, db, tenants, bankAccounts, ct);
        if (!validated.IsSuccess)
        {
            return validated.Error.ToProblem();
        }

        service.Update(validated.Value);
        db.Entry(service).State = EntityState.Modified;
        await db.SaveChangesAsync(ct);
        await ReplaceChildrenAsync(db, ids, service, validated.Value.Draft.Stages, body.StaffIds!, ct);
        return VersionedResults.Versioned(http, service.Version, await ToResponseAsync(db, service, ct));
    }

    private static async Task<Result<ServiceDetails>> ValidateAsync(
        ServiceWriteRequest body, Guid brandId, Guid? exceptId, int defaultSort, CatalogDbContext db, ITenantQueries tenants,
        IBrandBankAccounts bankAccounts, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var details = body.ToDetails(defaultSort, errors);
        if (details is null)
        {
            return Error.Validation(errors);
        }

        // Kural: alan-modeli.md § BrandBankAccount — "Her zaman iste" kaporası aktif hesap (IBAN) ister.
        var rules = ServiceRules.Validate(details.Draft, await bankAccounts.HasActiveAsync(brandId, ct));
        if (!rules.IsSuccess)
        {
            return rules.Error;
        }

        if (!await db.Categories.AnyAsync(c => c.Id == details.CategoryId && c.BrandId == brandId, ct))
        {
            errors["categoryId"] = ["invalid_format"];
        }

        var name = details.Draft.Name.Trim();
        if (details.IsActive && await db.Services.AnyAsync(s => s.BrandId == brandId && s.IsActive && s.Name == name && s.Id != exceptId, ct))
        {
            errors["name"] = ["already_exists"];
        }

        var staffIds = body.StaffIds!.Distinct().ToList();
        var active = await tenants.GetActiveStaffIdsAsync(brandId, staffIds, ct);
        if (staffIds.Any(id => !active.Contains(id)))
        {
            errors["staffIds"] = ["invalid_format"];
        }

        return errors.Count > 0 ? Error.Validation(errors) : details;
    }

    // Aşamalar baştan yazılır. Personel: çıkarılan bağ silinir, kalanın personele özel süre ve ücreti korunur.
    private static async Task ReplaceChildrenAsync(
        CatalogDbContext db, IIdGenerator ids, Service service, IReadOnlyList<ServiceStage> stages, IReadOnlyList<Guid> staffIds, CancellationToken ct)
    {
        await db.Stages.Where(st => st.ServiceId == service.Id).ExecuteDeleteAsync(ct);
        db.Stages.AddRange(stages.Select((stage, i) => ServiceStageRow.Create(ids.New(), service.BrandId, service.Id, i + 1, stage)));

        var wanted = staffIds.Distinct().ToHashSet();
        await db.ServiceStaff.Where(x => x.ServiceId == service.Id && !wanted.Contains(x.StaffId)).ExecuteDeleteAsync(ct);
        var existing = await db.ServiceStaff.Where(x => x.ServiceId == service.Id).Select(x => x.StaffId).ToListAsync(ct);
        db.ServiceStaff.AddRange(wanted.Except(existing).Select(staffId => ServiceStaff.Create(ids.New(), service.BrandId, service.Id, staffId)));
        await db.SaveChangesAsync(ct);
    }

    private static async Task<ServiceResponse> ToResponseAsync(CatalogDbContext db, Service service, CancellationToken ct)
    {
        var stages = await db.Stages.AsNoTracking().Where(st => st.ServiceId == service.Id).OrderBy(st => st.SeqNo).ToListAsync(ct);
        var staff = await db.ServiceStaff.AsNoTracking().Where(x => x.ServiceId == service.Id && x.IsActive).Select(x => x.StaffId).ToListAsync(ct);
        return ServiceResponse.From(service, [.. stages.Select(st => st.ToStage())], staff, branchPrice: null, branchRequested: false);
    }
}
