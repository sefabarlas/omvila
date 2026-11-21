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

const api = 'http://localhost:4010/api/v1/public';

const fulfill = (route: Route, body: unknown, status = 200) =>
  route.fulfill({
    status,
    contentType: 'application/json',
    // Tarayıcıdan Prism'e çapraz köken istekleri; yanıtı test verdiğinde CORS başlıkları da gerekir.
    headers: {
      'Access-Control-Allow-Origin': 'http://localhost:3100',
      'Access-Control-Allow-Credentials': 'true',
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
