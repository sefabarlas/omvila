using System.Text.Json.Serialization;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.AdminPlans;

/// <summary>Planlar ve kontör paketleri uçlarının kayıtları (sözleşme admin-plans).</summary>
internal static class AdminPlanViews
{
    /// <summary>Sözleşme AdminPlanCatalog.</summary>
    public sealed record AdminPlanCatalog(IReadOnlyList<AdminPlan> Plans, PlanRules Rules, string Version);

    /// <summary>Ortak kurallar (yapılandırmadan).</summary>
    public sealed record PlanRules(int TrialDays, int PastDueGraceDays, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? TrialCredits);

    /// <summary>Sözleşme AdminPlan.</summary>
    public sealed record AdminPlan(
        string Code,
        string Name,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Description,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? MonthlyPrice,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? YearlyPrice,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? IncludedBranchCount,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? ExtraBranchPrice,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? StaffLimit,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? IncludedSmsCredits,
        PlanFeatures Features,
        bool IsPublic,
        bool IsActive,
        int BrandCount);

    /// <summary>Sözleşme Money (istek).</summary>
    public sealed record MoneyInput(long? AmountMinor, string? Currency);

    /// <summary>Sözleşme PlanFeatures (istek).</summary>
    public sealed record FeaturesInput(bool? WalkInQueue, bool? Waitlist, bool? Packages, bool? CrossBranchReports, bool? PrioritySupport);

    /// <summary>Sözleşme AdminPlanUpdate.</summary>
    public sealed record AdminPlanUpdate(
        string? Code,
        string? Name,
        string? Description,
        MoneyInput? MonthlyPrice,
        MoneyInput? YearlyPrice,
        int? IncludedBranchCount,
        MoneyInput? ExtraBranchPrice,
        int? StaffLimit,
        int? IncludedSmsCredits,
        FeaturesInput? Features,
        bool? IsPublic,
        bool? IsActive);

    /// <summary>Sözleşme AdminPlanCatalogUpdate.</summary>
    public sealed record AdminPlanCatalogUpdate(IReadOnlyList<AdminPlanUpdate>? Plans);

    /// <summary>Sözleşme AdminCreditPackageCatalog.</summary>
    public sealed record AdminCreditPackageCatalog(IReadOnlyList<AdminCreditPackage> Packages, string Version);

    /// <summary>Sözleşme AdminCreditPackage.</summary>
    public sealed record AdminCreditPackage(
        string Code, string Name, int Credits, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? Price, bool IsActive);

    /// <summary>Sözleşme AdminCreditPackage (istek).</summary>
    public sealed record AdminCreditPackageUpdate(string? Code, string? Name, int? Credits, MoneyInput? Price, bool? IsActive);

    /// <summary>Sözleşme AdminCreditPackageCatalogUpdate.</summary>
    public sealed record AdminCreditPackageCatalogUpdate(IReadOnlyList<AdminCreditPackageUpdate>? Packages);
}
