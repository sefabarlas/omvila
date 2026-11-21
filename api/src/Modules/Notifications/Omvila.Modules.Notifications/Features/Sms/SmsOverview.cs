using System.Text.Json.Serialization;

namespace Omvila.Modules.Notifications.Features.Sms;

/// <summary>Sözleşme SmsOverview (estimatedDaysLeft zorunlu alan: tahmin yoksa null yazılır).</summary>
internal sealed record SmsOverview(
    int CreditBalance,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? EstimatedDaysLeft,
    int LowCreditThreshold,
    bool IsLow,
    SmsMonthUsage SentThisMonth,
    SmsSenderResponse Sender);
