namespace Omvila.Modules.Sales.Domain;

/// <summary>Prim kaydının durumu (durum-makinesi.md § 7): Accrued → Approved → Paid; düzeltme eksi tutarlı Reversed ters kayıttır.</summary>
internal enum CommissionStatus
{
    Accrued,
    Approved,
    Paid,
    Reversed,
}
