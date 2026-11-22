using NodaTime;

namespace Omvila.Modules.Sales.Features.Commissions;

/// <summary>Sözleşme CommissionDayAmount.</summary>
internal sealed record CommissionDayAmount(LocalDate Date, long AmountMinor);
