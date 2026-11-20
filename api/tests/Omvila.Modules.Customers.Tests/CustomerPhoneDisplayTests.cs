using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Tests;

// Kural: roller-yetkiler.md customer.phone.view ve ** dipnotu (MaskCustomerPhoneFromStaff açıkken personel
// "0532 *** ** 21" görür; ayar kapalıysa personel de tam görür), sözleşme CustomerSummary (phoneMasked her zaman).
public sealed class CustomerPhoneDisplayTests
{
    [Theory]
    [InlineData(true, true, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void FullPhone_NeedsPermissionOrUnmaskedBrand(bool hasPermission, bool brandMasks, bool expected) =>
        CustomerPhoneDisplay.CanSeeFullPhone(hasPermission, brandMasks).ShouldBe(expected);

    [Fact]
    public void Masked_IsAlwaysGiven_FullOnlyWhenAllowed()
    {
        CustomerPhoneDisplay.For("+905550004418", canSeeFullPhone: false).ShouldBe(new CustomerPhoneView(null, "0555 *** ** 18"));
        CustomerPhoneDisplay.For("+905550004418", canSeeFullPhone: true).ShouldBe(new CustomerPhoneView("+905550004418", "0555 *** ** 18"));
    }

    [Fact]
    public void AnonymisedNumber_IsNeverShown() =>
        CustomerPhoneDisplay.For("+000a1b2c3", canSeeFullPhone: true).ShouldBe(new CustomerPhoneView(null, null));
}
