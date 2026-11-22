using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Infrastructure.Messaging;
using Omvila.Infrastructure.Persistence;
using Omvila.Infrastructure.Sms;
using Omvila.Modules.Notifications.Contracts;
using Omvila.Modules.Notifications.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Infrastructure;

/// <summary><see cref="IPlatformSmsSenders"/>: platform servisi (ADR-0006, K4); yalnız platform bağlamında.</summary>
internal sealed class PlatformSmsSenders(
    NotificationsDbContext db, ITenantContext tenant, IOutbox outbox, IClock clock, IOptions<SmsOptions> sms) : IPlatformSmsSenders
{
    public async Task<IReadOnlyList<PendingSmsSender>> ListPendingAsync(CancellationToken ct)
    {
        RequirePlatformScope();
        var defaultSender = sms.Value.DefaultSender;
        var rows = await db.SmsSenders.AsNoTracking()
            .Where(s => s.Status == SmsSenderStatus.Pending)
            .OrderBy(s => s.SubmittedUtc).ThenBy(s => s.BrandId)
            .ToListAsync(ct);
        return [.. rows.Select(s => new PendingSmsSender(s.BrandId, s.BrandId, s.Name, BrandSmsSender.EffectiveName(s, defaultSender), s.SubmittedUtc, s.Version))];
    }

    // Kural: durum-makinesi.md § 10 BrandSmsSender (Pending → Approved / Rejected); sözleşme adminApproveSmsSender, adminRejectSmsSender
    public async Task<Result<SmsSenderDecision>> DecideAsync(Guid senderId, bool approve, string? reason, uint expectedVersion, CancellationToken ct)
    {
        RequirePlatformScope();
        var sender = await db.SmsSenders.SingleOrDefaultAsync(s => s.BrandId == senderId, ct);
        if (sender is null)
        {
            return Error.NotFound();
        }

        if (sender.Version != expectedVersion)
        {
            return Error.PreconditionFailed();
        }

        if (sender.Status != SmsSenderStatus.Pending)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        var now = clock.GetCurrentInstant();
        if (approve)
        {
            sender.Approve(now);
        }
        else
        {
            sender.Reject(reason ?? string.Empty, now);
        }

        db.Entry(sender).Property(s => s.Version).OriginalValue = expectedVersion;
        await db.SaveChangesAsync(ct);
        await outbox.EnqueueAsync(new SmsSenderDecided(sender.BrandId, sender.Name, approve, sender.RejectionReason), ct);
        return new SmsSenderDecision(sender.BrandId, sender.BrandId, sender.Name, sender.Status.ToString(), now, sender.RejectionReason);
    }

    private void RequirePlatformScope()
    {
        if (!tenant.Scope.IsPlatform)
        {
            throw new DomainException("Platform servisi yalnız platform bağlamında çağrılır (ADR-0006).");
        }
    }
}
