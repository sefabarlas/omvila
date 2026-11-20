namespace Omvila.Infrastructure.Messaging;

/// <summary>Bir olay türü için kayıtlı işleyici.</summary>
internal sealed record HandlerRegistration(
    Type EventType,
    string Name,
    Func<IServiceProvider, object, CancellationToken, Task> Invoke);
