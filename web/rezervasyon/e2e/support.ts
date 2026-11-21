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
import {
  accessCode,
  cancelledFixture,
  etag,
  linkAppointmentFixture,
  rescheduledFixture,
  rescheduleSlotsFixture,
} from '../src/test/appointment-fixtures';

const api = 'http://localhost:4010/api/v1/public';

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

/** Sabit "şimdi" (24 Kasım 2026 Salı 10:00) ve akış uçlarının test yanıtları. Ön uçuş (OPTIONS) Prism'e gider. */
export async function bookingApi(page: Page) {
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
      await fulfill(route, { status: 'CodeSent', challenge: challengeFixture });
    } else if (path.endsWith('/otp/verify')) {
      await fulfill(route, tokenFixture);
    } else if (path.endsWith('/appointments')) {
      await fulfill(route, appointmentFixture(), 201);
    } else {
      await route.fallback();
    }
  });
}

export interface LinkCalls {
  cancel: { ifMatch: string | undefined; body: unknown }[];
}

/** SMS bağlantısı (/r/{kod}) uçlarının test yanıtları; iptalden sonra GET iptal edilmiş randevuyu döner. */
export async function appointmentApi(page: Page): Promise<LinkCalls> {
  await page.clock.setFixedTime(new Date(nowUtc));
  const calls: LinkCalls = { cancel: [] };
  const base = `/api/v1/public/appointments/${accessCode}`;
  let cancelled = false;
  await page.route(`${api}/appointments/**`, async (route) => {
    const request = route.request();
    if (request.method() === 'OPTIONS') {
      await route.fallback();
      return;
    }
    const path = new URL(request.url()).pathname;
    if (path === base) {
      await fulfill(route, cancelled ? cancelledFixture() : linkAppointmentFixture(), 200, {
        ETag: etag,
      });
    } else if (path === `${base}/availability/slots`) {
      const { date } = request.postDataJSON() as { date: string };
      await fulfill(route, rescheduleSlotsFixture(date));
    } else if (path === `${base}/cancel`) {
      calls.cancel.push({
        ifMatch: (await request.headerValue('If-Match')) ?? undefined,
        body: request.postDataJSON(),
      });
      cancelled = true;
      await fulfill(route, cancelledFixture(), 200, { ETag: 'W/"8813"' });
    } else if (path === `${base}/reschedule`) {
      await fulfill(route, rescheduledFixture(), 200, { ETag: 'W/"8814"' });
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
