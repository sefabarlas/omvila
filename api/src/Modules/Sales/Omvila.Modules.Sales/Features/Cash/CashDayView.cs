using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>Sözleşme CashDay (Kasa, KasaBos).</summary>
internal sealed record CashDayView(
    Guid BranchId,
    LocalDate Date,
    string TimeZone,
    string Status,
    CashMethodTotalView Total,
    CashMethodTotalView Cash,
    CashMethodTotalView Card,
    CashTransferTotalView Transfer,
    Money CashRefunded,
    Money ExpectedCash,
    IReadOnlyList<CashPaymentView> Payments,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] CashDayClosingView? Closing);
