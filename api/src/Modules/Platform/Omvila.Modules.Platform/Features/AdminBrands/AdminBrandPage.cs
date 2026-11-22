using System.Text.Json.Serialization;

namespace Omvila.Modules.Platform.Features.AdminBrands;

/// <summary>Sözleşme AdminBrandPage.</summary>
internal sealed record AdminBrandPage(
    AdminBrandPage.BrandCounts Counts,
    IReadOnlyList<AdminBrandPage.Item> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor)
{
    /// <summary>"412 aktif · 64 denemede" (süzgeçten bağımsız).</summary>
    internal sealed record BrandCounts(int Active, int Trialing);

    /// <summary>Sözleşme AdminBrandListItem.</summary>
    internal sealed record Item(
        Guid Id,
        string Name,
        string IndustryCode,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? City,
        string BrandStatus,
        string PlanCode,
        string PlanName,
        string SubscriptionStatus,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? TrialDaysLeft,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? PastDueDays,
        int BranchCount,
        int AppointmentsThisMonth,
        int CreditBalance,
        bool DepositEnabled);
}
