using NodaTime;

namespace Omvila.Modules.Sales.Features.Commissions;

/// <summary>Sözleşme CommissionWeekDay ("Bu hafta" çubuğu).</summary>
internal sealed record CommissionWeekDay(LocalDate Date, long AmountMinor, int AppointmentCount);
