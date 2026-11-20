using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

internal sealed record PlannedService(string CategoryName, string Name, int DurationMinutes, Money Price);
