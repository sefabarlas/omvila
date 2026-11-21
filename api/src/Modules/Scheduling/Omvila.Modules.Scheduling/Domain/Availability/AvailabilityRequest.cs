using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Domain.Availability;

/// <summary>Müsaitlik sorusu: kalemler sırasıyla zincirlenir (musaitlik.md § Çok kalemli randevu).</summary>
internal sealed record AvailabilityRequest
{
    private AvailabilityRequest(IReadOnlyList<RequestedItem> items, bool sameStaffForAll, AvailabilityMode mode)
    {
        Items = items;
        SameStaffForAll = sameStaffForAll;
        Mode = mode;
    }

    public IReadOnlyList<RequestedItem> Items { get; }

    /// <summary>"İki hizmet de aynı kişiden": tek personel bütün kalemleri yapar (M13).</summary>
    public bool SameStaffForAll { get; }

    public AvailabilityMode Mode { get; }

    /// <summary>Soran müşteri: kendi bekleme teklifinin tuttuğu saat ona açıktır (M18).</summary>
    public Guid? CustomerId { get; init; }

    /// <summary>Müşterinin tercih ettiği personel; fark etmez eşitliğinde ikinci ölçüt.</summary>
    public Guid? PreferredStaffId { get; init; }

    /// <summary>Saat değişikliğinde randevunun kendisi: eski dilimleri meşguliyet sayılmaz (musaitlik.md § Saat değişikliği).</summary>
    public Guid? IgnoreAppointmentId { get; init; }

    public static Result<AvailabilityRequest> Create(IReadOnlyList<RequestedItem> items, bool sameStaffForAll, AvailabilityMode mode)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (items.Count == 0)
        {
            return Error.Validation("items", "required");
        }

        if (items.Count > PlatformDefaults.MaxItemsPerAppointment)
        {
            return Error.Validation("items", "out_of_range");
        }

        // Aynı kişiden seçiliyken kalemlerde iki farklı personel istenemez.
        if (sameStaffForAll)
        {
            var first = items.FirstOrDefault(i => i.StaffId is not null)?.StaffId;
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].StaffId is { } staffId && staffId != first)
                {
                    return Error.Validation($"items[{i}].staffId", "staff_mismatch");
                }
            }
        }

        return new AvailabilityRequest([.. items], sameStaffForAll, mode);
    }
}
