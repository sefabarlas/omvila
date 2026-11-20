using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>Hazır listeden oluşturulacak kategoriler (yalnız olmayanlar) ve hizmetler.</summary>
internal sealed record TemplatePlan(IReadOnlyList<string> NewCategories, IReadOnlyList<PlannedService> Services);
