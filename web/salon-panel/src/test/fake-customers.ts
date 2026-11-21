import type { components } from '@omvila/api-client-ts';
import { maskPhone, parseMobilePhone } from '@omvila/ui-web/format';
import type { ManageSession } from '@/app/api';

// Sahte müşteri uçları (manageListCustomers, manageCreateCustomer, manageGetCustomer,
// manageListCustomerAppointments). Şekiller sözleşmeyle aynı; arama T-46 (5) kuralıyla (4 hane son 4 hane, 5+ hane
// numara parçası, diğerleri ad). Personel rolünde telefon maskeli. Kurgusal kişiler; bugün 25 Kasım 2026.
type CustomerSummary = components['schemas']['CustomerSummary'];
type CustomerAppointmentRow = components['schemas']['CustomerAppointmentRow'];

export interface CustomersFakeOptions {
  /** Markada hiç müşteri yok (MusterilerBos). */
  empty?: boolean;
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
      return false;
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

  return async (request: Request): Promise<Response | null> => {
    const url = new URL(request.url);
    const path = url.pathname;
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

    const match = /^\/api\/v1\/manage\/customers\/([^/]+)(\/appointments)?$/.exec(path);
    if (!match) {
      return null;
    }

    const customer = store.find((c) => c.id === match[1]);
    if (!customer) {
      return json(404, { code: 'not_found' });
    }

    if (match[2]) {
      return json(200, {
        items: customer.id === customerIds.selin ? selinAppointments : [],
        nextCursor: null,
      });
    }

    return json(200, view(customer));
  };
}
