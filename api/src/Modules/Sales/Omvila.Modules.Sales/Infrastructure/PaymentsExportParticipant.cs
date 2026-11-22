using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Customers.Contracts;

namespace Omvila.Modules.Sales.Infrastructure;

/// <summary>
/// "Tüm verileri dışa aktar"da Sales'in payı (T-121): <c>odemeler.csv</c> (bütün tahsilat satırları, iptal edilenler dahil;
/// durum sütunu ayırır). Anlar UTC.
/// </summary>
internal sealed class PaymentsExportParticipant(SalesDbContext db) : IBrandDataExportParticipant
{
    private static readonly string[] Headers =
    [
        "Ödeme kimliği", "Müşteri kimliği", "Şube kimliği", "Yöntem", "Tutar (TL)", "Para birimi", "Kapora", "Durum", "İade (TL)",
        "Ödeme (UTC)", "İade (UTC)",
    ];

    public async Task<IReadOnlyList<BrandDataExportFile>> ExportAsync(Guid brandId, CancellationToken ct)
    {
        var payments = await db.Payments.AsNoTracking()
            .Where(p => p.BrandId == brandId)
            .OrderBy(p => p.PaidUtc).ThenBy(p => p.Id)
            .ToListAsync(ct);
        return
        [
            new BrandDataExportFile(
                "odemeler.csv",
                Headers,
                [.. payments.Select(p => (IReadOnlyList<string?>)
                [
                    p.Id.ToString(),
                    p.BrandCustomerId?.ToString(),
                    p.BranchId.ToString(),
                    p.Method.ToString(),
                    BrandDataExportFile.Lira(p.AmountMinor),
                    p.CurrencyCode,
                    p.IsDeposit ? "Evet" : "Hayır",
                    p.Status.ToString(),
                    BrandDataExportFile.Lira(p.RefundedAmountMinor),
                    BrandDataExportFile.Utc(p.PaidUtc),
                    BrandDataExportFile.Utc(p.RefundedUtc),
                ])]),
        ];
    }
}
