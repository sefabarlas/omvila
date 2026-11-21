namespace Omvila.Modules.Sales.Contracts;

/// <summary>
/// Randevu tahsilatları ve hizmet primi (durum-makinesi.md A11; alan-modeli.md § Payment, PaymentAllocation, CommissionEntry).
/// OMVILA para almaz: kayıtlar salonun kendi tahsilatıdır. Çağıranın isteğiyle aynı bağlantı ve transaction'da yazılır;
/// kiracı bağlamı kurulmuş olmalıdır.
/// </summary>
public interface IAppointmentPayments
{
    /// <summary>
    /// "Tamamla ve ödeme al": her satır için <c>Payment(Recorded)</c> + randevu tahsisi; personelli her kalem için
    /// <c>CommissionEntry(Service, Accrued)</c>. Oran personele özel hizmet oranı ?? personelin hizmet primi; taban tahsil
    /// edilen tutarın (satırlar + salonda kalan kapora) kalemlere tutarları oranında dağıtılmış payı (T-88, W2). Oranı 0 olan
    /// personele kayıt yazılmaz. Toplam doğrulaması çağırandadır.
    /// </summary>
    /// <param name="request">Girdi.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task RecordCompletionAsync(AppointmentCompletionRequest request, CancellationToken ct);

    /// <summary>Randevuya tahsis edilmiş bütün tahsilatlar (kapora dahil), eskiden yeniye.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="appointmentId">Randevu.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyList<AppointmentPaymentRecord>> ListAsync(Guid brandId, Guid appointmentId, CancellationToken ct);
}
