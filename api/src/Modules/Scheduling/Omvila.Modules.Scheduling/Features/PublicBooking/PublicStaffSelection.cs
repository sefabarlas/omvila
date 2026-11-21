using Omvila.Modules.Scheduling.Domain.Availability;
using Omvila.Modules.Scheduling.Features.Appointments;
using Omvila.Modules.Tenancy.Contracts;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.PublicBooking;

/// <summary>
/// Rezervasyon sayfası kalemleri (sözleşme AppointmentItemRequest; staffId null = fark etmez) ve salonun personel seçimi
/// ayarları (sureler-ve-ayarlar.md AllowStaffSelection, AllowAnyStaffOption): personel seçimi kapalıyken personel
/// gönderilemez, "fark etmez" kapalıyken personel zorunludur. "Aynı kişiden" seçiliyken tek bir personel verilmesi
/// bütün kalemler için o personeldir (fark etmez değildir).
/// </summary>
internal static class PublicStaffSelection
{
    public static Result<List<RequestedItem>> Read(IReadOnlyList<AppointmentItemRequest>? requested, bool sameStaffForAll, PublicBookingOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);
        var items = new List<RequestedItem>();
        if (requested is null || requested.Count == 0)
        {
            return Error.Validation("items", "required");
        }

        if (requested.Count > PlatformDefaults.MaxItemsPerAppointment)
        {
            return Error.Validation("items", "out_of_range");
        }

        for (var i = 0; i < requested.Count; i++)
        {
            if (requested[i]?.ServiceId is not { } serviceId)
            {
                errors[$"items[{i}].serviceId"] = ["required"];
                continue;
            }

            var staffId = requested[i].StaffId;
            if (staffId is not null && !options.AllowStaffSelection)
            {
                errors[$"items[{i}].staffId"] = ["out_of_range"];
            }

            items.Add(new RequestedItem(serviceId, staffId));
        }

        var anyStaff = sameStaffForAll ? items.TrueForAll(i => i.StaffId is null) : items.Exists(i => i.StaffId is null);
        if (items.Count > 0 && anyStaff && !options.AllowAnyStaffOption && options.AllowStaffSelection)
        {
            errors.TryAdd("items[0].staffId", ["required"]);
        }

        var request = AvailabilityRequest.Create(items, sameStaffForAll, AvailabilityMode.Public);
        if (errors.Count == 0 && !request.IsSuccess)
        {
            return request.Error;
        }

        return errors.Count > 0 ? Error.Validation(errors) : items;
    }

    /// <summary>Her kalemde personel belliyse o personeller (yükleyiciye aday listesi); fark etmez kalem varsa null.</summary>
    public static List<Guid>? FixedStaff(IReadOnlyList<RequestedItem> items, bool sameStaffForAll)
    {
        var common = sameStaffForAll ? items.FirstOrDefault(i => i.StaffId is not null)?.StaffId : null;
        var staff = items.Select(i => i.StaffId ?? common).ToList();
        return staff.Exists(s => s is null) ? null : [.. staff.Select(s => s!.Value).Distinct()];
    }
}
