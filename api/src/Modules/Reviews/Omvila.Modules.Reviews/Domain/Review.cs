using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Reviews.Domain;

/// <summary>
/// Değerlendirme (alan-modeli.md § Review). Bir randevu bir kez değerlendirilir (AppointmentId benzersiz); yalnız tamamlanan
/// randevu (çağıran denetler). Yayınlanmış olarak kaydedilir (S-15; sözleşme publicCreateReview). Salonun yanıtı üzerine yazılır,
/// müşteriye bildirim gitmez (T-86 (4)).
/// </summary>
internal sealed class Review
{
    public const int MinRating = 1;

    public const int MaxRating = 5;

    /// <summary>alan-modeli.md § Review.Comment (1000).</summary>
    public const int CommentMaxLength = 1000;

    /// <summary>Sözleşme ReviewReplyRequest.text: 2–1000.</summary>
    public const int ReplyMinLength = 2;

    public const int ReplyMaxLength = 1000;

    public const int RepliedByNameMaxLength = 100;

    private Review()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid BranchId { get; private set; }

    public Guid AppointmentId { get; private set; }

    public Guid BrandCustomerId { get; private set; }

    public Guid? StaffId { get; private set; }

    public int Rating { get; private set; }

    public string? Comment { get; private set; }

    /// <summary>Randevu kalemlerinin hizmet adları (randevuda kopya; tamamlanan randevu değişmez).</summary>
    public List<string> ServiceNames { get; private set; } = [];

    public bool IsPublished { get; private set; }

    public Instant CreatedUtc { get; private set; }

    public string? ReplyText { get; private set; }

    public Instant? RepliedUtc { get; private set; }

    public Guid? RepliedByUserId { get; private set; }

    /// <summary>Yanıtlayanın adı (yanıt anındaki personel adı; kullanıcı personele bağlı değilse boş; T-89).</summary>
    public string? RepliedByName { get; private set; }

    public uint Version { get; private set; }

    /// <summary>Puan 1–5 değilse veya yorum 1000 karakteri aşarsa geçersiz (çağıran 400 ile önceden denetler).</summary>
    // Kural: alan-modeli.md § Review (Rating 1–5, Comment 1000); sözleşme publicCreateReview (yayınlanmış kaydedilir)
    public static Review Submit(
        Guid id, Guid brandId, Guid branchId, Guid appointmentId, Guid brandCustomerId, Guid? staffId, IReadOnlyList<string> serviceNames,
        int rating, string? comment, Instant now)
    {
        ArgumentNullException.ThrowIfNull(serviceNames);
        if (!IsValidRating(rating))
        {
            throw new DomainException("Puan 1 ile 5 arasında olmalı.");
        }

        var cleaned = Clean(comment);
        if (cleaned is { Length: > CommentMaxLength })
        {
            throw new DomainException("Yorum çok uzun.");
        }

        return new Review
        {
            Id = id,
            BrandId = brandId,
            BranchId = branchId,
            AppointmentId = appointmentId,
            BrandCustomerId = brandCustomerId,
            StaffId = staffId,
            Rating = rating,
            Comment = cleaned,
            ServiceNames = [.. serviceNames],
            IsPublished = true,
            CreatedUtc = now,
        };
    }

    public static bool IsValidRating(int rating) => rating is >= MinRating and <= MaxRating;

    /// <summary>Boş veya yalnız boşluk yorum yok sayılır.</summary>
    public static string? Clean(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();

    /// <summary>Salonun yanıtı ("Yanıtla"). Varsa yenisiyle değiştirilir, zaman ve yanıtlayan güncellenir.</summary>
    // Kural: sözleşme manageReplyReview (yanıt üzerine yazılır, repliedUtc güncellenir)
    public void Reply(string text, Guid? userId, string? byName, Instant now)
    {
        var cleaned = Clean(text);
        if (cleaned is null || cleaned.Length < ReplyMinLength || cleaned.Length > ReplyMaxLength)
        {
            throw new DomainException("Yanıt 2–1000 karakter olmalı.");
        }

        ReplyText = cleaned;
        RepliedUtc = now;
        RepliedByUserId = userId;
        RepliedByName = Clean(byName) is { } name ? name[..Math.Min(name.Length, RepliedByNameMaxLength)] : null;
    }
}
