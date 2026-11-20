using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>
/// Fiyat ve süre çözümü (alan-modeli.md § BranchServicePrice): fiyat = şube fiyatı ?? hizmet fiyatı, + personel
/// ek ücreti; süre = personel özel süresi ?? şube süresi ?? hizmet süresi. Aşamalı hizmette fark son
/// personel-meşgul aşamaya eklenir veya çıkarılır; o aşama 5 dk'nın altına inemez.
/// </summary>
internal static class OfferResolver
{
    public static Result<ResolvedOffer> Resolve(
        Money basePrice, int baseDurationMinutes, IReadOnlyList<ServiceStage> stages,
        BranchPrice? branch, StaffServiceOverride? staff)
    {
        ArgumentNullException.ThrowIfNull(stages);
        if (branch is { IsAvailable: false })
        {
            return Error.NotFound();
        }

        var price = branch?.Price ?? basePrice;
        if (staff?.Surcharge is { } surcharge)
        {
            price += surcharge;
        }

        var duration = staff?.DurationOverrideMinutes ?? branch?.DurationOverrideMinutes ?? baseDurationMinutes;
        var adjusted = AdjustStages(stages, duration - baseDurationMinutes);
        return adjusted is null
            ? Error.Validation("durationOverrideMinutes", "out_of_range")
            : new ResolvedOffer(price, branch is not null, duration, adjusted);
    }

    /// <summary>Personel özel süresinin aşamalı hizmete uyduğunu denetler (kayıtta, "Keratin 75 dk (özel)").</summary>
    public static bool FitsStages(IReadOnlyList<ServiceStage> stages, int baseDurationMinutes, int overrideMinutes) =>
        overrideMinutes >= ServiceRules.MinDurationMinutes && AdjustStages(stages, overrideMinutes - baseDurationMinutes) is not null;

    private static List<ServiceStage>? AdjustStages(IReadOnlyList<ServiceStage> stages, int delta)
    {
        var result = stages.ToList();
        if (delta == 0 || result.Count == 0)
        {
            return result;
        }

        var last = result.FindLastIndex(s => s.StaffBusy);
        var updated = result[last].DurationMinutes + delta;
        if (updated < ServiceRules.MinDurationMinutes)
        {
            return null;
        }

        result[last] = result[last] with { DurationMinutes = updated };
        return result;
    }
}
