using NodaTime;
using Omvila.Modules.Scheduling.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: durum-makinesi.md § Arayüz gösterimi; alan-modeli.md § Appointment ("Kapora bekleniyor" bir durum değildir).
public sealed class DisplayStatusTests
{
    private static readonly Instant Received = Instant.FromUtc(2026, 11, 25, 9, 0);

    [Theory]
    [InlineData("Confirmed", "Confirmed")]
    [InlineData("CheckedIn", "CheckedIn")]
    [InlineData("Completed", "Completed")]
    [InlineData("Cancelled", "Cancelled")]
    [InlineData("NoShow", "NoShow")]
    public void SameNameAsStatus(string status, string expected) =>
        DisplayStatusRule.For(Enum.Parse<AppointmentStatus>(status), depositRequiredMinor: 20_000, depositReceivedUtc: null)
            .ShouldBe(Enum.Parse<DisplayStatus>(expected));

    [Fact]
    public void PendingApproval_WithoutDeposit_IsPendingApproval() =>
        DisplayStatusRule.For(AppointmentStatus.PendingApproval, null, null).ShouldBe(DisplayStatus.PendingApproval);

    [Fact]
    public void PendingApproval_WithAwaitedDeposit_IsDepositPending() =>
        DisplayStatusRule.For(AppointmentStatus.PendingApproval, 20_000, null).ShouldBe(DisplayStatus.DepositPending);

    [Fact]
    public void PendingApproval_WithReceivedDeposit_IsPendingApproval() =>
        DisplayStatusRule.For(AppointmentStatus.PendingApproval, 20_000, Received).ShouldBe(DisplayStatus.PendingApproval);

    [Fact]
    public void Draft_HasNoDisplay() =>
        Should.Throw<DomainException>(() => DisplayStatusRule.For(AppointmentStatus.Draft, null, null));
}
