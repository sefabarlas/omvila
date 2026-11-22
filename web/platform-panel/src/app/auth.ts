// Platform oturumu (T-116 (1)): admin yüzeyi çerez kabul etmez, jetonlar gövdede gelir. Erişim jetonu yalnız bellekte;
// yenileme jetonu sekme ömrü boyunca sessionStorage'da (sekme kapanınca oturum biter, localStorage'a yazılmaz).

const refreshKey = 'omvila.platform.yenileme';
let accessToken: string | null = null;

function readRefresh(): string | null {
  try {
    return sessionStorage.getItem(refreshKey);
  } catch {
    return null;
  }
}

function writeRefresh(token: string | null): void {
  try {
    if (token) {
      sessionStorage.setItem(refreshKey, token);
    } else {
      sessionStorage.removeItem(refreshKey);
    }
  } catch {
    // Depolama kapalıysa oturum yalnız bu sayfa yüklemesiyle sınırlı kalır.
  }
}

export const auth = {
  getToken: (): string | null => accessToken,
  getRefreshToken: readRefresh,
  /** Giriş ve yenileme sonrası iki jeton birlikte yazılır. */
  setTokens: (access: string, refresh: string): void => {
    accessToken = access;
    writeRefresh(refresh);
  },
  clear: (): void => {
    accessToken = null;
    writeRefresh(null);
  },
};
