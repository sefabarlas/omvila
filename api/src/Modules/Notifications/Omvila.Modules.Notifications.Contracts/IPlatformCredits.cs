using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Contracts;

/// <summary>
/// Platform paneli kontör ve gönderen adı servisi (Markalar, marka detayı, "Kontör ekle"; K4: platform kontörü bu arayüzle yazar).
/// Platform servisidir (ADR-0006): yalnız admin yüzeyinin platform bağlamında çağrılır, başka bağlamda istisna atar.
/// </summary>
public interface IPlatformCredits
{
    /// <summary>Markaların güncel bakiyesi (satırların toplamı); hareketi olmayan marka sözlükte yoktur.</summary>
    /// <param name="ct">İptal belirteci.</param>
    Task<IReadOnlyDictionary<Guid, int>> GetBalancesAsync(CancellationToken ct);

    /// <summary>
    /// Kontör hareketleri, yeniden eskiye. Gönderim düşümleri (Usage) gün başına (Europe/Istanbul) tek satırda toplanır; satır kimliği
    /// günün ilk hareketidir (sözleşme adminListBrandCreditTransactions).
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="limit">Sayfa boyu.</param>
    /// <param name="before">Bu satırdan (an ve kimlik; dahil değil) eskiler; null = en yeniden.</param>
    /// <param name="since">Bu andan yeniler; null = sınırsız.</param>
    /// <param name="ct">İptal belirteci.</param>
    /// <returns>En fazla <paramref name="limit"/> + 1 satır (fazlası sonraki sayfanın varlığını gösterir).</returns>
    Task<IReadOnlyList<PlatformCreditEntry>> ListAsync(Guid brandId, int limit, PlatformCreditCursor? before, Instant? since, CancellationToken ct);

    /// <summary>Verilen andan bu yana gönderimle düşülen kontör (tahmini kalan gün için).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="since">Başlangıç anı.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<int> GetConsumedSinceAsync(Guid brandId, Instant since, CancellationToken ct);

    /// <summary>
    /// Platform eklemesi veya düzeltmesi (CreditTransaction Grant; eksi = düzeltme, 0 olamaz). Defter kilitlenir; bakiye sıfırın
    /// altına inecekse 409 insufficient_credits.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="credits">Kontör (artı veya eksi).</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<Result<PlatformCreditEntry>> GrantAsync(Guid brandId, int credits, CancellationToken ct);

    /// <summary>Markanın gönderen adı (sözleşme SmsSender alanları).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<PlatformSmsSenderInfo> GetSmsSenderAsync(Guid brandId, CancellationToken ct);

    /// <summary>Onay bekleyen gönderen adı başvuruları.</summary>
    /// <param name="ct">İptal belirteci.</param>
    Task<int> CountPendingSmsSendersAsync(CancellationToken ct);
}
