using NodaTime;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// Havale bildirimi ile banka hareketinin karşılaştırması (sözleşme adminGetPaymentNotice; T-116 (3); dilim-06 KK2, KK3).
/// codeFound: açıklamada bildirimin kodu geçiyor (büyük/küçük harf, boşluk ve tire farkı yok sayılır); amountMatches: tutar
/// beklenen tutara eşit (beklenen tutar yoksa false). Yalnız ikisi birden doğruysa "Kod tutuyor", aksi hâlde "Kontrol edin".
/// </summary>
internal readonly record struct TransferMatch(bool CodeFound, bool AmountMatches)
{
    /// <summary>Aday aralığı: bildirimden 7 gün önce.</summary>
    public const int WindowDaysBefore = 7;

    /// <summary>Aday aralığı: bildirimden 3 gün sonra.</summary>
    public const int WindowDaysAfter = 3;

    public MatchVerdict Verdict => CodeFound && AmountMatches ? MatchVerdict.CodeMatches : MatchVerdict.CheckRequired;

    // Kural: sözleşme adminGetPaymentNotice — değer tarihi bildirim gününden 7 gün önce ile 3 gün sonrası arasında (dahil).
    public static (LocalDate From, LocalDate To) Window(LocalDate notifiedOn) =>
        (notifiedOn.PlusDays(-WindowDaysBefore), notifiedOn.PlusDays(WindowDaysAfter));

    public static TransferMatch Compare(string transferCode, long? expectedAmountMinor, string? description, long amountMinor) =>
        new(ContainsCode(description, transferCode), expectedAmountMinor is { } expected && expected == amountMinor);

    /// <summary>Kod açıklamada geçiyor mu ("abn ke 1125", "ABNKE1125" ve "ABN-KE-1125" aynıdır).</summary>
    public static bool ContainsCode(string? description, string transferCode)
    {
        var code = Normalize(transferCode);
        return code.Length > 0 && description is not null && Normalize(description).Contains(code, StringComparison.Ordinal);
    }

    private static string Normalize(string value) =>
        new([.. value.Where(char.IsLetterOrDigit).Select(c => c is 'ı' or 'i' or 'İ' ? 'I' : char.ToUpperInvariant(c))]);
}
