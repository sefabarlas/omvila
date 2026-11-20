using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using Omvila.Infrastructure.Security;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Kayıt jetonu (sözleşme ManageAuthResult.signupToken): OTP ile doğrulanmış telefonu kayıt ucuna taşır.
/// ES256, 15 dk, ayrı aud ("manage-signup"): hiçbir API yüzeyinde erişim jetonu olarak geçmez.
/// </summary>
internal sealed class SignupTokenService(SigningKeyStore keys, IClock clock, IOptions<AuthOptions> options)
{
    public const string Audience = "manage-signup";

    private readonly JsonWebTokenHandler _handler = new() { SetDefaultTimesOnTokenCreation = false };

    public string Issue(string phoneE164)
    {
        var now = clock.GetCurrentInstant();
        return _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = options.Value.Issuer,
            Audience = Audience,
            Subject = new ClaimsIdentity([new Claim(OmvilaClaims.Phone, phoneE164)]),
            IssuedAt = now.ToDateTimeUtc(),
            NotBefore = now.ToDateTimeUtc(),
            Expires = (now + Duration.FromMinutes(options.Value.SignupTokenMinutes)).ToDateTimeUtc(),
            SigningCredentials = keys.SigningCredentials,
        });
    }

    /// <summary>Geçerliyse telefonu döner; değilse 401 unauthorized.</summary>
    public async Task<Result<string>> ValidateAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Error.Unauthorized();
        }

        var now = clock.GetCurrentInstant().ToDateTimeUtc();
        var result = await _handler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidIssuer = options.Value.Issuer,
            ValidAudience = Audience,
            IssuerSigningKeys = keys.Keys,
            ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256],
            ClockSkew = TimeSpan.Zero,
            LifetimeValidator = (notBefore, expires, _, _) => expires > now && (notBefore is null || notBefore <= now),
        });
        return result.IsValid && result.Claims.TryGetValue(OmvilaClaims.Phone, out var phone) && phone is string value
            ? value
            : Error.Unauthorized();
    }
}
