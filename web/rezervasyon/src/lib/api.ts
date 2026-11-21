import { createPublicClient } from '@omvila/api-client-ts';

/** API kökü: NEXT_PUBLIC_API_URL, yoksa Prism mock (pnpm --filter @omvila/contracts mock). */
export const apiBaseUrl: string = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:4010';

interface ApiOptions {
  /**
   * Yalnız telefon doğrulama uçlarında `include`: güvenilir cihaz çerezi (`omvila_td`, HttpOnly) API kökenine
   * gider ve oradan gelir (T-51 (9)). Diğer uçlar çerezsiz çağrılır.
   */
  credentials?: RequestCredentials;
}

/** Herkese açık uçlar `X-Client: booking-page` taşır (docs/sozlesme/api-kurallari.md). */
export function createApi(options: ApiOptions = {}) {
  return createPublicClient({
    baseUrl: apiBaseUrl,
    headers: { 'X-Client': 'booking-page' },
    ...(options.credentials ? { credentials: options.credentials } : {}),
  });
}
