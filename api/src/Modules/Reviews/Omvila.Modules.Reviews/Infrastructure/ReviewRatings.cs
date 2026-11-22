using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Reviews.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Infrastructure;

/// <summary><see cref="IReviewRatings"/>: tüm yayınlanmış değerlendirmelerin ortalaması (son 90 gün değil; sözleşme PublicBrand.rating).</summary>
internal sealed class ReviewRatings(ReviewsDbContext db) : IReviewRatings
{
    // Kural: sureler-ve-ayarlar.md ReviewPublicThreshold (en az 5 yayınlanmış değerlendirme)
    public async Task<PublicReviewRating?> GetPublicRatingAsync(Guid brandId, CancellationToken ct)
    {
        var stats = await db.Reviews.AsNoTracking()
            .Where(r => r.BrandId == brandId && r.IsPublished)
            .GroupBy(_ => 1)
            .Select(g => new { Count = g.Count(), Average = g.Average(r => (double)r.Rating) })
            .SingleOrDefaultAsync(ct);
        return stats is null || stats.Count < PlatformDefaults.ReviewPublicThreshold
            ? null
            : new PublicReviewRating(Math.Round(stats.Average, 1, MidpointRounding.AwayFromZero), stats.Count);
    }

    public async Task<double?> GetStaffAverageAsync(Guid brandId, Guid staffId, NodaTime.Instant sinceUtc, CancellationToken ct)
    {
        var average = await db.Reviews.AsNoTracking()
            .Where(r => r.BrandId == brandId && r.StaffId == staffId && r.CreatedUtc >= sinceUtc)
            .Select(r => (double?)r.Rating)
            .AverageAsync(ct);
        return average is { } value ? Math.Round(value, 1, MidpointRounding.AwayFromZero) : null;
    }
}
