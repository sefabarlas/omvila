using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Tenancy.Domain;
using Omvila.Modules.Tenancy.Features.Onboarding;
using Omvila.Modules.Tenancy.Infrastructure;
using Omvila.Modules.Tenancy.Infrastructure.BankAccounts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.DemoData;

/// <summary>
/// Demo verisi (<c>--role=seed</c>; acik-sorular.md T-74) için Tenancy yazma yolu: şube, çalışma saatleri, personel (Identity
/// üyeliği <see cref="IStaffMemberships"/> ile, davet SMS'i gönderilmeden), onay süresi ve kapora politikası, kapora hesabı
/// (IBAN <see cref="IbanProtector"/> ile şifreli) ve kurulumun tamamlanması (<see cref="OnboardingService"/>). Değerler
/// uçlardaki alan kurallarından geçer (WeeklyHours, BrandSettingsRules, BankAccountRules).
/// Çağıranın transaction'ında ve markanın kiracı bağlamında çalışır.
/// </summary>
public sealed class TenancyDemoData
{
    private readonly TenancyDbContext _db;
    private readonly IStaffMemberships _memberships;
    private readonly OnboardingService _onboarding;
    private readonly IbanProtector _protector;
    private readonly IIdGenerator _ids;
    private readonly IClock _clock;

    internal TenancyDemoData(
        TenancyDbContext db, IStaffMemberships memberships, OnboardingService onboarding, IbanProtector protector, IIdGenerator ids, IClock clock)
    {
        _db = db;
        _memberships = memberships;
        _onboarding = onboarding;
        _protector = protector;
        _ids = ids;
        _clock = clock;
    }

    /// <summary>Kayıtta oluşan ilk şubeye ("Merkez") ad, adres ve saat verir.</summary>
    public async Task SetUpFirstBranchAsync(Guid brandId, Guid branchId, DemoBranch input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var branch = await _db.Branches.SingleAsync(b => b.Id == branchId && b.BrandId == brandId, ct);
        branch.Update(Details(input));
        await _db.SaveChangesAsync(ct);
        await AddBranchHoursAsync(brandId, branchId, input.Hours, ct);
    }

    /// <summary>Yeni şube (saat dilimi markanın varsayılanı). Fiyatlar şubeye özel satır olmadan hizmetin fiyatıdır.</summary>
    public async Task<Guid> AddBranchAsync(Guid brandId, DemoBranch input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var taken = (await _db.Branches.Where(b => b.BrandId == brandId).Select(b => b.Slug).ToListAsync(ct)).ToHashSet(StringComparer.Ordinal);
        var slug = SlugGenerator.FirstAvailable(input.Name, taken.Contains).Value;
        var branch = Branch.Create(_ids.New(), brandId, Details(input), slug, PlatformDefaults.DefaultTimeZoneId, _clock.GetCurrentInstant());
        _db.Branches.Add(branch);
        await _db.SaveChangesAsync(ct);
        await AddBranchHoursAsync(brandId, branch.Id, input.Hours, ct);
        return branch.Id;
    }

