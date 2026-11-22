using NodaTime;

namespace Omvila.Modules.Sales.Features.Reports;

/// <summary>"Günlük tahsilat" günü: net tahsilat (iade fazlaysa eksi) ve şubenin o gün kapalı olup olmadığı.</summary>
internal sealed record ReportDailyCollection(LocalDate Date, long AmountMinor, bool IsClosed);
