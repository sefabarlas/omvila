using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>Sözleşme PaymentNoticeSummary.</summary>
internal sealed record PaymentNoticeSummary(
    Guid Id,
    PaymentNoticePurpose Purpose,
    string TransferCode,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Money? ExpectedAmount,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? Credits,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? SenderName,
    bool HasReceipt,
    PaymentNoticeStatus Status,
    Instant NotifiedUtc,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] Instant? ResolvedUtc)
{
    public static PaymentNoticeSummary From(PaymentNotice notice)
    {
        ArgumentNullException.ThrowIfNull(notice);
        return new PaymentNoticeSummary(
            notice.Id,
            notice.Purpose,
            notice.TransferCode,
            notice.ExpectedAmountMinor is { } amount ? Money.Try(amount) : null,
            notice.Credits,
            notice.SenderName,
            notice.ReceiptFileKey is not null,
            notice.Status,
            notice.NotifiedUtc,
            notice.ResolvedUtc);
    }
}
