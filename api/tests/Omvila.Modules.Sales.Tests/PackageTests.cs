using NodaTime;
using Omvila.Modules.Sales.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Tests;

// Kural: alan-modeli.md § Package, PackageItem, CustomerPackage, CustomerPackageItem, PackageSessionUsage, CommissionEntry (satış
// primi, ters kayıt); durum-makinesi.md § 5 (Active → Expired / Refunded), § 7 (Reversed); dilim-05 kabul kriteri 6 (10 seanslık
// Fön paketi, %10 satış primi); docs/acik-sorular.md W1, W2, W9 (S-10).
public sealed class PackageTests
{
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly Guid Kadikoy = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2");
    private static readonly Guid Nisantasi = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b3");
    private static readonly Guid Customer = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2");
    private static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-0000000000e1");
    private static readonly Guid User = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
    private static readonly Guid Fon = Guid.Parse("0193a8f0-0000-7000-8000-0000000000f1");
    private static readonly Guid Kesim = Guid.Parse("0193a8f0-0000-7000-8000-0000000000f2");
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 22, 9, 15);
    private static readonly LocalDate Today = new(2026, 11, 22);

    [Fact]
    public void Create_CopiesDraft_TotalSessionsAndOrderedItems()
    {
        var package = Package.Create(new Ids(), Brand, Draft(items: [new(Fon, 10), new(Kesim, 2)]), Now);

        package.Name.ShouldBe("Fön paketi");
        package.Price.ShouldBe(Money.Try(150000));
        package.ValidityDays.ShouldBe(90);
        package.SaleCommissionPercentOverride.ShouldBe(10m);
        package.IsAvailableAllBranches.ShouldBeTrue();
        package.IsActive.ShouldBeTrue();
        package.CreatedUtc.ShouldBe(Now);
        package.TotalSessions.ShouldBe(12);
        package.Items.Select(i => (i.SeqNo, i.ServiceId, i.SessionCount, i.PackageId, i.BrandId))
            .ShouldBe([(1, Fon, 10, package.Id, Brand), (2, Kesim, 2, package.Id, Brand)]);
    }

    [Fact]
    public void Update_ReplacesItems_ReturnsRemovedRows_AndStampsTime()
    {
        var package = Package.Create(new Ids(), Brand, Draft(), Now);
        var old = package.Items.Single();

        var removed = package.Update(new Ids(100), Draft(name: "  Fön · 5 seans ", items: [new(Fon, 5)], active: false), Now + Duration.FromDays(1));

        removed.ShouldBe([old]);
        package.Name.ShouldBe("Fön · 5 seans");
        package.IsActive.ShouldBeFalse();
        package.TotalSessions.ShouldBe(5);
        package.UpdatedUtc.ShouldBe(Now + Duration.FromDays(1));
        package.CanSell().Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    private static readonly PackageDraft[] InvalidDrafts =
    [
        Draft(name: "F"),
        Draft(price: 0),
        Draft(items: []),
        Draft(items: [new(Fon, 0)]),
        Draft(items: [new(Fon, 101)]),
        Draft(items: [new(Fon, 1), new(Fon, 2)]),
        Draft(items: [.. Enumerable.Range(1, 11).Select(i => new PackageDraftItem(Guid.Parse($"0193a8f0-0000-7000-8000-{i:D12}"), 1))]),
        Draft(validity: 0),
        Draft(validity: 1096),
        Draft(percent: 101m),
        Draft(percent: -1m),
    ];

    [Fact]
    public void Create_InvalidDraft_Throws()
    {
        foreach (var draft in InvalidDrafts)
        {
            Should.Throw<DomainException>(() => Package.Create(new Ids(), Brand, draft, Now));
        }
    }

    [Fact]
    public void Sell_Criterion6_CopiesNamePriceItems_ExpiresOnSaleDayPlusValidity()
    {
        var package = Package.Create(new Ids(), Brand, Draft(items: [new(Fon, 10)]), Now);

        var sold = Sell(package).Value;

        sold.Status.ShouldBe(CustomerPackageStatus.Active);
        sold.PackageId.ShouldBe(package.Id);
        sold.PackageNameSnapshot.ShouldBe("Fön paketi");
        sold.PricePaid.ShouldBe(Money.Try(150000));
        sold.BranchId.ShouldBe(Kadikoy);
        sold.BrandCustomerId.ShouldBe(Customer);
        sold.SoldByStaffId.ShouldBe(Elif);
        sold.SoldByUserId.ShouldBe(User);
        sold.PurchasedUtc.ShouldBe(Now);
        sold.ExpiresOn.ShouldBe(new LocalDate(2027, 2, 20));
        sold.IsAvailableAllBranches.ShouldBeTrue();
        sold.TotalSessions.ShouldBe(10);
        sold.Items.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            i => i.ServiceId.ShouldBe(Fon), i => i.TotalSessions.ShouldBe(10), i => i.CustomerPackageId.ShouldBe(sold.Id));

        // Tanım sonradan değişse de satılmış paket değişmez.
        package.Update(new Ids(500), Draft(name: "Yeni ad", price: 99000, items: [new(Fon, 3)]), Now);
        sold.PackageNameSnapshot.ShouldBe("Fön paketi");
        sold.PricePaidMinor.ShouldBe(150000);
        sold.TotalSessions.ShouldBe(10);
    }

    [Fact]
    public void Sell_InactivePackage_IsInvalidStateTransition()
    {
        var package = Package.Create(new Ids(), Brand, Draft(active: false), Now);

        Sell(package).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Fact]
    public void SessionShare_W2_PricePerSessionAcrossItems_RoundedHalfUp()
    {
        Sell(Package.Create(new Ids(), Brand, Draft(items: [new(Fon, 10)]), Now)).Value.SessionShareMinor.ShouldBe(15000);
        // 1.000 ₺ / 3 seans = 333,333 → 33.333 kuruş; 1.000,05 / 2 = 500,025 → 50.003
        Sell(Package.Create(new Ids(), Brand, Draft(price: 100000, items: [new(Fon, 2), new(Kesim, 1)]), Now)).Value.SessionShareMinor.ShouldBe(33333);
        Sell(Package.Create(new Ids(), Brand, Draft(price: 100005, items: [new(Fon, 2)]), Now)).Value.SessionShareMinor.ShouldBe(50003);
    }

    [Fact]
    public void IsUsable_ActiveWithinExpiry_AndBranchRule()
    {
        var everywhere = Sell(Package.Create(new Ids(), Brand, Draft(), Now)).Value;
        var onlyKadikoy = Sell(Package.Create(new Ids(), Brand, Draft(allBranches: false), Now)).Value;

        everywhere.IsUsable(everywhere.ExpiresOn, Nisantasi).ShouldBeTrue("son gün dahil");
        everywhere.IsUsable(everywhere.ExpiresOn.PlusDays(1), Kadikoy).ShouldBeFalse("süresi geçmiş");
        onlyKadikoy.IsUsable(Today, Kadikoy).ShouldBeTrue();
        onlyKadikoy.IsUsable(Today, Nisantasi).ShouldBeFalse("yalnız satıldığı şubede");
    }

    [Fact]
    public void Expire_OnlyActive_AfterLastDay()
    {
        var package = Sell(Package.Create(new Ids(), Brand, Draft(), Now)).Value;
        var later = Now + Duration.FromDays(91);

        package.Expire(package.ExpiresOn, later).ShouldBeFalse("son gün henüz geçerli");
        package.Status.ShouldBe(CustomerPackageStatus.Active);
        package.Expire(package.ExpiresOn.PlusDays(1), later).ShouldBeTrue();
        package.Status.ShouldBe(CustomerPackageStatus.Expired);
        package.ExpiredUtc.ShouldBe(later);
        package.Expire(package.ExpiresOn.PlusDays(2), later).ShouldBeFalse("zaten Expired");
        package.IsUsable(package.ExpiresOn, Kadikoy).ShouldBeFalse("Expired paket kullanılamaz");
    }

    [Fact]
    public void Refund_W9_ManualAmount_ActiveOrExpired_OnlyOnce()
    {
        var package = Sell(Package.Create(new Ids(), Brand, Draft(), Now)).Value;

        package.Refund(Money.Try(0), null, User, Now).Error!.FieldErrors["amount"].ShouldBe(["out_of_range"]);
        package.Refund(Money.Try(150001), null, User, Now).Error!.FieldErrors["amount"].ShouldBe(["out_of_range"]);
        package.Refund(Money.Try(1000), new string('a', 301), User, Now).Error!.FieldErrors["note"].ShouldBe(["too_long"]);
        package.Status.ShouldBe(CustomerPackageStatus.Active);

        package.Refund(Money.Try(90000), "  3 seans kullanıldı ", User, Now).IsSuccess.ShouldBeTrue();

        package.Status.ShouldBe(CustomerPackageStatus.Refunded);
        package.RefundedAmountMinor.ShouldBe(90000);
        package.RefundedUtc.ShouldBe(Now);
        package.RefundNote.ShouldBe("3 seans kullanıldı");
        package.RefundedByUserId.ShouldBe(User);
        package.IsUsable(Today, Kadikoy).ShouldBeFalse();
        package.Refund(Money.Try(1000), null, User, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Fact]
    public void Refund_ExpiredPackage_IsAllowed()
    {
        var package = Sell(Package.Create(new Ids(), Brand, Draft(), Now)).Value;
        package.Expire(package.ExpiresOn.PlusDays(1), Now);

        package.Refund(Money.Try(150000), null, User, Now).IsSuccess.ShouldBeTrue();
        package.Status.ShouldBe(CustomerPackageStatus.Refunded);
    }

    [Fact]
    public void Remaining_IsTotalMinusActiveUsages_NeverNegative()
    {
        var item = Sell(Package.Create(new Ids(), Brand, Draft(items: [new(Fon, 10)]), Now)).Value.Items.Single();

        item.Remaining(0).ShouldBe(10);
        item.Remaining(3).ShouldBe(7);
        item.Remaining(12).ShouldBe(0);
    }

    [Fact]
    public void Usage_RevertStampsOnce()
    {
        var appointment = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1");
        var appointmentItem = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d2");
        var packageItem = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d3");
        var usage = PackageSessionUsage.Use(new Ids(), Brand, packageItem, appointment, appointmentItem, Now);

        usage.IsActive.ShouldBeTrue();
        usage.UsedUtc.ShouldBe(Now);
        usage.AppointmentId.ShouldBe(appointment);
        usage.AppointmentItemId.ShouldBe(appointmentItem);
        usage.CustomerPackageItemId.ShouldBe(packageItem);
        usage.BrandId.ShouldBe(Brand);

        usage.Revert(Now + Duration.FromHours(1));
        usage.Revert(Now + Duration.FromHours(2));

        usage.IsActive.ShouldBeFalse();
        usage.RevertedUtc.ShouldBe(Now + Duration.FromHours(1));
        Should.Throw<ArgumentNullException>(() => PackageSessionUsage.Use(null!, Brand, packageItem, appointment, appointmentItem, Now));
    }

    [Fact]
    public void AccrueSale_Criterion6_TenPercentOfPackagePrice()
    {
        var customerPackage = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d4");

        var entry = CommissionEntry.AccrueSale(new Ids(), Brand, Kadikoy, Elif, customerPackage, Money.Try(150000), 10m, Today, Now);

        entry.Type.ShouldBe(CommissionType.Sale);
        entry.Status.ShouldBe(CommissionStatus.Accrued);
        entry.CustomerPackageId.ShouldBe(customerPackage);
        entry.AppointmentItemId.ShouldBeNull();
        entry.BasisMinor.ShouldBe(150000);
        entry.Percent.ShouldBe(10m);
        entry.AmountMinor.ShouldBe(15000);
        entry.PeriodYearMonth.ShouldBe(202611);
        entry.StaffId.ShouldBe(Elif);
        entry.BranchId.ShouldBe(Kadikoy);
    }

    [Fact]
    public void Reverse_PartialAndFull_NegativeReversedEntry_OriginalUnchanged()
    {
        var sale = CommissionEntry.AccrueSale(new Ids(), Brand, Kadikoy, Elif, Customer, Money.Try(150005), 10m, Today, Now);
        sale.AmountMinor.ShouldBe(15001);

        var partial = sale.Reverse(new Ids(10), 90000, new LocalDate(2026, 12, 3), Now);
        var full = sale.Reverse(new Ids(20), 150005, new LocalDate(2026, 12, 3), Now);

        partial.Status.ShouldBe(CommissionStatus.Reversed);
        partial.ReversesEntryId.ShouldBe(sale.Id);
        partial.BasisMinor.ShouldBe(-90000);
        partial.AmountMinor.ShouldBe(-9000);
        partial.PeriodYearMonth.ShouldBe(202612);
        partial.Type.ShouldBe(CommissionType.Sale);
        partial.CustomerPackageId.ShouldBe(sale.CustomerPackageId);
        full.AmountMinor.ShouldBe(-15001, "tam iadede asılın tam tersi");
        sale.Status.ShouldBe(CommissionStatus.Accrued);
        sale.AmountMinor.ShouldBe(15001);

        Should.Throw<DomainException>(() => sale.Reverse(new Ids(), 0, Today, Now));
        Should.Throw<DomainException>(() => sale.Reverse(new Ids(), 150006, Today, Now));
        Should.Throw<DomainException>(() => partial.Reverse(new Ids(), 1, Today, Now));
    }

    [Theory]
    [InlineData(10.0, 5.0, 10.0)]
    [InlineData(null, 5.0, 5.0)]
    [InlineData(0.0, 5.0, 0.0)]
    [InlineData(null, null, 0.0)]
    public void SalePercent_PackageOverrideBeatsStaffRate(double? package, double? staff, double expected) =>
        CommissionEntry.SalePercent((decimal?)package, (decimal?)staff).ShouldBe((decimal)expected);

    [Fact]
    public void RecordPackageSale_WholePriceAllocatedToCustomerPackage()
    {
        var customerPackage = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d4");

        var payment = Payment.RecordPackageSale(new Ids(), Brand, Kadikoy, Customer, customerPackage, PaymentMethod.Card, Money.Try(150000), User, Now);

        payment.Method.ShouldBe(PaymentMethod.Card);
        payment.AmountMinor.ShouldBe(150000);
        payment.IsDeposit.ShouldBeFalse();
        payment.Status.ShouldBe(PaymentStatus.Recorded);
        var allocation = payment.Allocations.ShouldHaveSingleItem();
        allocation.TargetType.ShouldBe(AllocationTargetType.CustomerPackage);
        allocation.TargetId.ShouldBe(customerPackage);
        allocation.AmountMinor.ShouldBe(150000);
        Should.Throw<DomainException>(() => Payment.RecordPackageSale(new Ids(), Brand, Kadikoy, Customer, customerPackage, PaymentMethod.PackageRedemption, Money.Try(1), User, Now));
    }

    [Fact]
    public void RecordPackageRedemption_W1_ZeroAmountAllocatedToAppointment()
    {
        var appointment = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1");

        var payment = Payment.RecordPackageRedemption(new Ids(), Brand, Kadikoy, Customer, appointment, Currency.TRY, User, Now);

        payment.Method.ShouldBe(PaymentMethod.PackageRedemption);
        payment.AmountMinor.ShouldBe(0);
        payment.CurrencyCode.ShouldBe("TRY");
        payment.Status.ShouldBe(PaymentStatus.Recorded);
        payment.CollectedByUserId.ShouldBe(User);
        payment.PaidUtc.ShouldBe(Now);
        var allocation = payment.Allocations.ShouldHaveSingleItem();
        allocation.TargetType.ShouldBe(AllocationTargetType.Appointment);
        allocation.TargetId.ShouldBe(appointment);
        allocation.AmountMinor.ShouldBe(0);
        Should.Throw<ArgumentNullException>(() => Payment.RecordPackageRedemption(null!, Brand, Kadikoy, Customer, appointment, Currency.TRY, User, Now));
    }

    private static Result<CustomerPackage> Sell(Package package) =>
        CustomerPackage.Sell(new Ids(1000), package, Kadikoy, Customer, Elif, Today, User, Now);

    private static PackageDraft Draft(
        string name = "Fön paketi",
        long price = 150000,
        PackageDraftItem[]? items = null,
        int validity = 90,
        decimal? percent = 10m,
        bool allBranches = true,
        bool active = true) =>
        new(name, Money.Try(price), items ?? [new(Fon, 10)], validity, percent, allBranches, active);

    private sealed class Ids(int start = 0) : IIdGenerator
    {
        private int _next = start;

        public Guid New() => Guid.Parse($"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _next):D12}");
    }
}
