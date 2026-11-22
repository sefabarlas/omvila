using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Hosting;
using Omvila.Infrastructure.Security;

namespace Omvila.Modules.Integrations.Calendar;

/// <summary>
/// Açılışta (ValidateOnStart): jeton şifreleme anahtarı (Staging/Production'da zorunlu; T-73 deseni) ve sahte sağlayıcının
/// ortam izni (FakeProviderOptionsValidator gibi: Development, Testing ve Staging:AllowFakeProviders=true).
/// </summary>
internal sealed class CalendarOptionsValidator(IOptions<StagingOptions> staging, IHostEnvironment? environment = null) : IValidateOptions<CalendarOptions>
{
    public ValidateOptionsResult Validate(string? name, CalendarOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (environment is null)
        {
            return ValidateOptionsResult.Success;
        }

        if (ConfiguredSecretKey.Error(options.TokenEncryptionKey, CalendarOptions.TokenEncryptionKeySetting, environment) is { } keyError)
        {
            return ValidateOptionsResult.Fail(keyError);
        }

        if (string.Equals(options.Provider, CalendarOptions.FakeProvider, StringComparison.Ordinal) && !environment.AllowsFakeProviders(staging.Value))
        {
            return ValidateOptionsResult.Fail(
                $"Integrations:Calendar:Provider=Fake {environment.EnvironmentName} ortamında kullanılamaz (yalnız Development, Testing ve izinli Staging).");
        }

        return ValidateOptionsResult.Success;
    }
}
