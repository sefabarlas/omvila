using Omvila.Modules.Catalog.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Tests;

// Kural: sureler-ve-ayarlar.md § Kapora — "Bir kalem için kapora çözümü" her satırı; yüzde liraya yuvarlanır
public sealed class DepositResolverTests
{
    private static readonly Money Price = Money.Try(280_000);
    private static readonly DepositRule Percent30 = new(DepositMode.Inherit, null, 30m);

    [Fact]
    public void NoActiveBankAccount_NoDeposit() =>
        DepositResolver.Resolve(Percent30 with { Mode = DepositMode.Always }, Price, Context(bank: false)).ShouldBeNull();

    [Fact]
    public void NotOnlineChannel_NoDeposit() =>
        DepositResolver.Resolve(Percent30 with { Mode = DepositMode.Always }, Price, Context(online: false)).ShouldBeNull();

    [Fact]
    public void PackageRedemption_NoDeposit() =>
        DepositResolver.Resolve(Percent30 with { Mode = DepositMode.Always }, Price, Context(package: true)).ShouldBeNull();

    [Fact]
    public void ModeNever_NoDeposit_EvenIfBrandAlways() =>
        DepositResolver.Resolve(Percent30 with { Mode = DepositMode.Never }, Price, Context(policy: BrandDepositPolicy.Always)).ShouldBeNull();

    [Fact]
    public void ModeAlways_TakesDeposit_RegardlessOfBrandPolicy() =>
        DepositResolver.Resolve(Percent30 with { Mode = DepositMode.Always }, Price, Context(policy: BrandDepositPolicy.None)).ShouldBe(Money.Try(84_000));

    [Fact]
    public void InheritAndBrandNone_NoDeposit() =>
        DepositResolver.Resolve(Percent30, Price, Context(policy: BrandDepositPolicy.None)).ShouldBeNull();

    [Fact]
    public void InheritAndBrandAlways_WithoutServiceValue_NoDeposit() =>
        DepositResolver.Resolve(DepositRule.Inherit, Price, Context(policy: BrandDepositPolicy.Always)).ShouldBeNull();

    [Theory]
    [InlineData(1, null)]
    [InlineData(2, 84_000L)]
    [InlineData(3, 84_000L)]
    public void InheritAndRiskyOnly_DependsOnNoShowCount(int noShows, long? expected)
    {
        var deposit = DepositResolver.Resolve(Percent30, Price, Context(policy: BrandDepositPolicy.RiskyCustomersOnly, noShows: noShows));

        deposit?.AmountMinor.ShouldBe(expected!.Value);
        (deposit is null).ShouldBe(expected is null);
    }

    [Fact]
    public void FixedAmount_IsUsedAsIs() =>
        DepositResolver.Resolve(new DepositRule(DepositMode.Always, Money.Try(30_000), null), Price, Context()).ShouldBe(Money.Try(30_000));

    [Theory]
    [InlineData(45_050, 13_500)]
    [InlineData(45_150, 13_500)]
    [InlineData(45_170, 13_600)]
    [InlineData(45_500, 13_700)]
    public void Percent_RoundsToWholeLira(long priceMinor, long expectedMinor) =>
        DepositResolver.Resolve(new DepositRule(DepositMode.Always, null, 30m), Money.Try(priceMinor), Context())!.Value.AmountMinor.ShouldBe(expectedMinor);

    private static DepositContext Context(
        bool bank = true, bool online = true, bool package = false,
        BrandDepositPolicy policy = BrandDepositPolicy.RiskyCustomersOnly, int noShows = 0) =>
        new(bank, online, package, policy, noShows, RiskyCustomerNoShowThreshold: 2);
}
