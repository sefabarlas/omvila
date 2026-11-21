import type { components } from '@omvila/api-client-ts';
import { maskPhone, parseMobilePhone } from '@omvila/ui-web/format';
import type { ManageSession } from '@/app/api';

// Sahte müşteri uçları (manageListCustomers, manageCreateCustomer, manageGetCustomer,
// manageListCustomerAppointments; D5: Paketler ve Ödemeler sekmeleri, birleştirme, CSV içe/dışa aktarma, KVKK silme ve
// silme talepleri). Şekiller sözleşmeyle aynı; arama T-46 (5) kuralıyla (4 hane son 4 hane, 5+ hane
// numara parçası, diğerleri ad). Personel rolünde telefon maskeli. Kurgusal kişiler; bugün 25 Kasım 2026.
type CustomerSummary = components['schemas']['CustomerSummary'];
type CustomerAppointmentRow = components['schemas']['CustomerAppointmentRow'];
type CustomerPackage = components['schemas']['CustomerPackage'];
type PaymentView = components['schemas']['PaymentView'];
type DataDeletionRequestView = components['schemas']['DataDeletionRequestView'];

export interface CustomersFakeOptions {
  /** Markada hiç müşteri yok (MusterilerBos). */
  empty?: boolean;
  /** İçe aktarma önizlemesinde dosya hatası veya uygulamada süresi dolmuş önizleme. */
  importError?: 'too_large' | 'hold_expired';
  /** Silme talebi yok (AyarlarKvkk boş tablo). */
  noDeletionRequests?: boolean;
}

export const customerIds = {
  selin: '0193a8f0-0000-7000-8000-00000000c002',
  // Takvimdeki Derya Şahin randevusunun (a003) müşteri kimliği.
  derya: '0193a8f0-0000-7000-8000-00000000c003',
  gizem: '0193a8f0-0000-7000-8000-00000000c010',
  nur: '0193a8f0-0000-7000-8000-00000000c011',
  tulin: '0193a8f0-0000-7000-8000-00000000c012',
  created: '0193a8f0-0000-7000-8000-00000000c099',
} as const;

const money = (lira: number) => ({ amountMinor: lira * 100, currency: 'TRY' as const });

const base = {
  birthYear: null,
  noShowCount: 0,
  lastVisitOn: null,
  nextAppointmentUtc: null,
  isNew: false,
  isNoShowRisk: false,
  notes: null,
};

export const customersFixture: CustomerSummary[] = [
  {
    ...base,
    id: customerIds.selin,
    fullName: 'Selin Öztürk',
    phone: '+905324183321',
    birthYear: 1994,
    visitCount: 12,
    lastVisitOn: '2026-11-25',
    totalSpent: money(14860),
    tags: ['Düzenli'],
    notes: 'Boya: 7.1, %6 oksidan. Amonyaksız ürün tercih ediyor.',
  },
  {
    ...base,
    id: customerIds.derya,
    fullName: 'Derya Şahin',
    phone: '+905327602118',
    visitCount: 6,
    lastVisitOn: '2026-10-21',
    nextAppointmentUtc: '2026-11-25T10:00:00Z',
    totalSpent: money(3240),
    tags: ['Düzenli'],
  },
  {
    ...base,
    id: customerIds.gizem,
    fullName: 'Gizem Arslan',
    phone: '+905358814009',
    visitCount: 3,
    noShowCount: 2,
    lastVisitOn: '2026-09-14',
    totalSpent: money(960),
    isNoShowRisk: true,
    tags: ['2 kez gelmedi'],
  },
  {
    ...base,
    id: customerIds.nur,
    fullName: 'Nur Ateş',
    phone: '+905301175212',
    visitCount: 0,
    nextAppointmentUtc: '2026-11-25T14:00:00Z',
    totalSpent: money(0),
    isNew: true,
    tags: ['Yeni'],
  },
  {
    ...base,
    id: customerIds.tulin,
    fullName: 'Tülin Aksoy',
    phone: '+905329014418',
    visitCount: 3,
    lastVisitOn: '2026-09-14',
    totalSpent: money(2100),
    tags: [],
  },
];

const selinAppointments: CustomerAppointmentRow[] = [
  {
    appointmentId: '0193a8f0-0000-7000-8000-00000000a002',
    branchId: '0193a8f0-0000-7000-8000-0000000000b1',
    startUtc: '2026-11-25T07:30:00Z',
    status: 'CheckedIn',
    displayStatus: 'CheckedIn',
    serviceNames: ['Boya ve fön'],
    staffNames: ['Elif Kaya'],
    total: money(1440),
  },
  {
    appointmentId: '0193a8f0-0000-7000-8000-00000000a102',
    branchId: '0193a8f0-0000-7000-8000-0000000000b1',
    startUtc: '2026-10-02T08:00:00Z',
    status: 'Completed',
    displayStatus: 'Completed',
    serviceNames: ['Saç kesimi'],
    staffNames: ['Elif Kaya'],
    total: money(450),
  },
  {
    appointmentId: '0193a8f0-0000-7000-8000-00000000a103',
    branchId: '0193a8f0-0000-7000-8000-0000000000b1',
    startUtc: '2026-08-20T07:00:00Z',
    status: 'Cancelled',
    displayStatus: 'Cancelled',
    serviceNames: ['Keratin'],
    staffNames: ['Elif Kaya'],
    total: money(1800),
  },
];

