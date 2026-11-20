using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Testing;
using Npgsql;
using Omvila.Infrastructure.Persistence;
using Omvila.Modules.Identity.Domain;
using Omvila.Modules.Identity.Security;
using Omvila.Modules.Notifications;
using Omvila.SharedKernel;
using Omvila.TestKit;

namespace Omvila.Modules.Identity.IntegrationTests;

// Kural: dilim-01 kabul kriteri 2 (yanlış kod → otp_invalid + kalan deneme; 5. yanlışta otp_attempts_exceeded ve
// 5 dk bekleme; 60 sn dolmadan tekrar kod → otp_resend_too_soon), ADR-0007, sureler-ve-ayarlar.md § OTP
[Trait("Category", "Integration")]
[Collection(PostgresTests.Name)]
public sealed class OtpTests(PostgresFixture db) : IAsyncLifetime
{
    private static readonly PhoneNumber Phone = PhoneNumber.Parse("+905550000001");

    private ServiceProvider _services = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private FakeClock Clock => (FakeClock)_services.GetRequiredService<IClock>();

    public async ValueTask InitializeAsync()
    {
        await using (var connection = await db.OpenMigratorConnectionAsync(Ct))
        {
            await using var command = new NpgsqlCommand("TRUNCATE identity.otp_challenges, notifications.notification_outbox", connection);
            await command.ExecuteNonQueryAsync(Ct);
        }

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(db.Settings()).Build();
        _services = db.CreateServices(configure: s => s.AddIdentityModule(configuration).AddNotificationsModule(configuration));
    }

    public async ValueTask DisposeAsync() => await _services.DisposeAsync();

    [Fact]
    public async Task Request_StoresOnlyHmac_AndQueuesOtpSms()
    {
        var issued = (await RequestAsync(OtpPurpose.ManageLogin)).Value;

        issued.ExpiresInSeconds.ShouldBe(180);
        issued.ResendAfterSeconds.ShouldBe(60);
        issued.PhoneMasked.ShouldBe("0555 *** ** 01");
        var code = await LatestCodeAsync();
        code.Length.ShouldBe(6);
        code.All(char.IsAsciiDigit).ShouldBeTrue();
        var hash = await ScalarAsync<string>($"SELECT code_hash FROM identity.otp_challenges WHERE id = '{issued.OtpId}'");
        hash.Length.ShouldBe(64);
        hash.Contains(code, StringComparison.Ordinal).ShouldBeFalse();
        (await ScalarAsync<string>("SELECT event_code FROM notifications.notification_outbox")).ShouldBe("otp.code");
    }

    [Fact]
    public async Task CorrectCode_Verifies_AndIsSingleUse()
    {
        var issued = (await RequestAsync(OtpPurpose.ManageLogin)).Value;
        var code = await LatestCodeAsync();

        var verified = await VerifyAsync(issued.OtpId, code, OtpPurpose.ManageLogin);
        var again = await VerifyAsync(issued.OtpId, code, OtpPurpose.ManageLogin);

        verified.Value.PhoneE164.ShouldBe(Phone.E164);
        again.Error!.Code.ShouldBe(ErrorCodes.OtpInvalid);
    }

