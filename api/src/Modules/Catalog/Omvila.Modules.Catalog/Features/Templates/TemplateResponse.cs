namespace Omvila.Modules.Catalog.Features.Templates;

/// <summary>Sözleşme ServiceTemplate.</summary>
internal sealed record TemplateResponse(Guid Id, string CategoryName, string Name, int DefaultDurationMinutes);
