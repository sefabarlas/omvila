using NodaTime;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// Müşteri uygulamasında QR veya bağlantıyla eklenen salon (alan-modeli.md § CustomerSavedBrand). <see cref="BrandCustomer"/>
/// oluşturmaz: salon, randevu almamış birini müşteri listesinde görmez (dilim-05 KK1). Kişi başına markada tek satır.
/// </summary>
internal sealed class CustomerSavedBrand
{
    public const string QrSource = "Qr";

    public const string LinkSource = "Link";

    public const string BookingSource = "Booking";

    private CustomerSavedBrand()
    {
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid BrandId { get; private set; }

    public Instant AddedUtc { get; private set; }

    /// <summary>Qr, Link veya Booking (Booking yalnız sunucu yazar).</summary>
    public string Source { get; private set; } = LinkSource;

    public static CustomerSavedBrand Create(Guid id, Guid customerId, Guid brandId, string source, Instant now) => new()
    {
        Id = id,
        CustomerId = customerId,
        BrandId = brandId,
        Source = source,
        AddedUtc = now,
    };
}
