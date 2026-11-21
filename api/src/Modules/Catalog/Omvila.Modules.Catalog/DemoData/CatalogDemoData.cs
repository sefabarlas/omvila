using NodaTime;
using Omvila.Modules.Catalog.Domain;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-74) için Catalog yazma yolu: kategori ve hizmet (hizmet ekleme ucundaki
/// alan kuralı <see cref="ServiceRules"/> ile). Kapora kuralı markadan (Inherit; tutar veya yüzde verilebilir), aşama yok. Çağıranın transaction'ında ve
/// markanın kiracı bağlamında çalışır.
/// </summary>
public sealed class CatalogDemoData
{
    private readonly CatalogDbContext _db;
    private readonly IIdGenerator _ids;
    private readonly IClock _clock;
    private int _categorySort;
    private int _serviceSort;

    internal CatalogDemoData(CatalogDbContext db, IIdGenerator ids, IClock clock)
    {
        _db = db;
        _ids = ids;
        _clock = clock;
    }

    public async Task<Guid> AddCategoryAsync(Guid brandId, string name, CancellationToken ct)
    {
        var category = ServiceCategory.Create(_ids.New(), brandId, name, ++_categorySort, _clock.GetCurrentInstant());
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return category.Id;
    }

    public async Task<Guid> AddServiceAsync(Guid brandId, DemoService input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var draft = new ServiceDraft(
            input.Name, input.DurationMinutes, input.Price, input.StartingFrom ? PriceDisplay.StartingFrom : PriceDisplay.Fixed, 0, 0,
            new DepositRule(DepositMode.Inherit, input.DepositAmount, input.DepositPercent), []);
        var valid = ServiceRules.Validate(draft, hasActiveBankAccount: false);
        if (!valid.IsSuccess)
        {
            throw new DomainException($"Geçersiz demo hizmeti: {valid.Error.Code}");
        }

        var details = new ServiceDetails(
            input.CategoryId, draft, input.Description, input.RequiresApproval, input.IsOnlineBookable, null, null, ++_serviceSort, IsActive: true);
        var service = Service.Create(_ids.New(), brandId, details, _clock.GetCurrentInstant());
        _db.Services.Add(service);
        await _db.SaveChangesAsync(ct);
        _db.ServiceStaff.AddRange(input.StaffIds.Distinct().Select(staffId => ServiceStaff.Create(_ids.New(), brandId, service.Id, staffId)));
        await _db.SaveChangesAsync(ct);
        return service.Id;
    }
}
