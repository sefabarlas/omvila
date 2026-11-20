namespace Omvila.Infrastructure.Persistence;

/// <summary>Uç tek transaction'da çalışır (MapApiGroup ekler). DbSessionMiddleware oturumu açar, filtre kapatır.</summary>
internal sealed record TransactionalEndpointMetadata;
