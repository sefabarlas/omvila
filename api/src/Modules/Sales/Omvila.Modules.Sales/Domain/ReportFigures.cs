namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Raporların hesap kuralları (sözleşme ReportOverview, CommissionReport; dilim-06 kabul kriteri 6; docs/acik-sorular.md T-120).
/// </summary>
internal static class ReportFigures
{
    /// <summary>
    /// Tamamlanan kalemin hak edilen geliri: paketten düşülen kalemde paket fiyatının seans payı (seans kullanıldıkça), diğerlerinde
    /// kalem tutarı. Hizmet priminin tabanıyla aynı kuraldır; peşin paket satışı satış anında hak edilen gelire girmez.
    /// </summary>
    // Kural: dilim-06 kabul kriteri 6; alan-modeli.md § CommissionEntry ("paketten düşülen kalemde taban, paket fiyatının seans başına payıdır")
    public static long EarnedMinor(long netAmountMinor, Guid? customerPackageItemId, IReadOnlyDictionary<Guid, long> sessionShares)
    {
        ArgumentNullException.ThrowIfNull(sessionShares);
        return customerPackageItemId is { } item ? sessionShares.GetValueOrDefault(item) : netAmountMinor;
    }

    /// <summary>Gelmeyen randevunun kaybı: tahsil edilecek tutar − salonda kalan kapora, en az 0 (paket seansı düşülmüştür, kayıp değildir).</summary>
    // Kural: sözleşme ReportOverview.noShow.lostRevenue ("hizmet tutarı − alınmış kapora")
    public static long NoShowLossMinor(long payableAmountMinor, long retainedDepositMinor) => Math.Max(0, payableAmountMinor - retainedDepositMinor);

    /// <summary>Yüzde, bir ondalık (yarım yukarı); payda 0 ise null.</summary>
    public static double? Percent(long part, long whole) =>
        whole == 0 ? null : Math.Round(part * 100d / whole, 1, MidpointRounding.AwayFromZero);

    /// <summary>Önceki aralığa göre değişim yüzdesi; önceki aralıkta değer yoksa null.</summary>
    public static double? ChangePercent(long current, long previous) => previous == 0 ? null : Percent(current - previous, previous);
}
