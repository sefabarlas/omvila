namespace Omvila.Modules.Sales.Domain;

/// <summary>Tahsilat kaydının durumu. İade salonun kendi yaptığı işin işaretidir ("Salon iade etti"), para hareketi değildir.</summary>
internal enum PaymentStatus
{
    Recorded,
    Refunded,
    PartiallyRefunded,
    Voided,
}
