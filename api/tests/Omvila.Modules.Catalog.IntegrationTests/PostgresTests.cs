using Omvila.TestKit;

namespace Omvila.Modules.Catalog.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class PostgresTests : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres";
}
