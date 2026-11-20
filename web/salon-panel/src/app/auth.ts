// Erişim jetonu yalnız bellekte (kodlama-typescript.md: localStorage'a jeton yazılmaz). Sayfa yenilenince
// HttpOnly yenileme çereziyle yeniden alınır (ADR-0007).

let accessToken: string | null = null;

export const auth = {
  getToken: (): string | null => accessToken,
  setToken: (token: string | null): void => {
    accessToken = token;
  },
};