    [Fact]
    public async Task WrongCodes_CountDown_FifthLocksForFiveMinutes_EvenThoughEachRequestRollsBack()
    {
        var issued = (await RequestAsync(OtpPurpose.ManageLogin)).Value;
        var code = await LatestCodeAsync();
        var wrong = string.Equals(code, "000000", StringComparison.Ordinal) ? "111111" : "000000";

        for (var left = 4; left >= 1; left--)
        {
            var result = await VerifyAsync(issued.OtpId, wrong, OtpPurpose.ManageLogin);
            result.Error!.Code.ShouldBe(ErrorCodes.OtpInvalid);
            result.Error.Meta["attemptsLeft"].ShouldBe(left);
        }

        var fifth = await VerifyAsync(issued.OtpId, wrong, OtpPurpose.ManageLogin);
        fifth.Error!.Code.ShouldBe(ErrorCodes.OtpAttemptsExceeded);
        fifth.Error.Kind.ShouldBe(ErrorKind.TooManyRequests);
        fifth.Error.Meta["retryAfterSeconds"].ShouldBe(300);

        (await VerifyAsync(issued.OtpId, code, OtpPurpose.ManageLogin)).Error!.Code.ShouldBe(ErrorCodes.OtpAttemptsExceeded, "doğru kod da artık geçmez");
        Clock.Advance(Duration.FromSeconds(61));
        (await RequestAsync(OtpPurpose.ManageLogin)).Error!.Code.ShouldBe(ErrorCodes.OtpAttemptsExceeded, "kilit süresince yeni kod yok");

        Clock.Advance(Duration.FromMinutes(5));
        (await RequestAsync(OtpPurpose.ManageLogin)).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ExpiredCode_ReturnsOtpExpired()
    {
        var issued = (await RequestAsync(OtpPurpose.ManageLogin)).Value;
        var code = await LatestCodeAsync();
        Clock.Advance(Duration.FromSeconds(180));

        (await VerifyAsync(issued.OtpId, code, OtpPurpose.ManageLogin)).Error!.Code.ShouldBe(ErrorCodes.OtpExpired);
    }

    [Fact]
    public async Task ResendWithinCooldown_ReturnsRemainingSeconds_PerPurpose()
    {
        (await RequestAsync(OtpPurpose.ManageLogin)).IsSuccess.ShouldBeTrue();
        Clock.Advance(Duration.FromSeconds(18));

        var tooSoon = await RequestAsync(OtpPurpose.ManageLogin);
        tooSoon.Error!.Code.ShouldBe(ErrorCodes.OtpResendTooSoon);
        tooSoon.Error.Meta["retryAfterSeconds"].ShouldBe(42);

        (await RequestAsync(OtpPurpose.ManageSignup)).IsSuccess.ShouldBeTrue("bekleme amaç başınadır");
        Clock.Advance(Duration.FromSeconds(42));
        (await RequestAsync(OtpPurpose.ManageLogin)).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task SixthRequestWithinHour_IsRateLimited()
    {
        for (var i = 0; i < 5; i++)
        {
            (await RequestAsync(OtpPurpose.ManageLogin)).IsSuccess.ShouldBeTrue($"istek {i + 1}");
            Clock.Advance(Duration.FromSeconds(61));
        }

        var sixth = await RequestAsync(OtpPurpose.ManageLogin);
        sixth.Error!.Code.ShouldBe(ErrorCodes.RateLimited);
        sixth.Error.Meta["retryAfterSeconds"].ShouldBe(3600 - (5 * 61));

        Clock.Advance(Duration.FromSeconds(3600 - (5 * 61)));
        (await RequestAsync(OtpPurpose.ManageLogin)).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task CodeIsBoundToPurpose()
    {
        var issued = (await RequestAsync(OtpPurpose.ManageLogin)).Value;
        var code = await LatestCodeAsync();

        (await VerifyAsync(issued.OtpId, code, OtpPurpose.PublicBooking)).Error!.Code.ShouldBe(ErrorCodes.OtpInvalid);
        (await VerifyAsync(issued.OtpId, code, OtpPurpose.ManageLogin)).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ConcurrentVerificationsOfSameCode_OnlyOneSucceeds()
    {
        var issued = (await RequestAsync(OtpPurpose.ManageLogin)).Value;
        var code = await LatestCodeAsync();

        var results = await Task.WhenAll(
            VerifyAsync(issued.OtpId, code, OtpPurpose.ManageLogin),
            VerifyAsync(issued.OtpId, code, OtpPurpose.ManageLogin));

        results.Count(r => r.IsSuccess).ShouldBe(1);
    }

    private Task<Result<OtpIssued>> RequestAsync(OtpPurpose purpose) =>
        InRequestAsync(service => service.RequestAsync(Phone, purpose, Ct), r => r.IsSuccess);

    private Task<Result<VerifiedOtp>> VerifyAsync(Guid otpId, string code, OtpPurpose purpose) =>
        InRequestAsync(service => service.VerifyAsync(otpId, code, purpose, Ct), r => r.IsSuccess);

    // Uç davranışı: başarıda commit, hata sonucunda rollback (TransactionEndpointFilter).
    private async Task<T> InRequestAsync<T>(Func<OtpService, Task<T>> work, Func<T, bool> success)
    {
        await using var scope = _services.CreateAsyncScope();
        var session = scope.ServiceProvider.GetRequiredService<IDbSession>();
        await session.BeginAsync(Ct);
        var result = await work(scope.ServiceProvider.GetRequiredService<OtpService>());
        if (success(result))
        {
            await session.CommitAsync(Ct);
        }
        else
        {
            await session.RollbackAsync(Ct);
        }

        return result;
    }

    private Task<string> LatestCodeAsync() =>
        ScalarAsync<string>("SELECT payload->>'kod' FROM notifications.notification_outbox ORDER BY created_utc DESC, id DESC LIMIT 1");

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var connection = await db.OpenMigratorConnectionAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
