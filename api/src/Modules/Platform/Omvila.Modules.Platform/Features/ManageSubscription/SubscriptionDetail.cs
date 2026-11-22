using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme SubscriptionDetail (AyarlarAbonelik).</summary>
internal sealed record SubscriptionDetail(
    SubscriptionStatus Status,
    PlanRef Plan,
    BillingCycle BillingCycle,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] SubscriptionPaymentMethod? PaymentMethod,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? TrialEndsOn,
    LocalDate CurrentPeriodStart,
    LocalDate CurrentPeriodEnd,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? PastDueSince,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? ReadOnlyOn,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] LocalDate? ReadOnlySince,
    BranchCounts Branches,
    StaffCounts Staff,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? AmountDue,
    string TransferCode,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] OmvilaBankAccountView? OmvilaBankAccount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] PaymentNoticeSummary? PendingNotice);
