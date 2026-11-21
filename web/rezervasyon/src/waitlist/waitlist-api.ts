import { ApiProblem, ifMatch, unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { isAccessCode } from '@/appointment/appointment-api';
import { createApi } from '@/lib/api';

// Bekleme listesi uçları (public-waitlist, T-90). Katılma telefon doğrulamasından sonra rezervasyon jetonuyla; teklif
// bağlantısı (/b/{kod}) telefonu doğrular, kod istenmez. Hata yanıtları `ApiProblem` olarak fırlar.

type Schemas = components['schemas'];
export type WaitlistJoin = Schemas['PublicWaitlistJoin'];
export type WaitlistEntry = Schemas['PublicWaitlistEntry'];
export type WaitlistOffer = Schemas['PublicWaitlistOffer'];
export type TimeWindow = Schemas['WaitlistTimeWindow'];
export type PublicAppointment = Schemas['PublicAppointment'];

/** Teklif ve If-Match'e giden sürüm (ETag başlığı okunamazsa gövdedeki `version`). */
export interface LoadedOffer {
  offer: WaitlistOffer;
  etag: string;
}

let client: ReturnType<typeof createApi> | undefined;
const api = () => (client ??= createApi());

/** "Bekleme listesine katıl"; her denemede yeni `Idempotency-Key` (istemci sarmalayıcısı üretir). */
export function joinWaitlist(
  branchId: string,
  bookingToken: string,
  body: WaitlistJoin,
): Promise<WaitlistEntry> {
  return unwrap(
    api().POST('/api/v1/public/branches/{branchId}/waitlist', {
      params: { path: { branchId } },
      headers: { Authorization: `Bearer ${bookingToken}` },
      body,
    }),
  );
}

async function loadedOffer(
  call: Promise<{ data?: WaitlistOffer; error?: unknown; response: Response }>,
): Promise<LoadedOffer> {
  const { data, error, response } = await call;
  if (!response.ok || error !== undefined || data === undefined) {
    throw new ApiProblem(response.status, error);
  }
  return { offer: data, etag: response.headers.get('ETag') ?? data.version };
}

/** Bilinmeyen veya bozuk kod `null` (404 access_code_invalid). Süresi dolmuş teklif de 200 döner. */
export async function fetchOffer(code: string): Promise<LoadedOffer | null> {
  if (!isAccessCode(code)) return null;
  try {
    return await loadedOffer(
      api().GET('/api/v1/public/waitlist-offers/{accessCode}', {
        params: { path: { accessCode: code } },
        cache: 'no-store',
      }),
    );
  } catch (error) {
    if (error instanceof ApiProblem && error.status === 404) return null;
    throw error;
  }
}

/** "Randevuyu al": randevu online kurallarıyla oluşur (W4); kabul edilmiş teklif aynı randevuyu döner. */
export function acceptOffer(code: string, etag: string): Promise<PublicAppointment> {
  return unwrap(
    api().POST('/api/v1/public/waitlist-offers/{accessCode}/accept', {
      params: { path: { accessCode: code }, header: ifMatch(etag) },
    }),
  );
}

/** "Bu saat olmaz": kişi listede kalır. */
export function declineOffer(code: string, etag: string): Promise<LoadedOffer> {
  return loadedOffer(
    api().POST('/api/v1/public/waitlist-offers/{accessCode}/decline', {
      params: { path: { accessCode: code }, header: ifMatch(etag) },
    }),
  );
}

/** "Listeden çık"; kod bekleme kaydınındır (waitlistAccessCode). Kendiliğinden idempotent. */
export function leaveWaitlist(waitlistAccessCode: string): Promise<WaitlistEntry> {
  return unwrap(
    api().POST('/api/v1/public/waitlist/{accessCode}/leave', {
      params: { path: { accessCode: waitlistAccessCode } },
    }),
  );
}
