using Omvila.Modules.Catalog.Domain;

namespace Omvila.Modules.Catalog.Features.Categories;

/// <summary>Sözleşme ServiceCategory.</summary>
internal sealed record CategoryResponse(Guid Id, string Name, int SortOrder, bool IsActive, int ServiceCount)
{
    public static CategoryResponse From(ServiceCategory c, int serviceCount) => new(c.Id, c.Name, c.SortOrder, c.IsActive, serviceCount);
}
