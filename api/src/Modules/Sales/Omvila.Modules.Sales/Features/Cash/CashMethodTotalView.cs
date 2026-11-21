using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>Sözleşme CashMethodTotal.</summary>
internal sealed record CashMethodTotalView(Money Amount, int Count);
