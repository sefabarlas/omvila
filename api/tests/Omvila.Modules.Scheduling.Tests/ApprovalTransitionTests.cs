using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: durum-makinesi.md § 1 geçiş tablosu (Dilim 3: A1 onay gerekli, A2, A4, A13) ve § Oluşturma kuralı ("Onay gerekli" =
// Service.RequiresApproval ?? BrandSettings.RequiresApproval; kalemlerden biri yeter), § Değişmezler (HoldExpiresUtc yalnız
// PendingApproval'da); T-62 (1) hold_expired. ApprovalTimeoutMinutes 120 (sureler-ve-ayarlar.md).
public sealed class ApprovalTransitionTests
{
    private static readonly Instant Start = Offers.Tuesday1430;
    private static readonly Duration Timeout = Duration.FromMinutes(120);
    private static readonly Duration RescheduleCutoff = Duration.FromHours(12);
    private static readonly Guid UserId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");

    private readonly SequentialIds _ids = new();

    // ───────── Onay gerekli çözümü ─────────

    [Theory]
    [InlineData(false, null, null, false)]
    [InlineData(true, null, null, true)]
    [InlineData(false, true, null, true)]
    [InlineData(false, null, true, true)]
    [InlineData(true, false, false, false)]
    [InlineData(true, false, null, true)]
    public void HoldFor_ServiceOverridesBrand_AnyItemRequiringMakesWholeAppointmentRequire(bool brand, bool? haircut, bool? blowDry, bool requires)
    {
        var plan = AppointmentPlanner.Plan(Start, [Offers.Haircut() with { RequiresApproval = haircut }, Offers.BlowDry() with { RequiresApproval = blowDry }]).Value;

        new ApprovalPolicy(brand, Timeout).HoldFor(plan).ShouldBe(requires ? Timeout : null);
    }

    // ───────── A2 ─────────

    [Fact]
    public void A2_Approve_PendingBeforeHold_IsConfirmed_HoldCleared()
    {
        var created = Start - Duration.FromDays(1);
        var appointment = Pending(created);
        var now = created + Duration.FromMinutes(119);

        appointment.Approve(now, UserId).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
        appointment.ConfirmedUtc.ShouldBe(now);
        appointment.HoldExpiresUtc.ShouldBeNull();
        appointment.HoldReason.ShouldBeNull();
        appointment.UpdatedByUserId.ShouldBe(UserId);
        AllSegments(appointment).ShouldAllBe(s => s.IsActive);
    }

    [Fact]
    public void A2_Approve_AtOrAfterHold_IsHoldExpired_StateUnchanged()
    {
        var created = Start - Duration.FromDays(1);
        var appointment = Pending(created);

        appointment.Approve(created + Timeout, UserId).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);
        appointment.Approve(created + Timeout + Duration.FromMinutes(1), UserId).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);
        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.HoldExpiresUtc.ShouldBe(created + Timeout);
    }

    [Fact]
    public void A2_Approve_ConfirmedOrCancelled_IsInvalidTransition()
    {
        var now = Start - Duration.FromDays(1);
        Create(AppointmentChannel.Online, now, approvalTimeout: null).Approve(now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var declined = Pending(now);
        declined.Decline(now, UserId).IsSuccess.ShouldBeTrue();
        declined.Approve(now, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── A4 ─────────

    [Fact]
    public void A4_Decline_IsCancelledBySalon_DeclinedBySalon_SegmentsInactive()
    {
        var created = Start - Duration.FromDays(1);
        var appointment = Pending(created);
        var now = created + Duration.FromMinutes(30);

        appointment.Decline(now, UserId).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Cancelled);
        appointment.CancelledBy.ShouldBe(CancelledBy.Salon);
        appointment.CancelledByUserId.ShouldBe(UserId);
        appointment.CancellationReason.ShouldBe(CancellationReason.DeclinedBySalon);
        appointment.CancellationNote.ShouldBeNull();
        appointment.CancelledUtc.ShouldBe(now);
        appointment.HoldExpiresUtc.ShouldBeNull();
        AllSegments(appointment).ShouldAllBe(s => !s.IsActive);
        appointment.Items.ShouldAllBe(i => i.Status == AppointmentItemStatus.Cancelled);
    }

    [Fact]
    public void A4_Decline_AfterHold_IsHoldExpired_ConfirmedIsInvalidTransition()
    {
        var created = Start - Duration.FromDays(1);
        Pending(created).Decline(created + Timeout, UserId).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);
        Create(AppointmentChannel.Phone, created, approvalTimeout: null).Decline(created, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── A13 ─────────

    [Fact]
    public void A13_CustomerReschedule_OnApprovalSalon_ConfirmedBecomesPending_WithNewHold()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(3), approvalTimeout: null);
        var now = Start - Duration.FromDays(1);
        var newStart = Start + Duration.FromDays(1);

        appointment.Reschedule(Plan(newStart), ChangeActor.Customer, now, RescheduleCutoff, _ids, userId: null, approvalHold: Timeout).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.HoldExpiresUtc.ShouldBe(now + Timeout);
        appointment.HoldReason.ShouldBe(HoldReason.Approval);
        appointment.StartUtc.ShouldBe(newStart);
    }

    [Fact]
    public void A13_PendingReschedule_RestartsHold_AndHasNoCutoff()
    {
        var created = Start - Duration.FromHours(3);
        var appointment = Pending(created);
        var now = Start - Duration.FromHours(2); // erteleme sınırı (12 sa) içinde, ama talep onay bekliyor

        appointment.Reschedule(Plan(Start + Duration.FromHours(1)), ChangeActor.Customer, now, RescheduleCutoff, _ids, null, Timeout).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.HoldExpiresUtc.ShouldBe(now + Timeout);
    }

    [Fact]
    public void A13_SalonReschedule_OrNoApproval_KeepsConfirmed()
    {
        var now = Start - Duration.FromDays(1);
        var bySalon = Create(AppointmentChannel.Online, now - Duration.FromDays(1), approvalTimeout: null);
        bySalon.Reschedule(Plan(Start + Duration.FromHours(1)), ChangeActor.Salon, now, RescheduleCutoff, _ids, UserId, Timeout).IsSuccess.ShouldBeTrue();
        bySalon.Status.ShouldBe(AppointmentStatus.Confirmed);
        bySalon.HoldExpiresUtc.ShouldBeNull();

        var noApproval = Create(AppointmentChannel.Online, now - Duration.FromDays(1), approvalTimeout: null);
        noApproval.Reschedule(Plan(Start + Duration.FromHours(1)), ChangeActor.Customer, now, RescheduleCutoff, _ids, null).IsSuccess.ShouldBeTrue();
        noApproval.Status.ShouldBe(AppointmentStatus.Confirmed);
    }

    [Fact]
    public void A13_ConfirmedInsideCutoff_IsCutoffPassed_StaysConfirmed()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(3), approvalTimeout: null);

        appointment.Reschedule(Plan(Start + Duration.FromDays(1)), ChangeActor.Customer, Start - Duration.FromHours(11), RescheduleCutoff, _ids, null, Timeout)
            .Error!.Code.ShouldBe(ErrorCodes.CutoffPassed);
        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
    }

    // ───────── Geçmiş ─────────

    [Fact]
    public void History_ApprovedDeclinedAndHoldExpired_AreDistinctKinds()
    {
        var created = Start - Duration.FromDays(1);
        var approved = Pending(created);
        approved.Approve(created + Duration.FromMinutes(5), UserId);
        Kinds(approved).ShouldBe(["Created", "Approved"]);

        var declined = Pending(created);
        declined.Decline(created + Duration.FromMinutes(5), UserId);
        Kinds(declined).ShouldBe(["Created", "Declined"]);

        var expired = Pending(created);
        expired.ExpireHold(created + Timeout);
        Kinds(expired).ShouldBe(["Created", "HoldExpired"]);

        var confirmedAtOnce = Create(AppointmentChannel.Phone, created, approvalTimeout: null);
        confirmedAtOnce.CancelBySalon(CancellationReason.CustomerRequested, null, UserId, created + Duration.FromMinutes(5));
        Kinds(confirmedAtOnce).ShouldBe(["Created", "Cancelled"]);
    }

    private static string[] Kinds(Appointment appointment) => [.. AppointmentDetailReader.History(appointment).Select(e => e.Kind)];

    private Appointment Pending(Instant now) => Create(AppointmentChannel.Online, now, Timeout);

    private Appointment Create(AppointmentChannel channel, Instant now, Duration? approvalTimeout) =>
        Appointment.Create(
            new NewAppointment(
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2"),
                channel,
                ClientApp.BookingPage,
                CustomerNote: null,
                StaffNote: null,
                SeriesId: null,
                CreatedByUserId: null,
                approvalTimeout),
            Plan(Start),
            _ids,
            now);

    private static AppointmentPlan Plan(Instant start) => AppointmentPlanner.Plan(start, [Offers.Haircut(), Offers.BlowDry()]).Value;

    private static IEnumerable<AppointmentItemSegment> AllSegments(Appointment appointment) => appointment.Items.SelectMany(i => i.Segments);
}
