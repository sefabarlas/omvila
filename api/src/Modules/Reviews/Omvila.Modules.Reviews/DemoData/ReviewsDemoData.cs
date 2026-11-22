using Omvila.Modules.Reviews.Domain;
using Omvila.Modules.Reviews.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-114) için değerlendirme yazma yolu: <see cref="Review.Submit"/> ve
/// <see cref="Review.Reply"/>. Davet ve olay yazılmaz (personel bildirimi üretmez). Çağıranın transaction'ında ve markanın kiracı
/// bağlamında çalışır.
/// </summary>
public sealed class ReviewsDemoData
{
    private readonly ReviewsDbContext _db;
    private readonly IIdGenerator _ids;

    internal ReviewsDemoData(ReviewsDbContext db, IIdGenerator ids)
    {
        _db = db;
        _ids = ids;
    }

    /// <summary>Değerlendirmeyi (ve varsa yanıtını) yazar.</summary>
    /// <param name="input">Değerlendirme.</param>
    /// <param name="ct">İptal belirteci.</param>
    public async Task AddAsync(DemoReview input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var review = Review.Submit(
            _ids.New(), input.BrandId, input.BranchId, input.AppointmentId, input.BrandCustomerId, input.StaffId, input.ServiceNames, input.Rating,
            input.Comment, input.CreatedUtc);
        if (input.ReplyText is not null)
        {
            review.Reply(input.ReplyText, userId: null, input.RepliedByName, input.RepliedUtc ?? input.CreatedUtc);
        }

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(ct);
    }
}
