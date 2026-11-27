using NodaTime;
using Omvila.Infrastructure.Hosting;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.DemoData;
using Omvila.Modules.Platform.Contracts;
using Omvila.Modules.Platform.DemoData;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.DemoData;
using Omvila.SharedKernel;

namespace Omvila.Host.Seeding;

/// <summary>
/// Dilim 6 demo verisi (acik-sorular.md T-131): platform kullanıcıları, Kuaför Elif'te (Denemede) bekleyen kontör havalesi, SMS
/// gönderen adı başvurusu, sahte Google Takvim bağlantısı ve randevusuz yeni personel; Gecikti ve Aktif abonelikli iki küçük marka;
/// denetim kaydı satırları. Fiyat ve OMVILA hesabı uydurulmaz: plan fiyatı yoksa fatura kesilmez (S-14).
/// </summary>
internal sealed partial class DemoSeeder
{
    private const string CalendarKeySetting = "Integrations:Calendar:TokenEncryptionKey";

    /// <summary>
    /// Demo markaları sahiplerinin telefonuyla aranır. Biri varsa ve <c>--reset</c> yoksa onun kimliği döner (hiçbir şey yazılmaz);
    /// <c>--reset</c>'te hepsi ve önceki seed'in banka hareketleri (platform verisi, marka silinince kalır) silinir, null döner.
    /// </summary>
    private async Task<Guid?> FindOrEraseDemoBrandsAsync(bool reset, CancellationToken ct)
    {
        var existing = new List<Guid>();
        foreach (var phone in DemoDataSet.OwnerPhones())
        {
            if (await identity.FindOwnedBrandAsync(phone, ct) is { } id)
            {
                existing.Add(id);
            }
        }

        if (existing.Count > 0 && !reset)
        {
            return existing[0];
        }

        foreach (var brandId in existing)
        {
            await session.SetTenantAsync(TenantScope.ForBrand(brandId), ct);
            foreach (var eraser in erasers)
            {
                await eraser.EraseAsync(brandId, ct);
            }
        }

        await session.SetTenantAsync(TenantScope.Platform, ct);
        await platform.EraseBankTransactionsAsync(ct);
        return null;
    }

    private async Task<DemoSeedReport.PlatformExtras> AddDilim6Async(ProvisionedTenant main, Guid nisantasi, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        await session.SetTenantAsync(TenantScope.Platform, ct);
        var team = new Dictionary<string, Guid>(StringComparer.Ordinal);
        foreach (var seed in DemoDataSet.PlatformUsers)
        {
            var ensured = await platformUsers.EnsureAsync(PhoneNumber.Parse(seed.Phone), seed.FullName, seed.Role, ct);
            team[seed.Role] = ensured.IsSuccess ? ensured.Value.UserId : throw new DomainException("Demo platform kullanıcısı oluşturulamadı.");
        }

        var (admin, support, finance) = (team["PlatformAdmin"], team["PlatformSupport"], team["PlatformFinance"]);
        var audits = 0;
        foreach (var (userId, role, age) in new[] { (admin, "PlatformAdmin", Duration.FromHours(26)), (support, "PlatformSupport", Duration.FromHours(4)) })
        {
            await platform.AddAuditAsync(
                new AuditEntry(null, AuditActorType.PlatformUser, userId, AuditAction.Create, "PlatformSession", userId, Changes("platformRole", role)), now - age, ct);
            audits++;
        }

        await session.SetTenantAsync(TenantScope.ForBrand(main.BrandId), ct);
        var late = DemoDataSet.LateStaff;
        await tenancy.AddStaffAsync(
            main.BrandId,
            DemoDataSet.BusinessName,
            new DemoStaff(late.FullName, late.Title, late.Role, PhoneNumber.Parse(late.Phone), late.CalendarColor, late.IsBookableOnline, [new DemoStaffBranch(nisantasi, late.Hours)]),
            ct);
        var ownerUserId = await identity.FindUserIdAsync(DemoDataSet.OwnerPhone, ct) ?? throw new DomainException("Demo sahibinin kullanıcısı bulunamadı.");
        await notifications.SubmitSmsSenderAsync(main.BrandId, DemoDataSet.SmsSenderName, now - DemoDataSet.SmsSenderAge, ct);
        var creditNotice = await platform.AddCreditNoticeAsync(main.BrandId, DemoDataSet.OwnerName, ownerUserId, now - DemoDataSet.CreditNoticeAge, ct);

        // Jetonlar şifreli yazılır: seed ayrı süreçse (Testing dışı) API'nin de çözebilmesi için anahtar yapılandırılmış olmalı.
        var calendarKeyShared = !environment.DemoDataRequiresIbanKey() || !string.IsNullOrWhiteSpace(configuration[CalendarKeySetting]);
        var calendarLinked = calendarKeyShared
            && await integrations.ConnectAsync(main.BrandId, main.OwnerStaffId, ownerUserId, DemoDataSet.CalendarAccountMasked, now - Duration.FromDays(5), ct);
        await platform.AddAuditAsync(ViewEntry(main.BrandId, support, "Summary"), now - Duration.FromHours(3), ct);
        audits++;

        var brands = new List<DemoSeedReport.BrandSubscription>();
        foreach (var seed in DemoDataSet.ExtraBrands)
        {
            var (brandId, result) = await AddSmallBrandAsync(seed, finance, now, ct);
            await platform.AddAuditAsync(ViewEntry(brandId, finance, "Payments"), now - Duration.FromHours(1), ct);
            audits += string.Equals(result.Status, nameof(DemoSubscriptionState.Active), StringComparison.Ordinal) ? 2 : 1;
            brands.Add(new DemoSeedReport.BrandSubscription(seed.BusinessName, result.Status, result.InvoiceNumber, result.BankTransactions));
        }

        return new DemoSeedReport.PlatformExtras(brands, creditNotice is not null, calendarLinked, audits);
    }

