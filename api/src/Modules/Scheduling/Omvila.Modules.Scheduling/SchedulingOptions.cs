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

    internal bool IsValid() =>
        ExpireAppointmentHoldsIntervalSeconds > 0 && MarkNoShowsIntervalSeconds > 0 && ExtendRecurringSeriesIntervalMinutes > 0;
}
