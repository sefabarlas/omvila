using Omvila.Modules.Identity.Domain;

namespace Omvila.Modules.Identity.Tests;

// Kural: alan-modeli.md § User (FullName; T-130), sözleşme SignupRequest.ownerFullName: zorunlu, baş/son boşluk kırpılır,
// aradaki çoklu boşluk teke iner, 2–120 karakter, harf + kesme işareti + tire + boşluk, rakam yok.
public sealed class PersonNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t \n")]
    public void EmptyOrWhitespaceOnly_IsRequired(string? input) =>
        PersonName.Validate(PersonName.Normalize(input)).ShouldBe("required");

    [Fact]
    public void SingleLetter_IsTooShort() =>
        PersonName.Validate(PersonName.Normalize(" A ")).ShouldBe("too_short");

    [Fact]
    public void Exactly120Characters_IsValid() =>
        PersonName.Validate(new string('a', 120)).ShouldBeNull();

    [Fact]
    public void MoreThan120Characters_IsTooLong() =>
        PersonName.Validate(new string('a', 121)).ShouldBe("too_long");

    [Fact]
    public void LengthIsMeasuredAfterNormalization() =>
        PersonName.Validate(PersonName.Normalize("  " + new string('a', 60) + "    " + new string('b', 59) + "  ")).ShouldBeNull();

    [Theory]
    [InlineData("Alya Kaya 2")]
    [InlineData("4lya Kaya")]
    [InlineData("Alya_Kaya")]
    [InlineData("Alya Kaya.")]
    [InlineData("Alya, Kaya")]
    [InlineData("Alya@Kaya")]
    [InlineData("-Alya Kaya")]
    [InlineData("'Alya Kaya")]
    public void DigitsAndOtherSymbols_AreInvalidFormat(string input) =>
        PersonName.Validate(PersonName.Normalize(input)).ShouldBe("invalid_format");

    [Theory]
    [InlineData("Alya Kaya")]
    [InlineData("Çağrı Öztürk Şahin")]
    [InlineData("İpek Işık Güneş Ünal")]
    [InlineData("Hâle Kâmil")]
    [InlineData("Ayşe-Nur Yıldız")]
    [InlineData("Dilek O'Brien")]
    [InlineData("Dilek O’Brien")]
    [InlineData("Su")]
    public void TurkishLettersApostropheAndHyphen_AreValid(string input) =>
        PersonName.Validate(PersonName.Normalize(input)).ShouldBeNull();

    [Theory]
    [InlineData("  Alya   Kaya  ", "Alya Kaya")]
    [InlineData("Alya\t\tKaya", "Alya Kaya")]
    [InlineData("Ayşe  Nur   Yıldız", "Ayşe Nur Yıldız")]
    public void Normalize_TrimsAndCollapsesWhitespace(string input, string expected) =>
        PersonName.Normalize(input).ShouldBe(expected);

    [Fact]
    public void Normalize_ComposesDecomposedLetters()
    {
        // "Çağrı" ayrık yazımla (C + birleşen çengel, g + birleşen kısa işareti).
        var normalized = PersonName.Normalize("Çağrı Kaya");

        normalized.ShouldBe("Çağrı Kaya");
        PersonName.Validate(normalized).ShouldBeNull();
    }
}
