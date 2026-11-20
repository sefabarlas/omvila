using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: musaitlik.md § Girdi (BufferBefore + aşamalar + BufferAfter), § Çok kalemli randevu (kalem n+1, kalem n'in son
// personel-meşgul diliminin bitişinde başlar), M3 (Boya 45 + 40 bekleme + 35), M15 (personel özel süresi);
// alan-modeli.md § BranchServicePrice (fiyat kopyası); dilim-02 kabul kriterleri 1, 2, 3.
public sealed class AppointmentPlannerTests
{
    private static readonly Instant Start = Offers.Tuesday1430;

    [Fact]
    public void StagedService_ProducesStageSegments_WithWaitingStageNotStaffBusy()
    {
        var item = AppointmentPlanner.Plan(Start, [Offers.Color()]).Value.Items.ShouldHaveSingleItem();

        item.Segments.Select(s => (s.Kind, s.During.Duration.TotalMinutes, s.StaffBusy)).ShouldBe(
        [
            (SegmentKind.Stage, 45d, true),
            (SegmentKind.Stage, 40d, false),
            (SegmentKind.Stage, 35d, true),
        ]);
        item.Segments[1].During.ShouldBe(new Interval(Start + Duration.FromMinutes(45), Start + Duration.FromMinutes(85)));
        item.Segments.Select(s => s.SeqNo).ShouldBe([1, 2, 3]);
        item.StartUtc.ShouldBe(Start);
        item.EndUtc.ShouldBe(Start + Duration.FromMinutes(120));
    }

    [Fact]
    public void Segments_AreContiguousAndHalfOpen()
    {
        var item = AppointmentPlanner.Plan(Start, [Offers.Color()]).Value.Items[0];

        item.Segments.Zip(item.Segments.Skip(1)).ShouldAllBe(pair => pair.First.During.End == pair.Second.During.Start);
    }

    [Fact]
    public void Buffers_WrapTheStages_AndCountAsStaffBusy()
    {
        var offer = Offers.Haircut() with { BufferBeforeMinutes = 10, BufferAfterMinutes = 15 };

        var item = AppointmentPlanner.Plan(Start, [offer]).Value.Items[0];

        item.Segments.Select(s => (s.Kind, s.During.Duration.TotalMinutes, s.StaffBusy)).ShouldBe(
        [
            (SegmentKind.BufferBefore, 10d, true),
            (SegmentKind.Stage, 45d, true),
            (SegmentKind.BufferAfter, 15d, true),
        ]);
        item.StartUtc.ShouldBe(Start);
        item.EndUtc.ShouldBe(Start + Duration.FromMinutes(70));
    }

    [Fact]
    public void ZeroBuffers_ProduceNoBufferSegments() =>
        AppointmentPlanner.Plan(Start, [Offers.Haircut()]).Value.Items[0].Segments.ShouldAllBe(s => s.Kind == SegmentKind.Stage);

    [Fact]
    public void StaffDurationOverride_AppearsOnTheLastStaffBusyStage()
    {
        // Catalog personel farkını (130 − 120 = +10) son personel-meşgul aşamaya uygular; plan bu aşamaları izler.
        var offer = Offers.Color() with
        {
            Stages = [new PlannedStage("Uygulama", 45, true), new PlannedStage("Bekleme", 40, false), new PlannedStage("Yıkama ve fön", 45, true)],
        };

        var item = AppointmentPlanner.Plan(Start, [offer]).Value.Items[0];

        item.Segments[2].During.ShouldBe(new Interval(Start + Duration.FromMinutes(85), Start + Duration.FromMinutes(130)));
        item.EndUtc.ShouldBe(Start + Duration.FromMinutes(130));
    }

    [Fact]
    public void MultipleItems_AreChained_HaircutAndBlowDryEndAt1545()
    {
        var plan = AppointmentPlanner.Plan(Start, [Offers.Haircut(), Offers.BlowDry()]).Value;

        plan.Items[1].StartUtc.ShouldBe(plan.Items[0].EndUtc);
        plan.Items.Select(i => i.SeqNo).ShouldBe([1, 2]);
        plan.StartUtc.ShouldBe(Start);
        plan.EndUtc.InZone(Offers.Istanbul).TimeOfDay.ShouldBe(new LocalTime(15, 45));
    }

    [Fact]
    public void NextItem_StartsAtTheLastStaffBusyEnd_NotAtABufferlessWaitingTail()
    {
        // Son aşaması bekleme olan hizmet: sonraki kalem beklemenin başında (son personel-meşgul dilimin sonunda) başlar.
        var offer = Offers.Color() with { Stages = [new PlannedStage("Uygulama", 45, true), new PlannedStage("Bekleme", 40, false)] };

        var plan = AppointmentPlanner.Plan(Start, [offer, Offers.BlowDry(Offers.Zeynep)]).Value;

        plan.Items[1].StartUtc.ShouldBe(Start + Duration.FromMinutes(45));
        plan.EndUtc.ShouldBe(Start + Duration.FromMinutes(85));
    }

    [Fact]
    public void NextItem_StartsAfterTheBufferAfter()
    {
        var plan = AppointmentPlanner.Plan(Start, [Offers.Haircut() with { BufferAfterMinutes = 10 }, Offers.BlowDry()]).Value;

        plan.Items[1].StartUtc.ShouldBe(Start + Duration.FromMinutes(55));
    }

    [Fact]
    public void PriceSnapshot_IncludesBranchPriceAndStaffSurcharge_AndTotalIsTheSum()
    {
        // Nişantaşı şube fiyatı 2.800 ₺ + Elif'in Balyaj ek ücreti 300 ₺ (Catalog çözümü); kriter 2.
        var balayage = new ItemOffer(
            Guid.Parse("0193a8f0-0000-7000-8000-0000000005a4"), Offers.Elif, "Balyaj", Money.Try(310_000), Money.Try(30_000), 0, 0,
            [new PlannedStage("Balyaj", 180, true)]);

        var plan = AppointmentPlanner.Plan(Start, [balayage, Offers.BlowDry()]).Value;
        var appointment = Appointment.Create(Input(), plan, new SequentialIds(), Start - Duration.FromDays(1));

        plan.Total.ShouldBe(Money.Try(335_000));
        appointment.TotalAmountMinor.ShouldBe(335_000);
        appointment.CurrencyCode.ShouldBe("TRY");
        appointment.Items[0].PriceMinor.ShouldBe(310_000);
        appointment.Items[0].StaffSurchargeMinor.ShouldBe(30_000);
        appointment.Items[0].ServiceNameSnapshot.ShouldBe("Balyaj");
    }

    [Fact]
    public void NoItems_IsRequired() =>
        AppointmentPlanner.Plan(Start, []).Error!.FieldErrors["items"].ShouldBe(["required"]);

    [Fact]
    public void MoreThanFiveItems_IsOutOfRange() =>
        AppointmentPlanner.Plan(Start, [.. Enumerable.Repeat(Offers.BlowDry(), PlatformDefaults.MaxItemsPerAppointment + 1)])
            .Error!.FieldErrors["items"].ShouldBe(["out_of_range"]);

    private static NewAppointment Input() =>
        new(Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, AppointmentChannel.Online, ClientApp.BookingPage, null, null, null, null);
}
