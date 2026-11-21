import type { Page } from '@playwright/test';
import { demoNow } from './calendar-demo';
import { sessionAs } from './support';

// Müşteri ekran görüntüleri için salon-paneli/Musteriler ve MusteriKarti verisi (Prism örneği tek müşteri döner).
// Arama T-46 (5) kuralıyla süzülür; telefonlar ve kişiler kurgusal.
const corsJson = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});

const money = (lira: number) => ({ amountMinor: lira * 100, currency: 'TRY' });

// [ad, telefon, ziyaret, son ziyaret, sonraki randevu (yerel), toplam ₺, etiketler, gelmedi]
type DemoCustomer = [
  string,
  string,
  number,
  string | null,
  string | null,
  number,
  string[],
  number?,
];
const demoCustomers: DemoCustomer[] = [
  ['Selin Öztürk', '+905324183321', 12, '2026-11-25', null, 14860, ['Düzenli']],
  ['Derya Şahin', '+905327602118', 6, '2026-10-21', '2026-11-25T13:00', 3240, ['Düzenli']],
  ['Pınar Yalçın', '+905335120941', 4, '2026-10-02', '2026-11-25T16:00', 3400, []],
  ['Gizem Arslan', '+905358814009', 3, '2026-09-14', '2026-11-25T14:30', 960, ['2 kez gelmedi'], 2],
  ['Buse Kaya', '+905412331877', 9, '2026-11-18', '2026-11-25T15:30', 7150, ['Düzenli']],
  ['Melis Tunç', '+905326497064', 15, '2026-11-25', '2026-12-12T11:00', 9900, ['Düzenli']],
  ['Nur Ateş', '+905301175212', 0, null, '2026-11-25T17:00', 0, ['Yeni']],
  ['Ayşe Yılmaz', '+905423906130', 21, '2026-11-25', null, 18300, ['Düzenli']],
  ['Hande Er', '+905367042288', 7, '2026-11-25', '2027-01-06T10:00', 8050, []],
  ['Ece Koç', '+905394189052', 5, '2026-11-25', null, 2100, ['2 kez gelmedi'], 2],
];

const customers = demoCustomers.map(
  ([fullName, phone, visits, last, next, total, tags, noShows], i) => ({
    id: `0193a8f0-0000-7000-8000-00000000d${String(i).padStart(3, '0')}`,
    fullName,
    phone,
    birthYear: i === 0 ? 1994 : null,
    visitCount: visits,
    noShowCount: noShows ?? 0,
    lastVisitOn: last,
    nextAppointmentUtc: next ? new Date(`${next}:00+03:00`).toISOString() : null,
    totalSpent: money(total),
    isNew: visits === 0,
    isNoShowRisk: (noShows ?? 0) >= 2,
    tags,
    notes:
      i === 0
        ? 'Boya: 7.1, %6 oksidan. Amonyaksız ürün tercih ediyor. Çarşamba öğleden sonra müsait.'
        : null,
  }),
);

export const demoCustomerId = customers[0]?.id ?? '';

const appointment = (
  n: number,
  local: string,
  service: string,
  staff: string,
  status: string,
  total: number,
) => ({
  appointmentId: `0193a8f0-0000-7000-8000-0000000a${String(n).padStart(4, '0')}`,
  branchId: '0193a8f0-0000-7000-8000-0000000000b1',
  startUtc: new Date(`${local}:00+03:00`).toISOString(),
  status,
  displayStatus: status,
  serviceNames: [service],
  staffNames: [staff],
  total: money(total),
});

const demoAppointments = [
  appointment(1, '2026-11-25T10:30', 'Boya ve fön', 'Elif Kaya', 'CheckedIn', 1440),
  appointment(2, '2026-10-21T14:00', 'Fön', 'Elif Kaya', 'Completed', 350),
  appointment(3, '2026-10-02T11:00', 'Saç kesimi', 'Elif Kaya', 'Completed', 450),
  appointment(4, '2026-09-12T16:30', 'Boya', 'Zeynep Aksoy', 'Completed', 1250),
  appointment(5, '2026-08-20T10:00', 'Keratin', 'Elif Kaya', 'Cancelled', 1800),
];

function matches(customer: (typeof customers)[number], q: string) {
  const digits = q.replace(/\D/g, '');
  if (/^[\d\s+]+$/.test(q) && digits.length > 0) {
    return digits.length === 4
      ? customer.phone.endsWith(digits)
      : customer.phone.includes(digits.replace(/^0/, ''));
  }

  return customer.fullName.toLocaleLowerCase('tr-TR').includes(q.toLocaleLowerCase('tr-TR'));
}

/** Müşteri listesi, kartı ve randevuları; tam yetkili sahip oturumu. `empty`: hiç müşteri yok (MusterilerBos). */
export async function customersDemo(page: Page, { empty = false } = {}) {
  await page.clock.setFixedTime(demoNow);
  await sessionAs(page, {
    permissions: [
      'settings.manage',
      'service.manage',
      'staff.manage',
      'calendar.view',
      'calendar.view.all-staff',
      'customer.view',
      'customer.phone.view',
      'customer.edit',
      'report.view',
    ],
    membership: { role: 'Owner', branchIds: [], staffId: '0193a8f0-0000-7000-8000-0000000000c1' },
  });
  await page.route(/\/api\/v1\/manage\/customers(\?.*)?$/, async (route) => {
    const request = route.request();
    if (request.method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    if (request.method() === 'POST') {
      await route.fulfill(
        corsJson(
          {
            code: 'phone_already_exists',
            status: 409,
            meta: { brandCustomerId: customers[3]?.id },
          },
          409,
        ),
      );
      return;
    }

    const url = new URL(request.url());
    const q = url.searchParams.get('q') ?? '';
    const filter = url.searchParams.get('filter');
    const items = empty
      ? []
      : customers.filter(
          (c) =>
            (q === '' || matches(c, q)) &&
            (filter !== 'Regular' || c.tags.includes('Düzenli')) &&
            filter !== 'WithPackage',
        );
    await route.fulfill(corsJson({ items, nextCursor: null }));
  });
  await page.route(/\/api\/v1\/manage\/customers\/[^/?]+(\/appointments)?(\?.*)?$/, (route) => {
    if (route.request().method() === 'OPTIONS') {
      return route.fallback();
    }

    const url = new URL(route.request().url());
    const [, , , , , id, tab] = url.pathname.split('/');
    const customer = customers.find((c) => c.id === id);
    if (!customer) {
      return route.fulfill(corsJson({ code: 'not_found', status: 404 }, 404));
    }

    return route.fulfill(
      corsJson(
        tab === 'appointments'
          ? {
              items: customer.id === demoCustomerId ? demoAppointments : [],
              nextCursor: null,
            }
          : customer,
      ),
    );
  });
}
