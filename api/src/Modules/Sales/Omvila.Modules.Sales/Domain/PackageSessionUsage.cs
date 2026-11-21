using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Sales.Domain;

/// <summary>
/// Seans kullanımı (alan-modeli.md § PackageSessionUsage): randevu oluşurken (veya tamamlamada "Paketten") yazılır; randevu
/// iptal olursa silinmez, <see cref="RevertedUtc"/> ile geri döner (müşteri kartında tarihli görünür). Gelmedi'de geri dönmez
/// (durum-makinesi.md A8, S-09). Randevu başka modülde olduğu için kimliği yabancı anahtarsız tutulur.
/// </summary>
internal sealed class PackageSessionUsage
{
    private PackageSessionUsage()
    {
    }

    public Guid Id { get; private set; }

    public Guid BrandId { get; private set; }

    public Guid CustomerPackageItemId { get; private set; }

    public Guid AppointmentId { get; private set; }

    public Guid AppointmentItemId { get; private set; }

    public Instant UsedUtc { get; private set; }

    public Instant? RevertedUtc { get; private set; }

    public bool IsActive => RevertedUtc is null;

    // Kural: durum-makinesi.md A1 ("paket seansı ayrılır")
    public static PackageSessionUsage Use(IIdGenerator ids, Guid brandId, Guid customerPackageItemId, Guid appointmentId, Guid appointmentItemId, Instant now)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return new PackageSessionUsage
        {
            Id = ids.New(),
            BrandId = brandId,
            CustomerPackageItemId = customerPackageItemId,
            AppointmentId = appointmentId,
            AppointmentItemId = appointmentItemId,
            UsedUtc = now,
        };
    }

    /// <summary>Randevu iptalinde seans geri döner (A4–A7); zaten dönmüşse değişmez.</summary>
    // Kural: durum-makinesi.md A4–A7 ("seans geri"); alan-modeli.md § PackageSessionUsage (RevertedUtc)
    public void Revert(Instant now) => RevertedUtc ??= now;
}
