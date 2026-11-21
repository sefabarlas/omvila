using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.Modules.Scheduling.Domain.Availability;

namespace Omvila.Modules.Scheduling.Tests.Availability;

/// <summary>
/// Müsaitlik anlık görüntüsü kurucusu (tasarım verisi: Kuaför Elif, Kadıköy ve Nişantaşı). Varsayılan: şube pazartesi–
/// cumartesi 10:00–20:00, pazar kapalı; personel şube saatlerinde çalışır; şimdi Pzt 23 Kas 2026 08:00; MinNotice 0,
/// MaxAdvance 60, ızgara 15 dk, geçiş 30 dk.
/// </summary>
internal sealed class Scenario
{
    public static readonly Guid Kadikoy = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b1");
    public static readonly Guid Nisantasi = Guid.Parse("0193a8f0-0000-7000-8000-0000000000b2");
    public static readonly Guid Elif = Offers.Elif;
    public static readonly Guid Zeynep = Offers.Zeynep;
    public static readonly Guid Mert = Guid.Parse("0193a8f0-0000-7000-8000-00000000e13b");
    public static readonly Guid Customer = Guid.Parse("0193a8f0-0000-7000-8000-0000000000c1");

    /// <summary>Salı 24 Kasım 2026.</summary>
    public static readonly LocalDate Tuesday = new(2026, 11, 24);

    private readonly Dictionary<Guid, StaffEntry> _staff = [];
    private readonly Dictionary<Guid, ServiceEntry> _services = [];
    private readonly Dictionary<LocalDate, IReadOnlyList<LocalTimeRange>> _branchOverrides = [];
    private readonly List<BranchResource> _resources = [];
    private readonly List<ResourceBlock> _downtimes = [];
    private readonly List<ResourceBlock> _usages = [];
    private Dictionary<IsoDayOfWeek, IReadOnlyList<LocalTimeRange>> _branchDays = Week(new LocalTime(10, 0), new LocalTime(20, 0));

    public Guid BranchId { get; private set; } = Kadikoy;

    public DateTimeZone Zone { get; private set; } = Offers.Istanbul;

    public Instant Now { get; private set; }

    public int MinNotice { get; set; }

    public int MaxAdvance { get; set; } = 60;

    public int SlotInterval { get; set; } = 15;

    public int TransferMinutes { get; set; } = 30;

    public Scenario() => Now = At(new LocalDate(2026, 11, 23), 8, 0);

    public static IReadOnlyList<LocalTimeRange> Range(int fromHour, int fromMinute, int toHour, int toMinute) =>
        [new LocalTimeRange(new LocalTime(fromHour, fromMinute), new LocalTime(toHour, toMinute))];

    /// <summary>Pazartesi–cumartesi aynı saatler (pazar kapalı).</summary>
    public static Dictionary<IsoDayOfWeek, IReadOnlyList<LocalTimeRange>> Week(LocalTime start, LocalTime end, bool sunday = false) =>
        Enum.GetValues<IsoDayOfWeek>().Where(d => d != IsoDayOfWeek.None && (sunday || d != IsoDayOfWeek.Sunday))
            .ToDictionary(d => d, _ => (IReadOnlyList<LocalTimeRange>)[new LocalTimeRange(start, end)]);

    public Instant At(LocalDate date, int hour, int minute) => ZoneTime.ToInstant(date, new LocalTime(hour, minute), Zone);

    public Instant At(int hour, int minute) => At(Tuesday, hour, minute);

    public Scenario InBranch(Guid branchId)
    {
        BranchId = branchId;
        return this;
    }

    public Scenario InZone(string zoneId)
    {
        Zone = DateTimeZoneProviders.Tzdb[zoneId];
        return this;
    }

    public Scenario NowAt(Instant now)
    {
        Now = now;
        return this;
    }

    public Scenario BranchDays(Dictionary<IsoDayOfWeek, IReadOnlyList<LocalTimeRange>> days)
    {
        _branchDays = days;
        return this;
    }

    public Scenario BranchOverride(LocalDate date, IReadOnlyList<LocalTimeRange> ranges)
    {
        _branchOverrides[date] = ranges;
        return this;
    }

    public Scenario Staff(Guid staffId, bool online = true, Dictionary<IsoDayOfWeek, IReadOnlyList<LocalTimeRange>>? hours = null)
    {
        _staff[staffId] = new StaffEntry(online, hours ?? Week(new LocalTime(10, 0), new LocalTime(20, 0)));
        return this;
    }

