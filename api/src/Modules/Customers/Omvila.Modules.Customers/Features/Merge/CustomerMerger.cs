using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Infrastructure;
using Omvila.Modules.Platform.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.Merge;

/// <summary>
/// "Kayıtları birleştir" (MusteriBirlestir; alan-modeli.md § Birleştirme; sözleşme manageMergeCustomers). Yoldaki kart kalır,
/// kaynak kart arşivlenir (<c>MergedIntoId</c>; silinmez, listede ve aramada görünmez, kartı 404). Sayaçlar toplanır, notlar
/// isteğe bağlı birleşir; randevu, seri ve tahsilat bağı katılımcı modüllerce (<see cref="ICustomerMergeParticipant"/>) taşınır.
/// Global kişiler birleştirilmez. Rızalar taşınmaz: rıza kişiye (telefona) aittir (T-92).
/// </summary>
internal sealed class CustomerMerger(
    CustomersDbContext db, IEnumerable<ICustomerMergeParticipant> participants, IAuditLog audit, IClock clock)
{
    public async Task<Result> MergeAsync(Guid brandId, Guid targetId, CustomerMergeRequest body, AuditActor actor, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(body);
        ArgumentNullException.ThrowIfNull(actor);
        if (body.SourceBrandCustomerId == targetId)
        {
            return Error.Validation("sourceBrandCustomerId", "invalid_format");
        }

        var target = await db.BrandCustomers.SingleOrDefaultAsync(b => b.Id == targetId && b.BrandId == brandId && b.MergedIntoId == null, ct);
        var source = await db.BrandCustomers.SingleOrDefaultAsync(b => b.Id == body.SourceBrandCustomerId && b.BrandId == brandId, ct);
        if (target is null || source is null)
        {
            return Error.NotFound();
        }

        var absorbed = target.Absorb(source, body.MergeNotes ?? true, clock.GetCurrentInstant());
        if (!absorbed.IsSuccess)
        {
            return absorbed;
        }

        await db.SaveChangesAsync(ct);
        var merge = new CustomerMerge(brandId, source.Id, target.Id, target.CustomerId);
        foreach (var participant in participants)
        {
            await participant.MergeAsync(merge, ct);
        }

        await audit.WriteAsync(
            actor.Entry(brandId, AuditAction.Update, nameof(Domain.BrandCustomer), target.Id, new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["operation"] = "merge",
                ["mergedBrandCustomerId"] = source.Id,
                ["mergeNotes"] = body.MergeNotes ?? true,
            }),
            ct);
        return Result.Success();
    }
}
