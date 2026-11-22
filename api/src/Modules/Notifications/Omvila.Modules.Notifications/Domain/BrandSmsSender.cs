using System.Text.RegularExpressions;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Notifications.Domain;

/// <summary>
/// Gönderen adı (alan-modeli.md § BrandSmsSender; sözleşme SmsSender; T-62 (6)). Marka başına tek satır: son başvuru
/// (<see cref="Name"/>, <see cref="Status"/>) ve şu an kullanılan onaylı ad (<see cref="ApprovedName"/>). Yeni başvuru
/// beklerken veya reddedilince önceki onaylı ad sürer; hiç onaylı ad yoksa SMS'ler <c>OMVILA</c> ile gider (kriter 11).
/// </summary>
internal sealed partial class BrandSmsSender
{
    private const int RegexTimeoutMilliseconds = 1000;

    private BrandSmsSender()
    {
    }

    public Guid BrandId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public SmsSenderStatus Status { get; private set; }

    public string? ApprovedName { get; private set; }

    public Instant SubmittedUtc { get; private set; }

    public Instant? DecidedUtc { get; private set; }

    public string? RejectionReason { get; private set; }

    /// <summary>Satır sürümü (xmin; platform kararında If-Match).</summary>
    public uint Version { get; private set; }

    /// <summary>SMS'lerin şu an gittiği ad: onaylı ad, yoksa platform varsayılanı.</summary>
    public static string EffectiveName(BrandSmsSender? sender, string defaultSender) =>
        sender?.ApprovedName is { Length: > 0 } approved ? approved : defaultSender;

    /// <summary>İlk başvuru.</summary>
    public static BrandSmsSender Submit(Guid brandId, string name, Instant now)
    {
        var sender = new BrandSmsSender { BrandId = brandId };
        sender.Resubmit(name, now);
        return sender;
    }

    /// <summary>Yeni ad başvurusu; onaylı ad (varsa) karar verilene kadar kullanılmaya devam eder.</summary>
    public void Resubmit(string name, Instant now)
    {
        // Kural: T-62 (6) — 3–11 büyük Latin harf/rakam, en az bir harf.
        if (name is null || !NamePattern().IsMatch(name))
        {
            throw new DomainException("Gönderen adı 3–11 büyük harf veya rakam olmalı, en az bir harf içermelidir.");
        }

        Name = name;
        Status = SmsSenderStatus.Pending;
        SubmittedUtc = now;
        DecidedUtc = null;
        RejectionReason = null;
    }

    public void Approve(Instant now)
    {
        EnsurePending();
        Status = SmsSenderStatus.Approved;
        ApprovedName = Name;
        DecidedUtc = now;
    }

    public void Reject(string reason, Instant now)
    {
        EnsurePending();
        Status = SmsSenderStatus.Rejected;
        RejectionReason = reason;
        DecidedUtc = now;
    }

    private void EnsurePending()
    {
        if (Status != SmsSenderStatus.Pending)
        {
            throw new DomainException("Yalnız bekleyen başvuruya karar verilir.");
        }
    }

    [GeneratedRegex("^(?=.*[A-Z])[A-Z0-9]{3,11}$", RegexOptions.None, RegexTimeoutMilliseconds)]
    private static partial Regex NamePattern();
}
