namespace Omvila.Modules.Sales.Contracts;

/// <summary>
/// Kapora havalesinin tahsilat kaydı (alan-modeli.md § Payment, PaymentAllocation; durum-makinesi.md A3). OMVILA para almaz:
/// bu yalnız salonun "Kapora geldi" işaretinin kaydıdır. Çağıranın isteğiyle aynı bağlantı ve transaction'da yazılır
/// (mimari.md § Modüller arası iletişim); çağıranın işi başarısız olursa kayıt da geri alınır.
/// </summary>
public interface IDepositPayments
{
    /// <summary>
    /// <c>Payment(Method = Transfer, IsDeposit = true, Status = Recorded)</c> ve tutarın tamamı için randevu tahsisini yazar.
    /// Kiracı bağlamı kurulmuş olmalıdır.
    /// </summary>
    /// <param name="request">Kayıt girdisi.</param>
    /// <param name="ct">İptal belirteci.</param>
    /// <returns>Oluşan tahsilatın kimliği (randevunun <c>DepositPaymentId</c>'si).</returns>
    Task<Guid> RecordDepositAsync(DepositPaymentRequest request, CancellationToken ct);
}
