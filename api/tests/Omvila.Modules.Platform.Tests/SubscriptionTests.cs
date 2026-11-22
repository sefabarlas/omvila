using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Tests;

// Kural: docs/alan/durum-makinesi.md § 8 (Abonelik) — tablodaki her geçiş; T-118 (dönem, vade, gecikme günü).
public sealed class SubscriptionTests
{
    private const int TrialDays = 14;
    private const int Grace = 7;
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly LocalDate SignupDay = new(2026, 11, 25);
    private static readonly LocalDate TrialEnd = SignupDay.PlusDays(TrialDays);
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);

    [Fact]
    public void StartTrial_SetsTrialingPeriodAndFixedCodes()
    {
        var subscription = Trial();

        subscription.Status.ShouldBe(SubscriptionStatus.Trialing);
        subscription.TrialEndsOn.ShouldBe(new LocalDate(2026, 12, 9));
        subscription.CurrentPeriodStart.ShouldBe(SignupDay);
        subscription.CurrentPeriodEnd.ShouldBe(TrialEnd);
        subscription.TransferCode.ShouldBe("ABN-KE-1125");
        subscription.CreditTransferCode.ShouldBe("KNT-KE-1125");
        subscription.PaymentMethod.ShouldBeNull();
    }

    [Fact]
    public void MarkPastDue_TrialOver_StartsFirstPaidPeriodAtTrialEnd()
    {
        var subscription = Trial();

        subscription.MarkPastDue(TrialEnd).IsSuccess.ShouldBeTrue();

        subscription.Status.ShouldBe(SubscriptionStatus.PastDue);
        subscription.PastDueSince.ShouldBe(TrialEnd);
        subscription.CurrentPeriodStart.ShouldBe(TrialEnd);
        subscription.CurrentPeriodEnd.ShouldBe(new LocalDate(2027, 1, 8));
        subscription.ReadOnlyOn(Grace).ShouldBe(TrialEnd.PlusDays(Grace));
    }

    [Fact]
    public void MarkPastDue_LateJobRun_CountsGraceFromDueDate()
    {
        var subscription = Trial();

        subscription.MarkPastDue(TrialEnd.PlusDays(3)).IsSuccess.ShouldBeTrue();

        subscription.PastDueSince.ShouldBe(TrialEnd, "gecikme vadeden sayılır");
    }

    [Fact]
    public void MarkPastDue_TrialNotOver_ReturnsInvalidStateTransition()
    {
        var subscription = Trial();

        subscription.MarkPastDue(TrialEnd.PlusDays(-1)).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        subscription.Status.ShouldBe(SubscriptionStatus.Trialing);
    }

    [Fact]
    public void MarkPastDue_ActivePeriodOver_StartsNextPeriod()
    {
        var subscription = Active(TrialEnd);
        var nextStart = new LocalDate(2027, 1, 9);

        subscription.MarkPastDue(nextStart.PlusDays(-1)).IsSuccess.ShouldBeFalse("dönem son günü hâlâ ödenmiş");
        subscription.MarkPastDue(nextStart).IsSuccess.ShouldBeTrue();

        subscription.Status.ShouldBe(SubscriptionStatus.PastDue);
        subscription.CurrentPeriodStart.ShouldBe(nextStart);
        subscription.PastDueSince.ShouldBe(nextStart);
    }

    [Fact]
    public void MarkReadOnly_AfterGraceDays_FromPastDue()
    {
        var subscription = Trial();
        subscription.MarkPastDue(TrialEnd);

        subscription.MarkReadOnly(TrialEnd.PlusDays(Grace - 1), Grace).IsSuccess.ShouldBeFalse();
        subscription.MarkReadOnly(TrialEnd.PlusDays(Grace), Grace).IsSuccess.ShouldBeTrue();

        subscription.Status.ShouldBe(SubscriptionStatus.ReadOnly);
        subscription.ReadOnlySince.ShouldBe(TrialEnd.PlusDays(Grace));
        subscription.ReadOnlyOn(Grace).ShouldBeNull();
    }

    [Fact]
    public void MarkReadOnly_NotPastDue_ReturnsInvalidStateTransition() =>
        Trial().MarkReadOnly(TrialEnd.PlusDays(30), Grace).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

    [Fact]
    public void Activate_FromTrialing_PaidPeriodStartsGivenDay()
    {
        var subscription = Trial();

        subscription.Activate(TrialEnd, SubscriptionPaymentMethod.BankTransfer).IsSuccess.ShouldBeTrue();

        subscription.Status.ShouldBe(SubscriptionStatus.Active);
        subscription.CurrentPeriodStart.ShouldBe(TrialEnd);
        subscription.CurrentPeriodEnd.ShouldBe(new LocalDate(2027, 1, 8));
        subscription.TrialEndsOn.ShouldBeNull();
        subscription.PaymentMethod.ShouldBe(SubscriptionPaymentMethod.BankTransfer);
    }

    [Fact]
    public void Activate_FromPastDue_ClearsPastDue()
    {
        var subscription = Trial();
        subscription.MarkPastDue(TrialEnd);

        subscription.Activate(subscription.CurrentPeriodStart, SubscriptionPaymentMethod.BankTransfer).IsSuccess.ShouldBeTrue();

        subscription.Status.ShouldBe(SubscriptionStatus.Active);
        subscription.PastDueSince.ShouldBeNull();
    }

    [Fact]
    public void Activate_FromReadOnly_ReturnsInvalidStateTransition_ReactivateSucceeds()
    {
        var subscription = ReadOnly();

        subscription.Activate(TrialEnd, SubscriptionPaymentMethod.BankTransfer).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        subscription.Reactivate(TrialEnd, SubscriptionPaymentMethod.BankTransfer).IsSuccess.ShouldBeTrue();

        subscription.Status.ShouldBe(SubscriptionStatus.Active);
        subscription.ReadOnlySince.ShouldBeNull();
        subscription.PastDueSince.ShouldBeNull();
    }

    [Fact]
    public void Reactivate_NotReadOnly_ReturnsInvalidStateTransition() =>
        Trial().Reactivate(TrialEnd, SubscriptionPaymentMethod.BankTransfer).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

    [Fact]
    public void RecordPayment_PicksTransitionByStatus()
    {
        var active = Active(TrialEnd);
        active.RecordPayment(TrialEnd, SubscriptionPaymentMethod.BankTransfer).IsSuccess.ShouldBeTrue();
        active.CurrentPeriodStart.ShouldBe(TrialEnd, "aktifken dönem değişmez");

        var readOnly = ReadOnly();
        readOnly.RecordPayment(TrialEnd, SubscriptionPaymentMethod.BankTransfer).IsSuccess.ShouldBeTrue();
        readOnly.Status.ShouldBe(SubscriptionStatus.Active);

        var cancelled = Cancelled();
        cancelled.RecordPayment(TrialEnd, SubscriptionPaymentMethod.BankTransfer).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Fact]
    public void Cancel_FromReadOnlyAndActive_OthersInvalid()
    {
        var readOnly = ReadOnly();
        readOnly.Cancel(TrialEnd.PlusDays(40), Now).IsSuccess.ShouldBeTrue();
        readOnly.Status.ShouldBe(SubscriptionStatus.Cancelled);
        readOnly.CancelledOn.ShouldBe(TrialEnd.PlusDays(40));
        readOnly.CancelledUtc.ShouldBe(Now);

        Active(TrialEnd).Cancel(TrialEnd, Now).IsSuccess.ShouldBeTrue();
        Trial().Cancel(TrialEnd, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        var pastDue = Trial();
        pastDue.MarkPastDue(TrialEnd);
        pastDue.Cancel(TrialEnd, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        Cancelled().Cancel(TrialEnd, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Fact]
    public void IsReadOnlyExpired_AfterConfiguredDays()
    {
        var subscription = ReadOnly();
        var since = subscription.ReadOnlySince!.Value;

        subscription.IsReadOnlyExpired(since.PlusDays(29), 30).ShouldBeFalse();
        subscription.IsReadOnlyExpired(since.PlusDays(30), 30).ShouldBeTrue();
    }

    [Fact]
    public void IsDueForArchive_OnceAfterCancelledDays()
    {
        var subscription = Cancelled();
        var on = subscription.CancelledOn!.Value;

        subscription.IsDueForArchive(on.PlusDays(29), 30).ShouldBeFalse();
        subscription.IsDueForArchive(on.PlusDays(30), 30).ShouldBeTrue();
        subscription.MarkBrandArchived(Now);
        subscription.IsDueForArchive(on.PlusDays(31), 30).ShouldBeFalse();
    }

    [Fact]
    public void MarkBrandArchived_NotCancelled_Throws() =>
        Should.Throw<DomainException>(() => Trial().MarkBrandArchived(Now));

    [Fact]
    public void TrialEndingNotice_WindowAndOnce()
    {
        var subscription = Trial();

        subscription.IsDueForTrialEndingNotice(TrialEnd.PlusDays(-4), 3).ShouldBeFalse();
        subscription.IsDueForTrialEndingNotice(TrialEnd.PlusDays(-3), 3).ShouldBeTrue();
        subscription.IsDueForTrialEndingNotice(TrialEnd, 3).ShouldBeFalse("deneme bitti");
        subscription.MarkTrialEndingNotified(Now);
        subscription.IsDueForTrialEndingNotice(TrialEnd.PlusDays(-2), 3).ShouldBeFalse();
    }

    [Fact]
    public void BillingCalendar_YearlyPeriodEndsDayBeforeAnniversary() =>
        BillingCalendar.PeriodEnd(new LocalDate(2026, 12, 9), BillingCycle.Yearly).ShouldBe(new LocalDate(2027, 12, 8));

    internal static Subscription Trial() =>
        Subscription.StartTrial(
            Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1"), Brand, Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1"), SignupDay,
            TrialDays, new TransferCodes("ABN-KE-1125", "KNT-KE-1125"), Now);

    private static Subscription Active(LocalDate periodStart)
    {
        var subscription = Trial();
        subscription.Activate(periodStart, SubscriptionPaymentMethod.BankTransfer);
        return subscription;
    }

    private static Subscription ReadOnly()
    {
        var subscription = Trial();
        subscription.MarkPastDue(TrialEnd);
        subscription.MarkReadOnly(TrialEnd.PlusDays(Grace), Grace);
        return subscription;
    }

    private static Subscription Cancelled()
    {
        var subscription = ReadOnly();
        subscription.Cancel(TrialEnd.PlusDays(Grace + 30), Now);
        return subscription;
    }
}
