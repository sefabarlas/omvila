using Omvila.SharedKernel;

namespace Omvila.SharedKernel.Tests;

// Kural: contracts BankAccountWrite.iban (boşluk ve küçük harf kabul, TR + 24 hane, mod-97), BankAccount.ibanMasked
// ("TR29 •••• 6789 01"), ibanLast4; DepositInstructions.iban (dörtlü gruplu); günlüğe tam IBAN yazılmaz.
public sealed class IbanTests
{
    [Theory]
    [InlineData("TR29 0006 4000 0011 2345 6789 01")]
    [InlineData("TR290006400000112345678901")]
    [InlineData("tr29 0006 4000 0011 2345 6789 01")]
    [InlineData("  TR29\t0006 4000 0011 2345 678901 ")]
    public void TryParse_ValidInput_Normalizes(string input)
    {
        Iban.TryParse(input, out var iban).ShouldBeTrue();

        iban.Value.ShouldBe("TR290006400000112345678901");
        iban.Masked.ShouldBe("TR29 •••• 6789 01");
        iban.Last4.ShouldBe("8901");
        iban.Grouped.ShouldBe("TR29 0006 4000 0011 2345 6789 01");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("TR12 0006 4000 0011 2345 6789 01")] // denetim hanesi yanlış (tasarımdaki örnek)
    [InlineData("TR29 0006 4000 0011 2345 6789 02")] // son hane değişti
    [InlineData("TR29 0006 4000 0011 2345 6789 0")] // 25 karakter
    [InlineData("TR29 0006 4000 0011 2345 6789 011")] // 27 karakter
    [InlineData("DE89 3704 0044 0532 0130 00")] // geçerli Alman IBAN'ı; v1'de yalnız TR
    [InlineData("TR29 0006 4000 0011 2345 6789 0A")] // harf
    [InlineData("TR29-0006-4000-0011-2345-6789-01")] // tire ayırıcı kabul edilmez
    public void TryParse_InvalidInput_ReturnsFalse(string? input) =>
        Iban.TryParse(input, out _).ShouldBeFalse();

    [Fact]
    public void ToString_NeverRevealsFullIban()
    {
        Iban.TryParse("TR290006400000112345678901", out var iban).ShouldBeTrue();

        iban.ToString().ShouldBe("TR29 •••• 6789 01");
        $"{iban}".ShouldNotContain("0006400000112345");
    }
}
