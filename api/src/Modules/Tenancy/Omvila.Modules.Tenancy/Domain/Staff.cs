using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Personel (alan-modeli.md § Staff). Sahip de bir personeldir (kayıtta IsBookableOnline = true).</summary>
internal sealed class Staff
{
    private Staff()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid? UserId { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string? DisplayName { get; private set; }

    public string? Title { get; private set; }

    public string CalendarColor { get; private set; } = string.Empty;

    public decimal ServiceCommissionPercent { get; private set; }

    public decimal SaleCommissionPercent { get; private set; }

    public bool IsBookableOnline { get; private set; }

    public bool IsActive { get; private set; }

    public Instant CreatedUtc { get; private set; }

    /// <summary>Eşzamanlı düzenleme sürümü (PostgreSQL xmin; If-Match/ETag).</summary>
    public uint Version { get; private set; }

    public static Staff CreateOwner(Guid id, Guid brandId, Guid userId, string fullName, string calendarColor, Instant now) => new()
    {
        Id = id,
        BrandId = brandId,
        UserId = userId,
        FullName = fullName,
        CalendarColor = calendarColor,
        IsBookableOnline = true,
        IsActive = true,
        CreatedUtc = now,
    };
}
