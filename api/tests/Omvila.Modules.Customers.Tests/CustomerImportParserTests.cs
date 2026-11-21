using Omvila.Modules.Customers.Domain.Import;
using Omvila.Modules.Customers.Features.Export;

namespace Omvila.Modules.Customers.Tests;

// Kural: sözleşme managePreviewImport (sütunlar başlıktan; "0532 123 45 67", "5321234567" ve "+90 532 123 4567" aynı kişi;
// satır hataları invalid_phone, missing_name, duplicate_in_file, too_long; sınır aşımı errors.file), T-16/T-17 (yalnız +90),
// sözleşme manageExportCustomers (CSV alanları).
public sealed class CustomerImportParserTests
{
    [Fact]
    public void Parse_MapsTurkishHeaders_NormalizesPhones_ReportsRowErrors()
    {
        const string csv = "﻿ADI SOYADI,Cep Telefonu,Notlar,E-posta\r\n"
            + "Selin Öztürk,0532 123 45 67,\"Kısa, katlı\",\r\n"
            + "Selin Ö.,+90 532 123 4567,,\r\n"
            + "Derya Şahin,+44 20 7946 0000,,\r\n"
            + "D,05551112233,,\r\n"
            + "\r\n"
            + "Pınar Yalçın,5559998877,\"İki\nsatır\",pinar@example.com\r\n";

        var parsed = CustomerImportParser.Parse(csv, maxRows: 10, currentYear: 2026);

        parsed.IsSuccess.ShouldBeTrue();
        var import = parsed.Value;
        import.MappedColumns.ShouldBe([ImportColumn.FullName, ImportColumn.Phone, ImportColumn.Notes, ImportColumn.Email]);
        import.TotalRows.ShouldBe(5, "boş satır sayılmaz");
        import.InvalidRows.ShouldBe(3);
        import.Rows.Select(r => $"{r.RowNumber}|{r.FullName}|{r.PhoneE164}|{r.Notes}").ShouldBe(
            ["2|Selin Öztürk|+905321234567|Kısa, katlı", "7|Pınar Yalçın|+905559998877|İki\nsatır"]);
        import.Errors.Select(e => $"{e.RowNumber}|{e.Column}|{e.Code}").ShouldBe(
            ["3|Cep Telefonu|duplicate_in_file", "4|Cep Telefonu|invalid_phone", "5|ADI SOYADI|missing_name"]);
    }

    [Fact]
    public void Parse_Semicolons_BirthYear_TooLong()
    {
        var csv = $"Ad;Telefon;Doğum yılı;Not\n"
            + "Selin;05321234567;1992;\n"
            + "Derya;05321234568;1850;\n"
            + $"{new string('x', 121)};05321234569;;\n"
            + $"Ece;05321234560;;{new string('n', 2001)}\n";

        var import = CustomerImportParser.Parse(csv, 10, 2026).Value;

        import.Rows.Select(r => r.BirthYear).ShouldBe([1992, (int?)null], "geçersiz yıl satırı bozmaz, yok sayılır");
        import.Errors.Select(e => $"{e.RowNumber}|{e.Column}|{e.Code}").ShouldBe(["4|Ad|too_long", "5|Not|too_long"]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Ad;Not\nSelin;x\n")]
    [InlineData("Telefon\n05321234567\n")]
    public void Parse_WithoutNameOrPhoneColumn_InvalidFormat(string csv) =>
        CustomerImportParser.Parse(csv, 10, 2026).Error!.FieldErrors["file"].ShouldBe(["invalid_format"]);

    [Fact]
    public void Parse_OverRowLimit_TooLarge() =>
        CustomerImportParser.Parse("Ad;Telefon\nA1;05321234561\nA2;05321234562\nA3;05321234563\n", 2, 2026).Error!.FieldErrors["file"].ShouldBe(["too_large"]);

    [Theory]
    [InlineData("Selin", "Selin")]
    [InlineData("Kısa; saç", "\"Kısa; saç\"")]
    [InlineData("Dedi ki \"evet\"", "\"Dedi ki \"\"evet\"\"\"")]
    [InlineData("=HYPERLINK(1)", "'=HYPERLINK(1)")]
    [InlineData("+90", "'+90")]
    [InlineData(null, "")]
    public void Field_QuotesAndNeutralizesFormulas(string? value, string expected) => CsvText.Field(value, ';').ShouldBe(expected);

    [Theory]
    [InlineData(0, "0,00")]
    [InlineData(45_000, "450,00")]
    [InlineData(1_234_550, "12345,50")]
    public void Lira_FromMinorUnits(long minor, string expected) => CustomerExporter.Lira(minor).ShouldBe(expected);
}
