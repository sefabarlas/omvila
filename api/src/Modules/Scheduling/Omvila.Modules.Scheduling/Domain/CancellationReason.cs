namespace Omvila.Modules.Scheduling.Domain;

/// <summary>İptal nedeni (alan-modeli.md § Appointment "İptal nedenleri"; sözleşme CancellationReason).</summary>
internal enum CancellationReason
{
    // Müşteri
    PlanChanged,
    Sick,
    WantsDifferentTime,

    // Müşteri ve salon ("Diğer": not zorunlu)
    Other,

    // Salon
    CustomerRequested,
    StaffUnavailable,
    DeclinedBySalon,

    // Sistem
    ApprovalExpired,
    DepositNotReceived,
    AccountDeleted,
    BrandReadOnly,
}
