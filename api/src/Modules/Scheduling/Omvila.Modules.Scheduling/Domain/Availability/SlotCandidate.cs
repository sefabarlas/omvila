using NodaTime;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>
/// Aday saat: arayüzdeki tek "saat" ilk kalemin başlangıcıdır; her kalemin personeli ve [başlangıç, bitiş) aralığı
/// <see cref="Items"/>'tadır. Yazma yolu aynı başlangıcı kilit altında <see cref="AvailabilityEngine.Evaluate"/> ile yeniden
/// doğrular ve <see cref="ToPlan"/> ile randevuyu oluşturur.
/// </summary>
internal sealed record SlotCandidate(Instant Start, LocalTime LocalTime, IReadOnlyList<AssignedItem> Items)
{
    public AppointmentPlan ToPlan() => new([.. Items.Select(i => i.Plan)]);
}
