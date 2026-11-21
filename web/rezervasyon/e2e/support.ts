import type { Page, Route } from '@playwright/test';
import {
  appointmentFixture,
  catalogFixture,
  challengeFixture,
  daysFixture,
  depositCatalogFixture,
  depositFixture,
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
  linkDepositFixture,
  rescheduledFixture,
  rescheduleSlotsFixture,
} from '../src/test/appointment-fixtures';
import {
  entryFixture,
  offerCode,
  offerFixture,
  packageFixture,
  reviewCode,
  reviewInviteFixture,
  reviewedFixture,
  waitlistCode,
} from '../src/test/d5-fixtures';
import type { ReviewInvite } from '../src/review/review-api';
import type { WaitlistOffer } from '../src/waitlist/waitlist-api';

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
  /** Kaporalı hizmet (Dilim 4): katalogda ipucu, oluşturma DepositPending, "Havaleyi yaptım" CustomerReported. */
  deposit?: boolean;
  /** Müşterinin Fön paketi var (Dilim 5, Onay.png paket kutusu). */
  packages?: boolean;
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
  /** "Havaleyi yaptım" istekleri (yol). */
  report: string[];
  /** "Bekleme listesine katıl" gövdeleri. */
  waitlist: unknown[];
}

/** Sabit "şimdi" (24 Kasım 2026 Salı 10:00) ve akış uçlarının test yanıtları. Ön uçuş (OPTIONS) Prism'e gider. */
export async function bookingApi(page: Page, options: BookingOptions = {}): Promise<BookingCalls> {
  const calls: BookingCalls = { create: [], report: [], waitlist: [] };
  await page.clock.setFixedTime(new Date(nowUtc));
  const catalog = options.deposit ? depositCatalogFixture() : catalogFixture();
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
    } else if (path.endsWith('/my-packages')) {
      await fulfill(route, { items: options.packages ? [packageFixture()] : [] });
    } else if (path.endsWith('/waitlist')) {
      calls.waitlist.push(request.postDataJSON());
      await fulfill(route, entryFixture(), 201);
    } else if (path.endsWith('/deposit/report')) {
      calls.report.push(path);
      await fulfill(route, depositFixture('CustomerReported'), 200, { ETag: 'W/"2"' });
    } else if (path.endsWith('/appointments')) {
      calls.create.push(request.postDataJSON());
      if (options.slotTaken) {
        await problem(route, 409, 'slot_taken', {
          alternatives: ['2026-11-24T11:45:00Z', '2026-11-24T12:15:00Z'],
        });
      } else {
        await fulfill(
          route,
          options.deposit
            ? depositFixture('Awaiting')
            : options.requiresApproval
              ? pendingRequest()
              : appointmentFixture(),
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
  /** "Havaleyi yaptım" istek sayısı. */
  report: number;
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
  const calls: LinkCalls = { report: 0, cancel: [], reschedule: [] };
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
    } else if (path === `${base}/deposit/report`) {
      calls.report += 1;
      current = linkDepositFixture('CustomerReported');
      await fulfill(route, current, 200, { ETag: 'W/"8820"' });
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

export interface OfferCalls {
  accept: (string | undefined)[];
  decline: (string | undefined)[];
  leave: number;
}

/**
 * Bekleme teklifi bağlantısı (/b/{kod}) uçları. Kabul `accepted` randevusunu döner; ret ve "Listeden çık" sonrası GET
 * yeni hâli döner. `install`: geri sayımın ilerlemesi için sahte saat (fastForward); yoksa sabit saat.
 */
export async function offerApi(
  page: Page,
  offer: Partial<WaitlistOffer> = {},
  options: { accepted?: PublicAppointment; install?: boolean } = {},
): Promise<OfferCalls> {
  if (options.install) await page.clock.install({ time: new Date(nowUtc) });
  else await page.clock.setFixedTime(new Date(nowUtc));
  const calls: OfferCalls = { accept: [], decline: [], leave: 0 };
  let current = offerFixture(offer);
  const base = `/api/v1/public/waitlist-offers/${offerCode}`;
  await page.route(/\/api\/v1\/public\/(waitlist-offers|waitlist)\//, async (route) => {
    const request = route.request();
    if (request.method() === 'OPTIONS') {
      await route.fallback();
      return;
    }
    const path = new URL(request.url()).pathname;
    const ifMatch = (await request.headerValue('If-Match')) ?? undefined;
    if (path === base) {
      await fulfill(route, current, 200, { ETag: current.version });
    } else if (path === `${base}/accept`) {
      calls.accept.push(ifMatch);
      await fulfill(route, options.accepted ?? appointmentFixture());
    } else if (path === `${base}/decline`) {
      calls.decline.push(ifMatch);
      current = { ...current, status: 'Declined', version: 'W/"9421"' };
      await fulfill(route, current, 200, { ETag: current.version });
    } else if (path === `/api/v1/public/waitlist/${waitlistCode}/leave`) {
      calls.leave += 1;
      await fulfill(route, entryFixture({ status: 'Cancelled' }));
    } else {
      await problem(route, 404, 'access_code_invalid');
    }
  });
  return calls;
}

/** Değerlendirme bağlantısı (/d/{kod}); gönderimden sonra GET de değerlendirilmiş hâli döner. */
export async function reviewApi(
  page: Page,
  invite: ReviewInvite = reviewInviteFixture(),
): Promise<{ posts: { key: string | undefined; body: unknown }[] }> {
  await page.clock.setFixedTime(new Date(nowUtc));
  const calls: { posts: { key: string | undefined; body: unknown }[] } = { posts: [] };
  let current = invite;
  await page.route(`${api}/reviews/**`, async (route) => {
    const request = route.request();
    if (request.method() === 'OPTIONS') {
      await route.fallback();
      return;
    }
    const path = new URL(request.url()).pathname;
    if (path !== `/api/v1/public/reviews/${reviewCode}`) {
      await problem(route, 404, 'access_code_invalid');
    } else if (request.method() === 'POST') {
      const body = request.postDataJSON() as { rating: number; comment: string | null };
      calls.posts.push({ key: (await request.headerValue('Idempotency-Key')) ?? undefined, body });
      current = {
        ...reviewedFixture(body.comment),
        review: { rating: body.rating, comment: body.comment, createdUtc: nowUtc },
      };
      await fulfill(route, current, 201);
    } else {
      await fulfill(route, current);
    }
  });
  return calls;
}