const branchMerkez = '0193a8f0-0000-7000-8000-0000000000b1';

const packageBase = {
  packageId: '0193a8f0-0000-7000-8000-00000000f101',
  branchId: branchMerkez,
  isAvailableAllBranches: true,
  soldByStaffId: '0193a8f0-0000-7000-8000-0000000000c1',
  soldByStaffName: 'Elif Kaya',
};

/**
 * Selin'in paketleri (KVKK silme önizlemesi). Paketler sekmesi fake-sales'ten gelir (sales.packageCustomerId).
 */
export const selinPackages: CustomerPackage[] = [
  {
    ...packageBase,
    id: '0193a8f0-0000-7000-8000-00000000f201',
    customer: { brandCustomerId: customerIds.selin, fullName: 'Selin Öztürk' },
    packageName: 'Fön paketi',
    price: money(1700),
    purchasedUtc: '2026-10-01T08:00:00Z',
    expiresOn: '2026-12-31',
    status: 'Active',
    items: [
      {
        customerPackageItemId: '0193a8f0-0000-7000-8000-00000000f301',
        serviceId: '0193a8f0-0000-7000-8000-00000000f0f3',
        serviceName: 'Fön',
        totalSessions: 5,
        remainingSessions: 2,
      },
    ],
    totalSessions: 5,
    remainingSessions: 2,
    usages: [],
    version: '3',
  },
  {
    ...packageBase,
    id: '0193a8f0-0000-7000-8000-00000000f202',
    customer: { brandCustomerId: customerIds.selin, fullName: 'Selin Öztürk' },
    packageName: 'Manikür · 6 seans',
    price: money(1200),
    purchasedUtc: '2026-03-02T08:00:00Z',
    expiresOn: '2026-06-01',
    status: 'Expired',
    items: [],
    totalSessions: 6,
    remainingSessions: 1,
    usages: [],
    version: '7',
  },
];

const paymentBase = {
  branchId: branchMerkez,
  status: 'Recorded' as const,
  collectedByName: 'Elif Kaya',
  reference: null,
  customer: { brandCustomerId: customerIds.selin, fullName: 'Selin Öztürk' },
  serviceNames: [],
  isBalanceAfterDeposit: false,
  staffName: 'Elif Kaya',
  customerPackageId: null,
  packageName: null,
  packageSessionCount: null,
  recordedAfterDayClose: false,
  appointmentId: null,
  appointmentStartUtc: null,
};

/** Selin'in ödemeleri: kalan tahsilatı, kapora havalesi, paket satışı (yeniden eskiye). */
export const selinPayments: PaymentView[] = [
  {
    ...paymentBase,
    id: '0193a8f0-0000-7000-8000-00000000b401',
    method: 'Card',
    amount: money(1140),
    isDeposit: false,
    paidUtc: '2026-11-25T08:40:00Z',
    purpose: 'Appointment',
    appointmentId: '0193a8f0-0000-7000-8000-00000000a002',
    appointmentStartUtc: '2026-11-25T07:30:00Z',
    serviceNames: ['Boya', 'Fön'],
    isBalanceAfterDeposit: true,
  },
  {
    ...paymentBase,
    id: '0193a8f0-0000-7000-8000-00000000b402',
    method: 'Transfer',
    amount: money(300),
    isDeposit: true,
    paidUtc: '2026-11-23T10:00:00Z',
    purpose: 'Appointment',
    reference: 'OMV-7K2X',
    appointmentId: '0193a8f0-0000-7000-8000-00000000a002',
    appointmentStartUtc: '2026-11-25T07:30:00Z',
    serviceNames: ['Boya', 'Fön'],
  },
  {
    ...paymentBase,
    id: '0193a8f0-0000-7000-8000-00000000b403',
    method: 'Cash',
    amount: money(1700),
    isDeposit: false,
    status: 'PartiallyRefunded',
    refundedAmount: money(340),
    paidUtc: '2026-10-01T08:00:00Z',
    purpose: 'CustomerPackage',
    customerPackageId: '0193a8f0-0000-7000-8000-00000000f201',
    packageName: 'Fön paketi',
    packageSessionCount: 5,
  },
];

