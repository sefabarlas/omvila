using System.Buffers.Text;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure.Security;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Reviews.Domain;
using Omvila.Modules.Reviews.Infrastructure;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Features.ManageReviews;

/// <summary>
/// Değerlendirmeler listesi ve özeti (sözleşme manageListReviews), müşteri kartı sekmesi (manageListCustomerReviews). Şube kapsamı
/// (review.view Ş) sorguda uygulanır. Müşteri adı Customers'tan, personel adı Tenancy'den toplu okunur.
/// </summary>
internal sealed class ReviewReader(ReviewsDbContext db, ICustomerDirectory customers, ITenantQueries tenants, IClock clock)
{
    public const int DefaultLimit = 50;

    private const string AwaitingReply = "AwaitingReply";
    private const string All = "All";
    private const string LowRating = "LowRating";

    /// <summary>"3 yıldız ve altı".</summary>
    private const int LowRatingMax = 3;

    // Kural: sözleşme manageListReviews (filtre sıraları; overview filtreden bağımsız, şube filtresine uyar)
    public async Task<Result<ReviewPage>> ListAsync(
        Guid brandId, Guid? branchId, string? filter, int? limit, string? cursor, BranchScope scope, CancellationToken ct)
    {
        filter ??= AwaitingReply;
        var errors = ValidatePaging(limit, cursor, out var after);
        if (filter is not (AwaitingReply or All or LowRating))
        {
            errors["filter"] = ["invalid_format"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        if (branchId is { } requested && (!scope.Includes(requested) || !await tenants.BranchExistsAsync(brandId, requested, ct)))
        {
            return Error.NotFound();
        }

        var scoped = Scoped(brandId, branchId, scope);
        var query = filter switch
        {
            AwaitingReply => After(scoped.Where(r => r.ReplyText == null), after, ascending: true)
                .OrderBy(r => r.Rating).ThenBy(r => r.CreatedUtc).ThenBy(r => r.Id),
            LowRating => After(scoped.Where(r => r.Rating <= LowRatingMax), after, ascending: false)
                .OrderByDescending(r => r.CreatedUtc).ThenByDescending(r => r.Id),
            _ => After(scoped, after, ascending: false).OrderByDescending(r => r.CreatedUtc).ThenByDescending(r => r.Id),
        };
        var take = limit ?? DefaultLimit;
        var rows = await query.Take(take + 1).ToListAsync(ct);
        var page = rows.Take(take).ToList();
        var next = rows.Count > take ? Encode(page[^1]) : null;
        var zoneId = branchId is { } b
            ? await tenants.GetBranchTimeZoneAsync(brandId, b, ct)
            : (await tenants.GetNotificationSettingsAsync(brandId, ct))?.TimeZoneId;
        return new ReviewPage(await MapAsync(brandId, page, ct), next, await OverviewAsync(brandId, branchId, scope, scoped, zoneId, ct));
    }

    public async Task<Result<CustomerReviewPage>> ListForCustomerAsync(
        Guid brandId, Guid brandCustomerId, int? limit, string? cursor, BranchScope scope, CancellationToken ct)
    {
        var errors = ValidatePaging(limit, cursor, out var after);
        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        var query = After(Scoped(brandId, null, scope).Where(r => r.BrandCustomerId == brandCustomerId), after, ascending: false)
            .OrderByDescending(r => r.CreatedUtc).ThenByDescending(r => r.Id);
        var take = limit ?? DefaultLimit;
        var rows = await query.Take(take + 1).ToListAsync(ct);
        var page = rows.Take(take).ToList();
        return new CustomerReviewPage(await MapAsync(brandId, page, ct), rows.Count > take ? Encode(page[^1]) : null);
    }

    public async Task<IReadOnlyList<ReviewView>> MapAsync(Guid brandId, IReadOnlyList<Review> reviews, CancellationToken ct)
    {
        if (reviews.Count == 0)
        {
            return [];
        }

        var names = await customers.GetDisplayInfoAsync(brandId, [.. reviews.Select(r => r.BrandCustomerId).Distinct()], ct);
        var staff = await tenants.GetStaffCardsAsync(
            brandId, [.. reviews.Where(r => r.StaffId is not null).Select(r => r.StaffId!.Value).Distinct()], ct);
        return [.. reviews.Select(r => new ReviewView(
            r.Id,
            r.AppointmentId,
            r.BranchId,
            r.Rating,
            r.Comment,
            new ReviewCustomerRef(r.BrandCustomerId, names.GetValueOrDefault(r.BrandCustomerId)?.FullName ?? string.Empty),
            r.ServiceNames,
            r.StaffId,
            r.StaffId is { } s && staff.TryGetValue(s, out var card) ? card.FullName : null,
            r.CreatedUtc,
            r.IsPublished,
            r is { ReplyText: { } text, RepliedUtc: { } at } ? new ReviewReplyView(text, at, r.RepliedByName) : null))];
    }

    // Kural: sözleşme ReviewOverview (son ReviewOverviewWindowDays gün; "Bu ay" şubenin, şube seçilmemişse markanın saat diliminde;
    // "En çok övülen" en az ReviewPublicThreshold değerlendirme)
    private async Task<ReviewOverview> OverviewAsync(
        Guid brandId, Guid? branchId, BranchScope scope, IQueryable<Review> scoped, string? zoneId, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var windowStart = now - Duration.FromDays(PlatformDefaults.ReviewOverviewWindowDays);
        var zone = (zoneId is null ? null : DateTimeZoneProviders.Tzdb.GetZoneOrNull(zoneId)) ?? DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var today = now.InZone(zone).Date;
        var monthStart = new LocalDate(today.Year, today.Month, 1).AtStartOfDayInZone(zone).ToInstant();

        var totals = await scoped.GroupBy(_ => 1).Select(g => new
        {
            Total = g.Count(),
            AwaitingReply = g.Count(r => r.ReplyText == null),
            ThisMonth = g.Count(r => r.CreatedUtc >= monthStart),
            Window = g.Count(r => r.CreatedUtc >= windowStart),
            WindowAverage = g.Where(r => r.CreatedUtc >= windowStart).Average(r => (double?)r.Rating),
        }).SingleOrDefaultAsync(ct);

        var invitations = ScopedInvitations(brandId, branchId, scope).Where(i => i.SendAtUtc >= monthStart && i.SendAtUtc <= now);
        var invited = await invitations.CountAsync(ct);
        var answered = invited == 0
            ? 0
            : await invitations.CountAsync(i => db.Reviews.Any(r => r.BrandId == brandId && r.AppointmentId == i.AppointmentId), ct);

        var top = await scoped.Where(r => r.CreatedUtc >= windowStart && r.StaffId != null)
            .GroupBy(r => r.StaffId!.Value)
            .Select(g => new { StaffId = g.Key, Count = g.Count(), Average = g.Average(r => (double)r.Rating) })
            .Where(g => g.Count >= PlatformDefaults.ReviewPublicThreshold)
            .OrderByDescending(g => g.Average).ThenByDescending(g => g.Count).ThenBy(g => g.StaffId)
            .FirstOrDefaultAsync(ct);
        ReviewTopStaff? topStaff = null;
        if (top is not null && (await tenants.GetStaffCardsAsync(brandId, [top.StaffId], ct)).TryGetValue(top.StaffId, out var card))
        {
            topStaff = new ReviewTopStaff(top.StaffId, card.FullName, Round(top.Average), top.Count);
        }

        return new ReviewOverview(
            totals?.WindowAverage is { } average ? Round(average) : null,
            totals?.Window ?? 0,
            totals?.Total ?? 0,
            totals?.AwaitingReply ?? 0,
            totals?.ThisMonth ?? 0,
            invited == 0 ? null : Math.Round(answered * 100.0 / invited, MidpointRounding.AwayFromZero),
            topStaff);
    }

    private IQueryable<Review> Scoped(Guid brandId, Guid? branchId, BranchScope scope)
    {
        var query = db.Reviews.AsNoTracking().Where(r => r.BrandId == brandId);
        if (branchId is { } only)
        {
            return query.Where(r => r.BranchId == only);
        }

        var ids = scope.BranchIds.ToList();
        return scope.All ? query : query.Where(r => ids.Contains(r.BranchId));
    }

    private IQueryable<ReviewInvitation> ScopedInvitations(Guid brandId, Guid? branchId, BranchScope scope)
    {
        var query = db.Invitations.AsNoTracking().Where(i => i.BrandId == brandId);
        if (branchId is { } only)
        {
            return query.Where(i => i.BranchId == only);
        }

        var ids = scope.BranchIds.ToList();
        return scope.All ? query : query.Where(i => ids.Contains(i.BranchId));
    }

    private static IQueryable<Review> After(IQueryable<Review> query, ReviewCursor? after, bool ascending)
    {
        if (after is not { } c)
        {
            return query;
        }

        return ascending
            ? query.Where(r => r.Rating > c.Rating
                || (r.Rating == c.Rating && (r.CreatedUtc > c.CreatedUtc || (r.CreatedUtc == c.CreatedUtc && r.Id.CompareTo(c.Id) > 0))))
            : query.Where(r => r.CreatedUtc < c.CreatedUtc || (r.CreatedUtc == c.CreatedUtc && r.Id.CompareTo(c.Id) < 0));
    }

    private static Dictionary<string, string[]> ValidatePaging(int? limit, string? cursor, out ReviewCursor? after)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        after = null;
        if (limit is < 1 or > 100)
        {
            errors["limit"] = ["out_of_range"];
        }

        if (cursor is not null)
        {
            if (ReviewCursor.TryDecode(cursor, out var decoded))
            {
                after = decoded;
            }
            else
            {
                errors["cursor"] = ["invalid_format"];
            }
        }

        return errors;
    }

    private static string Encode(Review last) => new ReviewCursor(last.Rating, last.CreatedUtc, last.Id).Encode();

    private static double Round(double value) => Math.Round(value, 1, MidpointRounding.AwayFromZero);

    /// <summary>Opak imleç: puan (yanıt bekleyen sırası), oluşturma anı, kimlik.</summary>
    private readonly record struct ReviewCursor(int Rating, Instant CreatedUtc, Guid Id)
    {
        public string Encode() =>
            Base64Url.EncodeToString(Encoding.UTF8.GetBytes(string.Create(
                CultureInfo.InvariantCulture, $"{Rating}|{CreatedUtc.ToUnixTimeTicks()}|{Id:D}")));

        public static bool TryDecode(string cursor, out ReviewCursor value)
        {
            value = default;
            try
            {
                var parts = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(cursor)).Split('|');
                if (parts.Length == 3
                    && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var rating)
                    && long.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks)
                    && Guid.TryParse(parts[2], out var id))
                {
                    value = new ReviewCursor(rating, Instant.FromUnixTimeTicks(ticks), id);
                    return true;
                }
            }
            catch (FormatException)
            {
            }

            return false;
        }
    }
}
