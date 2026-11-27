using NodaTime;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Catalog.DemoData;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.DemoData;
using Omvila.Modules.Integrations.DemoData;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.DemoData;
using Omvila.Modules.Platform.DemoData;
using Omvila.Modules.Reviews.DemoData;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.DemoData;
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
/// Geçmiş randevular tamamlanır (tahsilat, hizmet primi, paket seansı; T-114); paketler, kapatılmış kasa günü, değerlendirmeler,
/// bekleme listesi ve bugünün kapıda sırası da yazılır. Dilim 6 verisi (T-131) <c>DemoSeeder.Dilim6.cs</c>'tedir. Bütün adımlar çağıranın tek transaction'ındadır.
/// </summary>
internal sealed partial class DemoSeeder(
    IDbSession session,
    IdentityDemoData identity,
    TenancyDemoData tenancy,
    CatalogDemoData catalog,
    SchedulingDemoData scheduling,
    SalesDemoData sales,
    ReviewsDemoData reviews,
    ICustomerDirectory customers,
    ICreditLedger credits,
    IPublicBookingQueries booking,
    IEnumerable<IBrandDataEraser> erasers,
    IClock clock,
    PlatformDemoData platform,
    NotificationsDemoData notifications,
    IntegrationsDemoData integrations,
    IPlatformUserProvisioning platformUsers,
    IConfiguration configuration,
    IHostEnvironment environment)
{
    private const int FirstSlotOffsetMinutes = 30;

    public async Task<DemoSeedReport> RunAsync(bool reset, CancellationToken ct)
    {
        var bookingUrl = booking.BookingPageUrl(DemoDataSet.Slug);
        if (await FindOrEraseDemoBrandsAsync(reset, ct) is { } unchanged)
        {
            return DemoSeedReport.Unchanged(unchanged, bookingUrl);
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

        var (run, extras) = await AddActivityAsync(brandId, staff, services, cards, ct);
        var dilim6 = await AddDilim6Async(tenant, staff[^1].BranchId, ct);
        return new DemoSeedReport(
            Created: true,
            brandId,
            bookingUrl,
            DemoSeedReport.AllLogins(),
            Branches: 2,
            Services: services.Count,
            Customers: cards.Count,
            run.Outcomes,
            DemoDataSet.SmsCredits,
            extras,
            dilim6);
    }

    /// <summary>Paketler, randevular (tamamlananların tahsilatı ve primi), kasa günü, değerlendirmeler, bekleme listesi, kapıda sıra.</summary>
    private async Task<(AppointmentRun Run, DemoSeedReport.Extras Extras)> AddActivityAsync(
        Guid brandId, List<SeededStaff> staff, List<SeededService> services, List<EnsuredBrandCustomer> cards, CancellationToken ct)
    {
        var zone = DateTimeZoneProviders.Tzdb[PlatformDefaults.DefaultTimeZoneId];
        var run = new AppointmentRun(brandId, staff, services, cards, await AddPackagesAsync(brandId, staff, services, cards, zone, ct));
        await AddAppointmentsAsync(run, zone, ct);
        var cashDay = await CloseCashDayAsync(run, zone, ct);
        var reviewCount = await AddReviewsAsync(run, ct);
        await AddWaitlistAsync(run, zone, ct);
        await AddWalkInsAsync(run, ct);
        return (run, new DemoSeedReport.Extras(
            DemoDataSet.Packages.Count, run.Packages.Count, run.Completed.Count, cashDay, reviewCount, DemoDataSet.Waitlist.Count, DemoDataSet.WalkIns.Count));
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

        foreach (var member in staff)
        {
            var (service, sale) = DemoDataSet.Commissions[member.Name];
            await tenancy.SetCommissionAsync(member.Id, service, sale, ct);
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
            services.Add(new SeededService(id, seed.Name, seed.DurationMinutes, seed.RequiresApproval == true, seed.HasDeposit, staffIds));
        }

        return services;
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

            return ordinal % 9 == 4 ? DemoAppointmentOutcome.CancelledBySalon : DemoAppointmentOutcome.Completed;
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

    /// <summary>
    /// Personel × gün başına 2–3 randevu, çalışma aralıkları içinde art arda (çakışma olmaz). Geçmişte biten: Tamamlandı (tahsilat
    /// yöntemi sırayla <see cref="DemoDataSet.PaymentMethods"/>; paket planındaki hizmetin randevusu o müşteriye yazılır ve paketten
    /// düşülür), ilk iki müşterinin ilk iki geçmiş randevusu Gelmedi, bir kısmı salon iptali. Şu an süren: Geldi. Gelecek:
    /// Onaylandı, bir kısmı salon veya müşteri iptali. Online randevu alan personelin yarından sonraki ilk
    /// <see cref="DemoDataSet.PendingDays"/> çalışma gününde günün ilk randevusu onay gerektiren hizmetle Onay bekliyor, ikinci
    /// randevusu sırayla <see cref="DemoDataSet.DepositOutcomes"/> (kaporalı hizmet, riskli müşteri; T-85).
    /// </summary>
    private async Task AddAppointmentsAsync(AppointmentRun run, DateTimeZone zone, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var today = now.InZone(zone).Date;
        var futureWorkdays = new int[run.Staff.Count];
        for (var offset = -DemoDataSet.DaysBack; offset <= DemoDataSet.DaysAhead; offset++)
        {
            var date = today.PlusDays(offset);
            var dayOrdinal = 0;
            for (var s = 0; s < run.Staff.Count; s++)
            {
                var member = run.Staff[s];
                var intervals = member.Hours.Where(h => h.Day == date.DayOfWeek).OrderBy(h => h.Start).ToList();
                var offered = run.Services.Where(x => x.StaffIds.Contains(member.Id)).ToList();
                var count = 2 + (Math.Abs(offset + s) % 2);
                if (offset >= 1 && intervals.Count > 0)
                {
                    futureWorkdays[s]++;
                }

                var cursor = intervals.Count > 0 ? intervals[0].Start.PlusMinutes(FirstSlotOffsetMinutes) : default;
                for (var made = 0; made < count && intervals.Count > 0; made++)
                {
                    var early = offset >= 1 && futureWorkdays[s] <= DemoDataSet.PendingDays && member.IsBookableOnline;
                    var (service, forced) = Pick(offered, early, made, offset, s, run.DepositsMade);
                    if (Fit(intervals, cursor, service.DurationMinutes) is not { } local)
                    {
                        break;
                    }

                    var start = date.At(local).InZoneLeniently(zone).ToInstant();
                    var customer = ((offset + DemoDataSet.DaysBack) * 5 + dayOrdinal) % run.Cards.Count;
                    await WriteAppointmentAsync(run, member, service, start, customer, forced, now, ct);
                    dayOrdinal++;
                    cursor = local.PlusMinutes(service.DurationMinutes + (made % 2 == 0 ? 30 : 60));
                }
            }
        }
    }

    /// <summary>Tek randevu: müşteri (kaporalı → riskli; paket planı → paket sahibi), sonuç, yazım ve tamamlanınca tahsilat.</summary>
    private async Task WriteAppointmentAsync(
        AppointmentRun run, SeededStaff member, SeededService service, Instant start, int customer, DemoAppointmentOutcome? forced, Instant now, CancellationToken ct)
    {
        var end = start + Duration.FromMinutes(service.DurationMinutes);
        var deposit = forced is not null and not DemoAppointmentOutcome.PendingApproval;
        PackagePlan? plan = null;
        if (deposit)
        {
            customer = RiskyCustomer(run.NoShows, run.DepositsMade);
        }
        else if (forced is null && end <= now && customer >= 2)
        {
            // Gelmeme sayacı tutulan ilk iki müşteriye dokunulmaz (Decide).
            plan = run.Packages.FirstOrDefault(p => p.Remaining > 0 && p.ServiceId == service.Id);
            customer = plan?.Customer ?? customer;
        }

        var outcome = forced ?? Decide(start, end, now, customer, run.Ordinal, run.NoShows);
        var packageItem = outcome == DemoAppointmentOutcome.Completed && plan is not null ? plan.ItemId : (Guid?)null;
        var card = run.Cards[customer];
        var result = await scheduling.AddAsync(
            new DemoAppointment(
                run.BrandId, member.BranchId, card.CustomerId, card.BrandCustomerId, start, service.Id, member.Id, outcome, run.NoShows[customer], packageItem),
            now,
            ct);
        if (deposit)
        {
            run.DepositsMade++;
        }

        if (outcome == DemoAppointmentOutcome.Completed)
        {
            if (packageItem is not null)
            {
                plan!.Remaining--;
            }

            await RecordCompletionAsync(run, member, service, customer, result, ct);
        }

        run.Outcomes[outcome]++;
        run.Ordinal++;
    }

    /// <summary>Tamamlanan randevunun tahsilatı (salonda ödenecek tutarın tamamı, sıradaki yöntemle), seans kullanımı ve hizmet primi.</summary>
    private async Task RecordCompletionAsync(
        AppointmentRun run, SeededStaff member, SeededService service, int customer, DemoAppointmentResult result, CancellationToken ct)
    {
        var card = run.Cards[customer];
        var method = Enum.Parse<CollectedPaymentMethod>(DemoDataSet.PaymentMethods[run.Completed.Count % DemoDataSet.PaymentMethods.Count]);
        List<CompletionPaymentLine> lines = result.CollectedMinor > 0 ? [new(method, new Money(result.CollectedMinor, result.Currency))] : [];
        var reservation = result.CustomerPackageItemId is { } packageItem
            ? new PackageSessionReservation(
                run.BrandId, member.BranchId, card.BrandCustomerId, result.Id, result.LocalDate, [new PackageSessionClaim(result.ItemId, service.Id, packageItem)])
            : null;
        await sales.RecordCompletionAsync(
            new AppointmentCompletionRequest(
                run.BrandId, member.BranchId, card.BrandCustomerId, result.Id, result.Currency, lines,
                [new CompletedItem(result.ItemId, service.Id, member.Id, result.ItemAmountMinor, result.CustomerPackageItemId)],
                result.RetainedDepositMinor, result.LocalDate, CollectedByUserId: null),
            reservation,
            result.EndUtc,
            ct);
        run.Completed.Add(new CompletedVisit(result, member, service, customer, lines.Count > 0 ? method : null));
    }

    /// <summary>
    /// Paket tanımları ve satılmış müşteri paketleri (<see cref="DemoDataSet.CustomerPackages"/>); satış satan personelin şubesinde,
    /// satış gününün 15:00'inde. Dönüş: paketten düşülecek seans planları.
    /// </summary>
    private async Task<List<PackagePlan>> AddPackagesAsync(
        Guid brandId, List<SeededStaff> staff, List<SeededService> services, List<EnsuredBrandCustomer> cards, DateTimeZone zone, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var definitions = new Dictionary<string, (Guid Id, Guid ServiceId)>(StringComparer.Ordinal);
        foreach (var seed in DemoDataSet.Packages)
        {
            var serviceId = services.Single(x => string.Equals(x.Name, seed.Service, StringComparison.Ordinal)).Id;
            var price = new Money(seed.PriceMinor, PlatformDefaults.DefaultCurrency);
            var id = await sales.AddPackageAsync(
                brandId, new DemoPackage(seed.Name, serviceId, seed.SessionCount, price, seed.ValidityDays, seed.SaleCommissionPercent), now, ct);
            definitions[seed.Name] = (id, serviceId);
        }

        var plans = new List<PackagePlan>();
        foreach (var seed in DemoDataSet.CustomerPackages)
        {
            var (packageId, serviceId) = definitions[seed.Package];
            var seller = staff.Single(x => string.Equals(x.Name, seed.Seller, StringComparison.Ordinal));
            var saleDate = now.InZone(zone).Date.PlusDays(-seed.SoldDaysAgo);
            var itemId = await sales.SellPackageAsync(
                new DemoPackageSale(
                    brandId, packageId, seller.BranchId, cards[seed.Customer].BrandCustomerId, seller.Id, Enum.Parse<CollectedPaymentMethod>(seed.Method),
                    saleDate.At(DemoDataSet.PackageSoldAt).InZoneLeniently(zone).ToInstant(), saleDate),
                ct);
            plans.Add(new PackagePlan(seed.Customer, serviceId, itemId) { Remaining = seed.Redeem });
        }

        return plans;
    }

    /// <summary>Kadıköy'de bugünden önceki son nakit tahsilatlı gün kapatılır; sayılan beklenenden 50 ₺ az (KK7 örneği).</summary>
    private async Task<DemoSeedReport.CashDay?> CloseCashDayAsync(AppointmentRun run, DateTimeZone zone, CancellationToken ct)
    {
        var kadikoy = run.Staff[0].BranchId;
        var today = clock.GetCurrentInstant().InZone(zone).Date;
        var days = run.Completed
            .Where(v => v.Method == CollectedPaymentMethod.Cash && v.Staff.BranchId == kadikoy && v.Result.LocalDate < today)
            .Select(v => v.Result.LocalDate)
            .ToList();
        if (days.Count == 0)
        {
            return null;
        }

        var day = days.Max();
        var expected = await sales.CloseCashDayAsync(
            run.BrandId, kadikoy, day, zone, DemoDataSet.CashDayDifferenceMinor, DemoDataSet.CashDayNote,
            day.At(DemoDataSet.CashDayClosedAt).InZoneLeniently(zone).ToInstant(), ct);
        return new DemoSeedReport.CashDay(day, expected, expected + DemoDataSet.CashDayDifferenceMinor);
    }

    /// <summary>En yeni tamamlanan randevulardan (farklı müşteriler) değerlendirmeler; yazılma anı bitiş + 2 sa, şimdiden önce.</summary>
    private async Task<int> AddReviewsAsync(AppointmentRun run, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var visits = run.Completed
            .Where(v => v.Result.EndUtc + DemoDataSet.ReviewDelay < now)
            .OrderByDescending(v => v.Result.EndUtc)
            .DistinctBy(v => v.Customer)
            .Take(DemoDataSet.Reviews.Count)
            .ToList();
        for (var i = 0; i < visits.Count; i++)
        {
            var (visit, seed) = (visits[i], DemoDataSet.Reviews[i]);
            var created = visit.Result.EndUtc + DemoDataSet.ReviewDelay;
            var replied = seed.Reply is null ? (Instant?)null : Instant.Min(created + DemoDataSet.ReplyDelay, now);
            await reviews.AddAsync(
                new DemoReview(
                    run.BrandId, visit.Staff.BranchId, visit.Result.Id, run.Cards[visit.Customer].BrandCustomerId, visit.Staff.Id, [visit.Service.Name],
                    seed.Rating, seed.Comment, created, seed.Reply, seed.Reply is null ? null : DemoDataSet.OwnerName, replied),
                ct);
        }

        return visits.Count;
    }

    /// <summary>Kadıköy bekleme listesi (<see cref="DemoDataSet.Waitlist"/>); tarihler yarından başlayarak.</summary>
    private async Task AddWaitlistAsync(AppointmentRun run, DateTimeZone zone, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var today = now.InZone(zone).Date;
        foreach (var seed in DemoDataSet.Waitlist)
        {
            var card = run.Cards[seed.Customer];
            await scheduling.AddWaitlistEntryAsync(
                new DemoWaitlistEntry(
                    run.BrandId, run.Staff[0].BranchId, card.CustomerId, card.BrandCustomerId, [run.ServiceId(seed.Service)],
                    seed.Staff is null ? null : run.StaffId(seed.Staff), seed.AcceptOtherStaff,
                    [.. Enumerable.Range(1, seed.DatesAhead).Select(today.PlusDays)], seed.TimeWindows, Note: null, seed.Online, now - seed.JoinedAgo),
                ct);
        }
    }

    /// <summary>Bugün Kadıköy kapıda sırası (<see cref="DemoDataSet.WalkIns"/>); numaralar 1'den.</summary>
    private async Task AddWalkInsAsync(AppointmentRun run, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        foreach (var seed in DemoDataSet.WalkIns)
        {
            var service = run.Services.Single(x => string.Equals(x.Name, seed.Service, StringComparison.Ordinal));
            await scheduling.AddWalkInTicketAsync(
                new DemoWalkInTicket(
                    run.BrandId, run.Staff[0].BranchId, seed.Name ?? DemoDataSet.Customers[seed.Customer!.Value],
                    seed.Customer is { } customer ? run.Cards[customer].BrandCustomerId : null, service.Id, service.Name, service.DurationMinutes,
                    seed.Staff is null ? null : run.StaffId(seed.Staff), now - seed.CreatedAgo),
                ct);
        }
    }

    /// <summary>Randevu yazımının durumu: sayaçlar, paket planları ve tamamlanan ziyaretler.</summary>
    private sealed class AppointmentRun(
        Guid brandId, List<SeededStaff> staff, List<SeededService> services, List<EnsuredBrandCustomer> cards, List<PackagePlan> packages)
    {
        public Guid BrandId { get; } = brandId;

        public List<SeededStaff> Staff { get; } = staff;

        public List<SeededService> Services { get; } = services;

        public List<EnsuredBrandCustomer> Cards { get; } = cards;

        public List<PackagePlan> Packages { get; } = packages;

        public int[] NoShows { get; } = new int[cards.Count];

        public Dictionary<DemoAppointmentOutcome, int> Outcomes { get; } = Enum.GetValues<DemoAppointmentOutcome>().ToDictionary(o => o, _ => 0);

        public List<CompletedVisit> Completed { get; } = [];

        public int Ordinal { get; set; }

        public int DepositsMade { get; set; }

        public Guid ServiceId(string name) => Services.Single(x => string.Equals(x.Name, name, StringComparison.Ordinal)).Id;

        public Guid StaffId(string name) => Staff.Single(x => string.Equals(x.Name, name, StringComparison.Ordinal)).Id;
    }

    /// <summary>Müşteri paketinden geçmiş randevularda düşülecek seanslar.</summary>
    private sealed record PackagePlan(int Customer, Guid ServiceId, Guid ItemId)
    {
        public int Remaining { get; set; }
    }

    private sealed record CompletedVisit(DemoAppointmentResult Result, SeededStaff Staff, SeededService Service, int Customer, CollectedPaymentMethod? Method);

    private sealed record SeededStaff(string Name, Guid Id, Guid BranchId, bool IsBookableOnline, IReadOnlyList<DemoHours> Hours);

    private sealed record SeededService(Guid Id, string Name, int DurationMinutes, bool RequiresApproval, bool HasDeposit, IReadOnlyList<Guid> StaffIds);
}
