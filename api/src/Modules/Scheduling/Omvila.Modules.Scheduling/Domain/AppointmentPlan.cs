using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>Randevunun kalem planları (zincirlenmiş) ve toplamı.</summary>
internal sealed record AppointmentPlan(IReadOnlyList<ItemPlan> Items)
{
    public Instant StartUtc => Items.Min(i => i.StartUtc);

    public Instant EndUtc => Items.Max(i => i.EndUtc);

    /// <summary>Kalem fiyatlarının toplamı (personel ek ücreti fiyatın içinde; indirim oluşturmada yok).</summary>
    public Money Total => Items.Skip(1).Aggregate(Items[0].Offer.Price, (sum, item) => sum + item.Offer.Price);
}
