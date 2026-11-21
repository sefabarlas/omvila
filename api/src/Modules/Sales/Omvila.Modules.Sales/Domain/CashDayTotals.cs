namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Kasa ekranının toplamları (sözleşme CashDay; Kasa.png). Yöntem kartları brüt tahsilattır: o gün yazılan kayıtlar iade
/// işaretlense de tutarıyla sayılır; nakit iade ayrıca <see cref="CashRefundedMinor"/> olarak beklenen nakitten düşülür.
/// İptal (Voided) kayıt hiçbir toplama girmez. PackageRedemption ("Paketten · 0 ₺") sayıya girer, tutara girmez. Tanınmayan
/// yöntem (ileride eklenen) "Bugün tahsil edilen"e girer, yöntem kartlarına girmez.
/// </summary>
internal sealed record CashDayTotals(
    MethodTotal Total,
    MethodTotal Cash,
    MethodTotal Card,
    MethodTotal Transfer,
    long TransferDepositMinor,
    long CashRefundedMinor)
{
    /// <summary>Havale kartındaki "N ödeme" (kapora dışı havaleler).</summary>
    public long TransferOtherMinor => Transfer.AmountMinor - TransferDepositMinor;

    /// <summary>"kasada olmalı": nakit tahsilatlar − o gün işaretlenen nakit iadeler; tutar eksiye inmez.</summary>
    // Kural: docs/acik-sorular.md W3, T-86
    public long ExpectedCashMinor => Math.Max(0, Cash.AmountMinor - CashRefundedMinor);

    /// <param name="paidInDay">Şubenin o yerel gününde yazılan tahsilatlar.</param>
    /// <param name="cashRefundedMinor">O gün işaretlenen nakit iadelerin toplamı (tahsilat başka günün olabilir).</param>
    public static CashDayTotals Calculate(IEnumerable<CashEntry> paidInDay, long cashRefundedMinor)
    {
        ArgumentNullException.ThrowIfNull(paidInDay);
        MethodTotal total = default, cash = default, card = default, transfer = default;
        long deposit = 0;
        foreach (var entry in paidInDay.Where(e => e.Status != PaymentStatus.Voided))
        {
            switch (entry.Method)
            {
                case PaymentMethod.PackageRedemption:
                    total = Add(total, 0);
                    continue;
                case PaymentMethod.Cash:
                    cash = Add(cash, entry.AmountMinor);
                    break;
                case PaymentMethod.Card:
                    card = Add(card, entry.AmountMinor);
                    break;
                case PaymentMethod.Transfer:
                    transfer = Add(transfer, entry.AmountMinor);
                    deposit += entry.IsDeposit ? entry.AmountMinor : 0;
                    break;
            }

            total = Add(total, entry.AmountMinor);
        }

        return new CashDayTotals(total, cash, card, transfer, deposit, cashRefundedMinor);
    }

    private static MethodTotal Add(MethodTotal sum, long amountMinor) => new(checked(sum.AmountMinor + amountMinor), sum.Count + 1);
}
