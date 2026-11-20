using Omvila.Infrastructure.Persistence;

namespace Omvila.Infrastructure.Tests;

public sealed class MigrationBuilderExtensionsTests
{
    [Theory]
    [InlineData("Brand")]
    [InlineData("items; DROP TABLE x")]
    [InlineData("")]
    public void TenantIsolationSql_RejectsUnsafeIdentifiers(string table) =>
        Should.Throw<ArgumentException>(() => MigrationBuilderExtensions.TenantIsolationSql("scheduling", table));

    [Fact]
    public void TenantIsolationSql_WithCustomerColumn_AddsCustomerClause()
    {
        var sql = MigrationBuilderExtensions.TenantIsolationSql("scheduling", "appointments", "customer_id");

        sql.ShouldContain("FORCE ROW LEVEL SECURITY");
        sql.ShouldContain("\"customer_id\" = nullif(current_setting('app.customer_id', true), '')::uuid");
    }
}
