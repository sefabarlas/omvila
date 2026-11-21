using Microsoft.EntityFrameworkCore;
using NodaTime;
using Omvila.Modules.Sales.Contracts;
using Omvila.Modules.Sales.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>
/// <see cref="IDepositPayments"/>: kapora tahsilatını istek transaction'ında yazar. DbContext oturumun bağlantısını ve
/// transaction'ını kullanır (<c>AddModuleDbContext</c>); commit veya geri alma çağıranın oturumundadır.
/// </summary>
internal sealed class DepositPayments(SalesDbContext db, IIdGenerator ids, IClock clock) : IDepositPayments
{
    public async Task<Guid> RecordDepositAsync(DepositPaymentRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        var payment = Payment.RecordDeposit(
            ids,
            request.BrandId,
            request.BranchId,
            request.BrandCustomerId,
            request.AppointmentId,
            request.Amount,
            request.Reference,
            request.CollectedByUserId,
            clock.GetCurrentInstant());
        db.Payments.Add(payment);
        await db.SaveChangesAsync(ct);
        return payment.Id;
    }

    public async Task MarkDepositRefundedAsync(Guid paymentId, Money amount, CancellationToken ct)
    {
        var payment = await db.Payments.SingleOrDefaultAsync(p => p.Id == paymentId && p.IsDeposit, ct)
            ?? throw new DomainException("Kapora tahsilatı bulunamadı.");
        payment.MarkRefunded(amount);
        await db.SaveChangesAsync(ct);
    }
}
