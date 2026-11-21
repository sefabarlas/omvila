using Omvila.Modules.Notifications.Templates;

namespace Omvila.Modules.Notifications.Tests;

// Kural: bildirimler.md § SMS — GSM 03.38 Türkçe tekli kaydırma tablosuyla ≤ 155 birim = 1 kontör; kontör = parça sayısı.
public sealed class SmsEncodingTests
{
    [Fact]
    public void TurkishShiftCharacters_StayGsm_AndCountTwoUnits()
    {
        var measure = SmsEncoding.Measure("şğıİŞĞç");

        measure.Encoding.ShouldBe(SmsEncodingKind.Gsm7Turkish);
        measure.Units.ShouldBe(14);
        measure.Segments.ShouldBe(1);
    }

    [Fact]
    public void BasicTableTurkishLetters_CountOneUnit()
    {
        // ö ü Ö Ü Ç temel tabloda.
        SmsEncoding.Measure("öüÖÜÇ").Units.ShouldBe(5);
    }

    [Theory]
    [InlineData(155, 1)]
    [InlineData(156, 2)]
    [InlineData(298, 2)]
    [InlineData(299, 3)]
    public void Gsm_SingleUpTo155_ThenSegmentsOf149(int length, int segments) =>
        SmsEncoding.Measure(new string('a', length)).Segments.ShouldBe(segments);

    [Fact]
    public void Gsm_ShiftCharacterIsNotSplitAcrossSegments()
    {
        // 148 + ş (2) = 150 > 149: ş ikinci parçaya geçer; toplam 150 + 148 = 298 birim ama 3 parça değil 2.
        var text = new string('a', 148) + "ş" + new string('a', 147);
        var measure = SmsEncoding.Measure(text);

        measure.Units.ShouldBe(297);
        measure.Segments.ShouldBe(2);

        // 148 + ş, sonra 148 harf: ikinci parça 2 + 148 = 150 > 149 → 3 parça.
        SmsEncoding.Measure(new string('a', 148) + "ş" + new string('a', 148)).Segments.ShouldBe(3);
    }

    [Theory]
    [InlineData(70, 1)]
    [InlineData(71, 2)]
    [InlineData(134, 2)]
    [InlineData(135, 3)]
    public void CharacterOutsideTables_SwitchesToUcs2(int length, int segments)
    {
        var text = "✂" + new string('a', length - 1);
        var measure = SmsEncoding.Measure(text);

        measure.Encoding.ShouldBe(SmsEncodingKind.Ucs2);
        measure.Segments.ShouldBe(segments);
    }

    [Fact]
    public void Ucs2_SurrogatePairIsNotSplit()
    {
        // 66 birim + emoji (2 kod birimi) = 68 > 67: emoji ikinci parçaya geçer.
        var text = new string('a', 66) + "😀" + new string('a', 66);

        SmsEncoding.Measure(text).Segments.ShouldBe(3);
        SmsEncoding.Measure(new string('a', 65) + "😀" + new string('a', 67)).Segments.ShouldBe(2);
    }

    [Fact]
    public void EmptyText_IsOneSegment() => SmsEncoding.Measure(string.Empty).Segments.ShouldBe(1);
}
