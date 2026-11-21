using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Domain;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Deletion;

/// <summary>
/// KVKK silme talebi, salon tarafı (MusteriSil; sözleşme manageGetCustomerDeletionPreview, manageRequestCustomerDeletion;
/// guvenlik-kvkk.md § Haklar ve işleyiş, § Anonimleştirme). Yalnız o markadaki kart anonimleşir: ad "Silinmiş müşteri",
/// not ve doğum yılı silinir, kart yeni bir anonim global kişiye bağlanır (gerçek kişiyle bağ kopar; telefon <c>+000…</c>).
/// Gerçek kişinin global kaydı ve başka markalardaki kartları değişmez. Randevu ve ödeme kayıtları anonim kartta kalır;
/// yaklaşan randevular katılımcı modüllerce (<see cref="ICustomerErasureParticipant"/>) iptal edilir. Talep
/// <c>Panel/Completed</c> yazılır, bekleyen talep varsa tamamlanır; işlem <c>AuditLog(Anonymize)</c>.
/// </summary>
internal sealed class CustomerEraser(
    CustomersDbContext db, IEnumerable<ICustomerErasureParticipant> participants, IAuditLog audit, IIdGenerator ids, IClock clock)
{
    private static readonly CompareInfo Turkish = CultureInfo.GetCultureInfo("tr-TR").CompareInfo;

    public async Task<CustomerDeletionPreview?> PreviewAsync(Guid brandId, Guid brandCustomerId, CancellationToken ct)
    {
        var target = await LoadAsync(brandId, brandCustomerId, track: false, ct);
        if (target is not { Card.IsActive: true })
        {
            return null;
        }

        var footprint = await FootprintAsync(brandId, brandCustomerId, ct);
        return new CustomerDeletionPreview(
            target.Value.Name, footprint.Appointments, footprint.Payments, footprint.UpcomingAppointments, footprint.ActivePackages ?? []);
    }

    // Kural: guvenlik-kvkk.md § Anonimleştirme, alan-modeli.md § DataDeletionRequest
    public async Task<Result<DataDeletionRequest>> EraseAsync(
        Guid brandId, Guid brandCustomerId, CustomerDeletionRequest body, LocalDate today, AuditActor actor, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(actor);
        var target = await LoadAsync(brandId, brandCustomerId, track: true, ct);
        if (target is null || target.Value.Card.MergedIntoId is not null)
        {
            return Error.NotFound();
        }

        var (card, name) = target.Value;
        if (card.AnonymizedUtc is not null)
        {
            return Error.Conflict(ErrorCodes.InvalidStateTransition);
        }

        var confirm = body.ConfirmFullName?.Trim();
        if (string.IsNullOrEmpty(confirm))
        {
            return Error.Validation("confirmFullName", "required");
        }

        if (Turkish.Compare(confirm, name.Trim(), CompareOptions.IgnoreCase) != 0)
        {
            return Error.Validation("confirmFullName", "mismatch");
        }

        var now = clock.GetCurrentInstant();
        var footprint = await FootprintAsync(brandId, brandCustomerId, ct);
        var originalCustomerId = card.CustomerId;
        var anonymous = Customer.CreateAnonymous(ids.New(), now);
        db.Customers.Add(anonymous);
        card.Anonymize(anonymous.Id, now);
        var pending = await db.DataDeletionRequests
            .Where(r => r.BrandId == brandId && r.CustomerId == originalCustomerId && r.Status == DeletionRequestStatus.Pending)
            .ToListAsync(ct);
        pending.ForEach(r => r.Complete(now));
        var request = DataDeletionRequest.CompletedFromPanel(
            ids.New(), originalCustomerId, brandId, name, today.PlusDays(PlatformDefaults.DeletionRequestDueDays), now);
        db.DataDeletionRequests.Add(request);
        await db.SaveChangesAsync(ct);

        var erasure = new CustomerErasure(brandId, brandCustomerId, anonymous.Id, actor.UserId, now);
        foreach (var participant in participants)
        {
            await participant.EraseAsync(erasure, ct);
        }

        await audit.WriteAsync(
            actor.Entry(brandId, AuditAction.Anonymize, nameof(BrandCustomer), brandCustomerId, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["dataDeletionRequestId"] = request.Id,
                ["keptAppointments"] = footprint.Appointments,
                ["keptPayments"] = footprint.Payments,
                ["cancelledAppointments"] = footprint.UpcomingAppointments,
            }),
            ct);
        return request;
    }

    private async Task<CustomerErasureFootprint> FootprintAsync(Guid brandId, Guid brandCustomerId, CancellationToken ct)
    {
        var now = clock.GetCurrentInstant();
        var total = CustomerErasureFootprint.None;
        foreach (var participant in participants)
        {
            total = total.Add(await participant.GetFootprintAsync(brandId, brandCustomerId, now, ct));
        }

        return total;
    }

    // Kart ve salonun gördüğü ad (kart adı, yoksa kişinin adı).
    private async Task<(BrandCustomer Card, string Name)?> LoadAsync(Guid brandId, Guid brandCustomerId, bool track, CancellationToken ct)
    {
        // AsNoTracking sorgunun tamamına uygulanır; izlenecek kart ayrı sorguyla yüklenir.
        var cards = track ? db.BrandCustomers : db.BrandCustomers.AsNoTracking();
        var card = await cards.SingleOrDefaultAsync(b => b.Id == brandCustomerId && b.BrandId == brandId, ct);
        if (card is null)
        {
            return null;
        }

        var name = card.DisplayName ?? await db.Customers.AsNoTracking().Where(c => c.Id == card.CustomerId).Select(c => c.FullName).SingleAsync(ct);
        return (card, name);
    }
}
