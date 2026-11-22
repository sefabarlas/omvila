namespace Omvila.Modules.Scheduling;

/// <summary>appsettings "Scheduling" bölümü: sistem işlerinin çalışma aralıkları (teknik ayar; ADR-0005).</summary>
public sealed class SchedulingOptions
{
    public const string Section = "Scheduling";

    /// <summary>ExpireAppointmentHolds turu; iptal en fazla bu kadar gecikir (ADR-0005: dakikalık çözünürlük).</summary>
    public int ExpireAppointmentHoldsIntervalSeconds { get; set; }

    /// <summary>MarkNoShows turu.</summary>
    public int MarkNoShowsIntervalSeconds { get; set; }

    /// <summary>ExtendRecurringSeries turu; seri yerel 03:00'ten sonraki ilk turda ilerler.</summary>
    public int ExtendRecurringSeriesIntervalMinutes { get; set; }

    /// <summary>ExpireWaitlistOffers turu (durum-makinesi.md § 3: her dakika).</summary>
    public int ExpireWaitlistOffersIntervalSeconds { get; set; }

    /// <summary>ExpireWaitlistEntries turu (istenen günleri geçen bekleme kayıtları; T-90).</summary>
    public int ExpireWaitlistEntriesIntervalMinutes { get; set; }

    /// <summary>SendDailySummaries turu; gün sonu özeti şubenin yerel 20:00'sinden en fazla bu kadar sonra yazılır (T-120).</summary>
    public int SendDailySummariesIntervalSeconds { get; set; }

    internal bool IsValid() =>
        ExpireAppointmentHoldsIntervalSeconds > 0 && MarkNoShowsIntervalSeconds > 0 && ExtendRecurringSeriesIntervalMinutes > 0
        && ExpireWaitlistOffersIntervalSeconds > 0 && ExpireWaitlistEntriesIntervalMinutes > 0 && SendDailySummariesIntervalSeconds > 0;
}
