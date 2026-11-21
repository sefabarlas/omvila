namespace Omvila.Modules.Reviews.Contracts;

/// <summary>Rezervasyon sayfası puanı (sözleşme PublicBrand.rating). Markanın kiracı bağlamında çalışır.</summary>
public interface IReviewRatings
{
    /// <summary>
    /// Yayınlanmış değerlendirmelerin ortalaması ve sayısı; sayı <c>ReviewPublicThreshold</c>'un (5) altındaysa null
    /// (sureler-ve-ayarlar.md: "en az 5 yayınlanmış değerlendirme"). Salonun gösterme tercihi (showReviews) çağıranda.
    /// </summary>
    /// <param name="brandId">Marka.</param>
    /// <param name="ct">İptal belirteci.</param>
    Task<PublicReviewRating?> GetPublicRatingAsync(Guid brandId, CancellationToken ct);
}

