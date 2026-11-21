using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.Domain;

namespace Omvila.Modules.Tenancy.Infrastructure;

/// <summary>
/// Public yüzeyin Tenancy okumaları. Şube araması (markası henüz bilinmiyor) platform bağlamında yapılır, ardından oturum
/// yalnız o şubenin markasına kurulur (SlugRegistry ile aynı desen); sonraki bütün okumalar marka RLS'iyle sınırlıdır.
/// </summary>
internal sealed class PublicBookingQueries(
    TenancyDbContext db, IDbSession session, ISubscriptionQueries subscriptions, IOptions<TenancyOptions> options) : IPublicBookingQueries
{
    // durum-makinesi.md § Abonelik: salt okunur aboneliğde online rezervasyon kapalı; iptal edilen abonelik de randevu almaz.
    private static readonly HashSet<string> ClosedSubscriptionStatuses = new(["ReadOnly", "Cancelled"], StringComparer.Ordinal);

    public async Task<PublicBranchScope?> EnterBranchAsync(Guid branchId, CancellationToken ct)
    {
        var previous = session.Scope;
        await session.SetTenantAsync(TenantScope.Platform, ct);
        BranchRow? row;
        try
        {
            row = await (from b in db.Branches.AsNoTracking()
                         join brand in db.Brands.AsNoTracking() on b.BrandId equals brand.Id
                         where b.Id == branchId && b.IsActive && brand.Status != BrandStatus.Archived
                         select new BranchRow(b.BrandId, b.TimeZoneId, brand.Status)).SingleOrDefaultAsync(ct);
        }
        finally
        {
            await session.SetTenantAsync(previous, ct);
        }

        if (row is null)
        {
            return null;
        }

        await session.SetTenantAsync(TenantScope.ForBrand(row.BrandId), ct);
        return new PublicBranchScope(row.BrandId, branchId, row.TimeZoneId, await IsBookingEnabledAsync(row.BrandId, row.Status, ct));
    }

    public async Task<PublicBookingOptions> GetBookingOptionsAsync(Guid brandId, CancellationToken ct) =>
        await (from s in db.BrandSettings.AsNoTracking()
               join p in db.BookingPageSettings.AsNoTracking() on s.BrandId equals p.BrandId
               where s.BrandId == brandId
               select new PublicBookingOptions(
                   s.AllowStaffSelection, s.AllowAnyStaffOption, p.ShowPrices, s.DepositPolicy,
                   db.BankAccounts.Any(a => a.BrandId == brandId && a.IsActive))).SingleAsync(ct);

    public async Task<IReadOnlyList<PublicStaffCard>> GetBookableStaffAsync(Guid brandId, Guid branchId, CancellationToken ct)
    {
        // Fotoğraf alanı modelde henüz yok (StaffCard.PhotoUrl gibi null).
        var rows = await (from sb in db.StaffBranches.AsNoTracking()
                          join s in db.Staff.AsNoTracking() on sb.StaffId equals s.Id
                          where sb.BrandId == brandId && sb.BranchId == branchId && sb.IsActive && s.IsActive && s.IsBookableOnline
                          select new { s.Id, s.FullName, s.DisplayName, s.Title }).ToListAsync(ct);
        return [.. rows
            .Select(r => new PublicStaffCard(r.Id, r.DisplayName ?? r.FullName, r.Title, null))
            .OrderBy(r => r.DisplayName, StringComparer.Create(System.Globalization.CultureInfo.GetCultureInfo("tr-TR"), ignoreCase: true))
            .ThenBy(r => r.StaffId)];
    }

    public Task<PublicBranchCard?> GetBranchCardAsync(Guid brandId, Guid branchId, CancellationToken ct) =>
        (from b in db.Branches.AsNoTracking()
         join brand in db.Brands.AsNoTracking() on b.BrandId equals brand.Id
         join p in db.BookingPageSettings.AsNoTracking() on brand.Id equals p.BrandId
         where b.BrandId == brandId && b.Id == branchId
         select new PublicBranchCard(
             brand.Name, brand.Slug, brand.LogoUrl, b.Id, b.Name, b.AddressLine, b.District, b.Phone, b.Latitude, b.Longitude, b.TimeZoneId, p.PreAppointmentNote))
        .SingleOrDefaultAsync(ct);

    public Uri AppointmentPageUrl(string accessCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessCode);
        return new Uri(options.Value.BookingPageBaseUrl!, $"r/{accessCode}");
    }

    public Uri WaitlistOfferPageUrl(string accessCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessCode);
        return new Uri(options.Value.BookingPageBaseUrl!, $"b/{accessCode}");
    }

    public Uri BookingPageUrl(string brandSlug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(brandSlug);
        return new Uri(options.Value.BookingPageBaseUrl!, brandSlug);
    }

    /// <summary>Marka bağlamında çağrılır (abonelik RLS'li).</summary>
    internal async Task<bool> IsBookingEnabledAsync(Guid brandId, BrandStatus status, CancellationToken ct)
    {
        if (status is BrandStatus.Suspended or BrandStatus.Archived)
        {
            return false;
        }

        var banner = await subscriptions.GetBannerAsync(brandId, ct);
        return banner is null || !ClosedSubscriptionStatuses.Contains(banner.Status);
    }

    private sealed record BranchRow(Guid BrandId, string TimeZoneId, BrandStatus Status);
}
