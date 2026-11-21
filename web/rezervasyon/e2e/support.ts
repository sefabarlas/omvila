import type { Page, Route } from '@playwright/test';
import {
  appointmentFixture,
  catalogFixture,
  challengeFixture,
  daysFixture,
  nowUtc,
  slotsFixture,
  tokenFixture,
} from '../src/test/booking-fixtures';
import type { DaySearchRequest, SlotSearchRequest } from '../src/booking/booking-api';
import type { PublicAppointment } from '../src/appointment/appointment-api';
import {
  accessCode,
  cancelledFixture,
  linkAppointmentFixture,
  rescheduledFixture,
  rescheduleSlotsFixture,
} from '../src/test/appointment-fixtures';

// Uygulamanın API kökü e2e/api-stub.mjs'tir (playwright.config.ts).
const api = 'http://localhost:4011/api/v1/public';

const fulfill = (route: Route, body: unknown, status = 200, headers: Record<string, string> = {}) =>
  route.fulfill({
    status,
    contentType: 'application/json',
    // Tarayıcıdan Prism'e çapraz köken istekleri; yanıtı test verdiğinde CORS başlıkları da gerekir.
    headers: {
      'Access-Control-Allow-Origin': 'http://localhost:3100',
      'Access-Control-Allow-Credentials': 'true',
      'Access-Control-Expose-Headers': 'ETag',
      ...headers,
    },
    body: JSON.stringify(body),
  });

const problem = (route: Route, status: number, code: string, meta: Record<string, unknown> = {}) =>
  fulfill(route, { type: 'about:blank', title: code, status, code, meta }, status);

export interface BookingOptions {
  /** Kod isteği 429 döner (Durumlar: SMS kodu deneme sınırı). */
  otpLimited?: boolean;
  /** Oluşturma 409 slot_taken döner (Durumlar: saat az önce doldu). */
  slotTaken?: boolean;
  /** Onay gerektiren salon (Dilim 3, Talep): oluşturma PendingApproval, yanıt süresi 120 dk. */
  requiresApproval?: boolean;
}

/** Onay bekleyen talep: tutma şimdi + 120 dk (sureler-ve-ayarlar.md ApprovalTimeoutMinutes varsayılanı). */
export const pendingRequest = (): PublicAppointment =>
  appointmentFixture({
    status: 'PendingApproval',
    displayStatus: 'PendingApproval',
    holdExpiresUtc: new Date(new Date(nowUtc).getTime() + 120 * 60_000).toISOString(),
  });

export interface BookingCalls {
  create: unknown[];
}

/** Sabit "şimdi" (24 Kasım 2026 Salı 10:00) ve akış uçlarının test yanıtları. Ön uçuş (OPTIONS) Prism'e gider. */
export async function bookingApi(page: Page, options: BookingOptions = {}): Promise<BookingCalls> {
  const calls: BookingCalls = { create: [] };
  await page.clock.setFixedTime(new Date(nowUtc));
  const catalog = catalogFixture();
  await page.route(`${api}/**`, async (route) => {
    const request = route.request();
    if (request.method() === 'OPTIONS') {
      await route.fallback();
      return;
    }
    const path = new URL(request.url()).pathname;
    if (path.startsWith('/api/v1/public/brands/')) {
      await route.fallback();
    } else if (path.endsWith('/catalog')) {
      await fulfill(route, catalog);
    } else if (path.endsWith('/availability/days')) {
      await fulfill(route, daysFixture(request.postDataJSON() as DaySearchRequest));
    } else if (path.endsWith('/availability/slots')) {
      await fulfill(route, slotsFixture(request.postDataJSON() as SlotSearchRequest, catalog));
    } else if (path.endsWith('/otp/request')) {
      if (options.otpLimited) {
        await problem(route, 429, 'otp_attempts_exceeded', { retryAfterSeconds: 300 });
      } else {
        await fulfill(route, { status: 'CodeSent', challenge: challengeFixture });
      }
    } else if (path.endsWith('/otp/verify')) {
      await fulfill(route, tokenFixture);
    } else if (path.endsWith('/appointments')) {
      calls.create.push(request.postDataJSON());
      if (options.slotTaken) {
        await problem(route, 409, 'slot_taken', {
          alternatives: ['2026-11-24T11:45:00Z', '2026-11-24T12:15:00Z'],
        });
      } else {
        await fulfill(
          route,
          options.requiresApproval ? pendingRequest() : appointmentFixture(),
          201,
        );
      }
    } else {
      await route.fallback();
    }
  });
  return calls;
}

export interface LinkCalls {
  cancel: { ifMatch: string | undefined; body: unknown }[];
  reschedule: { ifMatch: string | undefined; body: unknown }[];
}

/**
 * SMS bağlantısı (/r/{kod}) uçlarının test yanıtları; iptal veya saat değişikliğinden sonra GET yeni hâli döner.
 * `appointment` ilk okumanın alanlarını değiştirir (ör. süresi geçmiş randevu).
 */
export async function appointmentApi(
  page: Page,
  appointment: Partial<PublicAppointment> = {},
): Promise<LinkCalls> {
  await page.clock.setFixedTime(new Date(nowUtc));
  const calls: LinkCalls = { cancel: [], reschedule: [] };
  let current = linkAppointmentFixture(appointment);
  const base = `/api/v1/public/appointments/${accessCode}`;
  await page.route(`${api}/appointments/**`, async (route) => {
    const request = route.request();
    if (request.method() === 'OPTIONS') {
      await route.fallback();
      return;
    }
    const path = new URL(request.url()).pathname;
    if (path === base) {
      await fulfill(route, current, 200, { ETag: `W/"${current.version}"` });
    } else if (path === `${base}/availability/slots`) {
      const { date } = request.postDataJSON() as { date: string };
      await fulfill(route, rescheduleSlotsFixture(date));
    } else if (path === `${base}/cancel`) {
      calls.cancel.push({
        ifMatch: (await request.headerValue('If-Match')) ?? undefined,
        body: request.postDataJSON(),
      });
      current = cancelledFixture();
      await fulfill(route, current, 200, { ETag: 'W/"8813"' });
    } else if (path === `${base}/reschedule`) {
      calls.reschedule.push({
        ifMatch: (await request.headerValue('If-Match')) ?? undefined,
        body: request.postDataJSON(),
      });
      current = rescheduledFixture();
      await fulfill(route, current, 200, { ETag: 'W/"8814"' });
    } else {
      await fulfill(
        route,
        { type: 'about:blank', title: 'not_found', status: 404, code: 'not_found' },
        404,
      );
    }
  });
  return calls;
}
