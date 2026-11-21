import { unwrap } from '@omvila/api-client-ts';
import type { components } from '@omvila/api-client-ts';
import { createApi } from '@/lib/api';

// Rezervasyon akışının API çağrıları (public-booking, T-51). Hata yanıtları `ApiProblem` olarak fırlar;
// adımlar `code`'a göre metin seçer (@omvila/ui-web/errors).

type Schemas = components['schemas'];
export type Catalog = Schemas['PublicCatalog'];
export type CatalogService = Catalog['services'][number];
export type CatalogStaff = Catalog['staff'][number];
export type DaySearchRequest = Schemas['DaySearchRequest'];
export type DaySearchResult = Schemas['DaySearchResult'];
export type SlotSearchRequest = Schemas['SlotSearchRequest'];
export type SlotSearchResult = Schemas['SlotSearchResult'];
export type Slot = Schemas['Slot'];
export type ItemRequest = Schemas['AppointmentItemRequest'];
export type OtpRequestResult = Schemas['PublicOtpRequestResult'];
export type OtpChallenge = Schemas['OtpChallengeResponse'];
export type BookingToken = Schemas['BookingTokenResult'];
export type AppointmentCreate = Schemas['PublicAppointmentCreate'];
export type PublicAppointment = Schemas['PublicAppointment'];
export type Money = Schemas['Money'];
export type UsablePackageItem = Schemas['UsablePackageItem'];

let plain: ReturnType<typeof createApi> | undefined;
let withCookies: ReturnType<typeof createApi> | undefined;

/** Çerezsiz istemci (katalog, gün, saat, oluşturma). */
const api = () => (plain ??= createApi());
/** Telefon doğrulama: güvenilir cihaz çerezi için `credentials: 'include'`. */
const otpApi = () => (withCookies ??= createApi({ credentials: 'include' }));

export function fetchCatalog(branchId: string): Promise<Catalog> {
  return unwrap(
    api().GET('/api/v1/public/branches/{branchId}/catalog', {
      params: { path: { branchId } },
    }),
  );
}

export function searchDays(branchId: string, body: DaySearchRequest): Promise<DaySearchResult> {
  return unwrap(
    api().POST('/api/v1/public/branches/{branchId}/availability/days', {
      params: { path: { branchId } },
      body,
    }),
  );
}

export function searchSlots(branchId: string, body: SlotSearchRequest): Promise<SlotSearchResult> {
  return unwrap(
    api().POST('/api/v1/public/branches/{branchId}/availability/slots', {
      params: { path: { branchId } },
      body,
    }),
  );
}

export function requestOtp(branchId: string, phone: string): Promise<OtpRequestResult> {
  return unwrap(otpApi().POST('/api/v1/public/otp/request', { body: { phone, branchId } }));
}

/** `rememberDevice`: aynı tarayıcıda sonraki randevuda kod adımı atlanır (kriter 9, T-51 (9)). */
export function verifyOtp(otpId: string, code: string): Promise<BookingToken> {
  return unwrap(
    otpApi().POST('/api/v1/public/otp/verify', {
      body: { otpId, code, rememberDevice: true },
    }),
  );
}

/** Her denemede yeni `Idempotency-Key` (istemci sarmalayıcısı üretir; 503 tekrarında aynı anahtar). */
export function createAppointment(
  branchId: string,
  bookingToken: string,
  body: AppointmentCreate,
): Promise<PublicAppointment> {
  return unwrap(
    api().POST('/api/v1/public/branches/{branchId}/appointments', {
      params: { path: { branchId } },
      headers: { Authorization: `Bearer ${bookingToken}` },
      body,
    }),
  );
}

/** Doğrulanmış telefonun bu şubede kullanabileceği paket seansları ("Fön paketinizden düşülsün", T-95). */
export async function listMyPackages(
  branchId: string,
  bookingToken: string,
): Promise<UsablePackageItem[]> {
  const result = await unwrap(
    api().GET('/api/v1/public/branches/{branchId}/my-packages', {
      params: { path: { branchId } },
      headers: { Authorization: `Bearer ${bookingToken}` },
    }),
  );
  return result.items;
}
