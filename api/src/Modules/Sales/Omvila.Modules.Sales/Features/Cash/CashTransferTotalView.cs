using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>Sözleşme CashDay.transfer: "500 ödeme · 600 kapora".</summary>
internal sealed record CashTransferTotalView(Money Amount, int Count, Money DepositAmount, Money OtherAmount);
