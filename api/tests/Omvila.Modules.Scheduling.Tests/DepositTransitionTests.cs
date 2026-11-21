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

    // ───────── "Havaleyi yaptım" (dilim-04 kabul kriteri 4) ─────────

    [Fact]
    public void ReportDepositTransfer_Awaiting_IsCustomerReported_StatusUnchanged_SecondCallIsNoop()
    {
        var appointment = DepositPending();
        var first = Created + Duration.FromMinutes(10);

        appointment.ReportDepositTransfer(first).Value.ShouldBeTrue();
        appointment.ReportDepositTransfer(first + Duration.FromMinutes(5)).Value.ShouldBeFalse("ikinci bildirim yeni olay üretmez");

        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.DisplayStatus.ShouldBe(DisplayStatus.DepositPending);
        appointment.DepositStatus.ShouldBe(DepositStatus.CustomerReported);
        appointment.DepositCustomerReportedUtc.ShouldBe(first);
        appointment.HoldExpiresUtc.ShouldBe(Created + Window, "tutma süresi değişmez");
        Kinds(appointment).ShouldBe(["Created", "DepositReported"]);
    }

    [Fact]
    public void ReportDepositTransfer_NotAwaitedOrExpired_IsRejected()
    {
        var now = Created + Duration.FromMinutes(5);
        Create(approvalTimeout: Duration.FromMinutes(120), deposit: null).ReportDepositTransfer(now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        var received = DepositPending();
        received.MarkDepositReceived(PaymentId, now, UserId);
        received.ReportDepositTransfer(now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        DepositPending().ReportDepositTransfer(Created + Window).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);
    }

    // ───────── "Hatırlat" (V6) ─────────

    [Fact]
    public void RemindDeposit_OncePerAppointment_RecordsTime()
    {
        var appointment = DepositPending();
        var now = Created + Duration.FromMinutes(30);
        appointment.ReportDepositTransfer(now - Duration.FromMinutes(1));

        appointment.RemindDeposit(now, UserId).IsSuccess.ShouldBeTrue();

        appointment.DepositRemindedUtc.ShouldBe(now);
        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.CanRemindDeposit(now).Error!.Code.ShouldBe(ErrorCodes.DepositReminderAlreadySent);
        appointment.RemindDeposit(now, UserId).Error!.Code.ShouldBe(ErrorCodes.DepositReminderAlreadySent);
        Kinds(appointment).ShouldBe(["Created", "DepositReported", "DepositReminded"]);
    }

    [Fact]
    public void RemindDeposit_NotAwaitedOrExpired_IsRejected()
    {
        var now = Created + Duration.FromMinutes(5);
        var waived = DepositPending();
        waived.Approve(now, UserId, waiveDeposit: true);
        waived.RemindDeposit(now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        var expired = DepositPending();
        expired.RemindDeposit(Created + Window, UserId).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);
        expired.DepositRemindedUtc.ShouldBeNull();
    }

    // ───────── "Salon iade etti" (V7) ─────────

    [Fact]
    public void MarkDepositRefunded_Partial_IsRefunded_AmountDueRestoresRefundedPart_HistoryCarriesNote()
    {
        var appointment = DepositPending();
        appointment.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(5), UserId);
        var now = Created + Duration.FromHours(2);

        appointment.MarkDepositRefunded(5_000, "  Müşteri erteledi  ", now, UserId).IsSuccess.ShouldBeTrue();

        appointment.DepositStatus.ShouldBe(DepositStatus.Refunded);
        appointment.DepositRefundedUtc.ShouldBe(now);
        appointment.DepositRefundedAmountMinor.ShouldBe(5_000);
        appointment.DepositRefundNote.ShouldBe("Müşteri erteledi");
        appointment.DepositRefundedByUserId.ShouldBe(UserId);
        appointment.Status.ShouldBe(AppointmentStatus.Confirmed, "randevu durumu değişmez");
        appointment.AmountDueMinor.ShouldBe(appointment.TotalAmountMinor - (Deposit - 5_000));
        var refunded = AppointmentDetailReader.History(appointment)[^1];
        refunded.Kind.ShouldBe("DepositRefunded");
        refunded.Detail.ShouldBe("Müşteri erteledi");
    }

    [Fact]
    public void MarkDepositRefunded_Full_AfterSalonCancel_AmountDueIsTotal()
    {
        var appointment = DepositPending();
        appointment.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(5), UserId);
        appointment.CancelBySalon(CancellationReason.StaffUnavailable, null, UserId, Created + Duration.FromMinutes(10));

        appointment.MarkDepositRefunded(Deposit, null, Created + Duration.FromMinutes(20), UserId).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Cancelled);
        appointment.DepositStatus.ShouldBe(DepositStatus.Refunded);
        appointment.AmountDueMinor.ShouldBe(appointment.TotalAmountMinor);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(Deposit + 1)]
    public void MarkDepositRefunded_OutOfRange_IsValidationError_StateUnchanged(long amount)
    {
        var appointment = DepositPending();
        appointment.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(5), UserId);

        var error = appointment.MarkDepositRefunded(amount, null, Created + Duration.FromMinutes(6), UserId).Error!;

        error.Code.ShouldBe(ErrorCodes.ValidationFailed);
        error.FieldErrors["amount"].ShouldBe(["out_of_range"]);
        appointment.DepositStatus.ShouldBe(DepositStatus.Received);
    }

    [Fact]
    public void MarkDepositRefunded_NotReceivedOrTwice_IsInvalidTransition()
    {
        var now = Created + Duration.FromMinutes(5);
        DepositPending().MarkDepositRefunded(100, null, now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        var waived = DepositPending();
        waived.Approve(now, UserId, waiveDeposit: true);
        waived.MarkDepositRefunded(100, null, now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        Create(approvalTimeout: null, deposit: null).MarkDepositRefunded(100, null, now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var once = DepositPending();
        once.MarkDepositReceived(PaymentId, now, UserId);
        once.MarkDepositRefunded(100, null, now, UserId).IsSuccess.ShouldBeTrue();
        once.MarkDepositRefunded(100, null, now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        once.DepositRefundedAmountMinor.ShouldBe(100);
    }

    // ───────── A13 / A14 kapora aktarımı ─────────

    [Fact]
    public void A14_Reschedule_ReceivedDeposit_IsCarriedOver()
    {
        var appointment = DepositPending();
        appointment.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(5), UserId);
        var now = Created + Duration.FromMinutes(30);

        appointment.Reschedule(Plan(Start + Duration.FromHours(2)), ChangeActor.Salon, now, Duration.FromHours(12), _ids, UserId).IsSuccess.ShouldBeTrue();

        appointment.StartUtc.ShouldBe(Start + Duration.FromHours(2));
        appointment.DepositStatus.ShouldBe(DepositStatus.Received);
        appointment.DepositPaymentId.ShouldBe(PaymentId);
        appointment.DepositReceivedUtc.ShouldBe(Created + Duration.FromMinutes(5));
        appointment.AmountDueMinor.ShouldBe(appointment.TotalAmountMinor - Deposit);
    }

    [Fact]
    public void A13_CustomerReschedule_ReceivedDeposit_PendingApproval_KeepsDeposit_ApprovalHold()
    {
        var appointment = DepositPending();
        appointment.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(5), UserId);
        var now = Created + Duration.FromMinutes(30);

        appointment.Reschedule(Plan(Start + Duration.FromHours(2)), ChangeActor.Customer, now, Duration.FromHours(12), _ids, null,
            approvalHold: Duration.FromMinutes(60), depositWindow: Window).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.DisplayStatus.ShouldBe(DisplayStatus.PendingApproval, "alınmış kapora korunur: Onay bekliyor");
        appointment.HoldExpiresUtc.ShouldBe(now + Duration.FromMinutes(60));
        appointment.HoldReason.ShouldBe(HoldReason.Approval);
        appointment.DepositStatus.ShouldBe(DepositStatus.Received);
    }

    [Fact]
    public void A13_CustomerReschedule_AwaitedDeposit_HoldIsTransferWindow_DepositReasonKept()
    {
        var appointment = DepositPending(approvalTimeout: Duration.FromMinutes(60));
        appointment.ReportDepositTransfer(Created + Duration.FromMinutes(1));
        var now = Created + Duration.FromMinutes(30);

        appointment.Reschedule(Plan(Start + Duration.FromHours(2)), ChangeActor.Customer, now, Duration.FromHours(12), _ids, null,
            approvalHold: Duration.FromMinutes(60), depositWindow: Window).IsSuccess.ShouldBeTrue();

        appointment.DisplayStatus.ShouldBe(DisplayStatus.DepositPending);
        appointment.HoldExpiresUtc.ShouldBe(now + Window, "kapora ve onay birlikteyken tek tutma süresi havale süresidir");
        appointment.HoldReason.ShouldBe(HoldReason.ApprovalAndDeposit);
        appointment.DepositStatus.ShouldBe(DepositStatus.CustomerReported);
        appointment.DepositTransferCode.ShouldBe("OMV-7K2X");
    }

    // Kural: durum-makinesi.md A11; dilim-05 KK5 (toplam − alınmış kapora salonda alınır).
    [Fact]
    public void A11_Complete_ReceivedDeposit_DueIsTotalMinusDeposit_ThenZero()
    {
        var appointment = DepositPending();
        appointment.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(40), UserId).IsSuccess.ShouldBeTrue();
        appointment.CheckIn(Start, UserId).IsSuccess.ShouldBeTrue();
        appointment.RetainedDepositMinor.ShouldBe(Deposit);

        appointment.Complete(appointment.TotalAmountMinor, Start, UserId).Error!.Code.ShouldBe(ErrorCodes.PaymentTotalMismatch);
        appointment.Complete(appointment.TotalAmountMinor - Deposit, Start, UserId).IsSuccess.ShouldBeTrue();

        appointment.AmountDueMinor.ShouldBe(0);
        appointment.RetainedDepositMinor.ShouldBe(Deposit, "kapora tahsilatı randevuda kalır");
    }

    [Fact]
    public void RetainedDeposit_PartialRefund_IsReceivedMinusRefunded_NoDepositIsZero()
    {
        var appointment = DepositPending();
        appointment.RetainedDepositMinor.ShouldBe(0, "kapora gelmeden salonda kapora yok");
        appointment.MarkDepositReceived(PaymentId, Created + Duration.FromMinutes(40), UserId).IsSuccess.ShouldBeTrue();
        appointment.MarkDepositRefunded(5_000, null, Created + Duration.FromHours(1), UserId).IsSuccess.ShouldBeTrue();

        appointment.RetainedDepositMinor.ShouldBe(Deposit - 5_000);
    }

    private static AppointmentPlan Plan(Instant start) => AppointmentPlanner.Plan(start, [Offers.Haircut(), Offers.BlowDry()]).Value;

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
