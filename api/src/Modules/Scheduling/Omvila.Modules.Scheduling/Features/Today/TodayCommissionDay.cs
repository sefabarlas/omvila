using NodaTime;

namespace Omvila.Modules.Scheduling.Features.Today;

/// <summary>Sözleşme CommissionDayAmount.</summary>
internal sealed record TodayCommissionDay(LocalDate Date, long AmountMinor);
