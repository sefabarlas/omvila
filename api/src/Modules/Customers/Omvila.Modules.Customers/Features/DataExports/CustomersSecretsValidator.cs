using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Omvila.Infrastructure.Security;

namespace Omvila.Modules.Customers.Features.DataExports;

/// <summary>Açılışta indirme bağlantısı imza anahtarını denetler (TenancySecretsValidator deseni, T-73).</summary>
internal sealed class CustomersSecretsValidator(IHostEnvironment? environment = null) : IValidateOptions<DataExportOptions>
{
    public ValidateOptionsResult Validate(string? name, DataExportOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return environment is null || ConfiguredSecretKey.Error(options.LinkSigningKey, DataExportOptions.LinkSigningKeySetting, environment) is not { } error
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(error);
    }
}
