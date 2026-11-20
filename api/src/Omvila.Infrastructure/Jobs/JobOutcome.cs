namespace Omvila.Infrastructure.Jobs;

public enum JobOutcome
{
    Succeeded,
    Failed,

    /// <summary>Başka bir örnek aynı işi çalıştırıyor (advisory lock alınamadı).</summary>
    Skipped,
}
