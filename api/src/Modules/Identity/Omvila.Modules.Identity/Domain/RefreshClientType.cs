namespace Omvila.Modules.Identity.Domain;

/// <summary>X-Client-Type: web ise jeton HttpOnly çerezde, mobile ise gövdede döner (sözleşme ClientType).</summary>
internal enum RefreshClientType
{
    Web,
    Mobile,
}
