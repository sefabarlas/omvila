namespace Omvila.Infrastructure.Persistence;

/// <summary>Migration çalıştırıcısının bildiği DbContext.</summary>
internal sealed record MigratableContext(Type ContextType, string Schema);
