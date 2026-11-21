using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Hizmet priminin tabanı (alan-modeli.md § CommissionEntry: "Taban, kalemin tahsil edilen tutarıdır"; docs/acik-sorular.md
/// T-88, W2). Randevuda tahsil edilen tutar (salonda alınanlar + salonda kalan kapora) kalemlere tutarları oranında dağıtılır;
/// böylece kapora ve indirim kalemlere fiyat oranında yayılır. Kuruş artığı en büyük kalan yöntemiyle verilir (eşitlikte
/// önceki kalem), toplam her zaman dağıtılan tutara eşittir. Tahsil edilen, kalemlerin toplamını aşarsa (kapora toplamdan
/// büyükse) taban kalem tutarında kalır.
/// </summary>
internal static class CommissionBasis
{
    // Kural: alan-modeli.md § CommissionEntry; docs/acik-sorular.md W2
    public static long[] Distribute(IReadOnlyList<long> itemAmountsMinor, long collectedMinor)
    {
        ArgumentNullException.ThrowIfNull(itemAmountsMinor);
        if (collectedMinor < 0 || itemAmountsMinor.Any(a => a < 0))
        {
            throw new DomainException("Prim tabanı negatif tutarla hesaplanamaz.");
        }

        var total = itemAmountsMinor.Sum();
        var bases = new long[itemAmountsMinor.Count];
        if (total == 0)
        {
            return bases;
        }

        var collected = Math.Min(collectedMinor, total);
        var remainders = new long[bases.Length];
        for (var i = 0; i < bases.Length; i++)
        {
            var share = (Int128)collected * itemAmountsMinor[i];
            bases[i] = (long)(share / total);
            remainders[i] = (long)(share % total);
        }

        var left = (int)(collected - bases.Sum());
        foreach (var i in Enumerable.Range(0, bases.Length).OrderByDescending(i => remainders[i]).ThenBy(i => i).Take(left))
        {
            bases[i]++;
        }

        return bases;
    }
}
