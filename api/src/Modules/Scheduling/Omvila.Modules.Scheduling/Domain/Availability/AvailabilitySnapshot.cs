using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Müsaitlik anlık görüntüsü (musaitlik.md giriş): motorun bütün girdisi. Veritabanı, saat ve HTTP bilgisi içermez;
/// "şimdi" <see cref="Now"/> olarak gelir. Yükleyici: <c>AvailabilitySnapshotLoader</c>.
/// </summary>
/// <param name="BranchId">Randevunun şubesi.</param>
/// <param name="Zone">Şubenin saat dilimi.</param>
/// <param name="Now">Hesap anı (IClock'tan, yükleyici verir).</param>
/// <param name="Settings">Pencere ve ızgara ayarları.</param>
/// <param name="BranchHours">Şube haftalık saatleri ve özel günleri.</param>
/// <param name="Staff">Şubede çalışan aday personel.</param>
/// <param name="Services">İstenen hizmetler.</param>
/// <param name="Transfers">Şubeler arası geçiş süreleri.</param>
/// <param name="Resources">Şube kaynakları.</param>
internal sealed record AvailabilitySnapshot(
    Guid BranchId,
    DateTimeZone Zone,
    Instant Now,
    AvailabilitySettings Settings,
    WeeklySchedule BranchHours,
    IReadOnlyDictionary<Guid, StaffSchedule> Staff,
    IReadOnlyDictionary<Guid, ServiceAvailability> Services,
    TransferTimes Transfers,
    ResourceInventory Resources)
{
    /// <summary>Şubenin saat diliminde bugün.</summary>
    public LocalDate Today => Now.InZone(Zone).Date;
}
