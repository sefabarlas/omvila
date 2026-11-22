using System.Text.Json.Serialization;
using NodaTime;
using Omvila.Modules.Platform.Domain;
using Omvila.SharedKernel;

namespace Omvila.Modules.Platform.Features.ManageSubscription;

/// <summary>"PERSONEL 5 · … dahil".</summary>
internal sealed record StaffCounts(int Count, [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] int? Limit);
