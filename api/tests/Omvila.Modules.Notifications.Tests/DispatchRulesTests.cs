using NodaTime;
using Omvila.Modules.Notifications.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Tests;

// Kural: dilim-03 kriter 10 (üstel geri çekilme, 5. denemede Failed), kriter 11 (onaylanana kadar OMVILA),
// alan-modeli.md § BrandSmsSender, § CreditTransaction (bakiye negatif olamaz).
public sealed class DispatchRulesTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);
    private static readonly Duration Base = Duration.FromSeconds(30);
    private static readonly Duration Max = Duration.FromHours(1);

    [Theory]
    [InlineData(1, 30)]
    [InlineData(2, 60)]
    [InlineData(3, 120)]
    [InlineData(4, 240)]
    [InlineData(8, 3600)]
    [InlineData(100, 3600)]
    public void Backoff_DoublesFromBase_UpToMax(int failedAttempts, int expectedSeconds) =>
        RetryBackoff.Delay(failedAttempts, Base, Max).ShouldBe(Duration.FromSeconds(expectedSeconds));

    [Fact]
    public void Message_FailsOnFifthAttempt_RetryingWithBackoffBefore()
    {
        var message = Message();
        var at = Now;
        for (var attempt = 1; attempt <= 4; attempt++)
        {
            message.MarkAttemptFailed("provider_down", at, Backoff, maxAttempts: 5);
            message.Status.ShouldBe(NotificationStatus.Pending);
            message.ScheduledUtc.ShouldBe(at + RetryBackoff.Delay(attempt, Base, Max));
            at = message.ScheduledUtc;
        }

        message.MarkAttemptFailed("provider_down", at, Backoff, maxAttempts: 5);

        message.Status.ShouldBe(NotificationStatus.Failed);
        message.AttemptCount.ShouldBe(5);
    }

    [Fact]
    public void PermanentFailure_FailsImmediately()
    {
        var message = Message();

        message.MarkAttemptFailed("invalid_number", Now, Backoff, maxAttempts: 5, permanent: true);

        message.Status.ShouldBe(NotificationStatus.Failed);
    }

    [Fact]
    public void DeliveryFailed_ReturnsCreditsForRefund_AndSchedulesResend_OnlyOnce()
    {
        var message = Message();
        message.MarkSent("metin", "fake-1", credits: 1);

        message.MarkDeliveryFailed("undelivered", Now, Backoff, maxAttempts: 5).ShouldBe(1);

        message.Status.ShouldBe(NotificationStatus.Pending);
        message.CreditsConsumed.ShouldBe(0);
        message.ScheduledUtc.ShouldBe(Now + Base);
        message.MarkDeliveryFailed("undelivered", Now, Backoff, maxAttempts: 5).ShouldBeNull("tekrar gelen rapor yok sayılır");
    }

    [Fact]
    public void DeliveryFailed_OnFifthSend_Fails()
    {
        var message = Message();
        for (var attempt = 1; attempt <= 4; attempt++)
        {
            message.MarkSent("metin", $"fake-{attempt}", credits: 1);
            message.MarkDeliveryFailed("undelivered", Now, Backoff, maxAttempts: 5);
        }

        message.MarkSent("metin", "fake-5", credits: 1);
        message.MarkDeliveryFailed("undelivered", Now, Backoff, maxAttempts: 5).ShouldBe(1);

        message.Status.ShouldBe(NotificationStatus.Failed);
    }

    [Fact]
    public void Delivered_IsRecordedOnce_AndBlocksLateFailure()
    {
        var message = Message();
        message.MarkSent("metin", "fake-1", credits: 1);

        message.MarkDelivered(Now).ShouldBeTrue();
        message.MarkDelivered(Now).ShouldBeFalse();
        message.MarkDeliveryFailed("undelivered", Now, Backoff, 5).ShouldBeNull();
        message.Status.ShouldBe(NotificationStatus.Sent);
    }

    [Fact]
    public void SentMessage_CannotBeSentOrSkippedAgain()
    {
        var message = Message();
        message.MarkSent("metin", "fake-1");

        Should.Throw<DomainException>(() => message.MarkSent("metin", "fake-2"));
        Should.Throw<DomainException>(() => message.MarkSkipped("insufficient_credits"));
    }

    [Fact]
    public void EffectiveSender_IsDefaultUntilApproved_AndPreviousApprovedNameSurvivesNewApplication()
    {
        BrandSmsSender.EffectiveName(null, "OMVILA").ShouldBe("OMVILA");

        var sender = BrandSmsSender.Submit(Guid.Empty, "KUAFORELIF", Now);
        sender.Status.ShouldBe(SmsSenderStatus.Pending);
        BrandSmsSender.EffectiveName(sender, "OMVILA").ShouldBe("OMVILA");

        sender.Approve(Now);
        BrandSmsSender.EffectiveName(sender, "OMVILA").ShouldBe("KUAFORELIF");

        sender.Resubmit("ELIF2", Now);
        BrandSmsSender.EffectiveName(sender, "OMVILA").ShouldBe("KUAFORELIF");
        sender.Reject("Belge eksik", Now);
        BrandSmsSender.EffectiveName(sender, "OMVILA").ShouldBe("KUAFORELIF");
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("KUAFORELIF12")]
    [InlineData("kuafor")]
    [InlineData("12345")]
    [InlineData("KUAFÖR")]
    public void SenderName_OutsidePattern_IsRejected(string name) =>
        Should.Throw<DomainException>(() => BrandSmsSender.Submit(Guid.Empty, name, Now));

    [Fact]
    public void Consume_CannotTakeBalanceBelowZero()
    {
        Should.Throw<DomainException>(() => CreditTransaction.Consume(Guid.Empty, Guid.Empty, 2, 1, Guid.Empty, Now));

        var consume = CreditTransaction.Consume(Guid.Empty, Guid.Empty, 1, 1, Guid.Empty, Now);
        consume.Amount.ShouldBe(-1);
        consume.BalanceAfter.ShouldBe(0);
    }

    private static Duration Backoff(int failedAttempts) => RetryBackoff.Delay(failedAttempts, Base, Max);

    private static NotificationMessage Message() =>
        NotificationMessage.ForPhone(Guid.Empty, Guid.Empty, "appointment.reminder", "+905550000001", "{}", Now);
}
