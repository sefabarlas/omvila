using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Email;
using Omvila.Infrastructure.Push;
using Omvila.Infrastructure.Sms;

namespace Omvila.Infrastructure.Hosting;

/// <summary>
/// Sahte SMS/push/e-posta sağlayıcısı seçiliyse ortam izin vermeli (<see cref="HostEnvironmentExtensions.AllowsFakeProviders"/>).
/// Açılışta denetlenir (ValidateOnStart). Ortam kayıtlı değilse (barındırıcısız test servis koleksiyonu) denetim yapılmaz:
/// gerçek barındırıcı her zaman IHostEnvironment kaydeder.
/// </summary>
internal sealed class FakeProviderOptionsValidator(IOptions<StagingOptions> staging, IHostEnvironment? environment = null)
    : IValidateOptions<SmsOptions>, IValidateOptions<PushOptions>, IValidateOptions<EmailOptions>
{
    public ValidateOptionsResult Validate(string? name, SmsOptions options) =>
        Check(string.Equals(options.Provider, SmsOptions.FakeProvider, StringComparison.Ordinal), "Sms:Provider");

    public ValidateOptionsResult Validate(string? name, PushOptions options) =>
        Check(string.Equals(options.Provider, PushOptions.FakeProvider, StringComparison.Ordinal), "Push:Provider");

    public ValidateOptionsResult Validate(string? name, EmailOptions options) =>
        Check(string.Equals(options.Provider, EmailOptions.FakeProvider, StringComparison.Ordinal), "Email:Provider");

    private ValidateOptionsResult Check(bool isFake, string key)
    {
        if (!isFake || environment is null || environment.AllowsFakeProviders(staging.Value))
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail(environment.IsStaging()
            ? $"{key}=Fake Staging'de yalnız Staging:AllowFakeProviders=true iken kullanılabilir (ortam değişkeni Staging__AllowFakeProviders)."
            : $"{key}=Fake {environment.EnvironmentName} ortamında kullanılamaz (yalnız Development, Testing ve izinli Staging).");
    }
}
