using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Modules.Catalog.Domain;
using Omvila.Modules.Catalog.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.PublicCatalog;

/// <summary>
/// Rezervasyon sayfasının hizmet ve personel listesi (sözleşme publicGetBranchCatalog; ekranlar Main, Personel). Kiracı
/// bağlamı yoldaki şubeden kurulur (IPublicBookingQueries.EnterBranchAsync). Listelenen hizmet: etkin, online açık, şubede
/// verilen ve şubede online açık en az bir personelin verdiği hizmet. Fiyat ve süre şubenin değeridir; fiyatlar gizliyse
/// fiyat null döner (personel ek ücreti de yazılmaz).
/// </summary>
internal static class PublicCatalogEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/public/branches");

        // Rezervasyon sayfası üyeliksizdir (ürün kuralı 7): hizmet listesi herkese açık bağlantıyla görüntülenir.
        group.MapGet("/{branchId:guid}/catalog", GetAsync).WithName("publicGetBranchCatalog").AllowAnonymous();
    }

    private static async Task<IResult> GetAsync(Guid branchId, IPublicBookingQueries booking, CatalogDbContext db, CancellationToken ct)
    {
        if (await booking.EnterBranchAsync(branchId, ct) is not { } scope)
        {
            return Error.NotFound().ToProblem();
        }

        if (!scope.BookingEnabled)
        {
            return Error.Forbidden(ErrorCodes.BookingDisabled).ToProblem();
        }

        var brandId = scope.BrandId;
        var options = await booking.GetBookingOptionsAsync(brandId, ct);
        var staff = await booking.GetBookableStaffAsync(brandId, branchId, ct);
        var staffIds = staff.Select(s => s.StaffId).ToList();

        var services = await db.Services.AsNoTracking()
            .Where(s => s.BrandId == brandId && s.IsActive && s.IsOnlineBookable)
            .ToListAsync(ct);
        var serviceIds = services.Select(s => s.Id).ToList();
        var branchPrices = await db.BranchPrices.AsNoTracking()
            .Where(p => p.BrandId == brandId && p.BranchId == branchId && serviceIds.Contains(p.ServiceId))
            .ToDictionaryAsync(p => p.ServiceId, ct);
        var assignments = (await db.ServiceStaff.AsNoTracking()
                .Where(x => x.BrandId == brandId && x.IsActive && serviceIds.Contains(x.ServiceId) && staffIds.Contains(x.StaffId))
                .ToListAsync(ct))
            .ToLookup(x => x.ServiceId);
        var categories = await db.Categories.AsNoTracking()
            .Where(c => c.BrandId == brandId && c.IsActive)
            .ToDictionaryAsync(c => c.Id, ct);

        var listed = services
            .Where(s => categories.ContainsKey(s.CategoryId)
                && (!branchPrices.TryGetValue(s.Id, out var price) || price.IsAvailable)
                && assignments[s.Id].Any())
            .OrderBy(s => categories[s.CategoryId].SortOrder)
            .ThenBy(s => s.SortOrder)
            .ThenBy(s => s.Name, StringComparer.Ordinal)
            .ToList();
        var serviceItems = ServiceItems(listed, branchPrices, assignments, staffIds, options);
        var staffItems = StaffItems(listed, assignments, staff, options.ShowPrices);

        var categoryItems = listed.Select(s => s.CategoryId).Distinct()
            .Select(id => new PublicCatalogResponse.Category(id, categories[id].Name))
            .ToList();

        return TypedResults.Ok(new PublicCatalogResponse(
            branchId, scope.TimeZoneId, options.AllowStaffSelection, options.AllowAnyStaffOption, PriceNote: null, categoryItems, serviceItems, staffItems));
    }

    private static List<PublicCatalogResponse.ServiceItem> ServiceItems(
        List<Service> listed,
        Dictionary<Guid, BranchServicePrice> branchPrices,
        ILookup<Guid, ServiceStaff> assignments,
        List<Guid> staffIds,
        PublicBookingOptions options)
    {
        var staffOrder = staffIds.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index);
        return [.. listed.Select(s =>
        {
            var branch = branchPrices.GetValueOrDefault(s.Id);
            var price = branch is null ? s.BasePrice : new Money(branch.PriceMinor, s.BasePrice.Currency);
            var deposit = DepositResolver.ResolveHint(s.Deposit, price, options.DepositAvailable, ParsePolicy(options.DepositPolicy));
            return new PublicCatalogResponse.ServiceItem(
                s.Id,
                s.CategoryId,
                s.Name,
                s.Description,
                branch?.DurationOverrideMinutes ?? s.DurationMinutes,
                options.ShowPrices ? price : null,
                s.PriceDisplay == PriceDisplay.StartingFrom ? "StartingFrom" : "Fixed",
                HasDeposit: deposit is not null,
                // Fiyatlar gizliyken yüzde kaporanın tutarı fiyatı açık eder; tutar yalnız sabit kaporada yazılır (T-78).
                DepositHint: options.ShowPrices || s.Deposit.Amount is not null ? deposit : null,
                [.. assignments[s.Id].Select(a => a.StaffId).OrderBy(id => staffOrder[id])]);
        })];
    }

    private static BrandDepositPolicy ParsePolicy(string policy) =>
        Enum.TryParse<BrandDepositPolicy>(policy, ignoreCase: false, out var parsed) ? parsed : BrandDepositPolicy.None;

    /// <summary>Listelenen hizmetlerden en az birini veren personel; ek ücretler fiyatlar görünürse yazılır.</summary>
    private static List<PublicCatalogResponse.StaffItem> StaffItems(
        List<Service> listed, ILookup<Guid, ServiceStaff> assignments, IReadOnlyList<PublicStaffCard> staff, bool showPrices)
    {
        var currencyByService = listed.ToDictionary(s => s.Id, s => s.BasePrice.Currency);
        var givers = listed.SelectMany(s => assignments[s.Id]).ToLookup(a => a.StaffId);
        return [.. staff
            .Where(s => givers[s.StaffId].Any())
            .Select(s => new PublicCatalogResponse.StaffItem(
                s.StaffId,
                s.DisplayName,
                s.Title,
                s.PhotoUrl,
                showPrices
                    ? [.. givers[s.StaffId]
                        .Where(a => a.SurchargeMinor > 0)
                        .Select(a => new PublicCatalogResponse.Surcharge(a.ServiceId, new Money(a.SurchargeMinor, currencyByService[a.ServiceId])))]
                    : []))];
    }
}
