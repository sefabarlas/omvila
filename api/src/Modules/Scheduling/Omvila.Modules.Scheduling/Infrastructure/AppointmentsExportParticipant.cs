using Microsoft.EntityFrameworkCore;
using Omvila.Modules.Customers.Contracts;
using Omvila.Modules.Scheduling.Domain;

namespace Omvila.Modules.Scheduling.Infrastructure;

/// <summary>
/// "Tüm verileri dışa aktar"da Scheduling'in payı (T-121): <c>randevular.csv</c>, randevu başına bir satır (taslaklar yok).
/// Hizmetler ve personel kalem sırasıyla " + " ile birleşir; anlar UTC.
/// </summary>
internal sealed class AppointmentsExportParticipant(SchedulingDbContext db) : IBrandDataExportParticipant
{
    private static readonly string[] Headers =
    [
        "Randevu kimliği", "Müşteri kimliği", "Şube kimliği", "Durum", "Kanal", "Başlangıç (UTC)", "Bitiş (UTC)", "Hizmetler",
        "Personel kimlikleri", "Tutar (TL)", "Kapora (TL)", "Kapora alındı (UTC)", "Müşteri notu", "İptal nedeni", "İptal (UTC)",
    ];

    public async Task<IReadOnlyList<BrandDataExportFile>> ExportAsync(Guid brandId, CancellationToken ct)
    {
        var appointments = await db.Appointments.AsNoTracking()
            .Where(a => a.BrandId == brandId && a.Status != AppointmentStatus.Draft)
            .OrderBy(a => a.StartUtc).ThenBy(a => a.Id)
            .ToListAsync(ct);
        var items = (await db.AppointmentItems.AsNoTracking()
                .Where(i => i.BrandId == brandId)
                .Select(i => new { i.AppointmentId, i.SeqNo, i.ServiceNameSnapshot, i.StaffId })
                .ToListAsync(ct))
            .ToLookup(i => i.AppointmentId);
        return
        [
            new BrandDataExportFile(
                "randevular.csv",
                Headers,
                [.. appointments.Select(a =>
                {
                    var lines = items[a.Id].OrderBy(i => i.SeqNo).ToList();
                    return (IReadOnlyList<string?>)
                    [
                        a.Id.ToString(),
                        a.BrandCustomerId.ToString(),
                        a.BranchId.ToString(),
                        a.Status.ToString(),
                        a.Channel.ToString(),
                        BrandDataExportFile.Utc(a.StartUtc),
                        BrandDataExportFile.Utc(a.EndUtc),
                        string.Join(" + ", lines.Select(i => i.ServiceNameSnapshot)),
                        string.Join(" + ", lines.Select(i => i.StaffId?.ToString() ?? string.Empty)),
                        BrandDataExportFile.Lira(a.TotalAmountMinor),
                        a.DepositRequiredMinor is { } deposit ? BrandDataExportFile.Lira(deposit) : null,
                        BrandDataExportFile.Utc(a.DepositReceivedUtc),
                        a.CustomerNote,
                        a.CancellationReason?.ToString(),
                        BrandDataExportFile.Utc(a.CancelledUtc),
                    ];
                })]),
        ];
    }
}
