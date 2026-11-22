using System.Text.Json.Serialization;
using NodaTime;

namespace Omvila.Modules.Platform.Features.AdminBrands;

/// <summary>Sözleşme AdminCreditTransaction.</summary>
internal sealed record AdminCreditTransaction(
    Guid Id,
    Instant OccurredUtc,
    string Type,
    int Credits,
    int BalanceAfter,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string? Note);
