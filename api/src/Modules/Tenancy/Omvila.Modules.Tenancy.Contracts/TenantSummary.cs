namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Oturum için marka özeti.</summary>
/// <param name="BrandId">Marka.</param>
/// <param name="Name">İşletme adı.</param>
/// <param name="Slug">Bağlantı adı.</param>
/// <param name="LogoUrl">Logo.</param>
/// <param name="Status">Onboarding, Active, Suspended, Archived.</param>
/// <param name="Branches">Kapsamdaki etkin şubeler (oluşturulma sırasıyla; ilk şube başta).</param>
public sealed record TenantSummary(Guid BrandId, string Name, string Slug, string? LogoUrl, string Status, IReadOnlyList<BranchSummary> Branches);
