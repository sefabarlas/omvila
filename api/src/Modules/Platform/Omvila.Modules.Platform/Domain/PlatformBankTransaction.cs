using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Domain;

/// <summary>
/// OMVILA hesabına gelen havale (alan-modeli.md § PlatformBankTransaction). Banka dökümünden CSV ile içe aktarılır veya elle
/// girilir; banka referansı bütün platformda benzersizdir. Yalnız gelen (artı) tutar saklanır. Bir hareket tek bildirime bağlanır.
/// Platform verisidir (markaya ait değil, RLS yok).
/// </summary>
internal sealed class PlatformBankTransaction
{
    public const int SenderNameMaxLength = 160;
    public const int DescriptionMaxLength = 300;
    public const int BankReferenceMaxLength = 80;

    private PlatformBankTransaction()
    {
    }

    public Guid Id { get; private set; }

    public LocalDate ValueDate { get; private set; }

    /// <summary>Dökümde saat varsa hareketin anı.</summary>
    public Instant? ReceivedUtc { get; private set; }

    public long AmountMinor { get; private set; }

    public string SenderName { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string BankReference { get; private set; } = string.Empty;

    public BankTransactionSource Source { get; private set; }

    public Guid? MatchedNoticeId { get; private set; }

    public Guid? CreatedByUserId { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public static PlatformBankTransaction Create(
        Guid id, LocalDate valueDate, Instant? receivedUtc, long amountMinor, string senderName, string? description, string bankReference,
        BankTransactionSource source, Guid? createdBy, Instant now)
    {
        if (amountMinor <= 0)
        {
            throw new DomainException("Banka hareketi yalnız gelen (artı) tutarla kaydedilir.");
        }

        var sender = senderName?.Trim();
        var reference = bankReference?.Trim();
        if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(reference))
        {
            throw new DomainException("Gönderen ve banka referansı zorunludur.");
        }

        return new PlatformBankTransaction
        {
            Id = id,
            ValueDate = valueDate,
            ReceivedUtc = receivedUtc,
            AmountMinor = amountMinor,
            SenderName = Cut(sender, SenderNameMaxLength),
            Description = PaymentNotice.Clean(description) is { } d ? Cut(d, DescriptionMaxLength) : null,
            BankReference = Cut(reference, BankReferenceMaxLength),
            Source = source,
            CreatedByUserId = createdBy,
            CreatedUtc = now,
        };
    }

    private static string Cut(string value, int max) => value.Length > max ? value[..max] : value;
}
