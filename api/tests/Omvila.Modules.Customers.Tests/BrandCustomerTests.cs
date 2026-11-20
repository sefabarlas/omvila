using NodaTime;
using Omvila.Modules.Customers.Domain;

namespace Omvila.Modules.Customers.Tests;

// Kural: alan-modeli.md § BrandCustomer (FirstVisitOn, LastVisitOn, VisitCount, CancelCount, NoShowCount, TotalSpentMinor
// türetilmiş sayaçlardır; olaylarla güncellenir), durum-makinesi "Geldiyse geri al" (gelmeme geri alınır).
public sealed class BrandCustomerTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);

    private static BrandCustomer Card() =>
        BrandCustomer.Create(TestIds.Next(), TestIds.Next(), TestIds.Next(), " Selin Öztürk ", 1992, "  ", BrandCustomerSource.Panel, true, Now);

    [Fact]
    public void Create_TrimsName_DropsBlankNotes_StartsAtZero()
    {
        var card = Card();

        card.DisplayName.ShouldBe("Selin Öztürk");
        card.Notes.ShouldBeNull();
        card.VisitCount.ShouldBe(0);
        card.FirstVisitOn.ShouldBeNull();
        card.AppointmentSmsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Visits_ExtendFirstAndLastVisitDays()
    {
        var card = Card();

        card.ApplyCounters(1, 0, 0, 45_000, new LocalDate(2026, 10, 21));
        card.ApplyCounters(1, 0, 0, 30_000, new LocalDate(2026, 11, 25));
        card.ApplyCounters(1, 0, 0, 10_000, new LocalDate(2026, 9, 1));

        card.VisitCount.ShouldBe(3);
        card.TotalSpentMinor.ShouldBe(85_000);
        card.FirstVisitOn.ShouldBe(new LocalDate(2026, 9, 1));
        card.LastVisitOn.ShouldBe(new LocalDate(2026, 11, 25));
    }

    [Fact]
    public void UndoNoShow_DecrementsButNeverBelowZero()
    {
        var card = Card();
        card.ApplyCounters(0, 0, 1, 0, null);

        card.ApplyCounters(0, 0, -1, 0, null);
        card.ApplyCounters(0, -1, -1, -5, null);

        card.NoShowCount.ShouldBe(0);
        card.CancelCount.ShouldBe(0);
        card.TotalSpentMinor.ShouldBe(0);
    }
}
