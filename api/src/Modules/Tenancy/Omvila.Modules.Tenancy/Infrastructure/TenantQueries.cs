using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Infrastructure;

internal sealed class TenantQueries(TenancyDbContext db) : ITenantQueries
{
    public async Task<TenantSummary?> GetSummaryAsync(Guid brandId, IReadOnlyCollection<Guid>? branchIds, CancellationToken ct)
    {
        var brand = await db.Brands.AsNoTracking().SingleOrDefaultAsync(b => b.Id == brandId, ct);
        if (brand is null)
        {
            return null;
        }

        var branches = db.Branches.AsNoTracking().Where(b => b.BrandId == brandId && b.IsActive);
        if (branchIds is not null)
        {
            branches = branches.Where(b => branchIds.Contains(b.Id));
        }

        var list = await branches.OrderBy(b => b.CreatedUtc).Select(b => new BranchSummary(b.Id, b.Name)).ToListAsync(ct);
        return new TenantSummary(brand.Id, brand.Name, brand.Slug, brand.LogoUrl, brand.Status.ToString(), list);
    }

    public async Task<IReadOnlySet<Guid>> GetActiveStaffIdsAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct) =>
        (await db.Staff.AsNoTracking().Where(s => s.BrandId == brandId && s.IsActive && staffIds.Contains(s.Id)).Select(s => s.Id).ToListAsync(ct))
            .ToHashSet();

    public Task<bool> BranchExistsAsync(Guid brandId, Guid branchId, CancellationToken ct) =>
        db.Branches.AnyAsync(b => b.BrandId == brandId && b.Id == branchId, ct);

    public async Task<BrandCustomerSettings?> GetCustomerSettingsAsync(Guid brandId, CancellationToken ct)
    {
        var row = await (from b in db.Brands.AsNoTracking()
                         join s in db.BrandSettings.AsNoTracking() on b.Id equals s.BrandId
                         where b.Id == brandId
                         select new { s.MaskCustomerPhoneFromStaff, s.RiskyCustomerNoShowThreshold, b.DefaultCurrencyCode }).SingleOrDefaultAsync(ct);
        if (row is null)
        {
            return null;
        }

        var timeZoneId = await db.Branches.AsNoTracking().Where(b => b.BrandId == brandId && b.IsActive)
            .OrderBy(b => b.CreatedUtc).Select(b => b.TimeZoneId).FirstOrDefaultAsync(ct);
        return new BrandCustomerSettings(
            row.MaskCustomerPhoneFromStaff, row.RiskyCustomerNoShowThreshold, row.DefaultCurrencyCode, timeZoneId ?? PlatformDefaults.DefaultTimeZoneId);
    }

    // Kural: sureler-ve-ayarlar.md § Kapora; aktif hesap yoksa kapora yok (alan-modeli.md § BrandBankAccount).
    public async Task<BrandDepositSettings?> GetDepositSettingsAsync(Guid brandId, CancellationToken ct) =>
        await db.BrandSettings.AsNoTracking()
            .Where(s => s.BrandId == brandId)
            .Select(s => new BrandDepositSettings(
                s.DepositPolicy, s.RiskyCustomerNoShowThreshold, db.BankAccounts.Any(a => a.BrandId == brandId && a.IsActive)))
            .SingleOrDefaultAsync(ct);

    public Task<string?> GetBranchTimeZoneAsync(Guid brandId, Guid branchId, CancellationToken ct) =>
        db.Branches.AsNoTracking().Where(b => b.BrandId == brandId && b.Id == branchId).Select(b => (string?)b.TimeZoneId).SingleOrDefaultAsync(ct);

    public Task<BrandAppointmentSettings?> GetAppointmentSettingsAsync(Guid brandId, CancellationToken ct) =>
        db.BrandSettings.AsNoTracking()
            .Where(s => s.BrandId == brandId)
            .Select(s => new BrandAppointmentSettings(s.NoShowAfterMinutes, s.CancellationCutoffHours, s.RescheduleCutoffHours, s.MaskCustomerPhoneFromStaff,
                s.RequiresApproval, s.ApprovalTimeoutMinutes))
            .SingleOrDefaultAsync(ct);

    public async Task<int?> GetTransactionRetentionYearsAsync(Guid brandId, CancellationToken ct) =>
        await db.BrandSettings.AsNoTracking().Where(s => s.BrandId == brandId).Select(s => (int?)s.TransactionRetentionYears).SingleOrDefaultAsync(ct);

    public async Task<BrandNotificationSettings?> GetNotificationSettingsAsync(Guid brandId, CancellationToken ct)
    {
        var row = await db.BrandSettings.AsNoTracking()
            .Where(s => s.BrandId == brandId)
            .Select(s => new { s.ReminderOffsetsMinutes, s.ReviewInviteEnabled })
            .SingleOrDefaultAsync(ct);
        if (row is null)
        {
            return null;
        }

        var timeZoneId = await db.Branches.AsNoTracking().Where(b => b.BrandId == brandId && b.IsActive)
            .OrderBy(b => b.CreatedUtc).Select(b => b.TimeZoneId).FirstOrDefaultAsync(ct);
        return new BrandNotificationSettings(row.ReminderOffsetsMinutes, row.ReviewInviteEnabled, timeZoneId ?? PlatformDefaults.DefaultTimeZoneId);
    }

    public async Task<IReadOnlyDictionary<Guid, StaffCard>> GetStaffCardsAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct)
    {
        if (staffIds.Count == 0)
        {
            return new Dictionary<Guid, StaffCard>();
        }

        // Fotoğraf alanı modelde henüz yok (StaffSummary.photoUrl da null döner).
        return await db.Staff.AsNoTracking()
            .Where(s => s.BrandId == brandId && staffIds.Contains(s.Id))
            .Select(s => new StaffCard(s.Id, s.FullName, s.DisplayName, s.CalendarColor, null))
            .ToDictionaryAsync(s => s.StaffId, ct);
    }

    public async Task<IReadOnlyDictionary<Guid, decimal>> GetServiceCommissionPercentsAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct)
    {
        if (staffIds.Count == 0)
        {
            return new Dictionary<Guid, decimal>();
        }

        return await db.Staff.AsNoTracking()
            .Where(s => s.BrandId == brandId && staffIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.ServiceCommissionPercent, ct);
    }

    public async Task<decimal?> GetBranchSellerCommissionPercentAsync(Guid brandId, Guid staffId, Guid branchId, CancellationToken ct) =>
        await (from s in db.Staff.AsNoTracking()
               join l in db.StaffBranches.AsNoTracking() on s.Id equals l.StaffId
               where s.BrandId == brandId && s.Id == staffId && s.IsActive && l.BranchId == branchId && l.IsActive
               select (decimal?)s.SaleCommissionPercent).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<StaffTimeOffDetail>> GetTimeOffsAsync(
        Guid brandId, IReadOnlyCollection<Guid> staffIds, Instant fromUtc, Instant toUtc, CancellationToken ct)
    {
        if (staffIds.Count == 0)
        {
            return [];
        }

        var rows = await db.TimeOffs.AsNoTracking()
            .Where(t => t.BrandId == brandId && staffIds.Contains(t.StaffId) && t.StartUtc < toUtc && t.EndUtc > fromUtc)
            .OrderBy(t => t.StartUtc).ThenBy(t => t.Id)
            .ToListAsync(ct);
        return [.. rows.Select(t => new StaffTimeOffDetail(t.Id, t.StaffId, t.Type.ToString(), t.StartUtc, t.EndUtc, t.IsAllDay, t.Note))];
    }
}
