using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Domain;

/// <summary>Üyelik geçişleri yalnız tablodakilerdir; diğerleri invalid_state_transition.</summary>
internal static class MembershipTransitions
{
    public static Result<MembershipStatus> To(MembershipStatus from, MembershipStatus to) => (from, to) switch
    {
        // Personel aynı numarayla ilk girişte Active olur (ADR-0007, dilim-01 kabul kriteri 10).
        (MembershipStatus.Invited, MembershipStatus.Active) => to,
        (MembershipStatus.Active, MembershipStatus.Disabled) => to,
        // Personel pasife alınınca bekleyen davet de kapanır (T-32): davetli numara artık giriş yapamaz.
        (MembershipStatus.Invited, MembershipStatus.Disabled) => to,
        _ => Error.Conflict(ErrorCodes.InvalidStateTransition),
    };
}
