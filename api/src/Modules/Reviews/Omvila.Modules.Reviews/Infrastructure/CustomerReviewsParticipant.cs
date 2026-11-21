using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Customers.Contracts;

namespace Omvila.Modules.Reviews.Infrastructure;

/// <summary>
/// Müşteri birleştirmede Reviews'ın payı (sözleşme manageMergeCustomers: değerlendirmeler hedefe taşınır; T-92).
/// KVKK silmesinde değerlendirme anonim karta bağlı kalır (kişisel veri taşımaz), ayrıca bir şey yapılmaz.
/// </summary>
internal sealed class CustomerReviewsParticipant(ReviewsDbContext db) : ICustomerMergeParticipant
{
    public async Task MergeAsync(CustomerMerge merge, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(merge);
        await db.Reviews
            .Where(r => r.BrandId == merge.BrandId && r.BrandCustomerId == merge.SourceBrandCustomerId)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.BrandCustomerId, merge.TargetBrandCustomerId), ct);
    }
}
