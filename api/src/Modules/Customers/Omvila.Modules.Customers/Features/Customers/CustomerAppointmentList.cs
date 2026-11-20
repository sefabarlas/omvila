using System.Text.Json.Serialization;

namespace Omvila.Modules.Customers.Features.Customers;

/// <summary>Sözleşme manageListCustomerAppointments yanıtı.</summary>
internal sealed record CustomerAppointmentList(
    IReadOnlyList<CustomerAppointmentRow> Items,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? NextCursor);
