namespace Omvila.Host.Tests;

public sealed class HostRoleParserTests
{
    [Theory]
    [InlineData(null, "All")]
    [InlineData("", "All")]
    [InlineData("all", "All")]
    [InlineData("API", "Api")]
    [InlineData(" worker ", "Worker")]
    public void Parse_ReturnsRole(string? value, string expected) =>
        HostRoleParser.Parse(value).ToString().ShouldBe(expected);

    [Fact]
    public void Parse_WithUnknownRole_Throws() =>
        Should.Throw<ArgumentException>(() => HostRoleParser.Parse("cron"));
}
