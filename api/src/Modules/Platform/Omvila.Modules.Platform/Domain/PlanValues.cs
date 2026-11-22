namespace Omvila.Modules.Platform.Domain;

/// <summary>Planın düzenlenebilir değerleri.</summary>
internal sealed record PlanValues(
    string Name,
    string? Description,
    long? MonthlyPriceMinor,
    long? YearlyPriceMinor,
    int? IncludedBranchCount,
    long? ExtraBranchPriceMinor,
    int? StaffLimit,
    int? IncludedSmsCredits,
    PlanFeatures Features,
    bool IsPublic,
    bool IsActive);
