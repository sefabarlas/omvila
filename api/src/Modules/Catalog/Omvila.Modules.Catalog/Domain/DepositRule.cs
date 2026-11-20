using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>Hizmetin kapora kuralı: mod ve tutar veya yüzde (XOR; sözleşme DepositRule).</summary>
internal sealed record DepositRule(DepositMode Mode, Money? Amount, decimal? Percent)
{
    public static DepositRule Inherit { get; } = new(DepositMode.Inherit, null, null);

    public bool HasValue => Amount is not null || Percent is not null;
}
