namespace Omvila.Modules.Scheduling.Contracts;

/// <summary>Markanın randevu sayısı.</summary>
/// <param name="Total">Toplam.</param>
/// <param name="Online">Online kanaldan.</param>
public sealed record PlatformAppointmentCount(int Total, int Online);
