using NodaTime;
using Omvila.Modules.Notifications.Domain;

namespace Omvila.Modules.Notifications.Tests;

// Kural: sözleşme InAppNotification (title ≤ 80, body ≤ 200) ve manageMarkNotificationsRead (okunmuş bildirimin readUtc'si değişmez).
public sealed class InAppNotificationTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);

    [Fact]
    public void MarkRead_IsIdempotent()
    {
        var notification = Create("SMS kontörü azaldı", "180 SMS kaldı, tahmini 3 gün yeter.");

        notification.MarkRead(Now);
        notification.MarkRead(Now + Duration.FromMinutes(5));

        notification.ReadUtc.ShouldBe(Now);
    }

    [Fact]
    public void OverlongTexts_AreTrimmedToTheContractLimits()
    {
        var notification = Create(new string('a', 100), new string('b', 250));

        notification.Title.Length.ShouldBe(InAppNotification.TitleMaxLength);
        notification.Body.Length.ShouldBe(InAppNotification.BodyMaxLength);
        notification.Body.ShouldEndWith("…");
    }

    private static InAppNotification Create(string title, string body) =>
        InAppNotification.Create(Guid.Empty, Guid.Empty, Guid.Empty, "owner.credits_low", title, body, null, "SmsCredits", null, null, Now);
}
