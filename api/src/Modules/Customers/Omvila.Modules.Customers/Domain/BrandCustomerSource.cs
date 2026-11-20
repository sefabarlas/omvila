namespace Omvila.Modules.Customers.Domain;

/// <summary>"İlk kanal" (alan-modeli.md § BrandCustomer.Source).</summary>
internal enum BrandCustomerSource
{
    Online,
    Panel,
    Phone,
    WalkIn,
    Import,
}
