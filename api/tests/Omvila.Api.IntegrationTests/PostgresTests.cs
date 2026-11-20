using Omvila.TestKit;

namespace Omvila.Api.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class PostgresTests : ICollectionFixture<PostgresFixture>
{
    public const string Name = "postgres";
}
