namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Randevu sayfası; <see cref="NextCursor"/> null ise son sayfa.</summary>
/// <param name="Items">Satırlar.</param>
/// <param name="NextCursor">Sonraki sayfanın opak imleci.</param>
public sealed record CustomerAppointmentPage(IReadOnlyList<CustomerAppointmentInfo> Items, string? NextCursor);
