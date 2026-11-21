using NodaTime;

namespace Omvila.Modules.Customers.Domain;

/// <summary>
/// KVKK silme talebi (alan-modeli.md § DataDeletionRequest). <see cref="BrandId"/> null ise müşterinin kendi hesabını
/// tümden silmesidir (müşteri uygulaması). Panelden silme hemen uygulanır ve <see cref="DeletionRequestStatus.Completed"/>
/// yazılır. Tamamlanan talepte kart bağı (<see cref="BrandCustomerId"/>) silinir, ad yalnız maskeli kalır.
/// </summary>
internal sealed class DataDeletionRequest
{
    private DataDeletionRequest()
    {
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid? BrandId { get; private set; }

    public Guid? BrandCustomerId { get; private set; }

    /// <summary>Maskeli ad ("A•••• K••••"); tamamlanan talepte gösterilen tek ad.</summary>
    public string CustomerNameMasked { get; private set; } = string.Empty;

    public DeletionRequestedVia RequestedVia { get; private set; }

    public DeletionRequestStatus Status { get; private set; }

    public Instant RequestedUtc { get; private set; }

    /// <summary>Son gün (DeletionRequestDueDays).</summary>
    public LocalDate DueOn { get; private set; }

    public Instant? CompletedUtc { get; private set; }

    /// <summary>Panelden anonimleştirme: talep tamamlanmış olarak yazılır.</summary>
    public static DataDeletionRequest CompletedFromPanel(Guid id, Guid customerId, Guid brandId, string fullName, LocalDate dueOn, Instant now) => new()
    {
        Id = id,
        CustomerId = customerId,
        BrandId = brandId,
        BrandCustomerId = null,
        CustomerNameMasked = MaskName(fullName),
        RequestedVia = DeletionRequestedVia.Panel,
        Status = DeletionRequestStatus.Completed,
        RequestedUtc = now,
        DueOn = dueOn,
        CompletedUtc = now,
    };

    /// <summary>Bekleyen talep anonimleştirmeyle tamamlandı; kart bağı silinir.</summary>
    public void Complete(Instant now)
    {
        if (Status != DeletionRequestStatus.Pending)
        {
            return;
        }

        Status = DeletionRequestStatus.Completed;
        CompletedUtc = now;
        BrandCustomerId = null;
    }

    /// <summary>Her sözcüğün ilk harfi ve dört nokta: "Tuna Oral" → "T•••• O••••".</summary>
    public static string MaskName(string fullName) =>
        string.Join(' ', (fullName ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(w => $"{w[0]}••••"));
}
