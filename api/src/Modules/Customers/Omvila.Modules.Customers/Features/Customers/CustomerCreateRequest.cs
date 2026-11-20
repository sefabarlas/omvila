namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>Sözleşme CustomerCreate.</summary>
internal sealed record CustomerCreateRequest(string? FullName, string? Phone, int? BirthYear, string? Notes, bool? SendAppointmentSms);
