using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Domain;

/// <summary>
/// Kullanıcının bir markadaki rolü (alan-modeli.md § UserMembership). Şube kapsamı küme olarak tutulur
/// (roller-yetkiler.md: "atandığı şube(ler)"; sözleşme membership.branchIds): boş = bütün şubeler (Owner).
/// Bağlı personel kaydı <see cref="StaffId"/> (jeton iddiası staff_id). v1'de kullanıcı tek markaya üyedir (S-16).
/// </summary>
internal sealed class UserMembership
{
    private UserMembership()
    {
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public Guid BrandId { get; private set; }

    public string Role { get; private set; } = string.Empty;

    public Guid[] BranchIds { get; private set; } = [];

    public Guid? StaffId { get; private set; }

    public MembershipStatus Status { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public Instant? ActivatedUtc { get; private set; }

    public static UserMembership Create(
        Guid id, Guid userId, Guid brandId, string role, IReadOnlyCollection<Guid> branchIds, Guid? staffId, MembershipStatus status, Instant now) => new()
        {
            Id = id,
            UserId = userId,
            BrandId = brandId,
            Role = role,
            BranchIds = [.. branchIds],
            StaffId = staffId,
            Status = status,
            CreatedUtc = now,
            ActivatedUtc = status == MembershipStatus.Active ? now : null,
        };

    /// <summary>Personel pasife alınınca (T-32). Zaten devre dışıysa değişmez.</summary>
    public Result Disable()
    {
        if (Status == MembershipStatus.Disabled)
        {
            return Result.Success();
        }

        var next = MembershipTransitions.To(Status, MembershipStatus.Disabled);
        if (!next.IsSuccess)
        {
            return next.Error;
        }

        Status = next.Value;
        return Result.Success();
    }

    /// <summary>Rol ve şube kapsamı (personel düzenleme). Owner'ın rolü değişmez.</summary>
    public void ChangeScope(string? role, IReadOnlyCollection<Guid>? branchIds)
    {
        if (string.Equals(Role, Contracts.Roles.Owner, StringComparison.Ordinal))
        {
            return;
        }

        Role = role ?? Role;
        BranchIds = branchIds is null ? BranchIds : [.. branchIds];
    }

    /// <summary>Personel aynı numarayla ilk girişte Active olur (ADR-0007, kabul kriteri 10).</summary>
    public Result Activate(Instant now)
    {
        var next = MembershipTransitions.To(Status, MembershipStatus.Active);
        if (!next.IsSuccess)
        {
            return next.Error;
        }

        Status = next.Value;
        ActivatedUtc = now;
        return Result.Success();
    }
}
