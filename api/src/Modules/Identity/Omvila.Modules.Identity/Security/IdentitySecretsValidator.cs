using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Açılışta (ValidateOnStart) imza ve OTP anahtarını denetler: Staging/Production'da eksik gizli değer ilk istekte değil
/// açılışta anlaşılır hatayla durur. Ortam kayıtlı değilse (barındırıcısız test servis koleksiyonu) anahtar sınıfları denetler.
/// </summary>
internal sealed class IdentitySecretsValidator(IHostEnvironment? environment = null)
    : IValidateOptions<AuthOptions>, IValidateOptions<OtpOptions>
{
    public ValidateOptionsResult Validate(string? name, AuthOptions options) =>
        environment is null ? ValidateOptionsResult.Success : Result(IdentitySecrets.SigningKeysError(options, environment));

    public ValidateOptionsResult Validate(string? name, OtpOptions options) =>
        environment is null ? ValidateOptionsResult.Success : Result(IdentitySecrets.OtpKeyError(options, environment));

    private static ValidateOptionsResult Result(string? error) =>
        error is null ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(error);
}
