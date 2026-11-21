namespace Omvila.Modules.Customers.Features.Deletion;

/// <summary>Sözleşme CustomerDeletionPreview.</summary>
internal sealed record CustomerDeletionPreview(
    string FullName, int AppointmentCount, int PaymentCount, int UpcomingAppointmentCount, IReadOnlyList<Contracts.ActivePackageInfo> ActivePackages);
