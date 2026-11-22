using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Platform.Features.AdminBrands;

/// <summary>Sözleşme AdminOverview.</summary>
internal sealed record AdminOverview(
    Instant GeneratedUtc,
    AdminOverview.ActiveBrandsCard ActiveBrands,
    AdminOverview.PendingApprovalsCard PendingApprovals,
    AdminOverview.TrialingCard Trialing,
    AdminOverview.PastDueCard PastDue,
    AdminOverview.FunnelCard Funnel,
    IReadOnlyList<AdminOverview.AttentionBrand> AttentionBrands)
{
    /// <summary>"AKTİF MARKA 412 · +18 bu ay".</summary>
    internal sealed record ActiveBrandsCard(int Count, int NewThisMonth);

    /// <summary>"ONAY BEKLEYEN 7 · 5 havale bildirimi · 2 SMS başlığı".</summary>
    internal sealed record PendingApprovalsCard(int Total, int PaymentNotices, int SmsSenders);

    /// <summary>"DENEMEDE 64 · 9'u bu hafta bitiyor".</summary>
    internal sealed record TrialingCard(int Count, int EndingThisWeek);

    /// <summary>"ÖDEME GECİKTİ 11 · 3'ü salt okunura geçmek üzere".</summary>
    internal sealed record PastDueCard(int Count, int ReadOnlySoon);

    /// <summary>"Aktivasyon hunisi · son 30 gün".</summary>
    internal sealed record FunnelCard(int WindowDays, int SignedUp, int AddedServices, int SetWorkingHours, int FirstOnlineAppointment);

    /// <summary>Sözleşme AdminAttentionBrand.</summary>
    internal sealed record AttentionBrand(
        Guid BrandId,
        string BrandName,
        string IndustryCode,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? City,
        string PlanName,
        string Reason,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? PastDueDays,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? TrialDaysLeft,
        int BranchCount,
        int AppointmentsThisMonth);
}
