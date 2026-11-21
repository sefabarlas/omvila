using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: durum-makinesi.md § 1 geçiş tablosu (Dilim 2: A1, A5, A6, A7, A8, A9, A10, A14) ve § Değişmezler; her geçiş için
// mutlu yol ve koruma testi. Ayarlar sureler-ve-ayarlar.md varsayılanlarıyla: CancellationCutoffHours 24,
// RescheduleCutoffHours 12, NoShowAfterMinutes 15, ApprovalTimeoutMinutes 120.
public sealed class AppointmentTransitionTests
{
    private static readonly Instant Start = Offers.Tuesday1430;
    private static readonly Duration CancellationCutoff = Duration.FromHours(24);
    private static readonly Duration RescheduleCutoff = Duration.FromHours(12);
    private static readonly Duration NoShowAfter = Duration.FromMinutes(15);
    private static readonly Guid UserId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1");

    private readonly SequentialIds _ids = new();

    // ───────── A1 ─────────

    [Fact]
    public void A1_Online_WithoutApproval_IsConfirmed_WithSegmentsActive()
    {
        var now = Start - Duration.FromDays(2);

        var appointment = Create(AppointmentChannel.Online, now);

        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
        appointment.ConfirmedUtc.ShouldBe(now);
        appointment.HoldExpiresUtc.ShouldBeNull();
        appointment.CreatedUtc.ShouldBe(now);
        appointment.StartUtc.ShouldBe(Start);
        appointment.EndUtc.ShouldBe(Start + Duration.FromMinutes(75));
        appointment.Items.Count.ShouldBe(2);
        appointment.Items.ShouldAllBe(i => i.Status == AppointmentItemStatus.Active && i.AppointmentId == appointment.Id);
        AllSegments(appointment).ShouldAllBe(s => s.IsActive && s.StaffId == Offers.Elif);
    }

    [Fact]
    public void A1_Online_WithApproval_IsPendingApproval_WithHold()
    {
        var now = Start - Duration.FromDays(2);

        var appointment = Create(AppointmentChannel.Online, now, approvalTimeout: Duration.FromMinutes(120));

        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
        appointment.HoldExpiresUtc.ShouldBe(now + Duration.FromMinutes(120));
        appointment.HoldReason.ShouldBe(HoldReason.Approval);
        appointment.ConfirmedUtc.ShouldBeNull();
    }

    [Theory]
    [InlineData(nameof(AppointmentChannel.Panel))]
    [InlineData(nameof(AppointmentChannel.Phone))]
    public void A1_PanelAndPhone_AreConfirmed_EvenIfApprovalIsConfigured(string channel) =>
        Create(Enum.Parse<AppointmentChannel>(channel), Start - Duration.FromDays(1), approvalTimeout: Duration.FromMinutes(120)).Status.ShouldBe(AppointmentStatus.Confirmed);

    [Fact]
    public void A1_WalkIn_IsCheckedIn()
    {
        var appointment = Create(AppointmentChannel.WalkIn, Start);

        appointment.Status.ShouldBe(AppointmentStatus.CheckedIn);
        appointment.CheckedInUtc.ShouldBe(Start);
    }

    // ───────── A5 ─────────

