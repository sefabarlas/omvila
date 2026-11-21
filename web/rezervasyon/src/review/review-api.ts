import { ApiProblem, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { isAccessCode } from '@/appointment/appointment-api';
import { createApi } from '@/lib/api';

// Değerlendirme bağlantısı uçları (public-review, T-89). Giriş istemez; kod 7 gün geçerlidir.

type Schemas = components['schemas'];
export type ReviewInvite = Schemas['PublicReviewInvite'];
export type ReviewCreate = Schemas['PublicReviewCreate'];

let client: ReturnType<typeof createApi> | undefined;
const api = () => (client ??= createApi());

/** Bilinmeyen, süresi dolmuş veya bozuk kod `null` (404 access_code_invalid). */
export async function fetchReviewInvite(code: string): Promise<ReviewInvite | null> {
  if (!isAccessCode(code)) return null;
  try {
    return await unwrap(
      api().GET('/api/v1/public/reviews/{accessCode}', {
        params: { path: { accessCode: code } },
        cache: 'no-store',
      }),
    );
  } catch (error) {
    if (error instanceof ApiProblem && error.status === 404) return null;
    throw error;
  }
}

/**
 * "Gönder". `key` form başına bir kez üretilir: ağ hatasından sonra tekrar denemede aynı anahtar gider ve ikinci
 * kayıt oluşmaz.
 */
export function createReview(code: string, key: string, body: ReviewCreate): Promise<ReviewInvite> {
  return unwrap(
    api().POST('/api/v1/public/reviews/{accessCode}', {
      params: { path: { accessCode: code }, header: { 'Idempotency-Key': key } },
      body,
    }),
  );
}
