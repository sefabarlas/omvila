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
using Omvila.Modules.Catalog.Features.Services;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.Templates;

/// <summary>
/// Sektör hazır listesi (HizmetlerBos "Hazır listeden başla"; kabul kriteri 8). Seçilen şablonlar fiyatla hizmete
/// çevrilir, olmayan kategoriler oluşturulur (TemplatePlanner). Hizmeti isteği yapan kullanıcının personel kaydı verir.
/// </summary>
internal static class TemplateEndpoints
{
    // Sözleşme IndustryCode.
    private static readonly HashSet<string> IndustryCodes =
        new(["barber", "hair_salon", "beauty", "spa", "nail", "dietitian", "other"], StringComparer.Ordinal);

    // TemplatePlanner ile aynı karşılaştırma: kategori adı Türkçe kurallarla büyük/küçük harf duyarsız.
    private static readonly StringComparer TurkishIgnoreCase = StringComparer.Create(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"), ignoreCase: true);

    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapApiGroup("/api/v1/manage/industries", Audiences.Manage)
            .MapGet("/{industryCode}/service-templates", ListAsync).WithName("manageListServiceTemplates").RequireAuthenticated(Audiences.Manage);
        app.MapApiGroup("/api/v1/manage/services/from-templates", Audiences.Manage)
            .MapPost(string.Empty, CreateAsync).WithName("manageCreateServicesFromTemplates")
            .RequirePermission(Permissions.Service.Manage).RequireIdempotencyKey();
    }

    private static async Task<IResult> ListAsync(string industryCode, CatalogDbContext db, CancellationToken ct)
    {
        if (!IndustryCodes.Contains(industryCode))
        {
            return Error.NotFound().ToProblem();
        }

        var items = await db.Templates.AsNoTracking().Where(t => t.IndustryCode == industryCode && t.IsActive)
            .OrderBy(t => t.SortOrder).Select(t => new TemplateResponse(t.Id, t.CategoryName, t.Name, t.DefaultDurationMinutes)).ToListAsync(ct);
        return TypedResults.Ok(new TemplateList(items));
    }

    private static async Task<IResult> CreateAsync(
        FromTemplatesRequest body, HttpContext http, ITenantContext tenant, CatalogDbContext db, ITenantQueries tenants,
        IIdGenerator ids, IClock clock, CancellationToken ct)
    {
        if (!BranchScope.From(http.User).All)
        {
            return Error.Forbidden().ToProblem();
        }

        var brandId = tenant.RequireBrandId();
        var parsed = await ParseAsync(body, brandId, db, ct);
        if (!parsed.IsSuccess)
        {
            return parsed.Error.ToProblem();
        }

        var categories = await db.Categories.Where(c => c.BrandId == brandId).ToListAsync(ct);
        var plan = TemplatePlanner.Plan(parsed.Value, [.. categories.Select(c => c.Name)]);
        if (!plan.IsSuccess)
        {
            return plan.Error.ToProblem();
        }

        var now = clock.GetCurrentInstant();
        var nextCategorySort = categories.Count == 0 ? 1 : categories.Max(c => c.SortOrder) + 1;
        foreach (var name in plan.Value.NewCategories)
        {
            var category = ServiceCategory.Create(ids.New(), brandId, name, nextCategorySort++, now);
            db.Categories.Add(category);
            categories.Add(category);
        }

        var staffIds = await OwnStaffAsync(http, brandId, tenants, ct);
        var nextServiceSort = (await db.Services.Where(s => s.BrandId == brandId).MaxAsync(s => (int?)s.SortOrder, ct) ?? 0) + 1;
        var created = new List<Service>();
        foreach (var planned in plan.Value.Services)
        {
            var categoryId = categories.First(c => TurkishIgnoreCase.Equals(c.Name, planned.CategoryName)).Id;
            var draft = new ServiceDraft(planned.Name, planned.DurationMinutes, planned.Price, PriceDisplay.Fixed, 0, 0, DepositRule.Inherit, []);
            var service = Service.Create(ids.New(), brandId, new ServiceDetails(categoryId, draft, null, null, true, null, null, nextServiceSort++, true), now);
            db.Services.Add(service);
            db.ServiceStaff.AddRange(staffIds.Select(staffId => ServiceStaff.Create(ids.New(), brandId, service.Id, staffId)));
            created.Add(service);
        }

        await db.SaveChangesAsync(ct);
        return TypedResults.Created(
            "/api/v1/manage/services",
            new ServiceList([.. created.Select(s => ServiceResponse.From(s, [], staffIds, branchPrice: null, branchRequested: false))]));
    }

    private static async Task<Result<IReadOnlyList<TemplateSelection>>> ParseAsync(FromTemplatesRequest body, Guid brandId, CatalogDbContext db, CancellationToken ct)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var items = body.Items ?? [];
        if (items.Count is 0 or > 100)
        {
            return Error.Validation("items", items.Count == 0 ? "required" : "too_long");
        }

        var templateIds = items.Where(i => i.TemplateId is not null).Select(i => i.TemplateId!.Value).ToList();
        var templates = await db.Templates.AsNoTracking().Where(t => templateIds.Contains(t.Id) && t.IsActive).ToDictionaryAsync(t => t.Id, ct);
        var activeNames = (await db.Services.AsNoTracking().Where(s => s.BrandId == brandId && s.IsActive).Select(s => s.Name).ToListAsync(ct))
            .ToHashSet(StringComparer.Ordinal);
        var selections = new List<TemplateSelection>();
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i].TemplateId is not { } id || !templates.TryGetValue(id, out var template))
            {
                errors[$"items[{i}].templateId"] = [items[i].TemplateId is null ? "required" : "invalid_format"];
                continue;
            }

            // Aynı adla aktif hizmet varsa (aynı şablon iki kez seçildiyse de) eklenmez.
            if (!activeNames.Add(template.Name))
            {
                errors[$"items[{i}].templateId"] = ["already_exists"];
                continue;
            }

            var price = items[i].Price is { } p ? p.ToMoney($"items[{i}].price", errors) : null;
            if (price is { AmountMinor: 0 })
            {
                // S-24: sıfır fiyatla hizmet oluşturulmaz.
                errors[$"items[{i}].price.amountMinor"] = ["out_of_range"];
            }

            selections.Add(new TemplateSelection(id, template.CategoryName, template.Name, template.DefaultDurationMinutes, price, items[i].DurationMinutes));
        }

        return errors.Count > 0 ? Error.Validation(errors) : selections;
    }

    private static async Task<IReadOnlyList<Guid>> OwnStaffAsync(HttpContext http, Guid brandId, ITenantQueries tenants, CancellationToken ct)
    {
        if (!Guid.TryParse(http.User.FindFirst(OmvilaClaims.StaffId)?.Value, out var staffId))
        {
            return [];
        }

        return (await tenants.GetActiveStaffIdsAsync(brandId, [staffId], ct)).ToList();
    }
}
