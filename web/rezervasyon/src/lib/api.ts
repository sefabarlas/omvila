import { createPublicClient } from '@omvila/api-client-ts';

/** API kökü: NEXT_PUBLIC_API_URL, yoksa Prism mock (pnpm --filter @omvila/contracts mock). */
export const apiBaseUrl: string = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:4010';

/** Herkese açık uçlar `X-Client: booking-page` taşır (docs/sozlesme/api-kurallari.md). */
export function createApi() {
  return createPublicClient({
    baseUrl: apiBaseUrl,
    headers: { 'X-Client': 'booking-page' },
  });
}