export const deletionRequestsFixture: DataDeletionRequestView[] = [
  {
    id: '0193a8f0-0000-7000-8000-00000000d501',
    brandCustomerId: customerIds.tulin,
    customerDisplayName: 'Tülin Aksoy',
    requestedVia: 'CustomerApp',
    requestedUtc: '2026-11-23T09:00:00Z',
    dueOn: '2026-12-23',
    status: 'Pending',
    completedUtc: null,
  },
  {
    id: '0193a8f0-0000-7000-8000-00000000d502',
    brandCustomerId: null,
    customerDisplayName: 'A•••• K••••',
    requestedVia: 'Panel',
    requestedUtc: '2026-11-02T09:00:00Z',
    dueOn: '2026-12-02',
    status: 'Completed',
    completedUtc: '2026-11-02T09:00:00Z',
  },
];

export const importId = '0193a8f0-0000-7000-8000-00000000c501';

/** Sahte sayfa boyu (istemcinin limit'inden bağımsız, "Daha fazla göster" denensin diye). */
export const fakeCustomerPageSize = 4;

const lapsedBefore = '2026-09-26';

function matches(customer: CustomerSummary, q: string): boolean {
  const digits = q.replace(/\D/g, '');
  const national = customer.phone?.replace(/^\+90/, '0') ?? '';
  if (/^[\d\s+()-]+$/.test(q) && digits.length > 0) {
    if (digits.length === 4) {
      return national.endsWith(digits);
    }

    const full = parseMobilePhone(q);
    return full ? customer.phone === full : national.includes(digits);
  }

  return customer.fullName.toLocaleLowerCase('tr-TR').includes(q.toLocaleLowerCase('tr-TR'));
}

function byFilter(customer: CustomerSummary, filter: string | null): boolean {
  switch (filter) {
    case 'Regular':
      return customer.tags.includes('Düzenli');
    case 'New':
      return customer.isNew === true;
    case 'NoShowRisk':
      return customer.isNoShowRisk === true;
    case 'Lapsed60':
      return customer.lastVisitOn !== null && (customer.lastVisitOn ?? '') < lapsedBefore;
    case 'WithPackage':
      return customer.id === customerIds.selin;
    default:
      return true;
  }
}

