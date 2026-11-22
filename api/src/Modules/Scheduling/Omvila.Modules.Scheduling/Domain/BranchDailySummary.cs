using NodaTime;

namespace Omvila.Modules.Scheduling.Domain;

/// <summary>
/// Gün sonu özeti kaydı (bildirimler.md staff.daily_summary; T-120): şube + şubenin yerel günü başına tektir, böylece iş günde bir
/// kez özet yayımlar (iş yeniden çalışsa da). Özetin sayıları yayımlandığı anın değerleridir.
/// </summary>
internal sealed class BranchDailySummary
{
    private BranchDailySummary()
    {
    }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public LocalDate BusinessDate { get; private set; }

    public int CompletedCount { get; private set; }

    public long CollectedMinor { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public static BranchDailySummary Record(Guid brandId, Guid branchId, LocalDate businessDate, int completedCount, long collectedMinor, Instant now) =>
        new()
        {
            BrandId = brandId,
            BranchId = branchId,
            BusinessDate = businessDate,
            CompletedCount = completedCount,
            CollectedMinor = collectedMinor,
            CreatedUtc = now,
        };
}
