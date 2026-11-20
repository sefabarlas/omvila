namespace Omvila.Host.Tests;

public sealed class ProblemDetailsSetupTests
{
    [Theory]
    [InlineData(400, "validation_failed")]
    [InlineData(401, "unauthorized")]
    [InlineData(403, "forbidden")]
    [InlineData(404, "not_found")]
    [InlineData(405, "validation_failed")]
    [InlineData(412, "concurrency_conflict")]
    [InlineData(428, "precondition_required")]
    [InlineData(429, "rate_limited")]
    [InlineData(500, "try_again")]
    [InlineData(503, "try_again")]
    public void CodeForStatus_MapsToContractCode(int status, string expected) =>
        ProblemDetailsSetup.CodeForStatus(status).ShouldBe(expected);
}
