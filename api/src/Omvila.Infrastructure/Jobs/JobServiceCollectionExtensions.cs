using Microsoft.Extensions.DependencyInjection;

namespace Omvila.Infrastructure.Jobs;

public static class JobServiceCollectionExtensions
{
    /// <summary>Periyodik işi kaydeder. İşler yalnız worker rolünde zamanlanır (JobHostedService).</summary>
    public static IServiceCollection AddPeriodicJob<TJob>(this IServiceCollection services)
        where TJob : class, IPeriodicJob
    {
        services.AddScoped<TJob>();
        services.AddSingleton(new JobRegistration(typeof(TJob)));
        return services;
    }
}
