using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Gün kapatma (alan-modeli.md § CashDayClose). Şubenin bir yerel günü bir kez kapatılır; beklenen nakit kapanış anında
/// sunucuda hesaplanıp kopyalanır, fark = sayılan − beklenen. Kapanmış güne yeni tahsilat yazılabilir; okumada
/// "kapanıştan sonra" diye işaretlenir (<see cref="IsRecordedAfter"/>).
/// </summary>
internal sealed class CashDayClose
{
    public const int DifferenceNoteMaxLength = 300;

    private CashDayClose()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    /// <summary>Şubenin saat dilimindeki gün.</summary>
    public LocalDate BusinessDate { get; private set; }

    public long ExpectedCashMinor { get; private set; }

    public long CountedCashMinor { get; private set; }

    /// <summary>Sayılan − beklenen; eksi olabilir.</summary>
    public long DifferenceMinor { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    public string? DifferenceNote { get; private set; }

    public Guid? ClosedByUserId { get; private set; }

    public Instant ClosedUtc { get; private set; }

    public CashDifference Difference => DifferenceMinor switch
    {
        0 => CashDifference.None,
        < 0 => CashDifference.Shortage,
        _ => CashDifference.Surplus,
    };

    /// <summary>
    /// "Günü kapat". Gün şubenin bugününden sonra olamaz (date: out_of_range); fark varsa açıklama zorunludur
    /// (differenceNote: required). Açıklama kırpılır; farksız kapanışta da saklanır.
    /// </summary>
    // Kural: alan-modeli.md § CashDayClose; dilim-05 KK7 (beklenen 3.450, sayılan 3.400 → fark −50 ve açıklama); docs/acik-sorular.md W3
    public static Result<CashDayClose> Close(
        IIdGenerator ids, Guid brandId, Guid branchId, LocalDate businessDate, LocalDate today, Money expected, Money counted, string? note, Guid? userId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        if (expected.Currency != counted.Currency)
        {
            throw new DomainException("Beklenen ve sayılan nakit aynı para biriminde olmalı.");
        }

        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        if (businessDate > today)
        {
            errors["date"] = ["out_of_range"];
        }

        var cleaned = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        var difference = counted.AmountMinor - expected.AmountMinor;
        if (cleaned is { Length: > DifferenceNoteMaxLength })
        {
            errors["differenceNote"] = ["too_long"];
        }
        else if (difference != 0 && cleaned is null)
        {
            errors["differenceNote"] = ["required"];
        }

        if (errors.Count > 0)
        {
            return Error.Validation(errors);
        }

        return new CashDayClose
        {
            Id = ids.New(),
            BrandId = brandId,
            BranchId = branchId,
            BusinessDate = businessDate,
            ExpectedCashMinor = expected.AmountMinor,
            CountedCashMinor = counted.AmountMinor,
            DifferenceMinor = difference,
            CurrencyCode = expected.Currency.ToString(),
            DifferenceNote = cleaned,
            ClosedByUserId = userId,
            ClosedUtc = now,
        };
    }

    /// <summary>Tahsilat gün kapatıldıktan sonra mı yazıldı ("kapanıştan sonra").</summary>
    public bool IsRecordedAfter(Instant paidUtc) => paidUtc > ClosedUtc;
}
