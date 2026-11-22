import type { components } from '@omvila/api-client-ts';
import type { ManageSession } from '@/app/api';

// Sahte abonelik uçları (manageGetSubscription, manageListInvoices, manageGetInvoicePdf, manageNotifyTransfer,
// manageListCreditPackages, managePurchaseCredits). Veriler sözleşme örneklerinden; hesap ve IBAN kurgusal.
type SubscriptionDetail = components['schemas']['SubscriptionDetail'];
type SubscriptionInvoice = components['schemas']['SubscriptionInvoice'];

export interface SubscriptionFakeOptions {
  /** OMVILA hesabı ve fiyat tanımlı (S-14); yoksa ikisi null (test sunucusu). */
  configured?: boolean;
  /** Detay alanlarının üstüne yazılır (durum, bekleyen bildirim…). */
  detail?: Partial<SubscriptionDetail>;
  /** Fatura yok (deneme). */
  noInvoices?: boolean;
  /** Abonelik ucu 500 döner. */
  fails?: boolean;
  /** Havale bildirimi hata kodu. */
  notifyError?: 'payment_notice_already_pending' | 'receipt_too_large';
  /** Gönderilen bildirim formları (FormData alanları). */
  notices?: Record<string, string>[];
}

const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' as const });
const json = (status: number, body: unknown) =>
  new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } });

export const omvilaAccountFixture = {
  accountHolderName: 'Örnek Yazılım A.Ş.',
  iban: 'TR000000000000000000000001',
  bankName: null,
};

export const creditPackagesFixture = [
  { id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9c01', code: 'starter', name: 'Başlangıç', credits: 1000 },
  { id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9c02', code: 'medium', name: 'Orta', credits: 5000 },
  { id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9c03', code: 'large', name: 'Büyük', credits: 20000 },
];

const prices: Record<string, number> = { starter: 25000, medium: 100000, large: 350000 };

export function subscriptionFixture(configured: boolean): SubscriptionDetail {
  return {
    status: 'Trialing',
    plan: { code: 'salon', name: 'Salon' },
    billingCycle: 'Monthly',
    paymentMethod: null,
    trialEndsOn: '2026-12-04',
    currentPeriodStart: '2026-11-20',
    currentPeriodEnd: '2026-12-04',
    pastDueSince: null,
    readOnlyOn: null,
    readOnlySince: null,
    branches: { count: 2, included: 1, extra: 1 },
    staff: { count: 5, limit: null },
    amountDue: configured ? money(120000) : null,
    transferCode: 'ABN-KE-1125',
    omvilaBankAccount: configured ? omvilaAccountFixture : null,
    pendingNotice: null,
  };
}

const invoice = (id: string, no: string, start: string, end: string): SubscriptionInvoice => ({
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

export const invoicesFixture = [
  invoice('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9a01', 'OMV-2026-000184', '2026-10-01', '2026-10-31'),
  invoice('0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9a02', 'OMV-2026-000121', '2026-09-01', '2026-09-30'),
];

export function createSubscriptionFake(session: ManageSession, options: SubscriptionFakeOptions) {
  const configured = options.configured ?? false;
  const owner = session.permissions.includes('subscription.manage');
  return async (request: Request): Promise<Response | null> => {
    const path = new URL(request.url).pathname;
    const routes = [
      '/api/v1/manage/subscription',
      '/api/v1/manage/credit-packages',
      '/api/v1/manage/credits/purchase',
    ];
    if (!routes.some((route) => path.startsWith(route))) {
      return null;
    }

    if (!owner) {
      return json(403, { code: 'forbidden' });
    }

    if (path === '/api/v1/manage/subscription') {
      return options.fails
        ? json(500, { code: 'internal_error' })
        : json(200, { ...subscriptionFixture(configured), ...options.detail });
    }

    if (path === '/api/v1/manage/subscription/invoices') {
      return json(200, { items: options.noInvoices ? [] : invoicesFixture, nextCursor: null });
    }

    const pdf = /^\/api\/v1\/manage\/subscription\/invoices\/([^/]+)\/pdf$/.exec(path);
    if (pdf) {
      const found = invoicesFixture.find((item) => item.id === pdf[1]);
      return found
        ? new Response(new Blob(['%PDF-1.4'], { type: 'application/pdf' }), {
            status: 200,
            headers: {
              'Content-Type': 'application/pdf',
              'Content-Disposition': `attachment; filename="${found.invoiceNumber}.pdf"`,
            },
          })
        : json(404, { code: 'not_found' });
    }

    if (path === '/api/v1/manage/subscription/payment-notices') {
      // jsdom'un File'ı fetch'in formData() ayrıştırmasına uymaz; ham multipart gövde okunur. Dosya alanı "file" olarak
      // kaydedilir (jsdom dosya adını taşımayabilir).
      const contentType = request.headers.get('Content-Type') ?? '';
      if (!contentType.startsWith('multipart/form-data')) {
        return json(400, { code: 'validation_failed' });
      }

      const raw = await request.text();
      const fields: Record<string, string> = {};
      const part = /name="([^"]+)"(; filename="[^"]*")?\r\n(?:[^\r\n]+\r\n)*\r\n([^\r]*)/g;
      for (const [, name = '', file, value = ''] of raw.matchAll(part)) {
        fields[name] = file === undefined ? value : 'file';
      }
      options.notices?.push(fields);
      if (options.notifyError === 'payment_notice_already_pending') {
        return json(409, { code: 'payment_notice_already_pending', meta: {} });
      }

      if (options.notifyError === 'receipt_too_large') {
        return json(400, { code: 'validation_failed', errors: { receipt: ['too_large'] } });
      }

      const credits = creditPackagesFixture.find((item) => item.id === fields.creditPackageId);
      return json(201, {
        id: '0193a1b2-7c4d-7e8f-9a0b-1c2d3e4f9b01',
        purpose: fields.purpose,
        transferCode: credits ? 'KNT-KE-1125' : 'ABN-KE-1125',
        expectedAmount: null,
        credits: credits?.credits ?? null,
        senderName: fields.senderName ?? null,
        hasReceipt: fields.receipt !== undefined,
        status: 'Pending',
        notifiedUtc: '2026-11-25T09:00:00Z',
      });
    }

    if (path === '/api/v1/manage/credit-packages') {
      return json(200, {
        items: creditPackagesFixture.map((item) => ({
          ...item,
          price: configured ? money(prices[item.code] ?? 0) : null,
        })),
      });
    }

    if (path === '/api/v1/manage/credits/purchase') {
      const body = (await request.json()) as { creditPackageId: string };
      const found = creditPackagesFixture.find((item) => item.id === body.creditPackageId);
      return found
        ? json(200, {
            creditPackageId: found.id,
            credits: found.credits,
            amount: configured ? money(prices[found.code] ?? 0) : null,
            transferCode: 'KNT-KE-1125',
            omvilaBankAccount: configured ? omvilaAccountFixture : null,
          })
        : json(404, { code: 'not_found' });
    }

    return json(404, { code: 'not_found' });
  };
}
