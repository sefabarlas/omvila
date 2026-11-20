using Omvila.Modules.Catalog.Features.Services;

namespace Omvila.Modules.Catalog.Features.Templates;

internal sealed record FromTemplateItem(Guid? TemplateId, MoneyDto? Price, int? DurationMinutes);
