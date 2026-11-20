namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>Sözleşme manageListCustomers.filter (sozluk.md § Müşteri listesi filtreleri).</summary>
internal enum CustomerListFilter
{
    All,
    Regular,
    New,
    WithPackage,
    NoShowRisk,
    Lapsed60,
}
