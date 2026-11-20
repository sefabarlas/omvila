namespace Omvila.Modules.Catalog.Features.Categories;

/// <summary>Sözleşme ServiceCategoryWrite.</summary>
internal sealed record CategoryWriteRequest(string? Name, int? SortOrder, bool? IsActive);
