using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.SharedKernel;
using static Omvila.Modules.Scheduling.Tests.Availability.Scenario;

namespace Omvila.Modules.Scheduling.Tests.Availability;

// Kural: musaitlik.md § Algoritma (1–8), § Çok kalemli randevu, § "Fark etmez", § Gün listesi, § Zorunlu testler M1–M20;
// dilim-02 kabul kriterleri 3, 5, 6, 7, 8, 16, 18.
public sealed class AvailabilityEngineTests
{
    private static readonly RequestedItem HaircutElif = new(Offers.HaircutId, Elif);

    [Fact]
    public void M1_AppointmentEndingAt1400_AllowsStartAt1400()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(13, 15), s.At(14, 0));

        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(14, 0)).ShouldBeTrue();
        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(13, 15)).ShouldBeFalse();
    }

    [Fact]
    public void M2_StartThatSpillsIntoBusyTime_IsNotACandidate()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(14, 0), s.At(14, 45));
        var snapshot = s.Build();

        AvailabilityEngine.IsSlotAvailable(snapshot, Request(HaircutElif), s.At(13, 30)).ShouldBeFalse("13:30 + 45 dk = 14:15");
        AvailabilityEngine.Evaluate(snapshot, Request(HaircutElif), s.At(13, 30)).Violations.ShouldBe(SlotViolation.StaffBusy);
        AvailabilityEngine.IsSlotAvailable(snapshot, Request(HaircutElif), s.At(13, 15)).ShouldBeTrue();
    }

    [Fact]
    public void M3_WaitingStage_AllowsAnotherServiceInside()
    {
        var s = new Scenario().Offer(Offers.Color(Elif));
        s.Busy(Elif, s.At(11, 20), s.At(11, 50));

        // Boya 10:30: uygulama 10:30–11:15, bekleme 11:15–11:55, fön 11:55–12:30. Başka işin 11:20–11:50 bekleme içinde.
        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(new RequestedItem(Offers.ColorId, Elif)), s.At(10, 30)).ShouldBeTrue();
    }

    [Fact]
    public void Criterion3_Color1030_BlowDryFitsAt1115_ButNotAt1045()
    {
        var s = new Scenario().Offer(Offers.BlowDry(Elif));
        // Elif'e yazılmış Boya 10:30'un personel-meşgul dilimleri.
        s.Busy(Elif, s.At(10, 30), s.At(11, 15)).Busy(Elif, s.At(11, 55), s.At(12, 30));
        var snapshot = s.Build();
        var blowDry = Request(new RequestedItem(Offers.BlowDryId, Elif));

        AvailabilityEngine.IsSlotAvailable(snapshot, blowDry, s.At(11, 15)).ShouldBeTrue();
        AvailabilityEngine.IsSlotAvailable(snapshot, blowDry, s.At(10, 45)).ShouldBeFalse();
        AvailabilityEngine.IsSlotAvailable(snapshot, blowDry, s.At(11, 30)).ShouldBeFalse("11:30–12:00 fön dilimine taşar");
    }

    [Fact]
    public void M3_WaitingStage_MustStayWithinBranchHours()
    {
        var offer = new ItemOffer(Offers.ColorId, Elif, "Maske", Money.Try(50_000), Money.Zero(Currency.TRY), 0, 0,
            [new PlannedStage("Uygulama", 30, true), new PlannedStage("Bekleme", 30, false)]);
        var s = new Scenario().Offer(offer);
        var request = Request(new RequestedItem(Offers.ColorId, Elif));

        AvailabilityEngine.IsSlotAvailable(s.Build(), request, s.At(19, 0)).ShouldBeTrue();
        AvailabilityEngine.Evaluate(s.Build(), request, s.At(19, 30)).Violations.ShouldBe(SlotViolation.OutsideWorkingHours);
    }

    [Fact]
    public void M4_Criterion5_StaffBusyInAnotherBranchUntilNoon_FirstCandidateAfterTransfer()
    {
        var s = new Scenario().InBranch(Nisantasi).Offer(Offers.Haircut(Mert));
        s.Busy(Mert, s.At(10, 0), s.At(12, 0), branchId: Kadikoy);

        var day = AvailabilityEngine.GetSlots(s.Build(), Request(new RequestedItem(Offers.HaircutId, Mert)), Tuesday);

        day.Slots[0].LocalTime.ShouldBe(new LocalTime(12, 30));
        day.UnavailableTimes.ShouldContain(new LocalTime(12, 15));
    }

    [Fact]
    public void M4_TransferBuffer_AlsoAppliesBeforeTheOtherBranchAppointment()
    {
        var s = new Scenario().InBranch(Nisantasi).Offer(Offers.Haircut(Mert));
        s.Busy(Mert, s.At(15, 0), s.At(16, 0), branchId: Kadikoy);
        var snapshot = s.Build();
        var request = Request(new RequestedItem(Offers.HaircutId, Mert));

        AvailabilityEngine.IsSlotAvailable(snapshot, request, s.At(13, 45)).ShouldBeTrue("13:45–14:30 + 30 dk geçiş = 15:00");
        AvailabilityEngine.IsSlotAvailable(snapshot, request, s.At(14, 0)).ShouldBeFalse();
        AvailabilityEngine.IsSlotAvailable(snapshot, request, s.At(16, 30)).ShouldBeTrue();
        AvailabilityEngine.IsSlotAvailable(snapshot, request, s.At(16, 15)).ShouldBeFalse();
    }

    [Fact]
    public void M5_StaffOnFullDayTimeOff_HasNoCandidates_EvenThoughBranchIsOpen()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.TimeOff(Elif, s.At(0, 0), s.At(Tuesday.PlusDays(1), 0, 0));

        var day = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday);

        day.IsClosed.ShouldBeFalse();
        day.Slots.ShouldBeEmpty();
        AvailabilityEngine.Evaluate(s.Build(), Panel(HaircutElif), s.At(11, 0)).Violations.ShouldBe(SlotViolation.StaffOnTimeOff);
    }

    [Fact]
    public void M6_BranchClosedOverride_HasNoCandidates()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif)).BranchOverride(Tuesday, []);

        var day = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday);

        day.IsClosed.ShouldBeTrue();
        day.Slots.ShouldBeEmpty();
        day.UnavailableTimes.ShouldBeEmpty();
    }

    [Fact]
    public void M7_Criterion16_OnlyThisDayOpenOverride_OpensClosedSunday()
    {
        var sunday = new LocalDate(2026, 11, 29);
        var s = new Scenario()
            .Staff(Elif, hours: Week(new LocalTime(10, 0), new LocalTime(19, 0), sunday: true))
            .Offer(Offers.Haircut(Elif));

        var closed = AvailabilityEngine.GetDays(s.Build(), Request(HaircutElif), sunday, sunday).ShouldHaveSingleItem();
        closed.IsClosed.ShouldBeTrue();
        closed.HasAvailability.ShouldBeFalse();

        s.BranchOverride(sunday, Range(11, 0, 16, 0));
        var day = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), sunday);

        day.IsClosed.ShouldBeFalse();
        day.Slots[0].LocalTime.ShouldBe(new LocalTime(11, 0));
        day.Slots[^1].LocalTime.ShouldBe(new LocalTime(15, 15));
    }

    [Fact]
    public void M7_Criterion16_OnlyThisDayOpen_StaffWithoutHoursThatWeekday_FallsBackToOverrideHours()
    {
        // Elif'in pazar saati yok; özel gün 11:00–16:00 onun saati sayılır, izin ve meşguliyet yine çıkarılır.
        var sunday = new LocalDate(2026, 11, 29);
        var s = new Scenario().Offer(Offers.Haircut(Elif));

        AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), sunday).Slots.ShouldBeEmpty("özel gün yokken pazar kapalı");

        s.BranchOverride(sunday, Range(11, 0, 16, 0));
        s.Busy(Elif, s.At(sunday, 12, 0), s.At(sunday, 13, 0));
        var times = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), sunday).Slots.Select(x => x.LocalTime).ToList();

        times[0].ShouldBe(new LocalTime(11, 0));
        times[^1].ShouldBe(new LocalTime(15, 15));
        times.ShouldNotContain(new LocalTime(11, 30));
        times.ShouldNotContain(new LocalTime(12, 30));
        times.ShouldContain(new LocalTime(13, 0));
        AvailabilityEngine.GetDays(s.Build(), Request(HaircutElif), sunday, sunday)[0].HasAvailability.ShouldBeTrue();
    }

    [Fact]
    public void M7_OnlyThisDayOpen_StaffWithNarrowerSundayHours_KeepsTheIntersection()
    {
        var sunday = new LocalDate(2026, 11, 29);
        var hours = Week(new LocalTime(10, 0), new LocalTime(20, 0));
        hours[IsoDayOfWeek.Sunday] = Range(13, 0, 15, 0);
        var s = new Scenario().Staff(Elif, hours: hours).Offer(Offers.Haircut(Elif)).BranchOverride(sunday, Range(11, 0, 16, 0));

        var times = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), sunday).Slots.Select(x => x.LocalTime);

        times.ShouldBe([new LocalTime(13, 0), new LocalTime(13, 15), new LocalTime(13, 30), new LocalTime(13, 45), new LocalTime(14, 0), new LocalTime(14, 15)]);
    }

    [Fact]
    public void M7_StaffOverride_TakesPrecedenceOverTheFallback()
    {
        var sunday = new LocalDate(2026, 11, 29);
        var s = new Scenario().Offer(Offers.Haircut(Elif)).BranchOverride(sunday, Range(11, 0, 16, 0)).StaffOverride(Elif, sunday, Range(12, 0, 14, 0));

        AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), sunday).Slots.Select(x => x.LocalTime)
            .ShouldBe([new LocalTime(12, 0), new LocalTime(12, 15), new LocalTime(12, 30), new LocalTime(12, 45), new LocalTime(13, 0), new LocalTime(13, 15)]);
    }

    [Fact]
    public void ShortDayOverride_OnANormallyOpenWeekday_DoesNotOpenStaffWithoutHours()
    {
        // Geri düşüş yalnız haftalık düzende kapalı günü açan özel günde; açık günün kısaltılması personele saat vermez.
        var hours = Week(new LocalTime(10, 0), new LocalTime(20, 0));
        hours.Remove(IsoDayOfWeek.Tuesday);
        var s = new Scenario().Staff(Elif, hours: hours).Offer(Offers.Haircut(Elif)).BranchOverride(Tuesday, Range(10, 0, 14, 0));

        AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday).Slots.ShouldBeEmpty();
    }

    [Fact]
    public void M8_ResourceWithCapacityOne_UsedByAnotherItem_IsNotACandidate()
    {
        var room = new BranchResource(Guid.Parse("0193a8f0-0000-7000-8000-0000000000f1"), Guid.Parse("0193a8f0-0000-7000-8000-0000000000f0"), 1, IsExclusive: true);
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Resource(room).Need(Offers.HaircutId, new ResourceNeed(room.ResourceTypeId, 1));
        s.ResourceUsage(room.Id, s.At(14, 0), s.At(15, 0));
        var snapshot = s.Build();

        var taken = AvailabilityEngine.Evaluate(snapshot, Request(HaircutElif), s.At(14, 30));
        taken.Violations.ShouldBe(SlotViolation.ResourceUnavailable);
        taken.IsAllowed(allowOutsideWorkingHours: true).ShouldBeFalse();

        var free = AvailabilityEngine.Evaluate(snapshot, Request(HaircutElif), s.At(15, 0));
        free.IsAvailable.ShouldBeTrue();
        free.Candidate!.Items[0].Resources.ShouldBe([new ResourceAllocation(room.Id, 1, new Interval(s.At(15, 0), s.At(15, 45)))]);
    }

    [Fact]
    public void M8_ResourceWithCapacityTwo_AllowsASecondConcurrentUse()
    {
        var chairs = new BranchResource(Guid.Parse("0193a8f0-0000-7000-8000-0000000000f2"), Guid.Parse("0193a8f0-0000-7000-8000-0000000000f0"), 2, IsExclusive: false);
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Resource(chairs).Need(Offers.HaircutId, new ResourceNeed(chairs.ResourceTypeId, 1));
        s.ResourceUsage(chairs.Id, s.At(14, 0), s.At(15, 0));

        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(14, 30)).ShouldBeTrue();

        s.ResourceUsage(chairs.Id, s.At(14, 30), s.At(16, 0));
        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(14, 30)).ShouldBeFalse();
    }

    [Fact]
    public void M8_StageBoundResourceNeed_OnlyAppliesDuringThatStage()
    {
        // Boya: yalnız 3. aşama (yıkama ve fön, 11:55–12:30) yıkama koltuğu ister.
        var basin = new BranchResource(Guid.Parse("0193a8f0-0000-7000-8000-0000000000f3"), Guid.Parse("0193a8f0-0000-7000-8000-0000000000f4"), 1, IsExclusive: true);
        var s = new Scenario().Offer(Offers.Color(Elif)).Resource(basin).Need(Offers.ColorId, new ResourceNeed(basin.ResourceTypeId, 1, 3, 3));
        s.ResourceUsage(basin.Id, s.At(10, 30), s.At(11, 55));
        var request = Request(new RequestedItem(Offers.ColorId, Elif));

        AvailabilityEngine.IsSlotAvailable(s.Build(), request, s.At(10, 30)).ShouldBeTrue();
        AvailabilityEngine.IsSlotAvailable(s.Build(), request, s.At(10, 15)).ShouldBeFalse("fön 11:40–12:15 koltuk kullanımına değer");
    }

    [Fact]
    public void M9_ResourceDowntime_IsNotACandidate()
    {
        var room = new BranchResource(Guid.Parse("0193a8f0-0000-7000-8000-0000000000f1"), Guid.Parse("0193a8f0-0000-7000-8000-0000000000f0"), 1, IsExclusive: true);
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Resource(room).Need(Offers.HaircutId, new ResourceNeed(room.ResourceTypeId, 1));
        s.ResourceDowntime(room.Id, s.At(14, 0), s.At(16, 0));

        AvailabilityEngine.Evaluate(s.Build(), Panel(HaircutElif), s.At(14, 30)).Violations.ShouldBe(SlotViolation.ResourceUnavailable);
        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(16, 0)).ShouldBeTrue();
    }

    [Fact]
    public void M10_Criterion8_MinNotice120At1005_FirstCandidateIs1215()
    {
        var s = new Scenario { MinNotice = 120 }.Offer(Offers.Haircut(Elif));
        s.NowAt(s.At(10, 5));

        var day = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday);

        day.Slots[0].LocalTime.ShouldBe(new LocalTime(12, 15));
        day.UnavailableTimes.ShouldContain(new LocalTime(12, 0));
        AvailabilityEngine.Evaluate(s.Build(), Request(HaircutElif), s.At(12, 0)).Violations.ShouldBe(SlotViolation.BeforeMinNotice);
        AvailabilityEngine.IsSlotAvailable(s.Build(), Panel(HaircutElif), s.At(10, 15)).ShouldBeTrue("panelde pencere yok");
    }

    [Fact]
    public void M10_ServiceMinNotice_OverridesTheBrandValue()
    {
        var s = new Scenario { MinNotice = 120 }.Offer(Offers.Haircut(Elif), minNotice: 30);
        s.NowAt(s.At(10, 5));

        AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday).Slots[0].LocalTime.ShouldBe(new LocalTime(10, 45));
    }

    [Fact]
    public void M11_Criterion8_MaxAdvance60_The61stDayIsEmpty()
    {
        var s = new Scenario().Staff(Elif, hours: Week(new LocalTime(10, 0), new LocalTime(20, 0), sunday: true))
            .BranchDays(Week(new LocalTime(10, 0), new LocalTime(20, 0), sunday: true))
            .Offer(Offers.Haircut(Elif));
        var today = s.Now.InZone(s.Zone).Date;

        var days = AvailabilityEngine.GetDays(s.Build(), Request(HaircutElif), today.PlusDays(59), today.PlusDays(61));

        days.Select(d => d.HasAvailability).ShouldBe([true, true, false]);
        days[2].IsClosed.ShouldBeFalse();
        AvailabilityEngine.Evaluate(s.Build(), Request(HaircutElif), s.At(today.PlusDays(61), 10, 0)).Violations.ShouldBe(SlotViolation.BeyondMaxAdvance);
        AvailabilityEngine.GetDays(s.Build(), Panel(HaircutElif), today.PlusDays(61), today.PlusDays(61))[0].HasAvailability.ShouldBeTrue();
    }

    [Fact]
    public void M12_TwoItemsDifferentStaff_SecondStartsWhenFirstStaffBusyPartEnds()
    {
        var s = new Scenario().Offer(Offers.Color(Elif)).Offer(Offers.BlowDry(Zeynep));
        var request = Request(new RequestedItem(Offers.ColorId, Elif), new RequestedItem(Offers.BlowDryId, Zeynep));

        var slot = AvailabilityEngine.Evaluate(s.Build(), request, s.At(10, 0));

        slot.IsAvailable.ShouldBeTrue();
        slot.Candidate!.Items.Select(i => (i.StaffId, i.Plan.StartUtc, i.Plan.EndUtc)).ShouldBe(
        [
            (Elif, s.At(10, 0), s.At(12, 0)),
            (Zeynep, s.At(12, 0), s.At(12, 30)),
        ]);

        s.Busy(Zeynep, s.At(12, 15), s.At(12, 45));
        AvailabilityEngine.IsSlotAvailable(s.Build(), request, s.At(10, 0)).ShouldBeFalse();
    }

    [Fact]
    public void M12_TwoItemsAnyStaff_AssignsEachItem()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Offer(Offers.BlowDry(Zeynep));
        s.Busy(Elif, s.At(10, 0), s.At(11, 0));

        var slots = AvailabilityEngine.GetSlots(s.Build(), Request(new RequestedItem(Offers.HaircutId), new RequestedItem(Offers.BlowDryId)), Tuesday);

        slots.Slots[0].LocalTime.ShouldBe(new LocalTime(11, 0));
        slots.Slots[0].Items.Select(i => i.StaffId).ShouldBe([Elif, Zeynep]);
        slots.Slots[0].Items[1].Plan.StartUtc.ShouldBe(s.At(11, 45));
    }

    [Fact]
    public void M13_SameStaffForAll_OnlyStaffWhoProvidesBothServices()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Offer(Offers.Haircut(Zeynep)).Offer(Offers.BlowDry(Elif));
        var items = new[] { new RequestedItem(Offers.HaircutId), new RequestedItem(Offers.BlowDryId) };
        var same = AvailabilityRequest.Create(items, sameStaffForAll: true, AvailabilityMode.Public).Value;

        var slot = AvailabilityEngine.GetSlots(s.Build(), same, Tuesday).Slots[0];
        slot.Items.Select(i => i.StaffId).ShouldBe([Elif, Elif]);
        slot.Items[1].Plan.StartUtc.ShouldBe(slot.Start + Duration.FromMinutes(45));

        s.Busy(Elif, s.At(0, 0), s.At(Tuesday.PlusDays(1), 0, 0));
        AvailabilityEngine.GetSlots(s.Build(), same, Tuesday).Slots.ShouldBeEmpty("Zeynep fönü vermiyor");
    }

    [Fact]
    public void M14_Criterion6_AnyStaff_EqualOccupancy_PicksLowestStaffId()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Offer(Offers.Haircut(Zeynep));
        var any = Request(new RequestedItem(Offers.HaircutId));

        var slot = AvailabilityEngine.Evaluate(s.Build(), any, s.At(14, 0)).Candidate!;

        // Elif'in kimliği Zeynep'inkinden küçük (uuid sırası).
        Elif.CompareTo(Zeynep).ShouldBeLessThan(0);
        slot.Items.ShouldHaveSingleItem().StaffId.ShouldBe(Elif);
        AvailabilityEngine.Evaluate(s.Build(), any, s.At(14, 0)).Candidate!.Items[0].StaffId.ShouldBe(Elif, "deterministik");
    }

    [Fact]
    public void M14_AnyStaff_LowerOccupancyWins_ThenPreferredStaff()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Offer(Offers.Haircut(Zeynep));
        var any = Request(new RequestedItem(Offers.HaircutId));
        var preferringZeynep = any with { PreferredStaffId = Zeynep };

        AvailabilityEngine.Evaluate(s.Build(), preferringZeynep, s.At(14, 0)).Candidate!.Items[0].StaffId.ShouldBe(Zeynep);

        s.Busy(Zeynep, s.At(10, 0), s.At(11, 0));
        AvailabilityEngine.Evaluate(s.Build(), preferringZeynep, s.At(14, 0)).Candidate!.Items[0].StaffId.ShouldBe(Elif, "Zeynep o gün daha dolu");
        AvailabilityEngine.Evaluate(s.Build(), any, s.At(10, 0)).Candidate!.Items[0].StaffId.ShouldBe(Elif);
    }

    [Fact]
    public void M14_AnyStaff_OnlineClosedStaffIsNotAssignedOnline_ButIsInPanel()
    {
        var s = new Scenario().Staff(Elif, online: false).Offer(Offers.Haircut(Elif)).Offer(Offers.Haircut(Zeynep));

        AvailabilityEngine.Evaluate(s.Build(), Request(new RequestedItem(Offers.HaircutId)), s.At(14, 0)).Candidate!.Items[0].StaffId.ShouldBe(Zeynep);
        AvailabilityEngine.Evaluate(s.Build(), Panel(new RequestedItem(Offers.HaircutId)), s.At(14, 0)).Candidate!.Items[0].StaffId.ShouldBe(Elif);
    }

    [Fact]
    public void M15_StaffSpecificDuration_PlansThatLength()
    {
        var keratin = new ItemOffer(Offers.HaircutId, Elif, "Keratin", Money.Try(150_000), Money.Zero(Currency.TRY), 0, 0, [new PlannedStage("Keratin", 75, true)]);
        var s = new Scenario().Offer(keratin);

        var slot = AvailabilityEngine.Evaluate(s.Build(), Request(HaircutElif), s.At(18, 45));

        slot.IsAvailable.ShouldBeTrue();
        slot.Candidate!.Items[0].Plan.EndUtc.ShouldBe(s.At(20, 0));
        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(19, 0)).ShouldBeFalse();
    }

    [Fact]
    public void M16_BranchDurationOverride_PlansTheOverriddenLength()
    {
        // Şube ezmesi Catalog'da çözülür (IServiceOffers); motor çözülmüş aşamayı kullanır.
        var s = new Scenario().Offer(Offers.Haircut(Elif) with { Stages = [new PlannedStage("Saç kesimi", 50, true)] });

        AvailabilityEngine.Evaluate(s.Build(), Request(HaircutElif), s.At(10, 0)).Candidate!.Items[0].Plan.EndUtc.ShouldBe(s.At(10, 50));
        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(19, 15)).ShouldBeFalse();
    }

    [Fact]
    public void M17_ExpiredPendingApprovalHold_DoesNotBlock()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(14, 0), s.At(14, 45), holdExpires: s.Now - Duration.FromMinutes(1));

        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(14, 0)).ShouldBeTrue();

        var live = new Scenario().Offer(Offers.Haircut(Elif));
        live.Busy(Elif, live.At(14, 0), live.At(14, 45), holdExpires: live.Now + Duration.FromMinutes(30));
        AvailabilityEngine.IsSlotAvailable(live.Build(), Request(HaircutElif), live.At(14, 0)).ShouldBeFalse();
    }

    [Fact]
    public void M18_ActiveWaitlistOffer_OpenToItsOwner_ClosedToOthers()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(14, 0), s.At(14, 45), appointmentId: null, reservedFor: Customer);
        var snapshot = s.Build();

        AvailabilityEngine.IsSlotAvailable(snapshot, Request(HaircutElif) with { CustomerId = Customer }, s.At(14, 0)).ShouldBeTrue();
        AvailabilityEngine.IsSlotAvailable(snapshot, Request(HaircutElif), s.At(14, 0)).ShouldBeFalse();
        AvailabilityEngine.IsSlotAvailable(snapshot, Request(HaircutElif) with { CustomerId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c2") }, s.At(14, 0)).ShouldBeFalse();
    }

    [Fact]
    public void M19_LunchBreakBetweenTwoWorkingIntervals_IsNotACandidate()
    {
        var hours = Week(new LocalTime(10, 0), new LocalTime(13, 0));
        foreach (var day in hours.Keys.ToList())
        {
            hours[day] = [new LocalTimeRange(new LocalTime(10, 0), new LocalTime(13, 0)), new LocalTimeRange(new LocalTime(13, 45), new LocalTime(20, 0))];
        }

        var s = new Scenario().Staff(Elif, hours: hours).Offer(Offers.Haircut(Elif));
        var times = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday).Slots.Select(x => x.LocalTime).ToList();

        times.ShouldContain(new LocalTime(12, 15));
        times.ShouldNotContain(new LocalTime(12, 30));
        times.ShouldNotContain(new LocalTime(13, 0));
        times.ShouldNotContain(new LocalTime(13, 30));
        times.ShouldContain(new LocalTime(13, 45));
        AvailabilityEngine.Evaluate(s.Build(), Panel(HaircutElif), s.At(13, 0)).Violations.ShouldBe(SlotViolation.OutsideWorkingHours);
    }

    [Fact]
    public void M20_Criterion18_BerlinSpringForwardDay_Local1000IsTheRightInstant()
    {
        var sunday = new LocalDate(2027, 3, 28); // 02:00 → 03:00 (CET → CEST)
        var s = new Scenario().InZone("Europe/Berlin")
            .BranchDays(Week(new LocalTime(10, 0), new LocalTime(18, 0), sunday: true))
            .Staff(Elif, hours: Week(new LocalTime(10, 0), new LocalTime(18, 0), sunday: true))
            .Offer(Offers.Haircut(Elif));
        s.NowAt(Instant.FromUtc(2027, 3, 27, 9, 0));

        var day = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), sunday);

        day.Slots[0].LocalTime.ShouldBe(new LocalTime(10, 0));
        day.Slots[0].Start.ShouldBe(Instant.FromUtc(2027, 3, 28, 8, 0));
        day.Slots[^1].Start.ShouldBe(Instant.FromUtc(2027, 3, 28, 15, 15));
        day.Slots.Count.ShouldBe(30);
        ZoneTime.Grid(sunday, s.Zone, 15).Count.ShouldBe(92, "atlanan 02:00–02:45 tekrar etmez");
        ZoneTime.Day(sunday, s.Zone).Duration.ShouldBe(Duration.FromHours(23));
    }

    [Fact]
    public void M20_BerlinFallBackDay_Local1000IsTheRightInstant()
    {
        var sunday = new LocalDate(2026, 10, 25); // 03:00 → 02:00 (CEST → CET)
        var s = new Scenario().InZone("Europe/Berlin")
            .BranchDays(Week(new LocalTime(10, 0), new LocalTime(18, 0), sunday: true))
            .Staff(Elif, hours: Week(new LocalTime(10, 0), new LocalTime(18, 0), sunday: true))
            .Offer(Offers.Haircut(Elif));
        s.NowAt(Instant.FromUtc(2026, 10, 24, 9, 0));

        var day = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), sunday);

        day.Slots[0].Start.ShouldBe(Instant.FromUtc(2026, 10, 25, 9, 0));
        ZoneTime.Day(sunday, s.Zone).Duration.ShouldBe(Duration.FromHours(25));
    }

    [Fact]
    public void Grid_IsAlignedToLocalMidnight_NotToOpeningTime()
    {
        var s = new Scenario().BranchDays(Week(new LocalTime(10, 10), new LocalTime(20, 0))).Offer(Offers.Haircut(Elif));

        AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday).Slots[0].LocalTime.ShouldBe(new LocalTime(10, 15));
        AvailabilityEngine.Evaluate(s.Build(), Request(HaircutElif), s.At(10, 20)).Violations.ShouldBe(SlotViolation.OffGrid);
        AvailabilityEngine.IsSlotAvailable(s.Build(), Panel(HaircutElif), s.At(10, 20)).ShouldBeTrue("panel ızgaraya bağlı değil");
    }

    [Fact]
    public void Criterion7_FullDay_HasNoDot_AndNextDayFirstSlotIsReported()
    {
        var thursday = new LocalDate(2026, 11, 26);
        var friday = thursday.PlusDays(1);
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(thursday, 10, 0), s.At(thursday, 20, 0)).Busy(Elif, s.At(friday, 10, 0), s.At(friday, 10, 30));

        var days = AvailabilityEngine.GetDays(s.Build(), Request(HaircutElif), thursday, friday);

        days[0].HasAvailability.ShouldBeFalse();
        days[0].IsClosed.ShouldBeFalse();
        days[1].FirstSlot!.LocalTime.ShouldBe(new LocalTime(10, 30));
    }

    [Fact]
    public void GetDays_SundayClosed_IsReportedClosed()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));

        var days = AvailabilityEngine.GetDays(s.Build(), Request(HaircutElif), new LocalDate(2026, 11, 28), new LocalDate(2026, 11, 30));

        days.Select(d => (d.IsClosed, d.HasAvailability)).ShouldBe([(false, true), (true, false), (false, true)]);
    }

    [Fact]
    public void Panel_OutsideWorkingHours_IsAWarningWithACandidate()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));

        var evaluation = AvailabilityEngine.Evaluate(s.Build(), Panel(HaircutElif), s.At(20, 0));

        evaluation.Violations.ShouldBe(SlotViolation.OutsideWorkingHours);
        evaluation.Candidate.ShouldNotBeNull();
        evaluation.IsAvailable.ShouldBeFalse();
        evaluation.IsAllowed(allowOutsideWorkingHours: false).ShouldBeFalse();
        evaluation.IsAllowed(allowOutsideWorkingHours: true).ShouldBeTrue();
    }

    [Fact]
    public void Panel_Conflict_IsNeverAllowed()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(19, 30), s.At(20, 30));

        var evaluation = AvailabilityEngine.Evaluate(s.Build(), Panel(HaircutElif), s.At(20, 0));

        evaluation.Violations.ShouldBe(SlotViolation.StaffBusy | SlotViolation.OutsideWorkingHours);
        evaluation.IsAllowed(allowOutsideWorkingHours: true).ShouldBeFalse();
    }

    [Fact]
    public void Public_OnlineClosedStaffOrService_IsNotBookable_PanelIs()
    {
        var s = new Scenario().Staff(Elif, online: false).Offer(Offers.Haircut(Elif));

        AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday).Slots.ShouldBeEmpty();
        AvailabilityEngine.Evaluate(s.Build(), Request(HaircutElif), s.At(14, 0)).Violations.ShouldBe(SlotViolation.NotBookableOnline);
        AvailabilityEngine.GetSlots(s.Build(), Panel(HaircutElif), Tuesday).Slots.ShouldNotBeEmpty();

        var offline = new Scenario().Offer(Offers.Haircut(Elif), online: false);
        AvailabilityEngine.Evaluate(offline.Build(), Request(HaircutElif), offline.At(14, 0)).Violations.ShouldBe(SlotViolation.NotBookableOnline);
    }

    [Fact]
    public void StaffWhoDoesNotProvideTheService_IsNoEligibleStaff()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Staff(Zeynep);

        AvailabilityEngine.Evaluate(s.Build(), Panel(new RequestedItem(Offers.HaircutId, Zeynep)), s.At(14, 0)).Violations.ShouldBe(SlotViolation.NoEligibleStaff);
    }

    [Fact]
    public void AnyStaff_NoOneFree_IsNoStaffAvailable()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(14, 0), s.At(15, 0));

        AvailabilityEngine.Evaluate(s.Build(), Request(new RequestedItem(Offers.HaircutId)), s.At(14, 0)).Violations.ShouldBe(SlotViolation.NoStaffAvailable);
    }

    [Fact]
    public void Reschedule_OwnOldSegments_DoNotBlockTheNewTime()
    {
        var appointmentId = Guid.Parse("0193a8f0-0000-7000-8000-0000000000a9");
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(14, 0), s.At(14, 45), appointmentId: appointmentId);

        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif), s.At(14, 30)).ShouldBeFalse();
        AvailabilityEngine.IsSlotAvailable(s.Build(), Request(HaircutElif) with { IgnoreAppointmentId = appointmentId }, s.At(14, 30)).ShouldBeTrue();
    }

    [Fact]
    public void Candidate_ToPlan_CarriesItemTimesAndPrices()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif)).Offer(Offers.BlowDry(Elif));
        var request = Request(HaircutElif, new RequestedItem(Offers.BlowDryId, Elif));

        var plan = AvailabilityEngine.Evaluate(s.Build(), request, s.At(14, 30)).Candidate!.ToPlan();

        plan.StartUtc.ShouldBe(s.At(14, 30));
        plan.EndUtc.ShouldBe(s.At(15, 45));
        plan.Total.ShouldBe(Money.Try(70_000));
        plan.Items.Select(i => i.SeqNo).ShouldBe([1, 2]);
    }

    [Fact]
    public void UnavailableTimes_ListOpenGridTimesWithoutCandidate()
    {
        var s = new Scenario().Offer(Offers.Haircut(Elif));
        s.Busy(Elif, s.At(10, 0), s.At(19, 0));

        var day = AvailabilityEngine.GetSlots(s.Build(), Request(HaircutElif), Tuesday);

        day.Slots.Select(x => x.LocalTime).ShouldBe([new LocalTime(19, 0), new LocalTime(19, 15)]);
        day.UnavailableTimes.Count.ShouldBe(38);
        day.UnavailableTimes[0].ShouldBe(new LocalTime(10, 0));
    }

    [Fact]
    public void Request_Validation()
    {
        AvailabilityRequest.Create([], false, AvailabilityMode.Public).Error!.Code.ShouldBe(ErrorCodes.ValidationFailed);
        AvailabilityRequest.Create([.. Enumerable.Repeat(HaircutElif, 6)], false, AvailabilityMode.Public).Error!.FieldErrors["items"].ShouldBe(["out_of_range"]);
        // staff_mismatch yalnız validation_failed içindeki alan kodudur; üst düzey ErrorCode değildir.
        var mismatch = AvailabilityRequest.Create([HaircutElif, new RequestedItem(Offers.BlowDryId, Zeynep)], true, AvailabilityMode.Public).Error!;
        mismatch.Code.ShouldBe(ErrorCodes.ValidationFailed);
        mismatch.FieldErrors["items[1].staffId"].ShouldBe(["staff_mismatch"]);
        AvailabilityRequest.Create([HaircutElif, new RequestedItem(Offers.BlowDryId)], true, AvailabilityMode.Public).IsSuccess.ShouldBeTrue();
    }
}
