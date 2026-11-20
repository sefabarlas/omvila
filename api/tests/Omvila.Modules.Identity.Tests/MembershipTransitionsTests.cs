using Omvila.Modules.Identity.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Tests;

// Kural: durum-makinesi.md (UserMembership: Invited → Active → Disabled; Invited → Disabled, T-32), dilim-01 kabul kriteri 10
public sealed class MembershipTransitionsTests
{
    [Fact]
    public void InvitedMembership_BecomesActiveOnFirstLogin() =>
        MembershipTransitions.To(MembershipStatus.Invited, MembershipStatus.Active).Value.ShouldBe(MembershipStatus.Active);

    [Theory]
    [InlineData("Active")]
    [InlineData("Invited")]
    public void ActiveOrInvitedMembership_CanBeDisabled(string from) =>
        MembershipTransitions.To(Enum.Parse<MembershipStatus>(from), MembershipStatus.Disabled).Value.ShouldBe(MembershipStatus.Disabled);

    [Theory]
    [InlineData("Active", "Invited")]
    [InlineData("Disabled", "Active")]
    [InlineData("Disabled", "Invited")]
    public void OtherMembershipTransitions_AreInvalid(string from, string to) =>
        MembershipTransitions.To(Enum.Parse<MembershipStatus>(from), Enum.Parse<MembershipStatus>(to))
            .Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
}
