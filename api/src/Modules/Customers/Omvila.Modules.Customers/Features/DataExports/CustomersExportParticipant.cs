using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Customers.Features.Customers;
using Omvila.Modules.Customers.Features.Export;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Features.DataExports;

/// <summary>Dışa aktarmada Customers'ın payı: <c>musteriler.csv</c> (listede görünen kartlar; birleşmiş ve anonim kartlar yok).</summary>
internal sealed class CustomersExportParticipant(CustomerReader reader) : IBrandDataExportParticipant
{
    private static readonly string[] Headers =
        ["Müşteri kimliği", "Ad", "Telefon", "Doğum yılı", "İlk kanal", "İlk ziyaret", "Son ziyaret", "Ziyaret", "Gelmedi", "Toplam harcama (TL)", "Not"];

    public async Task<IReadOnlyList<BrandDataExportFile>> ExportAsync(Guid brandId, CancellationToken ct)
    {
        var rows = await reader.ListedRows(brandId).OrderBy(r => r.FullName).ThenBy(r => r.Id).ToListAsync(ct);
        return
        [
            new BrandDataExportFile(
                "musteriler.csv",
                Headers,
                [.. rows.Select(r => (IReadOnlyList<string?>)
                [
                    r.Id.ToString(),
                    r.FullName,
                    PhoneNumber.TryParse(r.PhoneE164, out var phone) ? phone.Display : null,
                    r.BirthYear?.ToString(CultureInfo.InvariantCulture),
                    CustomerExporter.SourceLabel(r.Source),
                    r.FirstVisitOn?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    r.LastVisitOn?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    r.VisitCount.ToString(CultureInfo.InvariantCulture),
                    r.NoShowCount.ToString(CultureInfo.InvariantCulture),
                    BrandDataExportFile.Lira(r.TotalSpentMinor),
                    r.Notes,
                ])]),
        ];
    }
}
