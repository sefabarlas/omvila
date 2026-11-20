namespace Omvila.Modules.Identity.Security;

/// <summary>Oturum açma veya yenileme sonucu: erişim jetonu ve döndürülmüş yenileme jetonu.</summary>
internal sealed record ManageSession(string AccessToken, int ExpiresInSeconds, IssuedRefreshToken RefreshToken);
