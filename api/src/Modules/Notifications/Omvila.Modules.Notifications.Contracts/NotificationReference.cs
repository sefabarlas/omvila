namespace Omvila.Modules.Notifications.Contracts;

/// <summary>
/// Bildirimin ait olduğu kayıt (alan-modeli.md § NotificationOutbox.ReferenceType/ReferenceId), ör. randevu. Bekleyen
/// hatırlatmalar bu bağla düşürülür (<see cref="INotificationOutbox.CancelPendingAsync"/>).
/// </summary>
/// <param name="Type">Kayıt türü, ör. <see cref="Appointment"/>.</param>
/// <param name="Id">Kayıt kimliği.</param>
public sealed record NotificationReference(string Type, Guid Id)
{
    /// <summary>Randevu bildirimlerinin kayıt türü.</summary>
    public const string Appointment = "Appointment";

    /// <summary>Randevuya bağlı bildirim.</summary>
    /// <param name="appointmentId">Randevu.</param>
    public static NotificationReference ForAppointment(Guid appointmentId) => new(Appointment, appointmentId);
}
