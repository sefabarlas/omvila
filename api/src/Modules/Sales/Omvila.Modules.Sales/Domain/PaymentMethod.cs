namespace Omvila.Modules.Sales.Domain;

/// <summary>Tahsilat yöntemi (alan-modeli.md § Payment). Card salonun kendi POS'udur; OMVILA kart ödemesi almaz.</summary>
internal enum PaymentMethod
{
    Cash,
    Card,
    Transfer,
    PackageRedemption,
}
