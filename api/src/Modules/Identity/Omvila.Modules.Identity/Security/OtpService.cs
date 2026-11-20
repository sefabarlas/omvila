using System.Globalization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Infrastructure;
using Omvila.Modules.Notifications.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Identity.Security;

/// <summary>
/// SMS kodu (ADR-0007, sureler-ve-ayarlar.md § OTP). İstek ve doğrulama istek transaction'ı içinde çağrılır.
/// Yanlış deneme sayacı ayrı transaction'da yazılır: istek 4xx döndüğü için kendi transaction'ı geri alınır,
/// sayaç alınmamalıdır. Kod bildirim outbox'ı üzerinden SMS olarak gider (bildirimler.md otp.code).
/// </summary>
internal sealed class OtpService(
    IdentityDbContext db,
    IDbSession session,
    OtpHasher hasher,
    INotificationOutbox notifications,
    IClock clock,
    IIdGenerator ids,
    IOptions<OtpOptions> options,
    IServiceScopeFactory scopes)
{
    public async Task<Result<OtpIssued>> RequestAsync(PhoneNumber phone, OtpPurpose purpose, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(phone);
        var settings = options.Value;
        await LockPhoneAsync(phone.E164, ct);

        var now = clock.GetCurrentInstant();
        var windowStart = now - Duration.FromHours(1);
        var recent = await db.OtpChallenges.AsNoTracking()
            .Where(c => c.PhoneE164 == phone.E164 && c.CreatedUtc > windowStart)
            .ToListAsync(ct);

        // Kural: deneme sınırı aşıldıysa kilit süresince yeni kod yok ("5 dakika sonra yeni kod isteyebilirsiniz").
        var lockedUntil = recent.Max(c => c.LockedUntilUtc);
        if (lockedUntil > now)
        {
            return TooMany(ErrorCodes.OtpAttemptsExceeded, lockedUntil.Value - now);
        }

        // Kural: aynı amaç için yeniden gönderme beklemesi ("0:42 sonra tekrar gönderebilirsiniz").
        var latest = recent.Where(c => c.Purpose == purpose).MaxBy(c => c.CreatedUtc);
        var cooldown = Duration.FromSeconds(settings.ResendCooldownSeconds);
        if (latest is not null && now - latest.CreatedUtc < cooldown)
        {
            return TooMany(ErrorCodes.OtpResendTooSoon, latest.CreatedUtc + cooldown - now);
        }

        // Kural: telefon başına saatte en fazla N kod (IP sınırı uçta, hız sınırlayıcıyla).
        if (recent.Count >= settings.MaxPerPhonePerHour)
        {
            return TooMany(ErrorCodes.RateLimited, recent.Min(c => c.CreatedUtc) + Duration.FromHours(1) - now);
        }

        var code = NewCode(settings.Length);
        var challenge = OtpChallenge.Create(
            ids.New(), phone.E164, purpose, hasher.Hash(purpose, phone.E164, code), now, Duration.FromSeconds(settings.TtlSeconds));
        db.OtpChallenges.Add(challenge);
        await db.SaveChangesAsync(ct);
        await notifications.EnqueueSmsAsync(
            new SmsNotification(NotificationEvents.OtpCode, phone.E164, new Dictionary<string, string>(StringComparer.Ordinal) { ["kod"] = code }),
            ct);

        return new OtpIssued(challenge.Id, settings.TtlSeconds, settings.ResendCooldownSeconds, phone.Masked);
    }

    public async Task<Result<VerifiedOtp>> VerifyAsync(Guid otpId, string code, OtpPurpose purpose, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(code);
        var settings = options.Value;
        var now = clock.GetCurrentInstant();
        var challenge = await db.OtpChallenges.AsNoTracking().SingleOrDefaultAsync(c => c.Id == otpId, ct);
        if (challenge is null || challenge.Purpose != purpose || challenge.ConsumedUtc is not null)
        {
            return Error.Unprocessable(ErrorCodes.OtpInvalid);
        }

        if (challenge.LockedUntilUtc > now || challenge.AttemptCount >= settings.MaxAttempts)
        {
            return TooMany(ErrorCodes.OtpAttemptsExceeded, (challenge.LockedUntilUtc ?? now) - now);
        }

        if (now >= challenge.ExpiresUtc)
        {
            return Error.Unprocessable(ErrorCodes.OtpExpired);
        }

        if (!hasher.Matches(challenge.CodeHash, purpose, challenge.PhoneE164, code))
        {
            var attempts = await RecordFailedAttemptAsync(challenge.Id, now, ct);
            return attempts >= settings.MaxAttempts
                ? TooMany(ErrorCodes.OtpAttemptsExceeded, Duration.FromMinutes(settings.LockoutMinutes))
                : Error.Unprocessable(ErrorCodes.OtpInvalid, Meta("attemptsLeft", settings.MaxAttempts - attempts));
        }

        // Tek kullanımlık: aynı kodla eşzamanlı iki doğrulamadan yalnız biri geçer.
        var consumed = await db.OtpChallenges
            .Where(c => c.Id == otpId && c.ConsumedUtc == null && c.AttemptCount < settings.MaxAttempts)
            .ExecuteUpdateAsync(set => set.SetProperty(c => c.ConsumedUtc, now), ct);
        return consumed == 1
            ? new VerifiedOtp(challenge.PhoneE164, purpose)
            : Error.Unprocessable(ErrorCodes.OtpInvalid);
    }

    private static string NewCode(int length) =>
        RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, length)).ToString($"D{length}", CultureInfo.InvariantCulture);

    private static Error TooMany(string code, Duration retryAfter) =>
        Error.TooManyRequests(code, Meta("retryAfterSeconds", (int)Math.Ceiling(Math.Max(retryAfter.TotalSeconds, 0))));

    private static Dictionary<string, object?> Meta(string key, object value) =>
        new(StringComparer.Ordinal) { [key] = value };

    // Aynı telefonun eşzamanlı istekleri sıralanır: bekleme ve saatlik sınır atlanamaz.
    private async Task LockPhoneAsync(string phoneE164, CancellationToken ct)
    {
        await using var command = new NpgsqlCommand("SELECT pg_advisory_xact_lock(hashtextextended($1, 0))", session.Connection, session.Transaction);
        command.Parameters.Add(new NpgsqlParameter { Value = $"otp:{phoneE164}" });
        await command.ExecuteNonQueryAsync(ct);
    }

    private async Task<int> RecordFailedAttemptAsync(Guid otpId, Instant now, CancellationToken ct)
    {
        var settings = options.Value;
        var lockUntil = now + Duration.FromMinutes(settings.LockoutMinutes);
        await using var scope = scopes.CreateAsyncScope();
        var independent = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await independent.BeginAsync(ct);
        var attempts = await scope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database
            .SqlQuery<int>($"""
                UPDATE identity.otp_challenges
                SET attempt_count = attempt_count + 1,
                    locked_until_utc = CASE WHEN attempt_count + 1 >= {settings.MaxAttempts} THEN {lockUntil} ELSE locked_until_utc END
                WHERE id = {otpId}
                RETURNING attempt_count AS "Value"
                """)
            .ToListAsync(ct);
        await independent.CommitAsync(ct);
        return attempts.Single();
    }
}
