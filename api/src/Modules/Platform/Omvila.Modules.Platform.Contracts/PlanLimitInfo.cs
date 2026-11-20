namespace Omvila.Modules.Platform.Contracts;

/// <summary>Markanın planındaki sınırlar (alan-modeli.md § Plan). Boş = sınırsız.</summary>
/// <param name="StaffLimit">Personel sınırı (sahip dahil).</param>
/// <param name="IncludedBranchCount">Plana dahil şube sayısı.</param>
/// <param name="AllowsExtraBranches">Dahil sayının üstüne ücretli ek şube açılabilir mi.</param>
public sealed record PlanLimitInfo(int? StaffLimit, int? IncludedBranchCount, bool AllowsExtraBranches);
