namespace Omvila.Modules.Reviews.Contracts;

/// <summary>Rezervasyon sayfası puanı.</summary>
/// <param name="Average">Ortalama (bir ondalık).</param>
/// <param name="Count">Yayınlanmış değerlendirme sayısı.</param>
public sealed record PublicReviewRating(double Average, int Count);
