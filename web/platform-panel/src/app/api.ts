import { createAdminClient } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { auth } from './auth';

export type AdminSession = components['schemas']['AdminSession'];
export type AdminPermission = components['schemas']['AdminPermission'];

/** API kökü: VITE_API_URL, yoksa Prism mock (pnpm --filter @omvila/contracts mock). */
export const apiBaseUrl: string =
  (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:4010';

// Admin yüzeyi çerez kullanmaz (AllowCredentials: false); credentials verilmez.
const webHeaders = { 'X-Client-Type': 'web', 'X-Requested-With': 'XMLHttpRequest' };

/** Saklanan yenileme jetonuyla yeni jeton çifti (rotation); jeton yoksa veya reddedilirse oturum yoktur. */
export async function refreshAccessToken(
  fetcher?: (request: Request) => Promise<Response>,
): Promise<boolean> {
  const refreshToken = auth.getRefreshToken();
  if (!refreshToken) {
    return false;
  }

  const client = createAdminClient({
    baseUrl: apiBaseUrl,
    headers: webHeaders,
    ...(fetcher ? { fetch: fetcher } : {}),
  });
  try {
    const { data } = await client.POST('/api/v1/admin/auth/refresh', { body: { refreshToken } });
    if (!data?.refreshToken) {
      auth.clear();
      return false;
    }

    auth.setTokens(data.accessToken, data.refreshToken);
    return true;
  } catch {
    // Ağ hatası: oturum açılamadı sayılır (giriş ekranı gösterilir).
    auth.clear();
    return false;
  }
}

export function createApi(fetcher?: (request: Request) => Promise<Response>) {
  return createAdminClient({
    baseUrl: apiBaseUrl,
    headers: webHeaders,
    getAccessToken: auth.getToken,
    refreshAccessToken: () => refreshAccessToken(fetcher),
    ...(fetcher ? { fetch: fetcher } : {}),
  });
}

export type Api = ReturnType<typeof createApi>;
