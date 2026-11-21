namespace Omvila.Modules.Sales.Domain;

/// <summary>Müşteri paketi durumu (durum-makinesi.md § 5). Transferred v1'de yazılmaz (devir ucu yok; T-86 (3)).</summary>
internal enum CustomerPackageStatus
{
    Active,
    Expired,
    Refunded,
    Transferred,
}
