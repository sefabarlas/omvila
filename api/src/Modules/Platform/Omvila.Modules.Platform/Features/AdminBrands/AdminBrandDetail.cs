using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Platform.Features.AdminBrands;

/// <summary>Sözleşme AdminBrandDetail (telefonlar maskeli).</summary>
internal sealed record AdminBrandDetail(
    Guid Id,
    string Name,
    string Slug,
    string IndustryCode,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? City,
    string BrandStatus,
    Instant CreatedUtc,
    Uri BookingUrl,
    AdminBrandDetail.OwnerInfo Owner,
    IReadOnlyList<AdminBrandDetail.BranchRef> Branches,
    int ActiveStaffCount,
    int CustomerCount,
    int GoogleCalendarConnectedStaffCount,
    AdminBrandDetail.SmsSenderInfo SmsSender,
    AdminBrandDetail.SubscriptionCard Subscription,
    AdminBrandDetail.CreditsCard Credits,
    AdminBrandDetail.AppointmentsCard AppointmentsThisMonth,
    IReadOnlyList<AdminCreditTransaction> RecentCreditTransactions,
    string Version)
{
    /// <summary>"Sahip" (telefon maskeli).</summary>
    internal sealed record OwnerInfo(string FullName, string PhoneMasked);

    /// <summary>Sözleşme BranchRef.</summary>
    internal sealed record BranchRef(Guid Id, string Name);

    /// <summary>Sözleşme SmsSender.</summary>
    internal sealed record SmsSenderInfo(
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Name,
        string Status,
        string EffectiveName,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? SubmittedUtc,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? DecidedUtc,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? RejectionReason);

    /// <summary>"ABONELİK" ve "ABONELİK ÖDEMESİ" kartları.</summary>
    internal sealed record SubscriptionCard(
        string Status,
        string PlanCode,
        string PlanName,
        string BillingCycle,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? PaymentMethod,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? TrialEndsOn,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? TrialDaysLeft,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? PastDueSince,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? ReadOnlyOn,
        string TransferCode,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? PendingNoticeId,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? PendingNoticeNotifiedUtc);

    /// <summary>"KONTÖR 1.240 · tahmini 26 gün yeter".</summary>
    internal sealed record CreditsCard(int Balance, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? EstimatedDaysLeft);

    /// <summary>"BU AY RANDEVU 412 · %61'i online".</summary>
    internal sealed record AppointmentsCard(int Count, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] double? OnlinePercent);
}
