using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.DemoData;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.DemoData;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Scheduling.DemoData;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.Modules.Tenancy.DemoData;
using Omvila.SharedKernel;

namespace Omvila.Host.Seeding;

/// <summary>
/// Demo markasını modüllerin kendi yazma yollarıyla kurar (acik-sorular.md T-74). Demo markası, sahibinin ayrılmış telefonuyla
/// (<see cref="DemoDataSet.OwnerPhone"/>) tanınır: varsa ve <c>--reset</c> yoksa hiçbir şey yazılmaz (idempotent);
/// <c>--reset</c>'te yalnız o markanın satırları her modülün <see cref="IBrandDataEraser"/>'ı ile silinip yeniden kurulur.
/// Randevu tarihleri saate (IClock) göredir: son <see cref="DemoDataSet.DaysBack"/> ve gelecek <see cref="DemoDataSet.DaysAhead"/> gün.
/// Bütün adımlar çağıranın tek transaction'ındadır.
/// </summary>
internal sealed class DemoSeeder(
    IDbSession session,
    IdentityDemoData identity,
    TenancyDemoData tenancy,
    CatalogDemoData catalog,
    SchedulingDemoData scheduling,
    ICustomerDirectory customers,
    ICreditLedger credits,
    IPublicBookingQueries booking,
    IEnumerable<IBrandDataEraser> erasers,
    IClock clock)
{
    private const int FirstSlotOffsetMinutes = 30;

    public async Task<DemoSeedReport> RunAsync(bool reset, CancellationToken ct)
    {
        var bookingUrl = booking.BookingPageUrl(DemoDataSet.Slug);
        var existing = await identity.FindOwnedBrandAsync(DemoDataSet.OwnerPhone, ct);
        if (existing is { } existingId)
        {
            if (!reset)
            {
                return DemoSeedReport.Unchanged(existingId, bookingUrl);
            }

            await session.SetTenantAsync(TenantScope.ForBrand(existingId), ct);
            foreach (var eraser in erasers)
            {
                await eraser.EraseAsync(existingId, ct);
            }
        }

        var tenant = await identity.CreateBrandWithOwnerAsync(
            DemoDataSet.OwnerPhone, DemoDataSet.OwnerName, DemoDataSet.BusinessName, DemoDataSet.IndustryCode, ct);
        if (!string.Equals(tenant.Slug, DemoDataSet.Slug, StringComparison.Ordinal))
        {
            throw new DomainException($"'{DemoDataSet.Slug}' bağlantı adı başka bir markada; demo markası kurulmadı.");
        }

        var brandId = tenant.BrandId;
        var staff = await AddBranchesAndStaffAsync(tenant, ct);
        var services = await AddCatalogAsync(brandId, staff, ct);
        if (!await tenancy.CompleteOnboardingAsync(brandId, ct))
        {
            throw new DomainException("Demo markasının kurulumu tamamlanmadı.");
        }

        var balance = await credits.GetBalanceAsync(brandId, ct);
        if (balance < DemoDataSet.SmsCredits)
        {
            await credits.GrantAsync(brandId, DemoDataSet.SmsCredits - balance, "DemoSeed", null, ct);
        }

        var cards = new List<EnsuredBrandCustomer>();
        for (var i = 0; i < DemoDataSet.Customers.Count; i++)
        {
            cards.Add(await customers.EnsureBrandCustomerAsync(
                new EnsureBrandCustomerRequest(brandId, DemoDataSet.CustomerPhone(i), DemoDataSet.Customers[i], CustomerSource.Panel, PhoneVerified: false), ct));
        }

        var outcomes = await AddAppointmentsAsync(brandId, staff, services, cards, ct);
        return new DemoSeedReport(
            Created: true,
            brandId,
            bookingUrl,
            DemoSeedReport.AllLogins(),
            Branches: 2,
            Services: services.Count,
            Customers: cards.Count,
            outcomes,
            DemoDataSet.SmsCredits);
    }

    private async Task<List<SeededStaff>> AddBranchesAndStaffAsync(ProvisionedTenant tenant, CancellationToken ct)
    {
        await tenancy.SetUpFirstBranchAsync(tenant.BrandId, tenant.BranchId, DemoDataSet.Kadikoy, ct);
        var nisantasi = await tenancy.AddBranchAsync(tenant.BrandId, DemoDataSet.Nisantasi, ct);
        await tenancy.SetBookingRulesAsync(
            tenant.BrandId, DemoDataSet.ApprovalTimeoutMinutes, DemoDataSet.DepositPolicy, DemoDataSet.RiskyCustomerNoShowThreshold, ct);
        await tenancy.AddBankAccountAsync(tenant.BrandId, DemoDataSet.BankAccount, ct);
        await tenancy.SetStaffHoursAsync(tenant.OwnerStaffId, tenant.BranchId, DemoDataSet.OwnerTitle, DemoDataSet.OwnerHours, ct);

        var staff = new List<SeededStaff> { new(DemoDataSet.OwnerName, tenant.OwnerStaffId, tenant.BranchId, IsBookableOnline: true, DemoDataSet.OwnerHours) };
        foreach (var seed in DemoDataSet.Staff)
        {
            var branchId = seed.Branch == DemoDataSet.Branch.Kadikoy ? tenant.BranchId : nisantasi;
            var id = await tenancy.AddStaffAsync(
                tenant.BrandId,
                DemoDataSet.BusinessName,
                new DemoStaff(seed.FullName, seed.Title, seed.Role, PhoneNumber.Parse(seed.Phone), seed.CalendarColor, seed.IsBookableOnline, [new DemoStaffBranch(branchId, seed.Hours)]),
                ct);
            staff.Add(new SeededStaff(seed.FullName, id, branchId, seed.IsBookableOnline, seed.Hours));
        }

        return staff;
    }

    private async Task<List<SeededService>> AddCatalogAsync(Guid brandId, List<SeededStaff> staff, CancellationToken ct)
    {
        var categories = new Dictionary<string, Guid>(StringComparer.Ordinal);
        foreach (var name in DemoDataSet.Categories)
        {
            categories[name] = await catalog.AddCategoryAsync(brandId, name, ct);
        }

        var services = new List<SeededService>();
        foreach (var seed in DemoDataSet.Services)
        {
            var staffIds = seed.Staff.Select(name => staff.Single(s => string.Equals(s.Name, name, StringComparison.Ordinal)).Id).ToList();
            var id = await catalog.AddServiceAsync(
                brandId,
                new DemoService(
                    categories[seed.Category], seed.Name, seed.Description, seed.DurationMinutes, new Money(seed.PriceMinor, PlatformDefaults.DefaultCurrency),
                    seed.StartingFrom, seed.RequiresApproval, IsOnlineBookable: true, staffIds, seed.DepositPercent,
                    seed.DepositAmountMinor is { } deposit ? new Money(deposit, PlatformDefaults.DefaultCurrency) : null),
                ct);
            services.Add(new SeededService(id, seed.DurationMinutes, seed.RequiresApproval == true, seed.HasDeposit, staffIds));
        }

        return services;
    }

    /// <summary>
    /// Personel × gün başına 2–3 randevu, çalışma aralıkları içinde art arda (çakışma olmaz). Geçmişte biten: Geldi (tamamlama
    /// adisyonla, D6), ilk iki müşterinin ilk iki geçmiş randevusu Gelmedi, bir kısmı salon iptali. Şu an süren: Geldi. Gelecek:
    /// Onaylandı, bir kısmı salon veya müşteri iptali. Online randevu alan personelin yarından sonraki ilk
    /// <see cref="DemoDataSet.PendingDays"/> çalışma gününde günün ilk randevusu onay gerektiren hizmetle Onay bekliyor, ikinci
    /// randevusu sırayla <see cref="DemoDataSet.DepositOutcomes"/> (kaporalı hizmet, riskli müşteri; T-85).
    /// </summary>
    private async Task<Dictionary<DemoAppointmentOutcome, int>> AddAppointmentsAsync(
        Guid brandId, List<SeededStaff> staff, List<SeededService> services, List<EnsuredBrandCustomer> cards, CancellationToken ct)
    {
        var zone = DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var now = clock.GetCurrentInstant();
        var today = now.InZone(zone).Date;
        var outcomes = Enum.GetValues<DemoAppointmentOutcome>().ToDictionary(o => o, _ => 0);
        var noShows = new int[cards.Count];
        var ordinal = 0;
        var futureWorkdays = new int[staff.Count];
        var depositsMade = 0;
        for (var offset = -DemoDataSet.DaysBack; offset <= DemoDataSet.DaysAhead; offset++)
        {
            var date = today.PlusDays(offset);
            var dayOrdinal = 0;
            for (var s = 0; s < staff.Count; s++)
            {
                var member = staff[s];
                var intervals = member.Hours.Where(h => h.Day == date.DayOfWeek).OrderBy(h => h.Start).ToList();
                var offered = services.Where(x => x.StaffIds.Contains(member.Id)).ToList();
                var count = 2 + (Math.Abs(offset + s) % 2);
                if (offset >= 1 && intervals.Count > 0)
                {
                    futureWorkdays[s]++;
                }

                var cursor = intervals.Count > 0 ? intervals[0].Start.PlusMinutes(FirstSlotOffsetMinutes) : default;
                for (var made = 0; made < count && intervals.Count > 0; made++)
                {
                    var early = offset >= 1 && futureWorkdays[s] <= DemoDataSet.PendingDays && member.IsBookableOnline;
                    var (service, forced) = Pick(offered, early, made, offset, s, depositsMade);
                    if (Fit(intervals, cursor, service.DurationMinutes) is not { } local)
                    {
                        break;
                    }

                    var start = date.At(local).InZoneLeniently(zone).ToInstant();
                    var end = start + Duration.FromMinutes(service.DurationMinutes);
                    var deposit = forced is not null and not DemoAppointmentOutcome.PendingApproval;
                    var customer = deposit ? RiskyCustomer(noShows, depositsMade) : ((offset + DemoDataSet.DaysBack) * 5 + dayOrdinal) % cards.Count;
                    var outcome = forced ?? Decide(start, end, now, customer, ordinal, noShows);
                    await scheduling.AddAsync(
                        new DemoAppointment(
                            brandId, member.BranchId, cards[customer].CustomerId, cards[customer].BrandCustomerId, start, service.Id, member.Id, outcome,
                            noShows[customer]),
                        now,
                        ct);
                    if (deposit)
                    {
                        depositsMade++;
                    }

                    outcomes[outcome]++;
                    ordinal++;
                    dayOrdinal++;
                    cursor = local.PlusMinutes(service.DurationMinutes + (made % 2 == 0 ? 30 : 60));
                }
            }
        }

        return outcomes;
    }

    /// <summary>
    /// Randevunun hizmeti ve (varsa) zorunlu sonucu: erken günlerde günün ilk randevusu onay gerektiren hizmetle Onay bekliyor,
    /// ikincisi sıradaki kaporalı sonuç; diğerleri <see cref="Decide"/> ile.
    /// </summary>
    private static (SeededService Service, DemoAppointmentOutcome? Forced) Pick(
        List<SeededService> offered, bool early, int made, int offset, int staffIndex, int depositsMade)
    {
        var approval = offered.Where(x => x.RequiresApproval).ToList();
        var withDeposit = offered.Where(x => x.HasDeposit).ToList();
        if (early && made == 0 && approval.Count > 0)
        {
            return (approval[offset % approval.Count], DemoAppointmentOutcome.PendingApproval);
        }

        if (early && made == 1 && withDeposit.Count > 0 && depositsMade < DemoDataSet.DepositOutcomes.Count)
        {
            return (withDeposit[depositsMade % withDeposit.Count], DemoDataSet.DepositOutcomes[depositsMade]);
        }

        return (offered[Math.Abs((offset * 3) + staffIndex + made) % offered.Count], null);
    }

    /// <summary>Kapora istenecek (riskli eşiğine ulaşmış) müşterilerden sıradaki; geçmiş Gelmedi'ler gelecekten önce yazılır.</summary>
    private static int RiskyCustomer(int[] noShows, int turn)
    {
        var risky = Enumerable.Range(0, noShows.Length).Where(i => noShows[i] >= DemoDataSet.RiskyCustomerNoShowThreshold).ToList();
        return risky.Count > 0
            ? risky[turn % risky.Count]
            : throw new DomainException("Demo verisinde kapora istenecek riskli müşteri yok.");
    }

    private static DemoAppointmentOutcome Decide(Instant start, Instant end, Instant now, int customer, int ordinal, int[] noShows)
    {
        if (end <= now)
        {
            // Musteriler.png: iki müşteri "2 kez gelmedi".
            if (customer < 2 && noShows[customer] < 2)
            {
                noShows[customer]++;
                return DemoAppointmentOutcome.NoShow;
            }

            return ordinal % 9 == 4 ? DemoAppointmentOutcome.CancelledBySalon : DemoAppointmentOutcome.CheckedIn;
        }

        if (start <= now)
        {
            return DemoAppointmentOutcome.CheckedIn;
        }

        // Müşteri iptali yalnız iptal sınırının (24 sa) dışında olur; bir saat pay bırakılır.
        return (ordinal % 9) switch
        {
            4 => DemoAppointmentOutcome.CancelledBySalon,
            7 when start - now >= Duration.FromHours(25) => DemoAppointmentOutcome.CancelledByCustomer,
            _ => DemoAppointmentOutcome.Confirmed,
        };
    }

    /// <summary>İmleçten başlayarak süreyi bütünüyle içine alan ilk aralıktaki başlangıç; yoksa null.</summary>
    private static LocalTime? Fit(List<DemoHours> intervals, LocalTime cursor, int minutes)
    {
        foreach (var interval in intervals)
        {
            var start = cursor < interval.Start ? interval.Start : cursor;
            if (start >= interval.Start && start.PlusMinutes(minutes) <= interval.End && start.PlusMinutes(minutes) > start)
            {
                return start;
            }
        }

        return null;
    }

    private sealed record SeededStaff(string Name, Guid Id, Guid BranchId, bool IsBookableOnline, IReadOnlyList<DemoHours> Hours);

    private sealed record SeededService(Guid Id, int DurationMinutes, bool RequiresApproval, bool HasDeposit, IReadOnlyList<Guid> StaffIds);
}