    public Scenario StaffOverride(Guid staffId, LocalDate date, IReadOnlyList<LocalTimeRange> ranges)
    {
        _staff[staffId].Overrides[date] = ranges;
        return this;
    }

    /// <summary>Teklif ekler; personel kaydı yoksa varsayılan saatlerle oluşturulur.</summary>
    public Scenario Offer(ItemOffer offer, bool online = true, int? minNotice = null, int? maxAdvance = null)
    {
        var staffId = offer.StaffId!.Value;
        if (!_staff.ContainsKey(staffId))
        {
            Staff(staffId);
        }

        if (!_services.TryGetValue(offer.ServiceId, out var service))
        {
            service = new ServiceEntry(online, minNotice, maxAdvance);
            _services[offer.ServiceId] = service;
        }

        service.Offers[staffId] = offer;
        return this;
    }

    public Scenario Need(Guid serviceId, ResourceNeed need)
    {
        _services[serviceId].Needs.Add(need);
        return this;
    }

    public Scenario Resource(BranchResource resource)
    {
        _resources.Add(resource);
        return this;
    }

    public Scenario ResourceUsage(Guid resourceId, Instant from, Instant to, Guid? appointmentId = null)
    {
        _usages.Add(new ResourceBlock(resourceId, new Interval(from, to), appointmentId));
        return this;
    }

    public Scenario ResourceDowntime(Guid resourceId, Instant from, Instant to)
    {
        _downtimes.Add(new ResourceBlock(resourceId, new Interval(from, to)));
        return this;
    }

    public Scenario Busy(Guid staffId, Instant from, Instant to, Guid? branchId = null, Guid? appointmentId = null, Instant? holdExpires = null, Guid? reservedFor = null)
    {
        _staff[staffId].Busy.Add(new BusySegment(appointmentId ?? Guid.Parse("0193a8f0-0000-7000-8000-0000000000a1"), branchId ?? BranchId, new Interval(from, to), holdExpires, reservedFor));
        return this;
    }

    public Scenario TimeOff(Guid staffId, Instant from, Instant to)
    {
        _staff[staffId].TimeOffs.Add(new Interval(from, to));
        return this;
    }

    public AvailabilitySnapshot Build() => new(
        BranchId,
        Zone,
        Now,
        new AvailabilitySettings(MinNotice, MaxAdvance, SlotInterval),
        new WeeklySchedule(_branchDays, new Dictionary<LocalDate, IReadOnlyList<LocalTimeRange>>(_branchOverrides)),
        _staff.ToDictionary(
            kv => kv.Key,
            kv => new StaffSchedule(kv.Key, kv.Value.Online, new WeeklySchedule(kv.Value.Hours, kv.Value.Overrides), [.. kv.Value.TimeOffs], [.. kv.Value.Busy])),
        _services.ToDictionary(
            kv => kv.Key,
            kv => new ServiceAvailability(kv.Key, kv.Value.Online, kv.Value.MinNotice, kv.Value.MaxAdvance, new Dictionary<Guid, ItemOffer>(kv.Value.Offers), [.. kv.Value.Needs])),
        TransferTimes.Uniform(TransferMinutes),
        new ResourceInventory([.. _resources], [.. _downtimes], [.. _usages]));

    public static AvailabilityRequest Request(params RequestedItem[] items) =>
        AvailabilityRequest.Create(items, sameStaffForAll: false, AvailabilityMode.Public).Value;

    public static AvailabilityRequest Panel(params RequestedItem[] items) =>
        AvailabilityRequest.Create(items, sameStaffForAll: false, AvailabilityMode.Panel).Value;

    private sealed record StaffEntry(bool Online, Dictionary<IsoDayOfWeek, IReadOnlyList<LocalTimeRange>> Hours)
    {
        public Dictionary<LocalDate, IReadOnlyList<LocalTimeRange>> Overrides { get; } = [];

        public List<Interval> TimeOffs { get; } = [];

        public List<BusySegment> Busy { get; } = [];
    }

    private sealed record ServiceEntry(bool Online, int? MinNotice, int? MaxAdvance)
    {
        public Dictionary<Guid, ItemOffer> Offers { get; } = [];

        public List<ResourceNeed> Needs { get; } = [];
    }
}