export function createCustomersFake(session: ManageSession, options: CustomersFakeOptions) {
  const store: CustomerSummary[] = options.empty ? [] : customersFixture.map((c) => ({ ...c }));
  const masked = session.membership.role === 'Staff';
  const view = (customer: CustomerSummary): CustomerSummary => {
    if (!masked || !customer.phone) {
      return customer;
    }

    const { phone, ...rest } = customer;
    return { ...rest, phoneMasked: maskPhone(phone) };
  };

  const json = (status: number, body: unknown) =>
    new Response(JSON.stringify(body), {
      status,
      headers: { 'Content-Type': 'application/json' },
    });

  const deletionRequests = options.noDeletionRequests
    ? []
    : deletionRequestsFixture.map((r) => ({ ...r }));
  const normalize = (name: string) => name.trim().toLocaleLowerCase('tr-TR');

  return async (request: Request): Promise<Response | null> => {
    const url = new URL(request.url);
    const path = url.pathname;
    if (path === '/api/v1/manage/customer-deletion-requests') {
      return json(200, { items: deletionRequests, nextCursor: null });
    }

    if (path === '/api/v1/manage/customers/export') {
      const rows = store.filter((c) => byFilter(c, url.searchParams.get('filter')));
      const csv = rows.map((c) => `${c.fullName};${c.phone ?? ''}`).join('\n');
      return new Response(`\uFEFFAd;Telefon\n${csv}`, {
        status: 200,
        headers: {
          'Content-Type': 'text/csv',
          'Content-Disposition': 'attachment; filename="musteriler-2026-11-25.csv"',
        },
      });
    }

    if (path === '/api/v1/manage/customers/imports') {
      // Gövde multipart ve "file" alanlı olmalı. jsdom'da formData() ayrıştırması ve dosya adı taşınmadığından ham
      // gövdeye bakılır; dosya adı sözleşme örneğindeki gibi sabittir.
      const contentType = request.headers.get('Content-Type') ?? '';
      const raw = await request.clone().text();
      const fileName = /name="file"; filename="[^"]+"/.test(raw)
        ? 'musteriler-eski-program.csv'
        : null;
      if (!contentType.startsWith('multipart/form-data') || !fileName) {
        return json(400, { code: 'validation_failed', errors: { file: ['invalid_format'] } });
      }

      if (options.importError === 'too_large') {
        return json(400, { code: 'validation_failed', errors: { file: ['too_large'] } });
      }

      return json(201, {
        importId,
        fileName,
        totalRows: 1248,
        toCreate: 1204,
        toMerge: 38,
        invalidRows: 6,
        mappedColumns: ['FullName', 'Phone', 'Notes'],
        errors: [14, 88, 301, 560, 902, 1101].map((rowNumber) => ({
          rowNumber,
          column: 'Telefon',
          code: 'invalid_phone',
        })),
        status: 'Previewed',
        expiresUtc: '2026-11-25T10:00:00Z',
      });
    }

    if (path === `/api/v1/manage/customers/imports/${importId}/commit`) {
      return options.importError === 'hold_expired'
        ? json(409, { code: 'hold_expired' })
        : json(200, { importId, status: 'Committed', created: 1204, merged: 38, skipped: 6 });
    }
    if (path === '/api/v1/manage/customers') {
      if (request.method === 'GET') {
        const q = url.searchParams.get('q')?.trim() ?? '';
        const filter = url.searchParams.get('filter');
        const start = Number(url.searchParams.get('cursor') ?? '0');
        const found = store.filter((c) => (q === '' || matches(c, q)) && byFilter(c, filter));
        const page = found.slice(start, start + fakeCustomerPageSize);
        const next = start + fakeCustomerPageSize;
        return json(200, {
          items: page.map(view),
          nextCursor: next < found.length ? String(next) : null,
        });
      }

      const body = (await request.clone().json()) as components['schemas']['CustomerCreate'];
      const phone = parseMobilePhone(body.phone);
      if (!phone) {
        return json(400, { code: 'validation_failed', errors: { phone: ['invalid_format'] } });
      }

      const existing = store.find((c) => c.phone === phone);
      if (existing) {
        return json(409, {
          code: 'phone_already_exists',
          meta: { brandCustomerId: existing.id },
        });
      }

      const created: CustomerSummary = {
        ...base,
        id: customerIds.created,
        fullName: body.fullName,
        phone,
        birthYear: body.birthYear ?? null,
        visitCount: 0,
        totalSpent: money(0),
        isNew: true,
        tags: ['Yeni'],
        notes: body.notes ?? null,
      };
      store.unshift(created);
      return json(201, view(created));
    }

    const match =
      /^\/api\/v1\/manage\/customers\/([^/]+)(\/appointments|\/payments|\/merge|\/deletion)?$/.exec(
        path,
      );
    if (!match) {
      return null;
    }

    const customer = store.find((c) => c.id === match[1]);
    if (!customer) {
      return json(404, { code: 'not_found' });
    }

    const isSelin = customer.id === customerIds.selin;
    switch (match[2]) {
      case '/appointments':
        return json(200, { items: isSelin ? selinAppointments : [], nextCursor: null });
      case '/payments':
        return session.permissions.includes('payment.collect')
          ? json(200, { items: isSelin ? selinPayments : [], nextCursor: null })
          : json(403, { code: 'forbidden' });
      case '/merge': {
        const body = (await request
          .clone()
          .json()) as components['schemas']['CustomerMergeRequest'];
        const source = store.find((c) => c.id === body.sourceBrandCustomerId);
        if (!source || source.id === customer.id) {
          return json(400, {
            code: 'validation_failed',
            errors: { sourceBrandCustomerId: ['invalid'] },
          });
        }

        store.splice(store.indexOf(source), 1);
        customer.visitCount += source.visitCount;
        customer.noShowCount += source.noShowCount;
        if (body.mergeNotes !== false && source.notes) {
          customer.notes = customer.notes ? `${customer.notes}\n${source.notes}` : source.notes;
        }

        return json(200, view(customer));
      }

      case '/deletion': {
        const active = isSelin ? selinPackages.filter((p) => p.status === 'Active') : [];
        if (request.method === 'GET') {
          return json(200, {
            fullName: customer.fullName,
            appointmentCount: isSelin ? 12 : customer.visitCount,
            paymentCount: isSelin ? 9 : 0,
            upcomingAppointmentCount: customer.nextAppointmentUtc ? 1 : 0,
            activePackages: active.map((p) => ({
              customerPackageId: p.id,
              packageName: p.packageName,
              remainingSessions: p.remainingSessions,
            })),
          });
        }

        const body = (await request.clone().json()) as { confirmFullName: string };
        if (normalize(body.confirmFullName) !== normalize(customer.fullName)) {
          return json(400, {
            code: 'validation_failed',
            errors: { confirmFullName: ['mismatch'] },
          });
        }

        store.splice(store.indexOf(customer), 1);
        const done: DataDeletionRequestView = {
          id: '0193a8f0-0000-7000-8000-00000000d599',
          brandCustomerId: null,
          customerDisplayName: 'S•••• Ö••••',
          requestedVia: 'Panel',
          requestedUtc: '2026-11-25T09:00:00Z',
          dueOn: '2026-12-25',
          status: 'Completed',
          completedUtc: '2026-11-25T09:00:00Z',
        };
        deletionRequests.unshift(done);
        return json(200, done);
      }

      default:
        return json(200, view(customer));
    }
  };
}
