using Omvila.Modules.Catalog.Contracts;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>Catalog teklifini alan girdisine çevirir (alan katmanı Catalog sözleşmesine bağlı değildir).</summary>
internal static class OfferMapping
{
    public static ItemOffer ToItemOffer(this ServiceOffer offer)
    {
        ArgumentNullException.ThrowIfNull(offer);
        return new ItemOffer(
            offer.ServiceId,
            offer.StaffId,
            offer.ServiceName,
            offer.Price,
            offer.StaffSurcharge,
            offer.BufferBeforeMinutes,
            offer.BufferAfterMinutes,
            [.. offer.Stages.Select(s => new PlannedStage(s.Name, s.DurationMinutes, s.StaffBusy))]);
    }
}
