using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>Kaleme kopyalanacak fiyat, süre ve aşamalar; <see cref="IsBranchPrice"/> listede şube fiyatı rozetidir.</summary>
internal sealed record ResolvedOffer(Money Price, bool IsBranchPrice, int DurationMinutes, IReadOnlyList<ServiceStage> Stages);
