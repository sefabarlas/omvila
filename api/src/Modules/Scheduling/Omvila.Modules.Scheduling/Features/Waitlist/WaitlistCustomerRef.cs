using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme CustomerRef.</summary>
internal sealed record WaitlistCustomerRef(Guid BrandCustomerId, string FullName, string? Phone, string? PhoneMasked);
