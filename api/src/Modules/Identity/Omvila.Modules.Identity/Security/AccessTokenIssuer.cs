using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NodaTime;
using Omvila.Infrastructure.Security;

namespace Omvila.Modules.Identity.Security;

/// <summary>Erişim jetonu (ES256, ADR-0007). Zaman IClock'tan; testte sabit.</summary>
internal sealed class AccessTokenIssuer(SigningKeyStore keys, IClock clock, IOptions<AuthOptions> options)
{
    private readonly JsonWebTokenHandler _handler = new() { SetDefaultTimesOnTokenCreation = false };

    public (string Token, int ExpiresInSeconds) IssueManage(ManageTokenClaims claims)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(OmvilaClaims.Subject, claims.UserId.ToString()),
            new Claim(OmvilaClaims.BrandId, claims.BrandId.ToString()),
            new Claim(OmvilaClaims.MembershipId, claims.MembershipId.ToString()),
            new Claim(OmvilaClaims.Role, claims.Role),
        ]);
        if (claims.BranchIds is null)
        {
            identity.AddClaim(new Claim(OmvilaClaims.BranchIds, OmvilaClaims.AllBranches));
        }
        else
        {
            identity.AddClaims(claims.BranchIds.Select(id => new Claim(OmvilaClaims.BranchIds, id.ToString())));
        }

        if (claims.StaffId is { } staffId)
        {
            identity.AddClaim(new Claim(OmvilaClaims.StaffId, staffId.ToString()));
        }

        return Issue(Audiences.Manage, identity, Duration.FromMinutes(options.Value.AccessTokenMinutes));
    }

    /// <summary>
    /// Rezervasyon sayfası jetonu (ADR-0007 public-booking: phone, otp_id; PublicBookingTokenMinutes). Yalnız randevu
    /// oluşturma uçlarında geçerlidir; kiracı bağlamı taşımaz (marka yoldaki şubeden çözülür).
    /// </summary>
    public (string Token, int ExpiresInSeconds) IssuePublicBooking(string phoneE164, Guid otpId) =>
        Issue(
            Audiences.PublicBooking,
            new ClaimsIdentity([new Claim(OmvilaClaims.Phone, phoneE164), new Claim(OmvilaClaims.OtpId, otpId.ToString())]),
            Duration.FromMinutes(options.Value.PublicBookingTokenMinutes));

    /// <summary>
    /// Müşteri uygulaması jetonu (ADR-0007 me: sub = global Customer). Kiracı bağlamı müşteri RLS'i (TenantScope.ForCustomer);
    /// marka iddiası yoktur. Ömür salon jetonuyla aynı (AccessTokenMinutes).
    /// </summary>
    public (string Token, int ExpiresInSeconds) IssueMe(Guid customerId) =>
        Issue(
            Audiences.Me,
            new ClaimsIdentity([new Claim(OmvilaClaims.Subject, customerId.ToString())]),
            Duration.FromMinutes(options.Value.AccessTokenMinutes));

    private (string Token, int ExpiresInSeconds) Issue(string audience, ClaimsIdentity identity, Duration lifetime)
    {
        var now = clock.GetCurrentInstant();
        var token = _handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = options.Value.Issuer,
            Audience = audience,
            Subject = identity,
            IssuedAt = now.ToDateTimeUtc(),
            NotBefore = now.ToDateTimeUtc(),
            Expires = (now + lifetime).ToDateTimeUtc(),
            SigningCredentials = keys.SigningCredentials,
        });
        return (token, (int)lifetime.TotalSeconds);
    }
}
