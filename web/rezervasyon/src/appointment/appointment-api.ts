import { ApiProblem, ifMatch } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { createApi } from '@/lib/api';

// SMS bağlantısı uçları (public-appointment-link, T-52). Hata yanıtları `ApiProblem` olarak fırlar; ekranlar
// `code`'a göre metin seçer (src/booking/problem.ts).

type Schemas = components['schemas'];
export type PublicAppointment = Schemas['PublicAppointment'];
export type CustomerCancelRequest = Schemas['CustomerCancelRequest'];
export type CancelReason = CustomerCancelRequest['reason'];
export type SlotSearchResult = Schemas['SlotSearchResult'];
export type Slot = Schemas['Slot'];

/** Randevu ve eşzamanlı düzenleme sürümü (If-Match'e giden değer). */
export interface Loaded {
  appointment: PublicAppointment;
  etag: string;
}

// Sözleşmedeki accessCode deseni; uymayan kod API'ye gitmeden "bulunamadı" sayılır.
const accessCodePattern = /^[A-Za-z0-9]{8,10}$/;
export const isAccessCode = (code: string): boolean => accessCodePattern.test(code);

let client: ReturnType<typeof createApi> | undefined;
const api = () => (client ??= createApi());

type Call<T> = Promise<{ data?: T; error?: unknown; response: Response }>;

async function body<T>(call: Call<T>): Promise<{ data: T; response: Response }> {
  const { data, error, response } = await call;
  if (!response.ok || error !== undefined || data === undefined) {
    throw new ApiProblem(response.status, error);
  }
  return { data, response };
}

/**
 * ETag başlığı; çapraz köken yanıtında başlık okunamazsa (API `Access-Control-Expose-Headers: ETag` vermezse)
 * gövdedeki `version`. Sunucu ikisini de kabul eder (`W/"8812"`, `8812`; ETagVersion.TryParse).
 */
async function loaded(call: Call<PublicAppointment>): Promise<Loaded> {
  const { data, response } = await body(call);
  return { appointment: data, etag: response.headers.get('ETag') ?? data.version };
}

/** Bilinmeyen, süresi dolmuş veya bozuk kod `null` (T-52: ayrımsız 404). */
export async function fetchAppointment(code: string): Promise<Loaded | null> {
  if (!isAccessCode(code)) return null;
  try {
    return await loaded(
      api().GET('/api/v1/public/appointments/{accessCode}', {
        params: { path: { accessCode: code } },
        cache: 'no-store',
      }),
    );
  } catch (error) {
    if (error instanceof ApiProblem && error.status === 404) return null;
    throw error;
  }
}

/** Müşteri iptali (A6); `If-Match` GET'in ETag'i. */
export function cancelAppointment(
  code: string,
  etag: string,
  request: CustomerCancelRequest,
): Promise<Loaded> {
  return loaded(
    api().POST('/api/v1/public/appointments/{accessCode}/cancel', {
      params: { path: { accessCode: code }, header: ifMatch(etag) },
      body: request,
    }),
  );
}

/** Aynı hizmetler ve personelle bir günün boş saatleri (kendi randevusu boş sayılır). */
export async function searchRescheduleSlots(code: string, date: string): Promise<SlotSearchResult> {
  const { data } = await body(
    api().POST('/api/v1/public/appointments/{accessCode}/availability/slots', {
      params: { path: { accessCode: code } },
      body: { date },
    }),
  );
  return data;
}

/** "Havaleyi yaptım, salona bildir" (KK4). Kendiliğinden idempotent, If-Match almaz; randevu durumu değişmez. */
export function reportDepositTransfer(code: string): Promise<Loaded> {
  return loaded(
    api().POST('/api/v1/public/appointments/{accessCode}/deposit/report', {
      params: { path: { accessCode: code } },
    }),
  );
}

/** Saat değişikliği (A14); `If-Match` GET'in ETag'i. */
export function rescheduleAppointment(
  code: string,
  etag: string,
  startUtc: string,
): Promise<Loaded> {
  return loaded(
    api().POST('/api/v1/public/appointments/{accessCode}/reschedule', {
      params: { path: { accessCode: code }, header: ifMatch(etag) },
      body: { startUtc },
    }),
  );
}
