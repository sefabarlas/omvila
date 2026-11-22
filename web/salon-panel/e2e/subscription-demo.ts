import type { Page } from '@playwright/test';
import { demoNow, ownerMembership, ownerPermissions } from './calendar-demo';
import { sessionAs } from './support';

// Abonelik ve fatura (salon-paneli/AyarlarAbonelik), kontör al ve abonelik şeritleri (GenelBakisGecikti,
// TakvimSaltOkunur, Durumlar). Prism örneği hesap ve fiyatı null döner (S-14; test sunucusu böyle görünür); "dolu"
// görüntü için alıcı, IBAN ve fiyatlar kurgusaldır. Gün 25 Kasım 2026 (takvim demosu).
const corsJson = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});
const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' });
const api = /\/api\/v1\/manage\//;

export const demoAccount = {
  accountHolderName: 'Örnek Yazılım A.Ş.',
  iban: 'TR000000000000000000000001',
  bankName: null,
};

const invoice = (id: string, no: string, start: string, end: string) => ({
  id,
  invoiceNumber: no,
  periodStart: start,
  periodEnd: end,
  amount: money(100000),
  tax: money(20000),
  total: money(120000),
  status: 'Paid',
  issuedUtc: `${start}T06:00:00Z`,
  paidUtc: `${start}T09:12:00Z`,
  paidVia: 'BankTransfer',
});

const prices: Record<string, number> = { starter: 25000, medium: 100000, large: 350000 };

/** Tam yetkili sahip oturumu, kurulumu bitmiş salon ve verilen abonelik özeti (şeritler). */
export async function subscriptionSession(
  page: Page,
  subscription: Record<string, unknown> = {},
  permissions: string[] = ownerPermissions,
) {
  await page.clock.setFixedTime(demoNow);
  await sessionAs(page, (session) => ({
    permissions,
    membership: ownerMembership,
    brand: { ...(session.brand as Record<string, unknown>), status: 'Active' },
    subscription: { ...(session.subscription as Record<string, unknown>), ...subscription },
  }));
}

/** AyarlarAbonelik: 9 gün kalan deneme, iki ödenmiş fatura; `configured` ise OMVILA hesabı ve fiyatlar tanımlı. */
export async function subscriptionDemo(page: Page, configured = true) {
  await subscriptionSession(page);
  if (!configured) {
    return;
  }

  await page.route(api, async (route) => {
    const request = route.request();
    const path = new URL(request.url()).pathname;
    if (request.method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    if (path.endsWith('/manage/subscription')) {
      const response = await route.fetch();
      const detail = (await response.json()) as Record<string, unknown>;
      await route.fulfill(
        corsJson({ ...detail, amountDue: money(120000), omvilaBankAccount: demoAccount }),
      );
    } else if (path.endsWith('/manage/subscription/invoices')) {
      await route.fulfill(
        corsJson({
          items: [
            invoice(
              '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9a01',
              'OMV-2026-000184',
              '2026-10-01',
              '2026-10-31',
            ),
            invoice(
              '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9a02',
              'OMV-2026-000121',
              '2026-09-01',
              '2026-09-30',
            ),
          ],
          nextCursor: null,
        }),
      );
    } else if (path.endsWith('/manage/credit-packages')) {
      const response = await route.fetch();
      const list = (await response.json()) as { items: { code: string }[] };
      await route.fulfill(
        corsJson({
          items: list.items.map((item) => ({ ...item, price: money(prices[item.code] ?? 0) })),
        }),
      );
    } else if (path.endsWith('/manage/credits/purchase')) {
      const response = await route.fetch();
      const info = (await response.json()) as Record<string, unknown>;
      await route.fulfill(
        corsJson({ ...info, amount: money(100000), omvilaBankAccount: demoAccount }),
      );
    } else {
      await route.fallback();
    }
  });
}
