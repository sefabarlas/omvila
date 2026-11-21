using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Slots;

/// <summary>Sözleşme Slot: arayüzdeki tek saat ilk kalemin başlangıcıdır.</summary>
internal sealed record Slot(Instant StartUtc, string LocalTime, string EndLocalTime, string Period, IReadOnlyList<SlotItem> Items, Money Total)
{
    /// <summary>Sözleşme Slot.period: Sabah &lt; 12:00 ≤ Öğleden sonra &lt; 17:00 ≤ Akşam.</summary>
    public static string PeriodOf(LocalTime time) => time.Hour < 12 ? "Morning" : time.Hour < 17 ? "Afternoon" : "Evening";
}
