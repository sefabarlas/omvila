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

    public string? Bio { get; private set; }

    /// <summary>"Personel ekle" (StaffRules ile doğrulanmış).</summary>
    public static Staff Create(Guid id, Guid brandId, string fullName, string? title, string calendarColor, decimal serviceCommission, decimal saleCommission, bool isBookableOnline, Instant now) => new()
    {
        Id = id,
        BrandId = brandId,
        FullName = fullName.Trim(),
        Title = title,
        CalendarColor = calendarColor,
        ServiceCommissionPercent = serviceCommission,
        SaleCommissionPercent = saleCommission,
        IsBookableOnline = isBookableOnline,
        IsActive = true,
        CreatedUtc = now,
    };

    /// <summary>Personel düzenleme (doğrulanmış değerler).</summary>
    public void Update(StaffProfile p)
    {
        ArgumentNullException.ThrowIfNull(p);
        FullName = p.FullName;
        DisplayName = p.DisplayName;
        Title = p.Title;
        Bio = p.Bio;
        CalendarColor = p.CalendarColor;
        ServiceCommissionPercent = p.ServiceCommissionPercent;
        SaleCommissionPercent = p.SaleCommissionPercent;
        IsBookableOnline = p.IsBookableOnline;
        IsActive = p.IsActive;
    }

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
