using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Identity.Contracts;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.Modules.Notifications.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

internal sealed class StaffMemberships(
    IdentityDbContext db, INotificationOutbox notifications, IIdGenerator ids, IClock clock, IOptions<AuthOptions> auth) : IStaffMemberships
{
    public async Task<Result<MembershipInfo>> InviteAsync(StaffInvitation invitation, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(invitation);
        var now = clock.GetCurrentInstant();

        // users markaya ait değildir (RLS yok); üyelik v1'de kullanıcı başına tektir (S-16).
        var user = await db.Users.SingleOrDefaultAsync(u => u.PhoneE164 == invitation.PhoneE164, ct);
        // Platform kullanıcısının marka üyeliği olamaz (alan-modeli.md § User.PlatformRole).
        if (user is not null && (user.PlatformRole is not null || await HasAnyMembershipAsync(user.Id, ct)))
        {
            return Error.Conflict(ErrorCodes.PhoneAlreadyExists);
        }

        if (user is null)
        {
            user = User.Create(ids.New(), invitation.PhoneE164, invitation.FullName, now);
            db.Users.Add(user);
        }

        var membership = UserMembership.Create(
            ids.New(), user.Id, invitation.BrandId, invitation.Role, invitation.BranchIds, invitation.StaffId, MembershipStatus.Invited, now);
        db.Memberships.Add(membership);
        await db.SaveChangesAsync(ct);

        if (invitation.SendInvite)
        {
            await notifications.EnqueueSmsAsync(
                new SmsNotification(
                    NotificationEvents.StaffInvite,
                    invitation.PhoneE164,
                    new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["salon"] = invitation.BrandName,
                        ["bağlantı"] = auth.Value.PanelSignInUrl!.ToString(),
                    },
                    invitation.BrandId),
                ct);
        }

        return Info(membership, user.PhoneE164);
    }

    public async Task<IReadOnlyDictionary<Guid, MembershipInfo>> GetByStaffAsync(Guid brandId, IReadOnlyCollection<Guid> staffIds, CancellationToken ct)
    {
        var rows = await (from m in db.Memberships.AsNoTracking()
                          join u in db.Users.AsNoTracking() on m.UserId equals u.Id
                          where m.BrandId == brandId && m.StaffId != null && staffIds.Contains(m.StaffId.Value)
                          select new { Membership = m, u.PhoneE164 }).ToListAsync(ct);
        return rows.ToDictionary(r => r.Membership.StaffId!.Value, r => Info(r.Membership, r.PhoneE164));
    }

    public async Task UpdateAsync(Guid brandId, Guid staffId, MembershipChange change, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(change);
        var membership = await db.Memberships.SingleOrDefaultAsync(m => m.BrandId == brandId && m.StaffId == staffId, ct);
        if (membership is null)
        {
            return;
        }

        membership.ChangeScope(change.Role, change.BranchIds);
        if (change.IsActive is false)
        {
            membership.Disable();
        }

        await db.SaveChangesAsync(ct);
    }

    // RLS yalnız bu markanın üyeliklerini gösterir; başka markadaki üyelik kaydetmede tek dizine takılır ve
    // DatabaseExceptionHandler onu da 409 phone_already_exists'e çevirir (IdentityModule kaydı).
    private Task<bool> HasAnyMembershipAsync(Guid userId, CancellationToken ct) => db.Memberships.AnyAsync(m => m.UserId == userId, ct);

    private static MembershipInfo Info(UserMembership m, string phoneE164) =>
        new(m.Role, m.Status.ToString(), PhoneNumber.TryParse(phoneE164, out var phone) ? phone.Masked : string.Empty);
}
