namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Randevu kaporasının türetilmiş durumu (sözleşme DepositStatus). Awaiting = havale bekleniyor, CustomerReported = müşteri
/// "Havaleyi yaptım" dedi, Received = salon "Kapora geldi" dedi. Refunded ve Waived değerleri sözleşmeyle eşli durur; onları
/// yazan akışlar ("Salon iade etti", "Kaporasız onayla") sonraki parçalardadır.
/// </summary>
internal enum DepositStatus
{
    Awaiting,
    CustomerReported,
    Received,
    Refunded,
    Waived,
}
