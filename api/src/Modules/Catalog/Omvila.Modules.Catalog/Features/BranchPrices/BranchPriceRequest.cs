using Omvila.Modules.Catalog.Features.Services;

namespace Omvila.Modules.Catalog.Features.BranchPrices;

/// <summary>Sözleşme BranchServicePrice (istek ve yanıt).</summary>
internal sealed record BranchPriceRequest(MoneyDto? Price, int? DurationOverrideMinutes, bool? IsAvailable);
