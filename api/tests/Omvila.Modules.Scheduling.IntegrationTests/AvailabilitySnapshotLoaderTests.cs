using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.TestKit;
using static Omvila.Modules.Scheduling.IntegrationTests.AvailabilityData;

namespace Omvila.Modules.Scheduling.IntegrationTests;

// Kural: musaitlik.md § Algoritma 1–5 (şube saati, özel gün, personel saati, bütün şubelerdeki izin ve meşguliyet),
// § Performans hedefi (tek sorgu turu, N+1 testi); dilim-02 kabul kriterleri 5 ve 16 gerçek veriyle.
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class AvailabilitySnapshotLoaderTests(PostgresFixture db) : IAsyncLifetime
{
    private readonly CommandCounter _counter = new();
    private ServiceProvider _services = null!;
    private AvailabilityData _data = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        _services = AvailabilityData.CreateServices(db, _counter);
        _data = await AvailabilityData.SeedAsync(db, _services, Ct);
        await _data.AddAppointmentsAsync(
        [
            // Mert Nişantaşı'nda perşembe 10:30–11:15 ve 11:15–12:00.
            (_data.Mert, _data.Nisantasi, At(Thursday, 10, 30)),
            (_data.Mert, _data.Nisantasi, At(Thursday, 11, 15)),
            (_data.Elif, _data.Merkez, At(Thursday, 14, 0)),
        ], Ct);
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task Loads_BranchHours_Overrides_Staff_TimeOffs_Offers_AndBusyAcrossBranches()
    {
        var snapshot = (await LoadAsync(Thursday, Sunday, [_data.Haircut, _data.Color], null))!;

        snapshot.Zone.Id.ShouldBe("Europe/Istanbul");
        snapshot.Now.ShouldBe(TestClock.Start);
        snapshot.Settings.ShouldBe(new AvailabilitySettings(120, 60, 15));
        snapshot.BranchHours.RangesOn(Thursday).ShouldBe([new LocalTimeRange(new LocalTime(10, 0), new LocalTime(20, 0))]);
        snapshot.BranchHours.RangesOn(Sunday).ShouldBe([new LocalTimeRange(new LocalTime(11, 0), new LocalTime(16, 0))], "Yalnızca bu gün için aç");
        snapshot.Staff.Keys.ShouldNotContain(_data.InactiveStaff, "şubedeki kaydı pasif");
        snapshot.Staff.Count.ShouldBe(StaffCount + 1, "12 personel + sahip");

        var mert = snapshot.Staff[_data.Mert];
        mert.Busy.Select(b => (b.BranchId, b.During)).ShouldBe(
        [
            (_data.Nisantasi, new Interval(At(Thursday, 10, 30), At(Thursday, 11, 15))),
            (_data.Nisantasi, new Interval(At(Thursday, 11, 15), At(Thursday, 12, 0))),
        ], ignoreOrder: true);
        snapshot.Staff[_data.Elif].TimeOffs.ShouldBe([new Interval(At(Thursday.PlusDays(1), 0, 0), At(Thursday.PlusDays(2), 0, 0))]);

        snapshot.Services[_data.Haircut].OffersByStaff.Count.ShouldBe(StaffCount, "sahip hizmeti vermiyor");
        var color = snapshot.Services[_data.Color];
        color.MinNoticeMinutes.ShouldBe(30);
        color.OffersByStaff.Keys.ShouldBe([_data.Elif]);
        color.OffersByStaff[_data.Elif].Stages.Select(s => (s.DurationMinutes, s.StaffBusy)).ShouldBe([(45, true), (40, false), (45, true)], "özel süre 130 son personel-meşgul aşamaya");
        snapshot.Transfers.Between(_data.Nisantasi, _data.Merkez).ShouldBe(Duration.FromMinutes(30));
    }

    [Fact]
    public async Task Criterion5_RealData_BusyInTheOtherBranchUntilNoon_FirstCandidateIs1230()
    {
        var snapshot = (await LoadAsync(Thursday, Thursday, [_data.Haircut], [_data.Mert]))!;
        var request = AvailabilityRequest.Create([new RequestedItem(_data.Haircut, _data.Mert)], false, AvailabilityMode.Public).Value;

        AvailabilityEngine.GetSlots(snapshot, request, Thursday).Slots[0].LocalTime.ShouldBe(new LocalTime(12, 30));
    }

    [Fact]
    public async Task Criterion16_RealData_SundayOpenedByOverride_ElifOnTimeOffFriday()
    {
        var snapshot = (await LoadAsync(Thursday, Sunday, [_data.Haircut], null))!;
        var elif = AvailabilityRequest.Create([new RequestedItem(_data.Haircut, _data.Elif)], false, AvailabilityMode.Public).Value;

        var days = AvailabilityEngine.GetDays(snapshot, elif, Thursday, Sunday);

        // Elif'in pazar saati yok; "Yalnızca bu gün için aç" (11:00–16:00) onu da açar (M7). Cuma izinli.
        days.Select(d => (d.IsClosed, d.HasAvailability)).ShouldBe([(false, true), (false, false), (false, true), (false, true)]);
        days[0].FirstSlot!.LocalTime.ShouldBe(new LocalTime(10, 0));
        days[3].FirstSlot!.LocalTime.ShouldBe(new LocalTime(11, 0));
    }

    [Fact]
    public async Task AnyStaff_OnLoadedSnapshot_AvoidsBusyStaffAndAssignsDeterministically()
    {
        var snapshot = (await LoadAsync(Thursday, Thursday, [_data.Haircut], null))!;
        var any = AvailabilityRequest.Create([new RequestedItem(_data.Haircut)], false, AvailabilityMode.Public).Value;

        var slot = AvailabilityEngine.Evaluate(snapshot, any, At(Thursday, 14, 0));

        slot.IsAvailable.ShouldBeTrue();
        var assigned = slot.Candidate!.Items[0].StaffId;
        assigned.ShouldNotBe(_data.Elif, "Elif 14:00'te dolu");
        assigned.ShouldNotBe(_data.Mert, "Mert'in o gün meşguliyeti var");
        assigned.ShouldBe(_data.Staff.Skip(2).Min());
    }

    [Fact]
    public async Task StaffFilter_AndUnknownBranch()
    {
        (await LoadAsync(Thursday, Thursday, [_data.Haircut], [_data.Elif]))!.Staff.Keys.ShouldBe([_data.Elif]);
        (await _data.InBrandAsync(
            sp => sp.GetRequiredService<AvailabilitySnapshotLoader>().LoadAsync(_data.BrandId, Guid.Parse("0193a8f0-0000-7000-8000-0000000000ff"), Thursday, Thursday, [_data.Haircut], null, Ct),
            Ct)).ShouldBeNull();
    }

    [Fact]
    public async Task QueryCount_IsIndependentOfDaysStaffAndServices()
    {
        await LoadAsync(Thursday, Thursday, [_data.Haircut], [_data.Elif]);
        _counter.Reset();
        await LoadAsync(Thursday, Thursday, [_data.Haircut], [_data.Elif]);
        var small = _counter.Count;

        _counter.Reset();
        await LoadAsync(Thursday, Thursday.PlusDays(13), [_data.Haircut, _data.Color], null);
        var large = _counter.Count;

        TestContext.Current.TestOutputHelper?.WriteLine($"Sorgu sayısı: küçük {small}, büyük {large}");
        large.ShouldBe(small, "döngüde sorgu yok (N+1)");
        large.ShouldBeLessThanOrEqualTo(12, "Tenancy 6 + Catalog 4 + Scheduling 2 (meşguliyet, bekleme teklifi tutmaları; T-90)");
    }

    private Task<AvailabilitySnapshot?> LoadAsync(LocalDate from, LocalDate to, IReadOnlyCollection<Guid> services, IReadOnlyCollection<Guid>? staff) =>
        _data.InBrandAsync(sp => sp.GetRequiredService<AvailabilitySnapshotLoader>().LoadAsync(_data.BrandId, _data.Merkez, from, to, services, staff, Ct), Ct);
}
