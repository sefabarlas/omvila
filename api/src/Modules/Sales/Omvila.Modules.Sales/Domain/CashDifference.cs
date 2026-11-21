namespace Omvila.Modules.Sales.Domain;

/// <summary>Kasa sayımının sonucu (sözleşme CashDayClosing.difference): fark yok, kasada eksik, kasada fazla.</summary>
internal enum CashDifference
{
    None,
    Shortage,
    Surplus,
}
