using System.Text.Json.Serialization;
using NodaTime;
using Omvila.SharedKernel;

namespace Omvila.Modules.Scheduling.Features.Waitlist;

/// <summary>Sözleşme WaitlistList.</summary>
internal sealed record WaitlistList(Guid BranchId, string TimeZone, Instant GeneratedUtc, IReadOnlyList<WaitlistEntryView> Items, WaitlistOverview Overview);
