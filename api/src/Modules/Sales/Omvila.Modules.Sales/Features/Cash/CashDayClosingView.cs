using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>Sözleşme CashDayClosing.</summary>
internal sealed record CashDayClosingView(
    Money ExpectedCash,
    Money CountedCash,
    long DifferenceMinor,
    string Difference,
    string? DifferenceNote,
    string? ClosedByName,
    Instant ClosedUtc);
