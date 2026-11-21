namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Tahmini bekleme (sureler-ve-ayarlar.md WalkInAverageServiceMinutes: "öndekilerin kalan süreleri / müsait personel"). Sırada
/// bekleyenler henüz başlamadığından kalan süre hizmet süresidir; hizmeti seçilmemiş biletin süresi bugünkü biletlerin
/// ortalamasıdır (T-91). Müsait personel yoksa veya öndekilerden birinin süresi bilinmiyorsa tahmin yoktur (null).
/// </summary>
internal static class WalkInWaitEstimator
{
    /// <param name="minutesInOrder">Sıradakilerin (numara sırasıyla) hizmet süreleri; hizmetsiz bilette null.</param>
    /// <param name="fallbackMinutes">Hizmetsiz bilet için süre; yoksa null.</param>
    /// <param name="availableStaff">Müsait personel sayısı.</param>
    /// <returns>Her biletin beklemesi ve yeni gelenin beklemesi (dakika, yukarı yuvarlanır).</returns>
    public static (IReadOnlyList<int?> PerTicket, int? ForNew) Estimate(IReadOnlyList<int?> minutesInOrder, int? fallbackMinutes, int availableStaff)
    {
        ArgumentNullException.ThrowIfNull(minutesInOrder);
        if (availableStaff <= 0)
        {
            return ([.. minutesInOrder.Select(_ => (int?)null)], null);
        }

        var waits = new List<int?>(minutesInOrder.Count);
        int? ahead = 0;
        foreach (var minutes in minutesInOrder)
        {
            waits.Add(Divide(ahead, availableStaff));
            ahead = ahead is { } sum && (minutes ?? fallbackMinutes) is { } own ? sum + own : null;
        }

        return (waits, Divide(ahead, availableStaff));
    }

    private static int? Divide(int? minutes, int staff) => minutes is { } m ? (m + staff - 1) / staff : null;
}
