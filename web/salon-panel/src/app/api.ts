import { createManageClient } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { auth } from './auth';

export type ManageSession = components['schemas']['ManageSession'];

/** API kökü: VITE_API_URL, yoksa Prism mock (pnpm --filter @omvila/contracts mock). */
export const apiBaseUrl: string =
  (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:4010';

const webHeaders = { 'X-Client-Type': 'web', 'X-Requested-With': 'XMLHttpRequest' };

/** Yenileme çereziyle yeni erişim jetonu; başarısızsa oturum yoktur. */
export async function refreshAccessToken(
  fetcher?: (request: Request) => Promise<Response>,
): Promise<boolean> {
  const client = createManageClient({
    baseUrl: apiBaseUrl,
    headers: webHeaders,
    credentials: 'include',
    ...(fetcher ? { fetch: fetcher } : {}),
  });
  const { data } = await client.POST('/api/v1/manage/auth/refresh', { body: {} });
  auth.setToken(data?.accessToken ?? null);
  return data !== undefined;
}

export function createApi(fetcher?: (request: Request) => Promise<Response>) {
  return createManageClient({
    baseUrl: apiBaseUrl,
    headers: webHeaders,
    credentials: 'include',
    getAccessToken: auth.getToken,
    refreshAccessToken: () => refreshAccessToken(fetcher),
    ...(fetcher ? { fetch: fetcher } : {}),
  });
}

export type Api = ReturnType<typeof createApi>;
