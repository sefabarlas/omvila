namespace Omvila.Infrastructure.Security;

/// <summary>Uç noktanın istediği izin (yetki denetimi ve "işaretsiz uç" denetimi için).</summary>
public sealed record PermissionMetadata(string Permission);
