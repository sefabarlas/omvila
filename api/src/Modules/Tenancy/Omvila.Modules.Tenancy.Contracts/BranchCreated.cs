using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>
/// Şube eklendi ("Şube ekle"). <paramref name="CopyServicePricesFromBranchId"/> doluysa hizmet fiyatları o şubeden
/// kopyalanır (Catalog işler).
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="BranchId">Yeni şube.</param>
/// <param name="CopyServicePricesFromBranchId">"Hizmet ve fiyatlar: …'den kopyala".</param>
public sealed record BranchCreated(Guid BrandId, Guid BranchId, Guid? CopyServicePricesFromBranchId) : IIntegrationEvent;
