using Omvila.TestKit;

namespace Omvila.Infrastructure.IntegrationTests;

/// <summary>
/// Veritabanı testleri tek PostgreSQL kapsayıcısını paylaşır. Koleksiyon fixture'ı yalnız bu koleksiyondan
/// bir test çalışınca başlar; entegrasyon dışı çalıştırmalar Docker gerektirmez.
/// </summary>
[CollectionDefinition(Name)]
public sealed class PostgresTests : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres";
}
