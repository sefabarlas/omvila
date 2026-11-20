using CsCheck;
using Omvila.SharedKernel;

namespace Omvila.SharedKernel.Tests;

// Kural: AGENTS.md § 5 (E.164 saklanır, 0532 123 45 67 gösterilir), contracts PhoneInput örnekleri
public sealed class PhoneNumberTests
{
    [Theory]
    [InlineData("0532 418 77 05")]
    [InlineData("5324187705")]
    [InlineData("+90 532 418 7705")]
    [InlineData("+905324187705")]
    [InlineData("905324187705")]
    [InlineData("0090 532 418 77 05")]
    [InlineData("(0532) 418-77-05")]
    [InlineData("  0532.418.77.05  ")]
    public void TryParse_WithTurkishMobileFormats_NormalizesToE164(string input)
    {
        PhoneNumber.TryParse(input, out var phone).ShouldBeTrue();

        phone.E164.ShouldBe("+905324187705");
        phone.Display.ShouldBe("0532 418 77 05");
        phone.IsMobile.ShouldBeTrue();
    }

    [Fact]
    public void TryParse_WithLandline_IsNotMobile()
    {
        PhoneNumber.TryParse("0216 345 12 12", out var phone).ShouldBeTrue();

        phone.E164.ShouldBe("+902163451212");
        phone.IsMobile.ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("0532 418 77")]
    [InlineData("0532 418 77 05 1")]
    [InlineData("+44 20 7946 0958")]
    [InlineData("0632 418 77 05")]
    [InlineData("0132 418 77 05")]
    [InlineData("0532a4187705")]
    [InlineData("+90+5324187705x")]
    public void TryParse_WithInvalidOrForeignInput_ReturnsFalse(string? input)
    {
        PhoneNumber.TryParse(input, out var phone).ShouldBeFalse();
        phone.ShouldBeNull();
    }

    [Fact]
    public void Masked_HidesMiddleDigits_AndToStringNeverLeaksFullNumber()
    {
        var phone = PhoneNumber.Parse("0532 418 77 05");

        phone.Masked.ShouldBe("0532 *** ** 05");
        phone.ToString().ShouldBe("0532 *** ** 05");
    }

    [Fact]
    public void Parse_WithInvalidInput_ThrowsDomainException() =>
        Should.Throw<DomainException>(() => PhoneNumber.Parse("12"));

    private static readonly Gen<string> NationalNumber =
        Gen.Select(Gen.Char['2', '5'], Gen.Char['0', '9'].Array[9], (first, rest) => first + new string(rest));

    [Fact]
    public void Parse_IsStableAcrossAllAcceptedFormats() =>
        NationalNumber.Sample(national =>
        {
            var expected = "+90" + national;
            var formats = new[] { national, "0" + national, "+90" + national, "90" + national, "0090" + national };

            foreach (var format in formats)
            {
                PhoneNumber.Parse(format).E164.ShouldBe(expected);
            }

            var phone = PhoneNumber.Parse(expected);
            PhoneNumber.Parse(phone.Display).ShouldBe(phone);
        });
}