    [Fact]
    public void A5_ExpiredApprovalHold_IsCancelledBySystem_ApprovalExpired_SegmentsInactive()
    {
        var created = Start - Duration.FromDays(1);
        var appointment = Create(AppointmentChannel.Online, created, approvalTimeout: Duration.FromMinutes(120));
        var now = created + Duration.FromMinutes(120);

        appointment.ExpireHold(now).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Cancelled);
        appointment.CancelledBy.ShouldBe(CancelledBy.System);
        appointment.CancellationReason.ShouldBe(CancellationReason.ApprovalExpired);
        appointment.CancelledUtc.ShouldBe(now);
        appointment.HoldExpiresUtc.ShouldBeNull();
        appointment.HoldReason.ShouldBeNull();
        AllSegments(appointment).ShouldAllBe(s => !s.IsActive);
    }

    [Fact]
    public void A5_HoldNotYetExpired_IsRejected()
    {
        var created = Start - Duration.FromDays(1);
        var appointment = Create(AppointmentChannel.Online, created, approvalTimeout: Duration.FromMinutes(120));

        appointment.ExpireHold(created + Duration.FromMinutes(119)).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        appointment.Status.ShouldBe(AppointmentStatus.PendingApproval);
    }

    [Fact]
    public void A5_ConfirmedAppointment_HasNoHoldToExpire() =>
        Create(AppointmentChannel.Phone, Start - Duration.FromDays(1)).ExpireHold(Start)
            .Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

    // ───────── A6 ─────────

    [Fact]
    public void A6_CustomerCancel_BeforeCutoff_CancelsAndDeactivatesSegments()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(3));
        var now = Start - Duration.FromHours(25);

        appointment.CancelByCustomer(CancellationReason.PlanChanged, null, now, CancellationCutoff).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Cancelled);
        appointment.CancelledBy.ShouldBe(CancelledBy.Customer);
        appointment.CancellationReason.ShouldBe(CancellationReason.PlanChanged);
        appointment.CancelledUtc.ShouldBe(now);
        appointment.Items.ShouldAllBe(i => i.Status == AppointmentItemStatus.Cancelled);
        AllSegments(appointment).ShouldAllBe(s => !s.IsActive);
    }

    [Fact]
    public void A6_CustomerCancel_ExactlyAtCutoff_IsAllowed() =>
        Create(AppointmentChannel.Online, Start - Duration.FromDays(3))
            .CancelByCustomer(CancellationReason.Sick, null, Start - CancellationCutoff, CancellationCutoff).IsSuccess.ShouldBeTrue();

    [Fact]
    public void A6_CustomerCancel_InsideCutoff_IsCutoffPassed()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(3));

        var result = appointment.CancelByCustomer(CancellationReason.PlanChanged, null, Start - Duration.FromHours(12), CancellationCutoff);

        result.Error!.Code.ShouldBe(ErrorCodes.CutoffPassed);
        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
        AllSegments(appointment).ShouldAllBe(s => s.IsActive);
    }

    [Fact]
    public void A6_CustomerCancel_PendingApproval_HasNoCutoff_AndClearsHold()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromHours(3), approvalTimeout: Duration.FromMinutes(120));

        appointment.CancelByCustomer(CancellationReason.WantsDifferentTime, null, Start - Duration.FromHours(2), CancellationCutoff).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Cancelled);
        appointment.HoldExpiresUtc.ShouldBeNull();
        appointment.HoldReason.ShouldBeNull();
    }

    [Fact]
    public void A6_Other_RequiresNote()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(3));
        var now = Start - Duration.FromDays(2);

        appointment.CancelByCustomer(CancellationReason.Other, "  ", now, CancellationCutoff).Error!.FieldErrors["note"].ShouldBe(["required"]);
        appointment.CancelByCustomer(CancellationReason.Other, " Taşınıyorum ", now, CancellationCutoff).IsSuccess.ShouldBeTrue();
        appointment.CancellationNote.ShouldBe("Taşınıyorum");
    }

    [Fact]
    public void A6_SalonReason_IsNotACustomerReason() =>
        Create(AppointmentChannel.Online, Start - Duration.FromDays(3))
            .CancelByCustomer(CancellationReason.StaffUnavailable, null, Start - Duration.FromDays(2), CancellationCutoff)
            .Error!.FieldErrors["reason"].ShouldBe(["invalid_format"]);

    [Fact]
    public void A6_CheckedIn_CannotBeCancelledByCustomer()
    {
        var appointment = Create(AppointmentChannel.WalkIn, Start);

        appointment.CancelByCustomer(CancellationReason.PlanChanged, null, Start, CancellationCutoff).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── A7 ─────────

    [Fact]
    public void A7_SalonCancel_HasNoCutoff()
    {
        var appointment = Create(AppointmentChannel.Phone, Start - Duration.FromDays(1));
        var now = Start - Duration.FromMinutes(10);

        appointment.CancelBySalon(CancellationReason.StaffUnavailable, null, UserId, now).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Cancelled);
        appointment.CancelledBy.ShouldBe(CancelledBy.Salon);
        appointment.CancelledByUserId.ShouldBe(UserId);
        appointment.UpdatedByUserId.ShouldBe(UserId);
        AllSegments(appointment).ShouldAllBe(s => !s.IsActive);
    }

    [Fact]
    public void A7_CheckedIn_CannotBeCancelledBySalonInDilim2() =>
        Create(AppointmentChannel.WalkIn, Start).CancelBySalon(CancellationReason.CustomerRequested, null, UserId, Start)
            .Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

    [Fact]
    public void A7_DeclinedBySalon_IsNotACancelReason() =>
        Create(AppointmentChannel.Phone, Start - Duration.FromDays(1)).CancelBySalon(CancellationReason.DeclinedBySalon, null, UserId, Start)
            .Error!.FieldErrors["reason"].ShouldBe(["invalid_format"]);

    [Fact]
    public void A7_Cancelled_IsFinal()
    {
        var appointment = Create(AppointmentChannel.Phone, Start - Duration.FromDays(1));
        appointment.CancelBySalon(CancellationReason.CustomerRequested, null, UserId, Start - Duration.FromHours(1));

        appointment.CancelBySalon(CancellationReason.CustomerRequested, null, UserId, Start).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        appointment.CheckIn(Start, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── A8 ─────────

    [Fact]
    public void A8_NoShow_AfterStartPlusThreshold_DeactivatesSegments_ItemsStayActive()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(1));
        var now = Start + NoShowAfter;

        appointment.MarkNoShow(now, NoShowAfter).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.NoShow);
        appointment.NoShowUtc.ShouldBe(now);
        AllSegments(appointment).ShouldAllBe(s => !s.IsActive);
        appointment.Items.ShouldAllBe(i => i.Status == AppointmentItemStatus.Active);
    }

    [Fact]
    public void A8_NoShow_BeforeThreshold_IsRejected()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(1));

        appointment.MarkNoShow(Start + Duration.FromMinutes(14), NoShowAfter).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
    }

    [Fact]
    public void A8_CheckedIn_IsNeverNoShow()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(1));
        appointment.CheckIn(Start, UserId);

        appointment.MarkNoShow(Start + Duration.FromHours(1), NoShowAfter).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── A9 ─────────

    [Fact]
    public void A9_UndoNoShow_SameLocalDay_ChecksInAndReactivatesSegments()
    {
        var appointment = NoShowAppointment();
        var now = Start + Duration.FromHours(3);

        appointment.UndoNoShow(now, Offers.Istanbul, UserId).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.CheckedIn);
        appointment.CheckedInUtc.ShouldBe(now);
        AllSegments(appointment).ShouldAllBe(s => s.IsActive);
    }

    [Fact]
    public void A9_UndoNoShow_NextLocalDay_IsRejected()
    {
        var appointment = NoShowAppointment();
        // 14:30 İstanbul randevusu; ertesi gün 00:05 İstanbul.
        var nextDay = Instant.FromUtc(2026, 11, 25, 21, 5);

        appointment.UndoNoShow(nextDay, Offers.Istanbul, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        appointment.Status.ShouldBe(AppointmentStatus.NoShow);
    }

    [Fact]
    public void A9_UndoNoShow_UsesTheBranchZone_NotUtc()
    {
        var appointment = NoShowAppointment();
        // 23:30 İstanbul aynı gündür, UTC'de de aynı gün; 00:30 İstanbul (21:30 UTC, UTC'de hâlâ aynı gün) değildir.
        appointment.UndoNoShow(Instant.FromUtc(2026, 11, 25, 21, 30), Offers.Istanbul, UserId).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        appointment.UndoNoShow(Instant.FromUtc(2026, 11, 25, 20, 30), Offers.Istanbul, UserId).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void A9_OnlyNoShow_CanBeUndone() =>
        Create(AppointmentChannel.Online, Start - Duration.FromDays(1)).UndoNoShow(Start, Offers.Istanbul, UserId)
            .Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

    // ───────── A10 ─────────

    [Fact]
    public void A10_CheckIn_UpTo60MinutesBeforeStart()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(1));
        var now = Start - Duration.FromMinutes(PlatformDefaults.CheckInEarliestMinutes);

        appointment.CheckIn(now, UserId).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.CheckedIn);
        appointment.CheckedInUtc.ShouldBe(now);
        AllSegments(appointment).ShouldAllBe(s => s.IsActive);
    }

    [Fact]
    public void A10_CheckIn_EarlierThan60Minutes_IsRejected() =>
        Create(AppointmentChannel.Online, Start - Duration.FromDays(1)).CheckIn(Start - Duration.FromMinutes(61), UserId)
            .Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

    [Fact]
    public void A10_PendingApproval_CannotBeCheckedIn() =>
        Create(AppointmentChannel.Online, Start - Duration.FromHours(3), approvalTimeout: Duration.FromMinutes(120)).CheckIn(Start, UserId)
            .Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

    // ───────── A14 ─────────

    [Fact]
    public void A14_CustomerReschedule_BeforeCutoff_MovesItems_OldSegmentsInactive_NewActive()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(3));
        var newStart = Start + Duration.FromDays(1);
        var now = Start - Duration.FromHours(13);

        appointment.Reschedule(Plan(newStart), ChangeActor.Customer, now, RescheduleCutoff, _ids, userId: null).IsSuccess.ShouldBeTrue();

        appointment.Status.ShouldBe(AppointmentStatus.Confirmed);
        appointment.StartUtc.ShouldBe(newStart);
        appointment.EndUtc.ShouldBe(newStart + Duration.FromMinutes(75));
        appointment.Items[1].StartUtc.ShouldBe(newStart + Duration.FromMinutes(45));
        foreach (var item in appointment.Items)
        {
            item.Segments.Count(s => s.IsActive).ShouldBe(1);
            item.Segments.Single(s => s.IsActive).During.Start.ShouldBe(item.StartUtc);
            item.Segments.Where(s => !s.IsActive).ShouldAllBe(s => s.SupersededUtc == now && s.During.Start < newStart);
            item.CurrentSegments.ShouldAllBe(s => s.IsActive);
        }
    }

    [Fact]
    public void A14_CustomerReschedule_InsideCutoff_IsCutoffPassed()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(3));

        appointment.Reschedule(Plan(Start + Duration.FromDays(1)), ChangeActor.Customer, Start - Duration.FromHours(11), RescheduleCutoff, _ids, null)
            .Error!.Code.ShouldBe(ErrorCodes.CutoffPassed);
        appointment.StartUtc.ShouldBe(Start);
        AllSegments(appointment).ShouldAllBe(s => s.IsActive && s.SupersededUtc == null);
    }

    [Fact]
    public void A14_SalonReschedule_HasNoCutoff_AndCanChangeStaff()
    {
        var appointment = Create(AppointmentChannel.Phone, Start - Duration.FromDays(1));
        var plan = AppointmentPlanner.Plan(Start + Duration.FromMinutes(30), [Offers.Haircut(Offers.Zeynep), Offers.BlowDry(Offers.Zeynep)]).Value;

        appointment.Reschedule(plan, ChangeActor.Salon, Start - Duration.FromMinutes(5), RescheduleCutoff, _ids, UserId).IsSuccess.ShouldBeTrue();

        appointment.Items.ShouldAllBe(i => i.StaffId == Offers.Zeynep);
        appointment.Items.SelectMany(i => i.CurrentSegments).ShouldAllBe(s => s.StaffId == Offers.Zeynep && s.IsActive);
        appointment.Items[0].PriceMinor.ShouldBe(45_000, "fiyat kopyası saat değişikliğinde korunur");
    }

    [Fact]
    public void A14_CheckedIn_CannotBeRescheduled() =>
        Create(AppointmentChannel.WalkIn, Start).Reschedule(Plan(Start + Duration.FromHours(1)), ChangeActor.Salon, Start, RescheduleCutoff, _ids, UserId)
            .Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

    [Fact]
    public void A14_PlanMustMatchTheItems() =>
        Should.Throw<DomainException>(() => Create(AppointmentChannel.Phone, Start - Duration.FromDays(1))
            .Reschedule(AppointmentPlanner.Plan(Start, [Offers.Color()]).Value, ChangeActor.Salon, Start, RescheduleCutoff, _ids, UserId));

    [Fact]
    public void A14_ThenNoShowUndo_ReactivatesOnlyTheCurrentSegments()
    {
        var appointment = Create(AppointmentChannel.Phone, Start - Duration.FromDays(1));
        var newStart = Start + Duration.FromHours(1);
        appointment.Reschedule(Plan(newStart), ChangeActor.Salon, Start - Duration.FromHours(2), RescheduleCutoff, _ids, UserId);
        appointment.MarkNoShow(newStart + NoShowAfter, NoShowAfter).IsSuccess.ShouldBeTrue();

        appointment.UndoNoShow(newStart + Duration.FromHours(1), Offers.Istanbul, UserId).IsSuccess.ShouldBeTrue();

        AllSegments(appointment).Where(s => s.IsActive).ShouldAllBe(s => s.SupersededUtc == null && s.During.Start >= newStart);
        AllSegments(appointment).Count(s => s.IsActive).ShouldBe(2);
    }

    private Appointment Create(AppointmentChannel channel, Instant now, Duration? approvalTimeout = null) =>
        Appointment.Create(
            new NewAppointment(
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1"),
                Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2"),
                channel,
                ClientApp.Panel,
                CustomerNote: null,
                StaffNote: null,
                SeriesId: null,
                CreatedByUserId: null,
                approvalTimeout),
            Plan(Start),
            _ids,
            now);

    private static AppointmentPlan Plan(Instant start) => AppointmentPlanner.Plan(start, [Offers.Haircut(), Offers.BlowDry()]).Value;

    private Appointment NoShowAppointment()
    {
        var appointment = Create(AppointmentChannel.Online, Start - Duration.FromDays(1));
        appointment.MarkNoShow(Start + NoShowAfter, NoShowAfter).IsSuccess.ShouldBeTrue();
        return appointment;
    }

    private static IEnumerable<AppointmentItemSegment> AllSegments(Appointment appointment) => appointment.Items.SelectMany(i => i.Segments);
}
