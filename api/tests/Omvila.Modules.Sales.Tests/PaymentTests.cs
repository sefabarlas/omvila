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

    private sealed class Ids : IIdGenerator
    {
        private int _next;

        public Guid New() => Guid.Parse($"0193a8f0-0000-7000-8000-{Interlocked.Increment(ref _next):D12}");
    }
}
