using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Slots;

/// <summary>Slot.items: kalemin personeli, aralığı ve kopyalanacak fiyatı.</summary>
internal sealed record SlotItem(Guid ServiceId, Guid StaffId, string? StaffName, Instant StartUtc, Instant EndUtc, Money Price);
