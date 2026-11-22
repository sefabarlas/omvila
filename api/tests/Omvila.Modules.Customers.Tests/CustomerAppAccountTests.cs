using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Tests;

// Kural: alan-modeli.md § Customer (IsVerified, HasAppAccount); sözleşme meVerifyOtp (isNewAccount), meUpdateProfile
// (gönderilmeyen alan değişmez, email null siler); T-101 (2) (ad ilk randevuda dolar), T-103.
public sealed class CustomerAppAccountTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);

    private static Customer Unnamed() => Customer.Create(TestIds.Next(), PhoneNumber.Parse("+905551112233"), string.Empty, null, isVerified: false, Now);

    [Fact]
    public void OpenAppAccount_FirstTimeIsNew_ThenNot_AndVerifies()
    {
        var customer = Unnamed();

        customer.OpenAppAccount().ShouldBeTrue();
        customer.OpenAppAccount().ShouldBeFalse();
        customer.IsVerified.ShouldBeTrue();
        customer.HasAppAccount.ShouldBeTrue();
    }

    [Fact]
    public void OpenAppAccount_OnAnonymizedRecord_Throws()
    {
        Should.Throw<DomainException>(() => Customer.CreateAnonymous(TestIds.Next(), Now).OpenAppAccount());
    }

    [Fact]
    public void FillNameIfMissing_OnlyFillsAnEmptyName()
    {
        var customer = Unnamed();
        customer.HasName.ShouldBeFalse();

        customer.FillNameIfMissing(" Merve Yıldız ");
        customer.FillNameIfMissing("Başka Ad");

        customer.FullName.ShouldBe("Merve Yıldız");
    }

    [Fact]
    public void UpdateProfile_NullNameKeepsName_BlankEmailClears()
    {
        var customer = Unnamed();
        customer.UpdateProfile("Merve Yıldız", " merve@example.com ");
        customer.Email.ShouldBe("merve@example.com");

        customer.UpdateProfile(null, null);

        customer.FullName.ShouldBe("Merve Yıldız");
        customer.Email.ShouldBeNull();
    }
}
