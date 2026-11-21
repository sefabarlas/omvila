using NodaTime;
using Omvila.Modules.Customers.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Customers.Tests;

// Kural: alan-modeli.md § Birleştirme (sayaçlar, notlar, kaynak arşivlenir), guvenlik-kvkk.md § Anonimleştirme ("Silinmiş
// müşteri", +000…{id hash}, notlar null), alan-modeli.md § DataDeletionRequest (tamamlanan talepte maskeli ad), § CustomerImport
// (süresi dolan önizleme hold_expired).
public sealed class CustomerRecordRulesTests
{
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);
    private static readonly Guid Brand = TestIds.Next();

    private static BrandCustomer Card(string name, string? notes, Instant created) =>
        BrandCustomer.Create(TestIds.Next(), Brand, TestIds.Next(), name, null, notes, BrandCustomerSource.Panel, true, created);

    [Fact]
    public void Absorb_SumsCounters_WidensVisitDays_MergesNotesOldestFirst_ArchivesSource()
    {
        var target = Card("Ayşe Y.", "Hedef", Now);
        var source = Card("Ayşe Yılmaz", "Kaynak", Now - Duration.FromDays(10));
        target.ApplyCounters(3, 0, 1, 150_000, new LocalDate(2026, 9, 1));
        source.ApplyCounters(2, 1, 2, 90_000, new LocalDate(2026, 10, 1));
        source.ApplyCounters(0, 0, 0, 0, null);

        target.Absorb(source, mergeNotes: true, Now).IsSuccess.ShouldBeTrue();

        target.VisitCount.ShouldBe(5);
        target.CancelCount.ShouldBe(1);
        target.NoShowCount.ShouldBe(3);
        target.TotalSpentMinor.ShouldBe(240_000);
        target.FirstVisitOn.ShouldBe(new LocalDate(2026, 9, 1));
        target.LastVisitOn.ShouldBe(new LocalDate(2026, 10, 1));
        target.Notes.ShouldBe("Kaynak\n\nHedef");
        source.MergedIntoId.ShouldBe(target.Id);
        source.MergedUtc.ShouldBe(Now);
        source.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Absorb_Rejections()
    {
        var target = Card("Ayşe Y.", new string('a', 1500), Now);
        var source = Card("Ayşe Yılmaz", new string('b', 600), Now);

        target.Absorb(target, true, Now).Error!.Code.ShouldBe(ErrorCodes.ValidationFailed);
        target.Absorb(source, true, Now).Error!.FieldErrors["mergeNotes"].ShouldBe(["too_long"]);
        target.Absorb(source, mergeNotes: false, Now).IsSuccess.ShouldBeTrue("notlar birleşmezse sınır yok");
        target.Notes!.Length.ShouldBe(1500);
        Card("Derya", null, Now).Absorb(source, false, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition, "kaynak zaten arşivde");
    }

    [Fact]
    public void Anonymize_ClearsPersonalData_LinksAnonymousPerson_Once()
    {
        var card = Card("Ayşe Yılmaz", "Boya tonu 7.1", Now);
        card.FillBlanks(null, 1990);
        var anonymous = Customer.CreateAnonymous(TestIds.Next(), Now);

        card.Anonymize(anonymous.Id, Now).IsSuccess.ShouldBeTrue();

        card.DisplayName.ShouldBe("Silinmiş müşteri");
        card.Notes.ShouldBeNull();
        card.BirthYear.ShouldBeNull();
        card.CustomerId.ShouldBe(anonymous.Id);
        card.AnonymizedUtc.ShouldBe(Now);
        card.Anonymize(anonymous.Id, Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }

    [Fact]
    public void AnonymousPerson_PhoneIsUniquePerId_NotAPhoneNumber()
    {
        var a = Customer.CreateAnonymous(TestIds.Next(), Now);
        var b = Customer.CreateAnonymous(TestIds.Next(), Now);

        a.FullName.ShouldBe("Silinmiş müşteri");
        a.PhoneE164.ShouldStartWith("+000");
        a.PhoneE164.Length.ShouldBe(20);
        string.Equals(a.PhoneE164, b.PhoneE164, StringComparison.Ordinal).ShouldBeFalse();
        PhoneNumber.TryParse(a.PhoneE164, out _).ShouldBeFalse("gösterilmez");
        a.AnonymizedUtc.ShouldBe(Now);
    }

    [Fact]
    public void FillBlanks_KeepsExistingValues()
    {
        var card = Card("Selin", "Mevcut", Now);
        card.FillBlanks("Yeni", 1992);

        card.Notes.ShouldBe("Mevcut");
        card.BirthYear.ShouldBe(1992);
    }

    [Theory]
    [InlineData("Tuna Oral", "T•••• O••••")]
    [InlineData("  Ayşe  Nur Yılmaz ", "A•••• N•••• Y••••")]
    public void DeletionRequest_MasksName(string name, string masked) => DataDeletionRequest.MaskName(name).ShouldBe(masked);

    [Fact]
    public void Import_ExpiresAtBoundary_AndCommitsOnce()
    {
        var import = CustomerImport.Preview(TestIds.Next(), Brand, "m.csv", 3, 2, 1, 0, "FullName,Phone", "[]", "[]", null, Now, Duration.FromMinutes(60));

        import.EnsureCommittable(Now + Duration.FromMinutes(59)).IsSuccess.ShouldBeTrue();
        import.EnsureCommittable(Now + Duration.FromMinutes(60)).Error!.Code.ShouldBe(ErrorCodes.HoldExpired);
        import.MarkCommitted(2, 1, sendInfoSms: true, Now);
        import.RowsJson.ShouldBeNull("uygulanınca kişisel veri kopyası silinir");
        import.EnsureCommittable(Now).Error!.Code.ShouldBe(ErrorCodes.InvalidStateTransition);
    }
}