    /// <summary>
    /// Onay yanıt süresi (ApprovalTimeoutMinutes; izinli değerlerden biri olmalı), kapora politikası ve riskli müşteri eşiği.
    /// Diğer ayarlar korunur.
    /// </summary>
    public async Task SetBookingRulesAsync(
        Guid brandId, int approvalTimeoutMinutes, string depositPolicy, int riskyCustomerNoShowThreshold, CancellationToken ct)
    {
        var settings = await _db.BrandSettings.SingleAsync(s => s.BrandId == brandId, ct);
        var values = new BrandSettingsValues(
            settings.RequiresApproval, approvalTimeoutMinutes, settings.AllowStaffSelection, settings.AllowAnyStaffOption, settings.CancellationCutoffHours,
            settings.RescheduleCutoffHours, settings.NoShowAfterMinutes, settings.MinNoticeMinutes, settings.MaxAdvanceDays,
            settings.SlotIntervalMinutes, settings.DefaultBranchTransferMinutes, depositPolicy, riskyCustomerNoShowThreshold,
            settings.MaskCustomerPhoneFromStaff, settings.ReminderOffsetsMinutes, settings.ReviewInviteEnabled);
        var errors = BrandSettingsRules.Validate(values);
        if (errors.Count > 0)
        {
            throw new DomainException($"Geçersiz demo ayarı: {string.Join(", ", errors.Keys)}");
        }

        settings.Apply(values);
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Aktif kapora hesabı (manageUpdateBankAccount ilk kaydı gibi, BankAccountRules); IBAN şifreli saklanır.</summary>
    public async Task AddBankAccountAsync(Guid brandId, DemoBankAccount input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        var errors = BankAccountRules.Validate(
            input.AccountHolderName, input.BankName, input.TransferCodePrefix, input.TransferWindowMinutes, input.Iban, isFirstSave: true, out var iban);
        if (errors.Count > 0 || iban is null)
        {
            throw new DomainException($"Geçersiz demo kapora hesabı: {string.Join(", ", errors.Keys)}");
        }

        var values = new BankAccountValues(input.AccountHolderName, input.BankName, input.TransferCodePrefix, input.TransferWindowMinutes);
        _db.BankAccounts.Add(BrandBankAccount.Create(brandId, values, _protector.Protect(brandId, iban), _clock.GetCurrentInstant()));
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Personel ekler: kayıt, şube bağları, saatler ve (davet SMS'i olmadan) panel üyeliği.</summary>
    public async Task<Guid> AddStaffAsync(Guid brandId, string brandName, DemoStaff input, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (!Enum.TryParse<StaffRole>(input.Role, out var role) || input.Branches.Count == 0)
        {
            throw new DomainException("Geçersiz demo personeli.");
        }

        var staff = Staff.Create(
            _ids.New(), brandId, input.FullName, input.Title, input.CalendarColor, 0, 0, input.IsBookableOnline, _clock.GetCurrentInstant());
        _db.Staff.Add(staff);
        await _db.SaveChangesAsync(ct);
        var links = input.Branches
            .Select((b, i) => StaffBranch.Create(_ids.New(), brandId, staff.Id, b.BranchId, isPrimary: i == 0))
            .ToList();
        _db.StaffBranches.AddRange(links);
        await _db.SaveChangesAsync(ct);
        for (var i = 0; i < links.Count; i++)
        {
            AddStaffHours(links[i], input.Branches[i].Hours);
        }

        await _db.SaveChangesAsync(ct);
        var invited = await _memberships.InviteAsync(
            new StaffInvitation(brandId, brandName, staff.Id, input.Phone.E164, staff.FullName, role.ToString(), [.. input.Branches.Select(b => b.BranchId)], SendInvite: false),
            ct);
        if (!invited.IsSuccess)
        {
            throw new DomainException($"Demo personelinin üyeliği açılamadı: {invited.Error.Code}");
        }

        return staff.Id;
    }

    /// <summary>Var olan personelin (ör. sahip) şubedeki saatlerini yazar; ünvanı verilirse günceller.</summary>
    public async Task SetStaffHoursAsync(Guid staffId, Guid branchId, string? title, IReadOnlyList<DemoHours> hours, CancellationToken ct)
    {
        var link = await _db.StaffBranches.SingleAsync(l => l.StaffId == staffId && l.BranchId == branchId, ct);
        AddStaffHours(link, hours);
        if (title is not null)
        {
            var staff = await _db.Staff.SingleAsync(s => s.Id == staffId, ct);
            staff.Update(new StaffProfile(
                staff.FullName, staff.DisplayName, title, staff.Bio, staff.CalendarColor, staff.ServiceCommissionPercent,
                staff.SaleCommissionPercent, staff.IsBookableOnline, staff.IsActive));
        }

        await _db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Kurulumun son adımı (bağlantı paylaşıldı) kaydedilir ve adımlar değerlendirilir; hepsi tamamsa marka Active olur.
    /// Dönüş: marka Active mi.
    /// </summary>
    public async Task<bool> CompleteOnboardingAsync(Guid brandId, CancellationToken ct)
    {
        await _onboarding.RecordAsync(brandId, OnboardingEvent.BookingLinkCopied, ct);
        return await _db.Brands.AnyAsync(b => b.Id == brandId && b.Status == BrandStatus.Active, ct);
    }

    private static BranchDetails Details(DemoBranch input) =>
        new(input.Name, input.AddressLine, input.District, input.City, null, null, null, null, null, null, IsActive: true);

    private static WeeklyHours Weekly(IReadOnlyList<DemoHours> hours)
    {
        var weekly = WeeklyHours.Create([.. hours.GroupBy(h => h.Day).Select(g =>
            (g.Key, (IReadOnlyList<(LocalTime, LocalTime)>)[.. g.Select(h => (h.Start, h.End))]))]);
        return weekly.IsSuccess ? weekly.Value : throw new DomainException($"Geçersiz demo saatleri: {weekly.Error.Code}");
    }

    private async Task AddBranchHoursAsync(Guid brandId, Guid branchId, IReadOnlyList<DemoHours> hours, CancellationToken ct)
    {
        _db.BranchHours.AddRange(BranchWorkingHours.From(brandId, branchId, Weekly(hours), _ids.New));
        await _db.SaveChangesAsync(ct);
    }

    private void AddStaffHours(StaffBranch link, IReadOnlyList<DemoHours> hours) =>
        _db.StaffHours.AddRange(Weekly(hours).Days.SelectMany(day => day.Value.Select(interval =>
            StaffWorkingHours.Create(_ids.New(), link.BrandId, link.Id, day.Key, interval.Start, interval.End))));
}
