using CsCheck;
using Omvila.SharedKernel;

namespace Omvila.SharedKernel.Tests;

// Kural: AGENTS.md § 5 (kuruş tamsayı), kodlama-dotnet.md § Alan katmanı (Money.Percent)
public sealed class MoneyTests
{
    [Fact]
    public void Add_WithSameCurrency_SumsMinorUnits() =>
        (Money.Try(45_000) + Money.Try(30_000)).ShouldBe(Money.Try(75_000));

    [Fact]
    public void Subtract_BelowZero_ThrowsDomainException() =>
        Should.Throw<DomainException>(() => Money.Try(100) - Money.Try(101));

    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsDomainException() =>
        Should.Throw<DomainException>(() => new Money(-1, Currency.TRY));

    [Theory]
    [InlineData(280_000, 30, 84_000)]
    [InlineData(45_050, 30, 13_515)]
    [InlineData(1_005, 50, 503)]
    [InlineData(999, 0, 0)]
    public void Percent_RoundsToMinorUnit(long amount, int percent, long expected) =>
        Money.Try(amount).Percent(percent, MidpointRounding.AwayFromZero).AmountMinor.ShouldBe(expected);

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Percent_OutsideRange_ThrowsDomainException(int percent) =>
        Should.Throw<DomainException>(() => Money.Try(1_000).Percent(percent, MidpointRounding.ToEven));

    [Fact]
    public void Percent_StaysWithinBounds() =>
        Gen.Select(Gen.Long[0, 100_000_000_00], Gen.Int[0, 100]).Sample((amount, percent) =>
        {
            var money = Money.Try(amount);
            var part = money.Percent(percent, MidpointRounding.AwayFromZero);

            part.AmountMinor.ShouldBeInRange(0, amount);
            if (percent == 100)
            {
                part.ShouldBe(money);
            }

            if (percent == 0)
            {
                part.ShouldBe(Money.Zero(Currency.TRY));
            }
        });
}
