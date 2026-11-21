using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Infrastructure.Availability;
using Omvila.TestKit;
using static Omvila.Modules.Scheduling.IntegrationTests.AvailabilityData;

namespace Omvila.Modules.Scheduling.IntegrationTests;

// Kural: musaitlik.md § Performans hedefi; dilim-02 kabul kriteri 19: 12 personelli şubede tek hizmet, 7 günlük gün
// listesi p95 < 300 ms; tek gün saat ızgarası (fark etmez) p95 < 200 ms. Ölçüm yükleme (veritabanı) + hesabı kapsar.
// Filtre: --filter-not-trait "Category=Performance".
[Trait("Category", "Integration")]
[Trait("Category", "Performance")]
[Collection(PostgresTests.Name)]
public sealed class AvailabilityPerformanceTests(PostgresFixture db) : IAsyncLifetime
{
    private const int Warmup = 5;
    private const int Runs = 40;

    private ServiceProvider _services = null!;
    private AvailabilityData _data = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        _services = AvailabilityData.CreateServices(db);
        _data = await AvailabilityData.SeedAsync(db, _services, Ct);

        // Her personele her açık gün dört randevu (personele göre kaydırılmış): 12 × 6 × 4 = 288 randevu.
        var appointments = new List<(Guid, Guid, Instant)>();
        for (var day = 0; day < 7; day++)
        {
            var date = Thursday.PlusDays(day);
            if (date.DayOfWeek == IsoDayOfWeek.Sunday)
            {
                continue;
            }

            for (var s = 0; s < StaffCount; s++)
            {
                foreach (var hour in new[] { 10, 12, 14, 17 })
                {
                    appointments.Add((_data.Staff[s], _data.Merkez, At(date, hour, 15 * (s % 4))));
                }
            }
        }

        await _data.AddAppointmentsAsync(appointments, Ct);
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task Criterion19_SevenDayList_P95Under300ms()
    {
        var request = AvailabilityRequest.Create([new RequestedItem(_data.Haircut)], false, AvailabilityMode.Public).Value;

        var p95 = await MeasureAsync("7 günlük gün listesi", async loader =>
        {
            var snapshot = (await loader.LoadAsync(_data.BrandId, _data.Merkez, Thursday, Thursday.PlusDays(6), [_data.Haircut], null, Ct))!;
            var days = AvailabilityEngine.GetDays(snapshot, request, Thursday, Thursday.PlusDays(6));
            days.Count(d => d.HasAvailability).ShouldBe(7, "pazar özel günle açık; saati olmayan personel özel gün saatine düşer (M7)");
        });

        p95.ShouldBeLessThan(300);
    }

    [Fact]
    public async Task Criterion19_SingleDayGrid_AnyStaff_P95Under200ms()
    {
        var request = AvailabilityRequest.Create([new RequestedItem(_data.Haircut)], false, AvailabilityMode.Public).Value;

        var p95 = await MeasureAsync("tek gün ızgarası (fark etmez)", async loader =>
        {
            var snapshot = (await loader.LoadAsync(_data.BrandId, _data.Merkez, Thursday, Thursday, [_data.Haircut], null, Ct))!;
            AvailabilityEngine.GetSlots(snapshot, request, Thursday).Slots.Count.ShouldBeGreaterThan(0);
        });

        p95.ShouldBeLessThan(200);
    }

    private async Task<double> MeasureAsync(string name, Func<AvailabilitySnapshotLoader, Task> run)
    {
        var samples = new List<double>(Runs);
        for (var i = 0; i < Warmup + Runs; i++)
        {
            var elapsed = await _data.InBrandAsync(
                async sp =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    await run(sp.GetRequiredService<AvailabilitySnapshotLoader>());
                    return stopwatch.Elapsed.TotalMilliseconds;
                },
                Ct);
            if (i >= Warmup)
            {
                samples.Add(elapsed);
            }
        }

        samples.Sort();
        var p50 = samples[(int)Math.Ceiling(samples.Count * 0.50) - 1];
        var p95 = samples[(int)Math.Ceiling(samples.Count * 0.95) - 1];
        TestContext.Current.TestOutputHelper?.WriteLine($"{name}: p50 {p50:F1} ms, p95 {p95:F1} ms, en çok {samples[^1]:F1} ms ({Runs} ölçüm)");
        TestContext.Current.SendDiagnosticMessage($"{name}: p50 {p50:F1} ms, p95 {p95:F1} ms, max {samples[^1]:F1} ms");
        return p95;
    }
}
