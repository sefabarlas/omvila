namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Bir personelin anlık görüntüsü.</summary>
/// <param name="StaffId">Personel.</param>
/// <param name="IsBookableOnline">"Online randevuya açık"; panel kipinde dikkate alınmaz.</param>
/// <param name="Hours">Bu şubedeki haftalık saatleri ve personel ezmeleri (musaitlik.md § 2).</param>
/// <param name="TimeOffs">Bütün şubelerdeki izinleri (§ 3).</param>
/// <param name="Busy">Bütün şubelerdeki aktif, personel-meşgul dilimleri (§ 4).</param>
internal sealed record StaffSchedule(
    Guid StaffId,
    bool IsBookableOnline,
    WeeklySchedule Hours,
    IReadOnlyList<NodaTime.Interval> TimeOffs,
    IReadOnlyList<BusySegment> Busy);
