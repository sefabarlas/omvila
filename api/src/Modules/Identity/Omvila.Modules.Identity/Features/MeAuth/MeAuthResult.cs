namespace Omvila.Modules.Identity.Features.MeAuth;

/// <summary>Sözleşme MeAuthResult.</summary>
/// <param name="AccessToken">Erişim jetonu (aud=me).</param>
/// <param name="ExpiresInSeconds">Erişim jetonu ömrü.</param>
/// <param name="RefreshToken">Yenileme jetonu (her kullanımda döner).</param>
/// <param name="IsNewAccount">Uygulama hesabı bu girişle açıldı.</param>
internal sealed record MeAuthResult(string AccessToken, int ExpiresInSeconds, string RefreshToken, bool IsNewAccount);
