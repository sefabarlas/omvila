using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Tests;

// Kural: sözleşme CustomerCreate (fullName 2–120, birthYear 1900..bu yıl, notes ≤ 2000), alan-modeli.md § Customer
// (telefon E.164, global tektir: farklı yazımlar aynı kişidir), ürün kuralı 4 (yalnız doğum yılı).
public sealed class CustomerRulesTests
{
    private const int ThisYear = 2026;

    [Fact]
    public void ValidInput_IsNormalised()
    {
        var result = CustomerRules.ValidateNew("  Selin Öztürk ", "0555 000 44 18", 1992, "  Kısa saç sever ", ThisYear);

        result.IsSuccess.ShouldBeTrue();
        result.Value.FullName.ShouldBe("Selin Öztürk");
        result.Value.Phone.E164.ShouldBe("+905550004418");
        result.Value.BirthYear.ShouldBe(1992);
        result.Value.Notes.ShouldBe("Kısa saç sever");
    }

    [Theory]
    [InlineData("0555 000 44 18")]
    [InlineData("5550004418")]
    [InlineData("+90 555 000 4418")]
    [InlineData("(0555) 000-44-18")]
    public void PhoneSpellings_MapToTheSameGlobalCustomerPhone(string spelling)
    {
        var input = CustomerRules.ValidateNew("Selin Öztürk", spelling, null, null, ThisYear).Value;

        Customer.Create(TestIds.Next(), input.Phone, input.FullName, null, false, Instant.FromUtc(2026, 11, 25, 9, 0))
            .PhoneE164.ShouldBe("+905550004418");
    }

    [Theory]
    [InlineData(null, "fullName", "required")]
    [InlineData("S", "fullName", "too_short")]
    public void Name_IsRequiredAndAtLeastTwoCharacters(string? name, string field, string code) =>
        CustomerRules.ValidateNew(name, "05550004418", null, null, ThisYear).Error!.FieldErrors[field].ShouldBe([code]);

    [Fact]
    public void Name_LongerThan120_IsTooLong() =>
        CustomerRules.ValidateNew(new string('a', 121), "05550004418", null, null, ThisYear).Error!.FieldErrors["fullName"].ShouldBe(["too_long"]);

    [Theory]
    [InlineData(null, "required")]
    [InlineData("12345", "invalid_format")]
    [InlineData("+44 20 7946 0958", "invalid_format")]
    public void Phone_MustBeATurkishNumber(string? phone, string code) =>
        CustomerRules.ValidateNew("Derya Şahin", phone, null, null, ThisYear).Error!.FieldErrors["phone"].ShouldBe([code]);

    [Theory]
    [InlineData(1899)]
    [InlineData(2027)]
    public void BirthYear_OutsideRange_IsRejected(int year) =>
        CustomerRules.ValidateNew("Derya Şahin", "05550004419", year, null, ThisYear).Error!.FieldErrors["birthYear"].ShouldBe(["out_of_range"]);

    [Fact]
    public void BirthYear_ThisYear_IsAccepted() =>
        CustomerRules.ValidateNew("Derya Şahin", "05550004419", ThisYear, null, ThisYear).IsSuccess.ShouldBeTrue();

    [Fact]
    public void Notes_LongerThan2000_AreTooLong() =>
        CustomerRules.ValidateNew("Derya Şahin", "05550004419", null, new string('n', 2001), ThisYear).Error!.FieldErrors["notes"].ShouldBe(["too_long"]);

    [Fact]
    public void AllErrors_AreReportedTogether() =>
        CustomerRules.ValidateNew("", "abc", 1800, null, ThisYear).Error!.FieldErrors.Keys.ShouldBe(["fullName", "phone", "birthYear"], ignoreOrder: true);

    [Fact]
    public void ValidationFailure_IsValidationKind() =>
        CustomerRules.ValidateNew(null, null, null, null, ThisYear).Error!.Kind.ShouldBe(ErrorKind.Validation);
}
