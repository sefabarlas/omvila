namespace Omvila.Modules.Sales.Contracts;

/// <summary>Salonda elle seçilen tahsilat yöntemi (sözleşme CollectedPaymentMethod). Card salonun kendi POS'udur.</summary>
public enum CollectedPaymentMethod
{
    /// <summary>Nakit.</summary>
    Cash,

    /// <summary>Salonun kendi POS'u.</summary>
    Card,

    /// <summary>Havale/EFT.</summary>
    Transfer,
}
