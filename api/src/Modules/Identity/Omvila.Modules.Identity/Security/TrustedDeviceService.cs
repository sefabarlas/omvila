using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// Güvenilir cihaz (ADR-0007 § Rezervasyon sayfası; TrustedDeviceDays). Çerez değeri 256 bit rastgele, yalnız SHA-256
/// özeti saklanır (yenileme jetonuyla aynı yöntem). İstek transaction'ı içinde çağrılır.
/// </summary>
internal sealed class TrustedDeviceService(IdentityDbContext db, IClock clock, IIdGenerator ids, IOptions<AuthOptions> options)
{
    public async Task<IssuedTrustedDevice> IssueAsync(string phoneE164, Guid verifiedOtpId, CancellationToken ct)
    {
        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        var device = TrustedDevice.Create(
            ids.New(), phoneE164, RefreshTokenService.Hash(token), verifiedOtpId, clock.GetCurrentInstant(), Duration.FromDays(options.Value.TrustedDeviceDays));
        db.TrustedDevices.Add(device);
        await db.SaveChangesAsync(ct);
        return new IssuedTrustedDevice(token, device.ExpiresUtc);
    }

    /// <summary>Çerez bu telefona ait ve süresi dolmamışsa cihazı kullanılmış işaretler ve döner; değilse null.</summary>
    public async Task<TrustedDevice?> UseAsync(string token, string phoneE164, CancellationToken ct)
    {
        var hash = RefreshTokenService.Hash(token);
        var device = await db.TrustedDevices.SingleOrDefaultAsync(d => d.TokenHash == hash, ct);
        var now = clock.GetCurrentInstant();
        if (device is null || !device.IsUsableFor(phoneE164, now))
        {
            return null;
        }

        device.MarkUsed(now);
        await db.SaveChangesAsync(ct);
        return device;
    }
}
