namespace Omvila.Modules.Catalog.Features.Templates;

/// <summary>Sözleşme manageCreateServicesFromTemplates gövdesi.</summary>
internal sealed record FromTemplatesRequest(IReadOnlyList<FromTemplateItem>? Items);
