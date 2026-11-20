using System.Globalization;
using Omvila.Modules.Catalog.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Features.Services;

/// <summary>Sözleşme Service. Şube verilerek listelenince fiyat ve süre şubeye göre çözülür.</summary>
internal sealed record ServiceResponse(
    Guid Id,
    Guid CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    Money Price,
    string PriceDisplay,
    bool? IsBranchPrice,
    int BufferBeforeMinutes,
    int BufferAfterMinutes,
    DepositDto Deposit,
    bool? RequiresApproval,
    int? MinNoticeMinutes,
    int? MaxAdvanceDays,
    bool IsOnlineBookable,
    bool IsActive,
    int SortOrder,
    IReadOnlyList<StageDto> Stages,
    IReadOnlyList<Guid> StaffIds,
    string Version)
{
    public static ServiceResponse From(Service s, IReadOnlyList<ServiceStage> stages, IReadOnlyList<Guid> staffIds, BranchServicePrice? branchPrice, bool branchRequested)
    {
        var offer = OfferResolver.Resolve(
            s.BasePrice,
            s.DurationMinutes,
            stages,
            branchPrice is null ? null : new BranchPrice(new Money(branchPrice.PriceMinor, s.BasePrice.Currency), branchPrice.DurationOverrideMinutes, branchPrice.IsAvailable),
            staff: null);
        var price = offer.IsSuccess ? offer.Value.Price : s.BasePrice;
        var duration = offer.IsSuccess ? offer.Value.DurationMinutes : s.DurationMinutes;
        var resolvedStages = offer.IsSuccess ? offer.Value.Stages : stages;
        return new ServiceResponse(
            s.Id, s.CategoryId, s.Name, s.Description, duration, price, s.PriceDisplay.ToString(),
            branchRequested ? branchPrice is not null : null,
            s.BufferBeforeMinutes, s.BufferAfterMinutes, DepositDto.From(s.Deposit), s.RequiresApproval, s.MinNoticeMinutes, s.MaxAdvanceDays,
            s.IsOnlineBookable, s.IsActive, s.SortOrder,
            [.. resolvedStages.Select(st => new StageDto(st.Name, st.DurationMinutes, st.StaffBusy))],
            staffIds,
            s.Version.ToString(CultureInfo.InvariantCulture));
    }
}
