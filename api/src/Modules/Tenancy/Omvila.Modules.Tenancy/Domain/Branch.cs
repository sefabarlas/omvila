using NodaTime;

namespace Omvila.Modules.Tenancy.Domain;

/// <summary>Şube (alan-modeli.md § Branch). Haftalık saat ayrı tablolardadır; yeni şubenin saati boştur.</summary>
internal sealed class Branch
{
    private Branch()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string AddressLine { get; private set; } = string.Empty;

    public string District { get; private set; } = string.Empty;

    public string City { get; private set; } = string.Empty;

    public string? PostalCode { get; private set; }

    public string CountryCode { get; private set; } = "TR";

    public decimal? Latitude { get; private set; }

    public decimal? Longitude { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string TimeZoneId { get; private set; } = string.Empty;

    public int? SlotIntervalMinutes { get; private set; }

    public bool IsActive { get; private set; }

    public Instant CreatedUtc { get; private set; }

    /// <summary>"Şube ekle": ad ve adres zorunlu; saat dilimi markanın varsayılanı (Europe/Istanbul).</summary>
    public static Branch Create(Guid id, Guid brandId, BranchDetails details, string slug, string timeZoneId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(details);
        var branch = new Branch { Id = id, BrandId = brandId, Slug = slug, TimeZoneId = timeZoneId, IsActive = true, CreatedUtc = now };
        branch.Update(details);
        return branch;
    }

    /// <summary>Doğrulanmış değerleri uygular (AyarlarSube).</summary>
    public void Update(BranchDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);
        Name = details.Name;
        AddressLine = details.AddressLine;
        District = details.District;
        City = details.City;
        PostalCode = details.PostalCode;
        Latitude = details.Latitude;
        Longitude = details.Longitude;
        Phone = details.Phone;
        Email = details.Email;
        SlotIntervalMinutes = details.SlotIntervalMinutes;
        IsActive = details.IsActive;
    }

    /// <summary>Kayıttaki ilk şube: ad "Merkez", saat dilimi Europe/Istanbul, adres sonra girilir (dilim-01).</summary>
    /// <summary>Eşzamanlı düzenleme sürümü (PostgreSQL xmin; If-Match/ETag).</summary>
    public uint Version { get; private set; }

    public static Branch CreateFirst(Guid id, Guid brandId, string name, string slug, string timeZoneId, Instant now) => new()
    {
        Id = id,
        BrandId = brandId,
        Name = name,
        Slug = slug,
        TimeZoneId = timeZoneId,
        IsActive = true,
        CreatedUtc = now,
    };
}
