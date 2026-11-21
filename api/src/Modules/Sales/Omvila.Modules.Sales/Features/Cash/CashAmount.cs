using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>Sözleşme Money (countedCash).</summary>
internal sealed record CashAmount(long? AmountMinor, string? Currency);
