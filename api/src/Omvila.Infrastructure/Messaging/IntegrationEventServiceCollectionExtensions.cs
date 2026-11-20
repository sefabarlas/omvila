using Microsoft.Extensions.DependencyInjection;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Messaging;

public static class IntegrationEventServiceCollectionExtensions
{
    /// <summary>Bir olay işleyicisini kaydeder. Bir olayın birden çok işleyicisi olabilir.</summary>
    public static IServiceCollection AddIntegrationEventHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : IIntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        services.AddScoped<THandler>();
        services.AddSingleton(new HandlerRegistration(
            typeof(TEvent),
            typeof(THandler).FullName ?? typeof(THandler).Name,
            (provider, integrationEvent, ct) => provider.GetRequiredService<THandler>().HandleAsync((TEvent)integrationEvent, ct)));
        return services;
    }
}
