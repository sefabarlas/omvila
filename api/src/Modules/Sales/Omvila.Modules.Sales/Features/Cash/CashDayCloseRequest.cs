namespace Omvila.Modules.Sales.Features.Cash;

/// <summary>Sözleşme CashDayCloseRequest ("Günü kapat", Kasa sayımı). Tarih metin olarak alınıp ayrıştırılır.</summary>
internal sealed record CashDayCloseRequest(Guid? BranchId, string? Date, CashAmount? CountedCash, string? DifferenceNote);
