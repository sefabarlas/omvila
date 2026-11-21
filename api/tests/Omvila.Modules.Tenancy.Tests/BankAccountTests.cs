using NodaTime;
using Omvila.Modules.Tenancy.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Tenancy.Tests;

// Kural: alan-modeli.md § BrandBankAccount, sureler-ve-ayarlar.md § Kapora (önek 2–5 büyük harf, havale süresi 60/120/240),
// sözleşme BankAccountWrite (iban ilk kayıtta zorunlu, sonra isteğe bağlı; geçersizse invalid_format) ve manageUpdateBankAccount
// (kayıttan sonra hesap aktif; iban gönderilmezse kayıtlı IBAN korunur).
public sealed class BankAccountTests
{
    private const string ValidIban = "TR29 0006 4000 0011 2345 6789 01";
    private static readonly Guid BrandId = Guid.Parse("0193a8f0-0000-7000-8000-000000000001");
    private static readonly Instant Now = Instant.FromUtc(2026, 11, 25, 9, 0);

    [Fact]
    public void Validate_FirstSaveWithAllFields_IsValid()
    {
        var errors = BankAccountRules.Validate("Elif Kaya", "Ziraat Bankası", "OMV", 120, ValidIban, isFirstSave: true, out var iban);

        errors.ShouldBeEmpty();
        iban!.Value.ShouldBe("TR290006400000112345678901");
    }

    [Fact]
    public void Validate_FirstSaveWithoutIban_RequiresIban() =>
        BankAccountRules.Validate("Elif Kaya", "Ziraat Bankası", "OMV", 120, null, isFirstSave: true, out _)["iban"].ShouldBe(["required"]);

    [Fact]
    public void Validate_UpdateWithoutIban_KeepsStoredIban()
    {
        var errors = BankAccountRules.Validate("Elif Kaya", "Ziraat Bankası", "OMV", 60, null, isFirstSave: false, out var iban);

        errors.ShouldBeEmpty();
        iban.ShouldBeNull();
    }

    [Fact]
    public void Validate_InvalidValues_AreReportedPerField()
    {
        var errors = BankAccountRules.Validate("E", new string('B', 81), "omv", 90, "TR12 0006 4000 0011 2345 6789 01", isFirstSave: true, out _);

        errors["accountHolderName"].ShouldBe(["too_short"]);
        errors["bankName"].ShouldBe(["too_long"]);
        errors["transferCodePrefix"].ShouldBe(["invalid_format"]);
        errors["transferWindowMinutes"].ShouldBe(["out_of_range"]);
        errors["iban"].ShouldBe(["invalid_format"]);
    }

    [Theory]
    [InlineData("O")]
    [InlineData("OMVILA")]
    [InlineData("OM1")]
    public void Validate_PrefixOutsideTwoToFiveUpperLetters_IsInvalid(string prefix) =>
        BankAccountRules.Validate("Elif Kaya", "Ziraat Bankası", prefix, 120, ValidIban, isFirstSave: true, out _)["transferCodePrefix"]
            .ShouldBe(["invalid_format"]);

    [Fact]
    public void Validate_MissingFields_AreRequired()
    {
        var errors = BankAccountRules.Validate(null, "  ", null, null, ValidIban, isFirstSave: true, out _);

        errors.Keys.Order(StringComparer.Ordinal).ShouldBe(["accountHolderName", "bankName", "transferCodePrefix", "transferWindowMinutes"]);
        errors.Values.ShouldAllBe(codes => codes.Single() == "required");
    }

    [Fact]
    public void Create_IsActive_AndTrimsNames()
    {
        var account = BrandBankAccount.Create(
            BrandId, new BankAccountValues("  Elif Kaya ", " Ziraat Bankası", "OMV", 120), new EncryptedIban([1, 2, 3], "8901"), Now);

        account.IsActive.ShouldBeTrue();
        account.AccountHolderName.ShouldBe("Elif Kaya");
        account.BankName.ShouldBe("Ziraat Bankası");
        account.IbanLast4.ShouldBe("8901");
        account.CreatedUtc.ShouldBe(Now);
        account.UpdatedUtc.ShouldBe(Now);
    }

    [Fact]
    public void Update_WithoutIban_KeepsStoredIban()
    {
        var account = BrandBankAccount.Create(
            BrandId, new BankAccountValues("Elif Kaya", "Ziraat Bankası", "OMV", 120), new EncryptedIban([1, 2, 3], "8901"), Now);
        var later = Now.Plus(Duration.FromHours(1));

        account.Update(new BankAccountValues("Kuaför Elif Ltd.", "Ziraat Bankası", "ELIF", 240), iban: null, later);

        account.IbanEncrypted.ShouldBe(new byte[] { 1, 2, 3 });
        account.IbanLast4.ShouldBe("8901");
        account.TransferCodePrefix.ShouldBe("ELIF");
        account.TransferWindowMinutes.ShouldBe(240);
        account.UpdatedUtc.ShouldBe(later);
        account.CreatedUtc.ShouldBe(Now);
    }

    [Fact]
    public void Defaults_MatchSettingsDocument()
    {
        PlatformDefaults.TransferCodePrefix.ShouldBe("OMV");
        PlatformDefaults.TransferWindowMinutes.ShouldBe(120);
    }
}
