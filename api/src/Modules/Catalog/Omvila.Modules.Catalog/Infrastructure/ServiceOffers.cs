using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Catalog.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Infrastructure;

/// <summary>
/// Randevu kalemi için fiyat ve süre çözümü (<see cref="OfferResolver"/>). İstek sayısından bağımsız dört sorgu
/// (hizmet, aşama, şube fiyatı, personel farkı); döngü içinde sorgu yok.
/// </summary>
internal sealed class ServiceOffers(CatalogDbContext db) : IServiceOffers
{
    public async Task<Result<ServiceOffer>> ResolveAsync(Guid brandId, Guid branchId, Guid serviceId, Guid? staffId, CancellationToken ct) =>
        (await ResolveAsync(brandId, branchId, [new ServiceOfferRequest(serviceId, staffId)], ct))[0];

    public async Task<IReadOnlyList<Result<ServiceOffer>>> ResolveAsync(
        Guid brandId, Guid branchId, IReadOnlyList<ServiceOfferRequest> requests, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(requests);
        if (requests.Count == 0)
        {
            return [];
        }

        var serviceIds = requests.Select(r => r.ServiceId).Distinct().ToList();
        var staffIds = requests.Where(r => r.StaffId is not null).Select(r => r.StaffId!.Value).Distinct().ToList();

        var services = await db.Services.AsNoTracking()
            .Where(s => s.BrandId == brandId && s.IsActive && serviceIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, ct);
        var stages = (await db.Stages.AsNoTracking()
                .Where(st => st.BrandId == brandId && serviceIds.Contains(st.ServiceId))
                .ToListAsync(ct))
            .ToLookup(st => st.ServiceId);
        var branchPrices = await db.BranchPrices.AsNoTracking()
            .Where(p => p.BrandId == brandId && p.BranchId == branchId && serviceIds.Contains(p.ServiceId))
            .ToDictionaryAsync(p => p.ServiceId, ct);
        var assignments = staffIds.Count == 0
            ? []
            : await db.ServiceStaff.AsNoTracking()
                .Where(x => x.BrandId == brandId && x.IsActive && serviceIds.Contains(x.ServiceId) && staffIds.Contains(x.StaffId))
                .ToDictionaryAsync(x => (x.ServiceId, x.StaffId), ct);

        var results = new List<Result<ServiceOffer>>(requests.Count);
        foreach (var request in requests)
        {
            results.Add(Resolve(request, services, stages, branchPrices, assignments));
        }

        return results;
    }

    private static Result<ServiceOffer> Resolve(
        ServiceOfferRequest request,
        Dictionary<Guid, Service> services,
        ILookup<Guid, ServiceStageRow> stages,
        Dictionary<Guid, BranchServicePrice> branchPrices,
        Dictionary<(Guid ServiceId, Guid StaffId), ServiceStaff> assignments)
    {
        if (!services.TryGetValue(request.ServiceId, out var service))
        {
            return Error.NotFound();
        }

        StaffServiceOverride? staffOverride = null;
        var surcharge = Money.Zero(service.BasePrice.Currency);
        if (request.StaffId is { } staffId)
        {
            // Değişmez: hizmeti yalnız ServiceStaff listesindeki aktif personel verir (alan-modeli.md § ServiceStaff).
            if (!assignments.TryGetValue((service.Id, staffId), out var assignment))
            {
                return Error.NotFound();
            }

            surcharge = new Money(assignment.SurchargeMinor, service.BasePrice.Currency);
            staffOverride = new StaffServiceOverride(assignment.DurationOverrideMinutes, assignment.SurchargeMinor > 0 ? surcharge : null);
        }

        var branch = branchPrices.TryGetValue(service.Id, out var row)
            ? new BranchPrice(new Money(row.PriceMinor, service.BasePrice.Currency), row.DurationOverrideMinutes, row.IsAvailable)
            : null;
        var serviceStages = stages[service.Id].OrderBy(st => st.SeqNo).Select(st => st.ToStage()).ToList();
        var resolved = OfferResolver.Resolve(service.BasePrice, service.DurationMinutes, serviceStages, branch, staffOverride);
        if (!resolved.IsSuccess)
        {
            return resolved.Error;
        }

        var offer = resolved.Value;
        // Aşaması olmayan hizmet tek örtük aşamalıdır (alan-modeli.md § ServiceStage).
        IReadOnlyList<ServiceOfferStage> offerStages = offer.Stages.Count == 0
            ? [new ServiceOfferStage(service.Name, offer.DurationMinutes, StaffBusy: true)]
            : [.. offer.Stages.Select(st => new ServiceOfferStage(st.Name, st.DurationMinutes, st.StaffBusy))];

        return new ServiceOffer(
            service.Id,
            request.StaffId,
            service.Name,
            offer.Price,
            surcharge,
            offer.IsBranchPrice,
            service.PriceDisplay == PriceDisplay.StartingFrom ? ServicePriceDisplay.StartingFrom : ServicePriceDisplay.Fixed,
            offer.DurationMinutes,
            service.BufferBeforeMinutes,
            service.BufferAfterMinutes,
            offerStages,
            service.RequiresApproval,
            service.IsOnlineBookable,
            service.MinNoticeMinutes,
            service.MaxAdvanceDays);
    }
}
