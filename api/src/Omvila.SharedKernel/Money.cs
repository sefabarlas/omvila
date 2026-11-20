namespace Omvila.SharedKernel;

/// <summary>
/// Kuruş cinsinden tutar ve para birimi. Ondalıklı sayıyla para hesabı yapılmaz (AGENTS.md § 5).
/// </summary>
public readonly record struct Money
{
    public Money(long amountMinor, Currency currency)
    {
        if (amountMinor < 0)
        {
            throw new DomainException("Tutar negatif olamaz.");
        }

        AmountMinor = amountMinor;
        Currency = currency;
    }

    public long AmountMinor { get; }

    public Currency Currency { get; }

    public static Money Zero(Currency currency) => new(0, currency);

    public static Money Try(long amountMinor) => new(amountMinor, Currency.TRY);

    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(checked(left.AmountMinor + right.AmountMinor), left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.AmountMinor - right.AmountMinor, left.Currency);
    }

    public static Money Add(Money left, Money right) => left + right;

    public static Money Subtract(Money left, Money right) => left - right;

    /// <summary>Tutarın yüzdesi; kuruşa <paramref name="rounding"/> ile yuvarlanır (ör. %30 kapora).</summary>
    public Money Percent(decimal percent, MidpointRounding rounding)
    {
        if (percent is < 0 or > 100)
        {
            throw new DomainException("Yüzde 0 ile 100 arasında olmalı.");
        }

        var amount = decimal.Round(AmountMinor * percent / 100m, 0, rounding);
        return new Money((long)amount, Currency);
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException("Farklı para birimleri birlikte hesaplanamaz.");
        }
    }
}
