using NodaTime;
using Omvila.Modules.Scheduling.Domain.Waitlist;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Tests;

// Kural: durum-makinesi.md § 3 bekleme listesi (kayıt Waiting → Converted | Cancelled | Expired; teklif Pending → Accepted |
// Declined | Expired | Withdrawn), alan-modeli.md § WaitlistEntry/WaitlistOffer, T-90 (saat dilimi sınırları, kaydın
// Expired olması), sureler-ve-ayarlar.md WaitlistOfferHoldMinutes 15. Saat: 2026-11-25 09:00Z.
public sealed class WaitlistTransitionTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);
    private static readonly Duration Hold = Duration.FromMinutes(PlatformDefaults.WaitlistOfferHoldMinutes);
    private static readonly Guid Elif = Guid.Parse("0193a8f0-0000-7000-8000-0000000000e1");
    private static readonly Guid Mert = Guid.Parse("0193a8f0-0000-7000-8000-0000000000e2");
    private static readonly Guid Fon = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d1");
    private static readonly Interval At1600 = new(Instant.FromUtc(2026, 11, 25, 13, 0), Instant.FromUtc(2026, 11, 25, 13, 45));
    private static readonly LocalDate Today = new(2026, 11, 25);

    private readonly SequentialIds _ids = new();

    // ───────── Tercih eşleşmesi ─────────

    [Theory]
    [InlineData(11, 59, "Morning")]
    [InlineData(12, 0, "Afternoon")]
    [InlineData(16, 59, "Afternoon")]
    [InlineData(17, 0, "Evening")]
    [InlineData(8, 0, "Morning")]
    public void WindowOf_UsesNoonAndFivePmBoundaries(int hour, int minute, string expected) =>
        WaitlistPreferences.WindowOf(new LocalTime(hour, minute)).ToString().ShouldBe(expected);

    [Fact]
    public void Accepts_EmptyDatesAndWindows_MeanAnyDayAnyTime()
    {
        var any = Preferences(dates: [], windows: []);

        any.Accepts(Elif, Today.At(new LocalTime(9, 0))).ShouldBeTrue();
        any.Accepts(Elif, Today.PlusDays(30).At(new LocalTime(19, 30))).ShouldBeTrue();
    }

    [Fact]
    public void Accepts_RequiresListedDayAndWindow()
    {
        var p = Preferences(dates: [Today], windows: [WaitlistTimeWindow.Afternoon]);

        p.Accepts(Elif, Today.At(new LocalTime(16, 0))).ShouldBeTrue();
        p.Accepts(Elif, Today.At(new LocalTime(10, 0))).ShouldBeFalse("Sabah seçilmedi");
        p.Accepts(Elif, Today.PlusDays(1).At(new LocalTime(16, 0))).ShouldBeFalse("gün listede yok");
    }

    [Fact]
    public void Accepts_PreferredStaff_OtherStaffOnlyWhenAccepted()
    {
        Preferences(preferred: Elif).Accepts(Mert, Today.At(new LocalTime(16, 0))).ShouldBeFalse();
        Preferences(preferred: Elif).Accepts(Elif, Today.At(new LocalTime(16, 0))).ShouldBeTrue();
        Preferences(preferred: Elif, acceptOther: true).Accepts(Mert, Today.At(new LocalTime(16, 0))).ShouldBeTrue();
        Preferences(preferred: null).Accepts(Mert, Today.At(new LocalTime(16, 0))).ShouldBeTrue("fark etmez");
    }

    [Fact]
    public void SameServices_IgnoresOrderAndDuplicates()
    {
        var other = Guid.Parse("0193a8f0-0000-7000-8000-0000000000d2");
        var p = Preferences() with { ServiceIds = [Fon, other] };

        p.SameServices([other, Fon, Fon]).ShouldBeTrue();
        p.SameServices([Fon]).ShouldBeFalse();
    }

    // ───────── — → Pending ─────────

    [Fact]
    public void Offer_WaitingWithoutPendingOffer_HoldsFor15Minutes()
    {
        var entry = Entry();

        var offer = entry.Offer(_ids.New(), Elif, At1600, Hold, Code(1), Now, userId: null).Value;

        offer.Status.ShouldBe(WaitlistOfferStatus.Pending);
        offer.HoldExpiresUtc.ShouldBe(Now + Duration.FromMinutes(15));
        offer.OfferedStartUtc.ShouldBe(At1600.Start);
        offer.OfferedEndUtc.ShouldBe(At1600.End);
        offer.CustomerId.ShouldBe(entry.CustomerId);
        entry.PendingOffer.ShouldBeSameAs(offer);
        entry.WasOffered(Elif, At1600.Start).ShouldBeTrue();
    }

    [Fact]
    public void Offer_WithPendingOffer_OrNotWaiting_IsInvalidTransition()
    {
        var entry = Entry();
        entry.Offer(_ids.New(), Elif, At1600, Hold, Code(1), Now, null);

        entry.Offer(_ids.New(), Mert, At1600, Hold, Code(2), Now, null).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var left = Entry();
        left.Leave(Now);
        left.Offer(_ids.New(), Elif, At1600, Hold, Code(3), Now, null).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── Pending → Accepted ─────────

    [Fact]
    public void Accept_LiveOffer_OfferAccepted_EntryConverted()
    {
        var (entry, offer) = Offered();
        var appointmentId = _ids.New();

        entry.Accept(offer, appointmentId, Now + Duration.FromMinutes(14)).IsSuccess.ShouldBeTrue();

        offer.Status.ShouldBe(WaitlistOfferStatus.Accepted);
        offer.AppointmentId.ShouldBe(appointmentId);
        entry.Status.ShouldBe(WaitlistEntryStatus.Converted);
        entry.ClosedUtc.ShouldNotBeNull();
        entry.PendingOffer.ShouldBeNull();
    }

    [Fact]
    public void Accept_AfterHold_IsHoldExpired()
    {
        var (entry, offer) = Offered();

        entry.Accept(offer, _ids.New(), Now + Hold).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);
        entry.Status.ShouldBe(WaitlistEntryStatus.Waiting);
    }

    [Fact]
    public void Accept_DeclinedOrWithdrawn_IsInvalidTransition()
    {
        var (declinedEntry, declined) = Offered();
        declinedEntry.Decline(declined, Now);
        declinedEntry.Accept(declined, _ids.New(), Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);

        var (withdrawnEntry, withdrawn) = Offered();
        withdrawnEntry.WithdrawOffer(Now);
        withdrawnEntry.Accept(withdrawn, _ids.New(), Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── Pending → Declined ─────────

    [Fact]
    public void Decline_LiveOffer_Declined_PersonStaysWaiting_SecondDeclineIsNoOp()
    {
        var (entry, offer) = Offered();

        entry.Decline(offer, Now).Value.ShouldBeTrue();

        offer.Status.ShouldBe(WaitlistOfferStatus.Declined);
        entry.Status.ShouldBe(WaitlistEntryStatus.Waiting);
        entry.Decline(offer, Now).Value.ShouldBeFalse("zaten reddedildi: aynı sonuç");
    }

    [Fact]
    public void Decline_Expired_IsHoldExpired_Accepted_IsInvalidTransition()
    {
        var (late, lateOffer) = Offered();
        late.Decline(lateOffer, Now + Hold).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);

        var (accepted, acceptedOffer) = Offered();
        accepted.Accept(acceptedOffer, _ids.New(), Now);
        accepted.Decline(acceptedOffer, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── Pending → Expired ─────────

    [Fact]
    public void ExpireOffer_AfterHold_Expired_PersonStaysWaiting_CanReceiveNextOffer()
    {
        var (entry, offer) = Offered();

        entry.ExpireOffer(offer, Now + Duration.FromMinutes(14)).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
        entry.ExpireOffer(offer, Now + Hold).IsSuccess.ShouldBeTrue();

        offer.Status.ShouldBe(WaitlistOfferStatus.Expired);
        entry.Status.ShouldBe(WaitlistEntryStatus.Waiting);
        entry.Offer(_ids.New(), Elif, new Interval(At1600.Start + Duration.FromDays(1), At1600.End + Duration.FromDays(1)), Hold, Code(9), Now + Hold, null)
            .IsSuccess.ShouldBeTrue();
    }

    // ───────── Pending → Withdrawn ─────────

    [Fact]
    public void WithdrawOffer_Pending_Withdrawn_PersonStaysWaiting()
    {
        var (entry, offer) = Offered();

        entry.WithdrawOffer(Now).Value.ShouldBeSameAs(offer);

        offer.Status.ShouldBe(WaitlistOfferStatus.Withdrawn);
        entry.Status.ShouldBe(WaitlistEntryStatus.Waiting);
        entry.PendingOffer.ShouldBeNull();
        entry.WithdrawOffer(Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── Waiting → Cancelled ─────────

    [Fact]
    public void Leave_WithLiveOffer_Cancelled_OfferDeclinedAndReleased_SecondLeaveIsNoOp()
    {
        var (entry, offer) = Offered();

        entry.Leave(Now).Value.ShouldBeSameAs(offer);

        entry.Status.ShouldBe(WaitlistEntryStatus.Cancelled);
        offer.Status.ShouldBe(WaitlistOfferStatus.Declined);
        entry.Leave(Now).Value.ShouldBeNull();
        entry.Status.ShouldBe(WaitlistEntryStatus.Cancelled);
    }

    [Fact]
    public void Leave_Converted_IsInvalidTransition()
    {
        var (entry, offer) = Offered();
        entry.Accept(offer, _ids.New(), Now);

        entry.Leave(Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    // ───────── Waiting → Expired (T-90) ─────────

    [Fact]
    public void Expire_AllDatesPast_Expired_AnyDayNever_PendingOfferWaits()
    {
        var past = Entry(Preferences(dates: [Today.PlusDays(-2), Today.PlusDays(-1)]));
        past.Expire(Today, Now).IsSuccess.ShouldBeTrue();
        past.Status.ShouldBe(WaitlistEntryStatus.Expired);

        Entry(Preferences(dates: [Today.PlusDays(-1), Today])).Expire(Today, Now).IsSuccess.ShouldBeFalse("bugün hâlâ istenen gün");
        Entry(Preferences(dates: [])).Expire(Today.PlusDays(400), Now).IsSuccess.ShouldBeFalse("herhangi bir gün");

        var offered = Entry(Preferences(dates: [Today.PlusDays(-1)]));
        offered.Offer(_ids.New(), Elif, At1600, Hold, Code(5), Now, null);
        offered.Expire(Today, Now).IsSuccess.ShouldBeFalse("bekleyen teklif sonuçlanana kadar");
    }

    [Fact]
    public void UpdatePreferences_WaitingOnly()
    {
        var entry = Entry();
        entry.UpdatePreferences(Preferences(windows: [WaitlistTimeWindow.Evening]), Now).IsSuccess.ShouldBeTrue();
        entry.TimeWindows.ShouldBe([WaitlistTimeWindow.Evening]);

        entry.Leave(Now);
        entry.UpdatePreferences(Preferences(), Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    private (WaitlistEntry Entry, WaitlistOffer Offer) Offered()
    {
        var entry = Entry();
        return (entry, entry.Offer(_ids.New(), Elif, At1600, Hold, Code(7), Now, null).Value);
    }

    private WaitlistEntry Entry(WaitlistPreferences? preferences = null) =>
        WaitlistEntry.Create(_ids.New(), _ids.New(), _ids.New(), _ids.New(), _ids.New(), preferences ?? Preferences(), null, WaitlistSource.Online, Code(0), Now);

    private static WaitlistPreferences Preferences(
        Guid? preferred = null, bool acceptOther = false, IReadOnlyList<LocalDate>? dates = null, IReadOnlyList<WaitlistTimeWindow>? windows = null) =>
        new([Fon], preferred, acceptOther, dates ?? [Today], windows ?? [WaitlistTimeWindow.Afternoon]);

    private static string Code(int n) => $"Wtest{n:D5}";
}
