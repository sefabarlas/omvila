namespace Omvila.Modules.Customers.Contracts;

/// <summary>
/// "Kayıtları birleştir" genişleme noktası (alan-modeli.md § Birleştirme; T-92). Müşteri kartına bağlı verisi olan her modül
/// (Scheduling: randevular ve seriler, Sales: tahsilatlar; paket, değerlendirme ve bekleme modülleri geldiğinde onlar da)
/// kendi tablosundaki kart bağını hedefe taşır. Customers modülü kartları günceller, sonra kayıtlı bütün katılımcıları aynı
/// istek transaction'ında ve markanın kiracı bağlamında çağırır; biri hata verirse birleştirmenin tamamı geri alınır.
/// </summary>
public interface ICustomerMergeParticipant
{
    /// <summary>Kaynak karta bağlı kayıtları hedef karta taşır.</summary>
    /// <param name="merge">Birleştirme.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task MergeAsync(CustomerMerge merge, CancellationToken ct);
}
