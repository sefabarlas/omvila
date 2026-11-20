using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Kalemleri zincirler (musaitlik.md § Çok kalemli randevu): kalem n+1, kalem n'in son personel-meşgul diliminin
/// bitişinde başlar; kalemler arası boşluk 0 dk. Arayüzdeki tek "saat" ilk kalemin başlangıcıdır. Müsaitlik ve
/// çakışma burada denetlenmez (motor ve yazma yolu).
/// </summary>
internal static class AppointmentPlanner
{
    public static Result<AppointmentPlan> Plan(Instant start, IReadOnlyList<ItemOffer> offers)
    {
        ArgumentNullException.ThrowIfNull(offers);
        if (offers.Count == 0)
        {
            return Error.Validation("items", "required");
        }

        if (offers.Count > PlatformDefaults.MaxItemsPerAppointment)
        {
            return Error.Validation("items", "out_of_range");
        }

        if (offers.Select(o => o.Price.Currency).Distinct().Count() > 1)
        {
            throw new DomainException("Bir randevunun kalemleri aynı para biriminde olmalı.");
        }

        var items = new List<ItemPlan>(offers.Count);
        var cursor = start;
        foreach (var offer in offers)
        {
            var item = ItemTemplate.From(offer).At(items.Count + 1, cursor);
            items.Add(item);
            cursor = item.StaffBusyEndUtc;
        }

        return new AppointmentPlan(items);
    }
}
