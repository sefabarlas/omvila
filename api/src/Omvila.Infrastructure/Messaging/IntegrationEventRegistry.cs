using System.Text.Json;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Omvila.SharedKernel;

namespace Omvila.Infrastructure.Messaging;

/// <summary>Olay türü adı (outbox'taki "type") ↔ CLR türü ve işleyiciler.</summary>
internal sealed class IntegrationEventRegistry(IEnumerable<HandlerRegistration> registrations)
{
    public static readonly JsonSerializerOptions Json =
        new JsonSerializerOptions(JsonSerializerDefaults.Web).ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

    private readonly ILookup<string, HandlerRegistration> _byTypeName =
        registrations.ToLookup(r => TypeName(r.EventType), StringComparer.Ordinal);

    public static string TypeName(Type eventType) => eventType.FullName ?? eventType.Name;

    public IReadOnlyList<HandlerRegistration> HandlersFor(string typeName) => [.. _byTypeName[typeName]];
}
