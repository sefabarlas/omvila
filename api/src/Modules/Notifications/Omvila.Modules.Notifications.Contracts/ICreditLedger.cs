namespace Omvila.Modules.Notifications.Contracts;

/// <summary>SMS kontör defteri (alan-modeli.md § CreditTransaction). İstek transaction'ı içinde, markanın kiracı bağlamında çağrılır.</summary>
public interface ICreditLedger
{
    /// <summary>Hediye kontör yazar (ör. deneme hediyesi). <paramref name="credits"/> sıfırdan büyük olmalıdır.</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="credits">Kontör sayısı.</param>
    /// <param name="referenceType">Kaynak türü (ör. "Subscription").</param>
    /// <param name="referenceId">Kaynak kimliği.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task GrantAsync(Guid brandId, int credits, string referenceType, Guid? referenceId, CancellationToken ct);

    /// <summary>
    /// Satın alınan kontörü yazar (havale eşleşmesi; ADR-0006 K4). <paramref name="credits"/> sıfırdan büyük olmalıdır.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="credits">Kontör sayısı.</param>
    /// <param name="priceMinor">Paket fiyatı (kuruş); tanımlı değilse null.</param>
    /// <param name="referenceType">Kaynak türü (ör. "PaymentNotice").</param>
    /// <param name="referenceId">Kaynak kimliği.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task PurchaseAsync(Guid brandId, int credits, long? priceMinor, string referenceType, Guid referenceId, CancellationToken ct);

    /// <summary>Güncel bakiye (satırların toplamı; kilit almaz, okuma içindir).</summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<int> GetBalanceAsync(Guid brandId, CancellationToken ct);
}
