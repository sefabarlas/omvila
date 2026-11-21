using NodaTime;
using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Slots;

/// <summary>Motor adayı → sözleşme Slot (panel ve rezervasyon sayfası aynı biçimi kullanır).</summary>
internal static class SlotMapping
{
    public static SlotSearchResult ToResult(DaySlots day, DateTimeZone zone, IReadOnlyDictionary<Guid, StaffCard> staff) => new(
        zone.Id,
        day.Date,
        [.. day.Slots.Select(s => ToSlot(s, zone, staff))],
        [.. day.UnavailableTimes.Select(TimeIntervalView.Format)]);

    public static Slot ToSlot(SlotCandidate candidate, DateTimeZone zone, IReadOnlyDictionary<Guid, StaffCard> staff)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        var items = candidate.Items.Select(i => new SlotItem(
            i.ServiceId, i.StaffId, staff.GetValueOrDefault(i.StaffId)?.FullName, i.Plan.StartUtc, i.Plan.EndUtc, i.Plan.Offer.Price)).ToList();
        var currency = items.Count > 0 ? items[0].Price.Currency : PlatformDefaults.DefaultCurrency;
        var total = new Money(items.Sum(i => i.Price.AmountMinor), currency);
        var end = candidate.Items.Max(i => i.Plan.EndUtc).InZone(zone).TimeOfDay;
        return new Slot(
            candidate.Start, TimeIntervalView.Format(candidate.LocalTime), TimeIntervalView.Format(end), Slot.PeriodOf(candidate.LocalTime), items, total);
    }
}
