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

    /// <summary>Bekleme teklifi bildirimlerinin kayıt türü.</summary>
    public const string WaitlistOffer = "WaitlistOffer";

    /// <summary>Bekleme teklifine bağlı bildirim (waitlist.offer).</summary>
    /// <param name="offerId">Teklif.</param>
    public static NotificationReference ForWaitlistOffer(Guid offerId) => new(WaitlistOffer, offerId);

    /// <summary>Kapıda sıra bileti (staff.walkin_queue).</summary>
    public const string WalkInTicket = "WalkInTicket";

    /// <summary>Kapıda sıra biletine bağlı bildirim.</summary>
    /// <param name="ticketId">Bilet.</param>
    public static NotificationReference ForWalkInTicket(Guid ticketId) => new(WalkInTicket, ticketId);
}
