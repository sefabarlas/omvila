namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Randevu kaporasının türetilmiş durumu (sözleşme DepositStatus). Awaiting = havale bekleniyor, CustomerReported = müşteri
/// "Havaleyi yaptım" dedi, Received = salon "Kapora geldi" dedi (A3), Waived = salon "Kaporasız onayla" dedi (A2). Refunded
/// sözleşmeyle eşli durur; onu yazan akış ("Salon iade etti") sonraki parçadadır.
/// </summary>
internal enum DepositStatus
{
    Awaiting,
    CustomerReported,
    Received,
    Refunded,
    Waived,
}
