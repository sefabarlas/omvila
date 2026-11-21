using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme ManageWaitlistCreate ("Listeye ekle").</summary>
internal sealed record ManageWaitlistCreate(
    Guid? BranchId,
    Guid? BrandCustomerId,
    ManageWaitlistCreate.NewCustomerRequest? NewCustomer,
    WaitlistPreferencesRequest? Preferences,
    string? Note)
{
    /// <summary>ManageWaitlistCreate.newCustomer.</summary>
    internal sealed record NewCustomerRequest(string? FullName, string? Phone);
}
