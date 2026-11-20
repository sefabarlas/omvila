namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Kurulan markanın kimlikleri.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="Slug">Rezervasyon bağlantı adı.</param>
/// <param name="BranchId">İlk şube ("Merkez").</param>
/// <param name="OwnerStaffId">Sahibin personel kaydı.</param>
public sealed record ProvisionedTenant(Guid BrandId, string Slug, Guid BranchId, Guid OwnerStaffId);
