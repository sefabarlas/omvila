namespace Omvila.Infrastructure.Jobs;

/// <summary>
/// Periyodik iş (ADR-0005). İş mantığı modülün Jobs/ sınıfındadır ve IClock alır; testte doğrudan
/// JobRunner.RunOnceAsync ile çağrılır. Her çalışma kendi kapsamında, platform kapsamlı (app.is_platform)
/// açık bir transaction içinde çözülür: DbContext kurucudan alınabilir. RunAsync başarıyla biterse commit,
/// istisnada rollback yapılır. İş, oturumu kendisi başlatmaz veya commit etmez.
/// Çalışma aralığı işin üyesi değildir: kayıtta yapılandırmadan seçilir
/// (<see cref="JobServiceCollectionExtensions.AddPeriodicJob{TJob, TOptions}"/>), böylece zamanlayıcı aralığı
/// veritabanı oturumu açmadan ve işi oluşturmadan okur.
/// </summary>
public interface IPeriodicJob
{
    /// <summary>Kararlı ad: advisory lock anahtarı ve job_runs kaydı (ör. "ExpireAppointmentHolds").</summary>
    string Key { get; }

    Task RunAsync(CancellationToken ct);
}
