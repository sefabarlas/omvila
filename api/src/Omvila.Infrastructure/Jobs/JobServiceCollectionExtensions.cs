using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Omvila.Infrastructure.Jobs;

public static class JobServiceCollectionExtensions
{
    /// <summary>
    /// Periyodik işi kaydeder. İşler yalnız worker rolünde zamanlanır (JobHostedService). Aralık
    /// <typeparamref name="TOptions"/> ayarından seçilir (sabit süre yok); zamanlayıcı onu açılışta, işi oluşturmadan ve
    /// veritabanına bağlanmadan okur. Ayar eksik veya geçersizse (options doğrulaması, sıfır veya eksi aralık) worker
    /// açılışta hata ile durur.
    /// </summary>
    public static IServiceCollection AddPeriodicJob<TJob, TOptions>(
        this IServiceCollection services, Func<TOptions, Duration> interval)
        where TJob : class, IPeriodicJob
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(interval);
        services.AddScoped<TJob>();
        services.AddSingleton(new JobRegistration(
            typeof(TJob), provider => interval(provider.GetRequiredService<IOptions<TOptions>>().Value)));
        return services;
    }
}
