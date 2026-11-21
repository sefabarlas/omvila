using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: durum-makinesi.md § 1 geçiş tablosu A2 (kapora istenmiş ve gelmemişse waiveDeposit zorunlu) ve A3 ("Kapora geldi":
// DepositReceivedUtc, Payment + tahsis); dilim-04 kabul kriterleri 5 ("Salonda ödenecek" düşer) ve 7 ("Kaporasız onayla" →
// kapora istenmiyor). Kapora 200 ₺, havale süresi 120 dk; toplam saç kesimi + fön.
public sealed class DepositTransitionTests
{
    private static readonly Instant Start = Offers.Tuesday1430;
    private static readonly Instant Created = Start - Duration.FromDays(1);
    private static readonly Duration Window = Duration.FromMinutes(120);
    private static readonly Guid UserId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");
    private static readonly Guid PaymentId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000e1");
    private const long Deposit = 20_000;

    private readonly SequentialIds _ids = new();

    // ───────── A3 ─────────

    [Fact]
    public void A3_MarkDepositReceived_Awaiting_IsConfirmed_DepositRecorded_AmountDueDrops()
    {
        var appointment = DepositPending();
        var now = Created + Duration.FromMinutes(40);

        appointment.MarkDepositReceived(PaymentId, now, UserId).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
        appointment.DisplayStatus.ShouldBe(DisplayStatus.Confirmed);
        appointment.ConfirmedUtc.ShouldBe(now);
        appointment.HoldExpiresUtc.ShouldBeNull();
        appointment.HoldReason.ShouldBeNull();
        appointment.DepositReceivedUtc.ShouldBe(now);
        appointment.DepositReceivedByUserId.ShouldBe(UserId);
        appointment.DepositPaymentId.ShouldBe(PaymentId);
        appointment.DepositStatus.ShouldBe(DepositStatus.Received);
        appointment.AmountDueMinor.ShouldBe(appointment.TotalAmountMinor - Deposit);
        appointment.UpdatedByUserId.ShouldBe(UserId);
    }

    [Fact]
    public void A3_MarkDepositReceived_ApprovalAndDeposit_ConfirmsWithoutSeparateApproval()
    {
        var appointment = DepositPending(approvalTimeout: Duration.FromMinutes(60));
        appointment.HoldReason.ShouldBe(HoldReason.ApprovalAndDeposit);

        appointment.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(5), UserId).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
    }

    [Fact]
    public void A3_CanMarkDepositReceived_MatchesTransitionGuard()
    {
        var appointment = DepositPending();
        appointment.CanMarkDepositReceived(Created).IsSuccess.ShouldBeTrue();
        appointment.CanMarkDepositReceived(Created + Window).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);
        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval, "denetim durumu değiştirmez");
    }

    [Fact]
    public void A3_MarkDepositReceived_AtOrAfterHold_IsHoldExpired_StateUnchanged()
    {
        var appointment = DepositPending();

        appointment.MarkDepositReceived(PaymentId, Created + Window, UserId).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);

        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.DepositReceivedUtc.ShouldBeNull();
        appointment.DepositPaymentId.ShouldBeNull();
    }

    [Fact]
    public void A3_MarkDepositReceived_NoDepositOrAlreadyReceivedOrWaivedOrNotPending_IsInvalidTransition()
    {
        var now = Created + Duration.FromMinutes(5);
        Create(approvalTimeout: Duration.FromMinutes(120), deposit: null).MarkDepositReceived(PaymentId, now, UserId)
            .Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var received = DepositPending();
        received.MarkDepositReceived(PaymentId, now, UserId).IsSuccess.ShouldBeTrue();
        received.MarkDepositReceived(PaymentId, now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var waived = DepositPending();
        waived.Approve(now, UserId, waiveDeposit: true).IsSuccess.ShouldBeTrue();
        waived.MarkDepositReceived(PaymentId, now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var cancelled = DepositPending();
        cancelled.Decline(now, UserId).IsSuccess.ShouldBeTrue();
        cancelled.MarkDepositReceived(PaymentId, now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── A2 + kapora ─────────

    [Fact]
    public void A2_Approve_DepositAwaited_WithoutWaive_IsDepositRequired_StateUnchanged()
    {
        var appointment = DepositPending();

        appointment.Approve(Created + Duration.FromMinutes(5), UserId).Error!.Code.ShouldBe(ErrorCodes.DepositRequired);

        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.DepositStatus.ShouldBe(DepositStatus.Awaiting);
    }

    [Fact]
    public void A2_Approve_DepositAwaited_Waive_IsConfirmed_DepositWaived_AmountDueIsTotal()
    {
        var appointment = DepositPending();
        var now = Created + Duration.FromMinutes(5);

        appointment.Approve(now, UserId, waiveDeposit: true).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
        appointment.ConfirmedUtc.ShouldBe(now);
        appointment.HoldExpiresUtc.ShouldBeNull();
        appointment.DepositWaivedUtc.ShouldBe(now);
        appointment.DepositWaivedByUserId.ShouldBe(UserId);
        appointment.DepositStatus.ShouldBe(DepositStatus.Waived);
        appointment.DepositReceivedUtc.ShouldBeNull();
        appointment.DepositPaymentId.ShouldBeNull();
        appointment.AmountDueMinor.ShouldBe(appointment.TotalAmountMinor);
        appointment.DepositRequiredMinor.ShouldBe(Deposit, "istenen tutar geçmiş için kalır");
    }

    [Fact]
    public void A2_Approve_WithoutDeposit_WaiveIsIgnored()
    {
        var appointment = Create(approvalTimeout: Duration.FromMinutes(120), deposit: null);

        appointment.Approve(Created + Duration.FromMinutes(5), UserId, waiveDeposit: true).IsSuccess.ShouldBeTrue();

        appointment.DepositWaivedUtc.ShouldBeNull();
        appointment.DepositStatus.ShouldBeNull();
    }

    [Fact]
    public void A2_Approve_DepositAwaited_HoldExpired_TakesPrecedenceOverDepositRequired() =>
        DepositPending().Approve(Created + Window, UserId).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);

    // ───────── Geçmiş ─────────

    [Fact]
    public void History_DepositReceived_WithoutSeparateApproved_Waived_AfterApproved()
    {
        var received = DepositPending();
        received.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(5), UserId);
        Kinds(received).ShouldBe(["Created", "DepositReceived"]);

        var waived = DepositPending();
        waived.Approve(Created + Duration.FromMinutes(5), UserId, waiveDeposit: true);
        Kinds(waived).ShouldBe(["Created", "Approved", "DepositWaived"]);
    }

    private static string[] Kinds(Appointment appointment) => [.. AppointmentDetailReader.History(appointment).Select(e => e.Kind)];

    private Appointment DepositPending(Duration? approvalTimeout = null) =>
        Create(approvalTimeout, new DepositHold(Deposit, "OMV-7K2X", Window));

    private Appointment Create(Duration? approvalTimeout, DepositHold? deposit) =>
        Appointment.Create(
            new NewAppointment(
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2"),
                AppointmentChannel.Online,
                ClientApp.BookingPage,
                CustomerNote: null,
                StaffNote: null,
                SeriesId: null,
                CreatedByUserId: null,
                approvalTimeout,
                deposit),
            AppointmentPlanner.Plan(Start, [Offers.Haircut(), Offers.BlowDry()]).Value,
            _ids,
            Created);
}
