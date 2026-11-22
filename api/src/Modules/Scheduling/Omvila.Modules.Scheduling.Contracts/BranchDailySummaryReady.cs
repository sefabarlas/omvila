using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>
/// Şubenin gün sonu özeti hazır (her şubede DailySummaryLocalTime, şube yerel; bildirimler.md staff.daily_summary; T-120).
/// Notifications raporları görenlere (report.view, şube kapsamı) kişisel tercihe (dailySummary, varsayılan kapalı) göre işletme
/// push'u yazar: "{n} randevu tamamlandı, {tutar} tahsilat."
/// </summary>
/// <param name="BrandId">Marka.</param>
/// <param name="BranchId">Şube.</param>
/// <param name="TimeZoneId">Şubenin IANA saat dilimi.</param>
/// <param name="CompletedCount">Bugün başlayıp tamamlanan randevu sayısı.</param>
/// <param name="CollectedMinor">Bugünün net tahsilatı (kuruş; paket satışı ve kapora dahil, iadeler düşülmüş).</param>
public sealed record BranchDailySummaryReady(Guid BrandId, Guid BranchId, string TimeZoneId, int CompletedCount, long CollectedMinor) : IIntegrationEvent;
