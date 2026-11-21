using NodaTime;

namespace Omvila.Modules.Tenancy.Contracts;

/// <summary>Takvimde gösterilen izin (sözleşme TimeOff).</summary>
/// <param name="Id">İzin.</param>
/// <param name="StaffId">Personel.</param>
/// <param name="Type">Leave, Break, Training, Sick, Other.</param>
/// <param name="StartUtc">Başlangıç.</param>
/// <param name="EndUtc">Bitiş (hariç).</param>
/// <param name="IsAllDay">Tam gün.</param>
/// <param name="Note">Not.</param>
public sealed record StaffTimeOffDetail(Guid Id, Guid StaffId, string Type, Instant StartUtc, Instant EndUtc, bool IsAllDay, string? Note);
