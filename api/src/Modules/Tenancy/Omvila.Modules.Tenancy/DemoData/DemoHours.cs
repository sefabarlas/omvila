using NodaTime;

namespace Omvila.Modules.Tenancy.DemoData;

/// <summary>Haftalık çalışma aralığı (şube veya personel).</summary>
public sealed record DemoHours(IsoDayOfWeek Day, LocalTime Start, LocalTime End);
