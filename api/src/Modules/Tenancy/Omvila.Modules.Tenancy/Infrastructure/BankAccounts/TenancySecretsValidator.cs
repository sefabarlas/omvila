using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Omvila.Modules.Tenancy.Infrastructure.BankAccounts;

/// <summary>
/// Açılışta (ValidateOnStart) IBAN şifreleme anahtarını denetler: Staging/Production'da eksik veya bozuk anahtar ilk IBAN
/// kaydında değil açılışta anlaşılır hatayla durur (IdentitySecretsValidator deseni, T-73). Ortam kayıtlı değilse
/// (barındırıcısız servis koleksiyonu) <see cref="IbanProtector"/> denetler.
/// </summary>
internal sealed class TenancySecretsValidator(IHostEnvironment? environment = null) : IValidateOptions<TenancyOptions>
{
    public ValidateOptionsResult Validate(string? name, TenancyOptions options) =>
        environment is null || IbanProtector.KeyError(options, environment) is not { } error
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(error);
}
