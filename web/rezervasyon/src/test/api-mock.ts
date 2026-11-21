import type { PublicBrand } from '@/lib/brand';
import { vi } from 'vitest';

/** Kurgusal salon (gerçek kişi yok). */
export function brandFixture(overrides: Partial<PublicBrand> = {}): PublicBrand {
  return {
    id: '0190a000-0000-7000-8000-000000000001',
    name: 'Kuaför Elif',
    slug: 'kuafor-elif',
    shortDescription: 'Saç, bakım, makyaj',
    logoUrl: null,
    industryLabel: 'Kuaför',
    coverColor: 'petrol',
    showPrices: true,
    bookingEnabled: true,
    rating: null,
    branches: [
      {
        id: '0190a000-0000-7000-8000-000000000002',
        name: 'Kadıköy',
        addressLine: 'Moda Cad. 42',
        district: 'Kadıköy',
        city: 'İstanbul',
        phone: '+905321234567',
        timeZone: 'Europe/Istanbul',
        openNow: true,
      },
    ],
    ...overrides,
  };
}

type Responder = (request: Request) => Response;

/** Global fetch'i taklit eder; istemci (@omvila/api-client-ts) varsayılan olarak onu kullanır. */
export function mockApi(respond: Responder) {
  const calls: Request[] = [];
  vi.stubGlobal(
    'fetch',
    vi.fn((input: Request) => {
      calls.push(input);
      return Promise.resolve(respond(input));
    }),
  );
  return calls;
}

export const json = (status: number, body: unknown, headers: Record<string, string> = {}) =>
  new Response(JSON.stringify(body), {
    status,
    headers: { 'Content-Type': 'application/json', ...headers },
  });

export const problem = (status: number, code: string) =>
  new Response(JSON.stringify({ type: 'about:blank', title: code, status, code }), {
    status,
    headers: { 'Content-Type': 'application/problem+json' },
  });
