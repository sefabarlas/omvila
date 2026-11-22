using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Platform.Domain;

namespace Omvila.Modules.Platform.Infrastructure;

/// <summary>
/// Boşluksuz fatura numarası (alan-modeli.md § SubscriptionInvoice): yılın sayaç satırı tek SQL ile artırılır (upsert … RETURNING).
/// Satır kilidi transaction sonuna kadar sürer; eşzamanlı ikinci kesim bekler ve sıradaki numarayı alır. Transaction geri alınırsa
/// sayaç da geri alınır, numara boşa gitmez. Çağıranın transaction'ında çalışır.
/// </summary>
internal sealed class InvoiceNumberAllocator(PlatformDbContext db)
{
    public async Task<string> NextAsync(int year, CancellationToken ct)
    {
        var values = await db.Database.SqlQuery<long>($"""
            INSERT INTO platform.invoice_number_counters (year, last_value) VALUES ({year}, 1)
            ON CONFLICT (year) DO UPDATE SET last_value = invoice_number_counters.last_value + 1
            RETURNING last_value AS "Value"
            """).ToListAsync(ct);
        return InvoiceNumbers.Format(year, values.Single());
    }
}
