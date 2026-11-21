using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: durum-makinesi.md § 4 kapıda sıra (Waiting → Called → Served | Left, Waiting → Served), bildirimler.md walkin.almost_turn
// (telefon verildiyse, bir kez), sureler-ve-ayarlar.md WalkInAverageServiceMinutes ("öndekilerin kalan süreleri / müsait personel").
public sealed class WalkInTicketTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);
    private static readonly Guid Brand = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    private static readonly Guid Branch = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2");
    private static readonly Guid AppointmentId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1");
    private static readonly Guid CardId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2");

    [Fact]
    public void Create_IsWaiting_WithNumberAndTrimmedName()
    {
        var ticket = Ticket(phone: null);

        ticket.Status.ShouldBe(WalkInTicketStatus.Waiting);
        ticket.QueueNumber.ShouldBe(3);
        ticket.QueueDate.ShouldBe(new LocalDate(2026, 11, 25));
        ticket.CustomerName.ShouldBe("Burak");
        ticket.JoinedUtc.ShouldBe(Now);
        ticket.IsInQueue.ShouldBeTrue();
    }

    [Fact]
    public void Create_NumberBelowOne_Throws() =>
        Should.Throw<DomainException>(() => WalkInTicket.Create(Input(null), AppointmentId, new LocalDate(2026, 11, 25), 0, Now));

    [Fact]
    public void Serve_FromWaiting_IsServed_WithAppointmentAndCustomer()
    {
        var ticket = Ticket(phone: null);

        ticket.Serve(AppointmentId, CardId, Now + Duration.FromMinutes(5)).IsSuccess.ShouldBeTrue();

        ticket.Status.ShouldBe(WalkInTicketStatus.Served);
        ticket.AppointmentId.ShouldBe(AppointmentId);
        ticket.BrandCustomerId.ShouldBe(CardId);
        ticket.ClosedUtc.ShouldBe(Now + Duration.FromMinutes(5));
        ticket.IsInQueue.ShouldBeFalse();
    }

    [Fact]
    public void Leave_FromWaiting_IsLeft()
    {
        var ticket = Ticket(phone: null);

        ticket.Leave(Now).IsSuccess.ShouldBeTrue();

        ticket.Status.ShouldBe(WalkInTicketStatus.Left);
        ticket.ClosedUtc.ShouldBe(Now);
    }

    [Fact]
    public void Serve_And_Leave_AfterClosing_AreInvalidStateTransitions()
    {
        var served = Ticket(phone: null);
        served.Serve(AppointmentId, CardId, Now);
        var left = Ticket(phone: null);
        left.Leave(Now);

        served.Serve(AppointmentId, CardId, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        served.Leave(Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        left.Serve(AppointmentId, CardId, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        left.Leave(Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Fact]
    public void AlmostTurn_OnlyWithPhone_OnlyOnce_OnlyInQueue()
    {
        var withPhone = Ticket(PhoneNumber.Parse("+905550004418"));
        var withoutPhone = Ticket(phone: null);
        var left = Ticket(PhoneNumber.Parse("+905550004419"));
        left.Leave(Now);

        withPhone.TryMarkAlmostTurn(Now).ShouldBeTrue();
        withPhone.AlmostTurnNotifiedUtc.ShouldBe(Now);
        withPhone.TryMarkAlmostTurn(Now + Duration.FromMinutes(1)).ShouldBeFalse();
        withoutPhone.TryMarkAlmostTurn(Now).ShouldBeFalse();
        left.TryMarkAlmostTurn(Now).ShouldBeFalse();
    }

    [Fact]
    public void Estimate_DividesRemainingMinutesAheadByAvailableStaff_RoundingUp()
    {
        var (perTicket, forNew) = WalkInWaitEstimator.Estimate([45, 30, null], fallbackMinutes: 40, availableStaff: 2);

        perTicket.ShouldBe([0, 23, 38]);
        forNew.ShouldBe(58);
    }

    [Fact]
    public void Estimate_WithoutAvailableStaff_IsNull()
    {
        var (perTicket, forNew) = WalkInWaitEstimator.Estimate([45, 30], fallbackMinutes: null, availableStaff: 0);

        perTicket.ShouldBe([null, null]);
        forNew.ShouldBeNull();
    }

    [Fact]
    public void Estimate_UnknownDurationAhead_MakesLaterEstimatesUnknown()
    {
        var (perTicket, forNew) = WalkInWaitEstimator.Estimate([null, 30], fallbackMinutes: null, availableStaff: 1);

        perTicket.ShouldBe([0, null]);
        forNew.ShouldBeNull();
    }

    [Fact]
    public void Estimate_EmptyQueue_NewComerWaitsNothing() =>
        WalkInWaitEstimator.Estimate([], fallbackMinutes: null, availableStaff: 1).ForNew.ShouldBe(0);

    private static WalkInTicket Ticket(PhoneNumber? phone) =>
        WalkInTicket.Create(Input(phone), Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1"), new LocalDate(2026, 11, 25), 3, Now);

    private static NewWalkInTicket Input(PhoneNumber? phone) =>
        new(Brand, Branch, " Burak ", phone, null, null, null, null, null, null);
}
