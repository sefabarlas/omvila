using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Infrastructure;
using Omvila.Infrastructure.Http;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Reviews.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Features.PublicBrand;

/// <summary>
/// Rezervasyon sayfası açılışı (sözleşme publicGetBrand; dilim-01 kabul kriteri 16, dilim-02 kriter 17, T-15). Slug
/// platform bağlamında çözülür: güncel ad → 200, yönlendirmesi süren eski ad → 301 (Location yeni adın ucu), diğerleri
/// 404. Gövde yalnız bulunan markanın kiracı bağlamında okunur.
/// </summary>
internal static class PublicBrandEndpoints
{
    // Sözleşme IndustryCode → salon kartındaki sektör adı (salon paneli kayıt ekranıyla aynı adlar).
    private static readonly Dictionary<string, string> IndustryLabels = new(StringComparer.Ordinal)
    {
        ["hair_salon"] = "Kuaför",
        ["barber"] = "Berber",
        ["beauty"] = "Güzellik salonu",
        ["nail"] = "Tırnak",
        ["spa"] = "Spa ve masaj",
        ["dietitian"] = "Diyetisyen",
    };

    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapApiGroup("/api/v1/public/brands");

        // Rezervasyon sayfası üyeliksizdir (ürün kuralı 7): salon kartı herkese açık bağlantıyla görüntülenir.
        group.MapGet("/{brandSlug}", GetAsync).WithName("publicGetBrand").AllowAnonymous();
    }

    private static async Task<IResult> GetAsync(
        string brandSlug,
        IDbSession session,
        TenancyDbContext db,
        PublicBookingQueries booking,
        IReviewRatings ratings,
        IClock clock,
        CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var resolved = await ResolveAsync(brandSlug, session, db, now, ct);
        if (resolved is null)
        {
            return Error.NotFound().ToProblem();
        }

        var (brandId, currentSlug) = resolved.Value;
        if (!string.Equals(currentSlug, brandSlug, StringComparison.Ordinal))
        {
            return TypedResults.Redirect($"/api/v1/public/brands/{currentSlug}", permanent: true);
        }

        await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
        var brand = await db.Brands.AsNoTracking().SingleAsync(b => b.Id == brandId, ct);
        var page = await db.BookingPageSettings.AsNoTracking().SingleAsync(p => p.BrandId == brandId, ct);
        var branches = await db.Branches.AsNoTracking().Where(b => b.BrandId == brandId && b.IsActive).OrderBy(b => b.CreatedUtc).ToListAsync(ct);
        var branchIds = branches.Select(b => b.Id).ToList();
        var weekly = (await db.BranchHours.AsNoTracking().Where(h => h.BrandId == brandId && branchIds.Contains(h.BranchId)).ToListAsync(ct))
            .ToLookup(h => h.BranchId);

        // Özel günler: en geniş saat dilimi farkını kapsayacak şekilde dün…iki hafta sonrası (BranchOpening ileri bakışı).
        var utcToday = now.InUtc().Date;
        var fromDate = utcToday.PlusDays(-1);
        var toDate = utcToday.PlusDays(16);
        var overrides = (await db.Overrides.AsNoTracking()
                .Where(o => o.BrandId == brandId && branchIds.Contains(o.BranchId) && o.Date >= fromDate && o.Date <= toDate)
                .ToListAsync(ct))
            .ToLookup(o => o.BranchId);

        var cards = branches.Select(b =>
        {
            var zone = DateTimeZoneProviders.Tzdb[b.TimeZoneId];
            var opening = BranchOpening.Compute(
                now,
                zone,
                [.. weekly[b.Id].Select(h => (h.DayOfWeek, h.StartTime, h.EndTime))],
                overrides[b.Id].ToDictionary(
                    o => o.Date,
                    o => (IReadOnlyList<(LocalTime Start, LocalTime End)>)(o.IsClosed ? [] : [.. o.OpeningIntervals])));
            return new PublicBranchResponse(
                b.Id, b.Name, b.AddressLine, b.District, b.City, b.Phone, b.Latitude, b.Longitude, b.TimeZoneId,
                opening.OpenNow, opening.TodayClosesAt?.ToString("HH:mm", CultureInfo.InvariantCulture), opening.NextOpening);
        }).ToList();

        // Kural: sureler-ve-ayarlar.md ReviewPublicThreshold; sözleşme PublicBrand.rating (salon gösteriyorsa, tüm yayınlanmışlar)
        var rating = page.ShowReviews && await ratings.GetPublicRatingAsync(brand.Id, ct) is { } r ? new PublicRating(r.Average, r.Count) : null;
        return TypedResults.Ok(new PublicBrandResponse(
            brand.Id,
            brand.Name,
            brand.Slug,
            brand.ShortDescription,
            brand.LogoUrl,
            IndustryLabels.GetValueOrDefault(brand.IndustryCode),
            page.CoverColorToken,
            page.ShowPrices,
            await booking.IsBookingEnabledAsync(brand.Id, brand.Status, ct),
            rating,
            cards));
    }

    /// <summary>Platform bağlamında: güncel adla marka veya yönlendirmesi süren eski adın markası ve güncel adı.</summary>
    private static async Task<(Guid BrandId, string CurrentSlug)?> ResolveAsync(
        string slug, IDbSession session, TenancyDbContext db, Instant now, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        try
        {
            var current = await db.Brands.AsNoTracking()
                .Where(b => b.Slug == slug && b.Status != BrandStatus.Archived)
                .Select(b => new { b.Id, b.Slug })
                .SingleOrDefaultAsync(ct);
            if (current is not null)
            {
                return (current.Id, current.Slug);
            }

            var redirect = await (from h in db.SlugHistory.AsNoTracking()
                                  join b in db.Brands.AsNoTracking() on h.BrandId equals b.Id
                                  where h.Slug == slug && h.RedirectExpiresUtc > now && b.Status != BrandStatus.Archived
                                  orderby h.ChangedUtc descending
                                  select new { b.Id, b.Slug }).FirstOrDefaultAsync(ct);
            return redirect is null ? null : (redirect.Id, redirect.Slug);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }
    }
}
