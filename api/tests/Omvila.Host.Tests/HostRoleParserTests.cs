namespace Omvila.Host.Tests;

public sealed class HostRoleParserTests
{
    [Theory]
    [InlineData(null, "All")]
    [InlineData("", "All")]
    [InlineData("all", "All")]
    [InlineData("API", "Api")]
    [InlineData(" worker ", "Worker")]
    [InlineData("migrate", "Migrate")]
    [InlineData("seed", "Seed")]
    [InlineData("platform-user", "PlatformUser")]
    public void Parse_ReturnsRole(string? value, string expected) =>
        HostRoleParser.Parse(value).ToString().ShouldBe(expected);

    [Fact]
    public void Migrate_IsNotPartOfAll() =>
        HostRole.All.HasFlag(HostRole.Migrate).ShouldBeFalse();

    [Fact]
    public void Parse_WithUnknownRole_Throws() =>
        Should.Throw<ArgumentException>(() => HostRoleParser.Parse("cron"));
}