    /// <summary>Tek şubeli, sahibi tek personel olan marka; kurulumu tamamlanır ve aboneliği hedef duruma getirilir.</summary>
    private async Task<(Guid BrandId, DemoSubscriptionResult Subscription)> AddSmallBrandAsync(
        DemoDataSet.BrandSeed seed, Guid platformUserId, Instant now, CancellationToken ct)
    {
        var ownerPhone = PhoneNumber.Parse(seed.OwnerPhone);
        var tenant = await identity.CreateBrandWithOwnerAsync(ownerPhone, seed.OwnerName, seed.BusinessName, seed.IndustryCode, ct);
        await tenancy.SetUpFirstBranchAsync(tenant.BrandId, tenant.BranchId, seed.Branch, ct);
        await tenancy.SetStaffHoursAsync(tenant.OwnerStaffId, tenant.BranchId, seed.OwnerTitle, seed.Branch.Hours, ct);
        var category = await catalog.AddCategoryAsync(tenant.BrandId, DemoDataSet.ExtraBrandCategory, ct);
        foreach (var service in seed.Services)
        {
            await catalog.AddServiceAsync(
                tenant.BrandId,
                new DemoService(
                    category, service.Name, null, service.DurationMinutes, new Money(service.PriceMinor, PlatformDefaults.DefaultCurrency),
                    StartingFrom: false, RequiresApproval: null, IsOnlineBookable: true, [tenant.OwnerStaffId]),
                ct);
        }

        if (!await tenancy.CompleteOnboardingAsync(tenant.BrandId, ct, solo: true))
        {
            throw new DomainException($"{seed.BusinessName} demo markasının kurulumu tamamlanmadı.");
        }

        var subscription = await platform.AgeSubscriptionAsync(
            new DemoSubscription(
                tenant.BrandId, Enum.Parse<DemoSubscriptionState>(seed.Subscription), seed.DaysSinceTrialEnd, seed.OwnerName,
                await identity.FindUserIdAsync(ownerPhone, ct), platformUserId, DemoDataSet.SubscriptionNoticeAge,
                DemoDataSet.BankTransferFallbackMinor, DemoDataSet.BankTransferMismatchMinor),
            now,
            ct);
        return (tenant.BrandId, subscription);
    }

    private static AuditEntry ViewEntry(Guid brandId, Guid userId, string section) =>
        new(brandId, AuditActorType.PlatformUser, userId, AuditAction.View, "Brand", brandId, Changes("section", section));

    private static Dictionary<string, object?> Changes(string key, string value) => new(StringComparer.Ordinal) { [key] = value };
}
