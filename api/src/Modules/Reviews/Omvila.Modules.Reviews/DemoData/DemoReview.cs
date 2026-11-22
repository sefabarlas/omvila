using NodaTime;

namespace Omvila.Modules.Reviews.DemoData;

/// <summary>Tamamlanmış demo randevusunun değerlendirmesi; <paramref name="ReplyText"/> verilirse salonun yanıtı.</summary>
public sealed record DemoReview(
    Guid BrandId,
    Guid BranchId,
    Guid AppointmentId,
    Guid BrandCustomerId,
    Guid? StaffId,
    IReadOnlyList<string> ServiceNames,
    int Rating,
    string? Comment,
    Instant CreatedUtc,
    string? ReplyText = null,
    string? RepliedByName = null,
    Instant? RepliedUtc = null);
