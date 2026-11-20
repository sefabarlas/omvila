namespace Omvila.Modules.Catalog.Contracts;

/// <summary>Fiyat gösterimi: <see cref="Fixed"/> "450 ₺", <see cref="StartingFrom"/> "1.800 ₺'den".</summary>
public enum ServicePriceDisplay
{
    /// <summary>Sabit fiyat.</summary>
    Fixed,

    /// <summary>Başlangıç fiyatı.</summary>
    StartingFrom,
}
