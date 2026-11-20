using Omvila.SharedKernel;

namespace Omvila.Modules.Catalog.Domain;

/// <summary>Hazır listeden seçilen şablon ve salonun girdiği fiyat (sözleşme manageCreateServicesFromTemplates.items).</summary>
internal sealed record TemplateSelection(Guid TemplateId, string CategoryName, string Name, int DefaultDurationMinutes, Money? Price, int? DurationMinutes);
