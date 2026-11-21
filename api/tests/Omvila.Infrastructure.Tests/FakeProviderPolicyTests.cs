using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Hosting;
using Omvila.Infrastructure.Push;
using Omvila.Infrastructure.Sms;

namespace Omvila.Infrastructure.Tests;

// Kural: T-73 — sahte SMS/push Development ve Testing'de serbest, Staging'de yalnız Staging:AllowFakeProviders=true iken,
// Production'da hiçbir ayarla yok.
public sealed class FakeProviderPolicyTests
{
    [Theory]
    [InlineData("Development", false, true)]
    [InlineData("Testing", false, true)]
    [InlineData("Staging", true, true)]
    [InlineData("Staging", false, false)]
    [InlineData("Production", false, false)]
    [InlineData("Production", true, false)]
    [InlineData("Demo", true, false)]
    public void AllowsFakeProviders_DependsOnEnvironmentAndStagingFlag(string environment, bool flag, bool expected) =>
        new Env(environment).AllowsFakeProviders(new StagingOptions { AllowFakeProviders = flag }).ShouldBe(expected);

    [Theory]
    [InlineData("Development", true)]
    [InlineData("Testing", true)]
    [InlineData("Staging", false)]
    [InlineData("Production", false)]
    public void IsDevelopmentOrTesting_OnlyForLocalEnvironments(string environment, bool expected) =>
        new Env(environment).IsDevelopmentOrTesting().ShouldBe(expected);

    [Fact]
    public void Validator_ProductionWithFlag_RejectsFakeSmsAndPush()
    {
        var validator = Validator("Production", flag: true);

        validator.Validate(null, new SmsOptions { Provider = SmsOptions.FakeProvider }).Failed.ShouldBeTrue();
        validator.Validate(null, new PushOptions { Provider = PushOptions.FakeProvider }).Failed.ShouldBeTrue();
    }

    [Fact]
    public void Validator_StagingWithoutFlag_RejectsFakeAndNamesTheSetting()
    {
        var result = Validator("Staging", flag: false).Validate(null, new SmsOptions { Provider = SmsOptions.FakeProvider });

        result.Failed.ShouldBeTrue();
        result.FailureMessage.ShouldContain("Staging__AllowFakeProviders");
    }

    [Fact]
    public void Validator_StagingWithFlag_AllowsFakeSmsAndPush()
    {
        var validator = Validator("Staging", flag: true);

        validator.Validate(null, new SmsOptions { Provider = SmsOptions.FakeProvider }).Succeeded.ShouldBeTrue();
        validator.Validate(null, new PushOptions { Provider = PushOptions.FakeProvider }).Succeeded.ShouldBeTrue();
    }

    [Fact]
    public void Validator_ProductionWithoutFakeProvider_Succeeds()
    {
        var validator = Validator("Production", flag: false);

        validator.Validate(null, new SmsOptions { Provider = "" }).Succeeded.ShouldBeTrue();
        validator.Validate(null, new PushOptions { Provider = PushOptions.FcmProvider }).Succeeded.ShouldBeTrue();
    }

    private static FakeProviderOptionsValidator Validator(string environment, bool flag) =>
        new(Options.Create(new StagingOptions { AllowFakeProviders = flag }), new Env(environment));

    private sealed class Env(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;

        public string ApplicationName { get; set; } = "Omvila.Tests";

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
