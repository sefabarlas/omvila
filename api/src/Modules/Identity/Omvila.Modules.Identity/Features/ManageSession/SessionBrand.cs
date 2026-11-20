namespace Omvila.Modules.Identity.Features.ManageSession;

internal sealed record SessionBrand(Guid Id, string Name, string Slug, string? LogoUrl, string Status);
