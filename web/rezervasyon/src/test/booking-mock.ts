import type { Catalog, DaySearchRequest, SlotSearchRequest } from '@/booking/booking-api';
import { vi } from 'vitest';
import { json } from './api-mock';
import {
  appointmentFixture,
  catalogFixture,
  challengeFixture,
  daysFixture,
  slotsFixture,
  tokenFixture,
} from './booking-fixtures';

export * from './booking-fixtures';

export interface Recorded {
  method: string;
  path: string;
  body: unknown;
  request: Request;
}

type Route = (request: Request, body: unknown) => Response | undefined;

/**
 * Akışın uçlarını taklit eder. `routes` ilk eşleşen yanıtı verir (undefined dönerse varsayılan); her istek
 * gövdesiyle kaydedilir.
 */
export function mockBooking(options: { catalog?: Catalog; routes?: Route[] } = {}) {
  const catalog = options.catalog ?? catalogFixture();
  const calls: Recorded[] = [];
  const fetchMock = async (request: Request): Promise<Response> => {
    const path = new URL(request.url).pathname;
    const text = request.method === 'GET' ? '' : await request.clone().text();
    const body: unknown = text ? JSON.parse(text) : undefined;
    calls.push({ method: request.method, path, body, request });
    for (const route of options.routes ?? []) {
      const response = route(request, body);
      if (response) return response;
    }
    if (path.endsWith('/catalog')) return json(200, catalog);
    if (path.endsWith('/availability/days'))
      return json(200, daysFixture(body as DaySearchRequest));
    if (path.endsWith('/availability/slots')) {
      return json(200, slotsFixture(body as SlotSearchRequest, catalog));
    }
    if (path.endsWith('/otp/request')) {
      return json(200, { status: 'CodeSent', challenge: challengeFixture });
    }
    if (path.endsWith('/otp/verify')) return json(200, tokenFixture);
    if (path.endsWith('/appointments')) return json(201, appointmentFixture());
    return json(404, { code: 'not_found' });
  };
  vi.stubGlobal('fetch', vi.fn(fetchMock));
  return calls;
}
