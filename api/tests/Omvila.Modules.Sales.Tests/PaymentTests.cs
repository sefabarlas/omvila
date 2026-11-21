using NodaTime;
using Omvila.Modules.Sales.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Tests;

// Kural: durum-makinesi.md A3 (Payment(Method=Transfer, IsDeposit) + tahsis), alan-modeli.md § Payment, PaymentAllocation.
public sealed class PaymentTests
{
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly Guid Branch = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2");
    private static readonly Guid Customer = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2");
    private static readonly Guid Appointment = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1");
    private static readonly Guid User = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 10);

    [Fact]
    public void RecordDeposit_IsRecordedTransfer_WholeAmountAllocatedToAppointment()
    {
        var payment = Payment.RecordDeposit(new Ids(), Brand, Branch, Customer, Appointment, Money.Try(84000), " OMV-7K2X ", User, Now);

        payment.Method.ShouldBe(PaymentMethod.Transfer);
        payment.IsDeposit.ShouldBeTrue();
        payment.Status.ShouldBe(PaymentStatus.Recorded);
        payment.AmountMinor.ShouldBe(84000);
        payment.CurrencyCode.ShouldBe("TRY");
        payment.RefundedAmountMinor.ShouldBe(0);
        payment.Reference.ShouldBe("OMV-7K2X");
        payment.PaidUtc.ShouldBe(Now);
        payment.CollectedByUserId.ShouldBe(User);
        payment.BrandId.ShouldBe(Brand);
        payment.BranchId.ShouldBe(Branch);
        payment.BrandCustomerId.ShouldBe(Customer);
        var allocation = payment.Allocations.ShouldHaveSingleItem();
        allocation.PaymentId.ShouldBe(payment.Id);
        allocation.BrandId.ShouldBe(Brand);
        allocation.TargetType.ShouldBe(AllocationTargetType.Appointment);
        allocation.TargetId.ShouldBe(Appointment);
        allocation.AmountMinor.ShouldBe(84000);
    }

    [Fact]
    public void RecordDeposit_ZeroAmount_IsProgrammerError() =>
        Should.Throw<DomainException>(() => Payment.RecordDeposit(new Ids(), Brand, Branch, Customer, Appointment, Money.Try(0), null, User, Now));

    // Kural: durum-makinesi.md § 6 (Recorded → PartiallyRefunded / Refunded); V7 ("Salon iade etti", kısmi olabilir, tek kez)
    [Theory]
    [InlineData(84000, nameof(PaymentStatus.Refunded))]
    [InlineData(30000, nameof(PaymentStatus.PartiallyRefunded))]
    public void MarkRefunded_SetsRefundedAmount_FullOrPartial(long amount, string expected)
    {
        var payment = Payment.RecordDeposit(new Ids(), Brand, Branch, Customer, Appointment, Money.Try(84000), null, User, Now);

        payment.MarkRefunded(Money.Try(amount));

        payment.Status.ToString().ShouldBe(expected);
        payment.RefundedAmountMinor.ShouldBe(amount);
        payment.AmountMinor.ShouldBe(84000, "tahsilat tutarı değişmez");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(84001)]
    public void MarkRefunded_OutOfRange_IsProgrammerError(long amount)
    {
        var payment = Payment.RecordDeposit(new Ids(), Brand, Branch, Customer, Appointment, Money.Try(84000), null, User, Now);

        Should.Throw<DomainException>(() => payment.MarkRefunded(Money.Try(amount)));
        payment.Status.ShouldBe(PaymentStatus.Recorded);
    }

    [Fact]
    public void MarkRefunded_Twice_IsProgrammerError()
    {
        var payment = Payment.RecordDeposit(new Ids(), Brand, Branch, Customer, Appointment, Money.Try(84000), null, User, Now);
        payment.MarkRefunded(Money.Try(10000));

        Should.Throw<DomainException>(() => payment.MarkRefunded(Money.Try(10000)));
        payment.RefundedAmountMinor.ShouldBe(10000);
    }

    // Kural: durum-makinesi.md A11 ("Tamamla ve ödeme al" satırları; ikinci ödeme ayrı kayıt).
    [Theory]
    [InlineData("Cash")]
    [InlineData("Card")]
    [InlineData("Transfer")]
    public void RecordCollected_IsRecordedNonDeposit_WholeAmountAllocatedToAppointment(string methodName)
    {
        var method = Enum.Parse<PaymentMethod>(methodName);
        var payment = Payment.RecordCollected(new Ids(), Brand, Branch, Customer, Appointment, method, Money.Try(124000), User, Now);

        payment.Method.ShouldBe(method);
        payment.IsDeposit.ShouldBeFalse();
        payment.Status.ShouldBe(PaymentStatus.Recorded);
        payment.AmountMinor.ShouldBe(124000);
        payment.CurrencyCode.ShouldBe("TRY");
        payment.RefundedAmountMinor.ShouldBe(0);
        payment.Reference.ShouldBeNull();
        payment.PaidUtc.ShouldBe(Now);
        payment.CollectedByUserId.ShouldBe(User);
        payment.BrandCustomerId.ShouldBe(Customer);
        var allocation = payment.Allocations.ShouldHaveSingleItem();
        allocation.TargetType.ShouldBe(AllocationTargetType.Appointment);
        allocation.TargetId.ShouldBe(Appointment);
        allocation.AmountMinor.ShouldBe(124000);
    }

    [Fact]
    public void RecordCollected_PackageRedemptionOrZero_IsProgrammerError()
    {
        Should.Throw<DomainException>(() =>
            Payment.RecordCollected(new Ids(), Brand, Branch, Customer, Appointment, PaymentMethod.PackageRedemption, Money.Try(100), User, Now));
        Should.Throw<DomainException>(() =>
            Payment.RecordCollected(new Ids(), Brand, Branch, Customer, Appointment, PaymentMethod.Cash, Money.Try(0), User, Now));
        Should.Throw<ArgumentNullException>(() =>
            Payment.RecordCollected(null!, Brand, Branch, Customer, Appointment, PaymentMethod.Cash, Money.Try(100), User, Now));
    }

    private sealed class Ids : IIdGenerator
    {
        private int _next;

        public Guid New() => Guid.Parse($"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _next):D12}");
    }
}
