import type { Page } from '@playwright/test';
import { calendarDemo, demoStaff } from './calendar-demo';

// "Tamamla ve ödeme al" (salon-paneli/OdemeAl) ve Kasa (Kasa, KasaBos) ekranlarının tasarım verisi. Prism'in örnek
// randevusu ve kasası tasarımdan farklı olduğu için yanıtı test verir. Saatler İstanbul yerel saatidir. Kurgusal kişiler.
const cors = {
  'Access-Control-Allow-Origin': 'http://localhost:5180',
  'Access-Control-Allow-Credentials': 'true',
};
const json = (body: unknown) => ({
  status: 200,
  contentType: 'application/json',
  headers: cors,
  body: JSON.stringify(body),
});
const utc = (date: string, time: string) => new Date(`${date}T${time}:00+03:00`).toISOString();
const money = (amountMinor: number) => ({ amountMinor, currency: 'TRY' });
const today = '2026-11-25';
const branchId = '0193a8f0-0000-7000-8000-0000000000b1';

/** demoCalendar'ın ilk günündeki 2. satır: Selin Öztürk 10:30–12:30, Geldi. */
export const selinAppointmentId = '0193a8f0-0000-7000-8000-000000000001';

/** KK5: Boya 1.250 + Fön 190, 11 Kasım'da alınan 200 ₺ kapora; alınacak 1.240 ₺. */
export function selinDetail(status: 'CheckedIn' | 'Completed' = 'CheckedIn') {
  const item = (id: string, serviceName: string, price: number, start: string, end: string) => ({
    id,
    serviceId: `0193a8f0-0000-7000-8000-0000000000${id.slice(-2)}`,
    serviceName,
    staffId: demoStaff[0]?.staffId,
    staffName: 'Elif Kaya',
    startUtc: utc(today, start),
    endUtc: utc(today, end),
    price: money(price),
    paidFromPackage: false,
    status: status === 'Completed' ? 'Completed' : 'Active',
    segments: [],
  });
  return {
    id: selinAppointmentId,
    reference: 'A-20498',
    branchId,
    status,
    displayStatus: status,
    channel: 'Online',
    startUtc: utc(today, '10:30'),
    endUtc: utc(today, '12:00'),
    timeZone: 'Europe/Istanbul',
    holdExpiresUtc: null,
    customer: {
      brandCustomerId: '0193a8f0-0000-7000-8000-0000000000d2',
      fullName: 'Selin Öztürk',
      phone: '+905321234521',
      visitCount: 12,
      isNew: false,
    },
    items: [
      item('0193a8f0-0000-7000-8000-0000000000e1', 'Boya', 125000, '10:30', '11:30'),
      item('0193a8f0-0000-7000-8000-0000000000e2', 'Fön', 19000, '11:30', '12:00'),
    ],
    deposit: {
      required: money(20000),
      status: 'Received',
      transferCode: 'OMV-4F7K',
      receivedUtc: utc('2026-11-11', '14:20'),
    },
    total: money(144000),
    amountDue: money(status === 'Completed' ? 0 : 124000),
    isRecurring: false,
    noShowAtUtc: null,
    cancellation: null,
    payments: [
      {
        id: '0193a8f0-0000-7000-8000-0000000006a1',
        branchId,
        method: 'Transfer',
        amount: money(20000),
        isDeposit: true,
        status: 'Recorded',
        paidUtc: utc('2026-11-11', '14:20'),
        purpose: 'Appointment',
        appointmentId: selinAppointmentId,
        recordedAfterDayClose: false,
      },
    ],
    history: [],
    version: '4',
  };
}

/** OdemeAl: takvim demosu; randevu ayrıntısı Selin'in KK5 randevusu, müşterinin paketi yok. */
export async function checkoutDemo(page: Page) {
  await calendarDemo(page);
  await page.route(/\/api\/v1\/manage\/appointments\/[^/]+$/, (route) =>
    route.request().method() === 'OPTIONS' ? route.fallback() : route.fulfill(json(selinDetail())),
  );
  await page.route(/\/api\/v1\/manage\/customers\/[^/]+\/packages/, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(json({ items: [], nextCursor: null })),
  );
}

// [saat, müşteri, kalem alanları, yöntem, personel, tutar]
type Row = [string, string, Record<string, unknown>, string, string | null, number];
const kasaRows: Row[] = [
  [
    '12:05',
    'Selin Öztürk',
    { serviceNames: ['Boya', 'Fön'], isBalanceAfterDeposit: true },
    'Card',
    'Elif Kaya',
    124000,
  ],
  ['11:40', 'Okan Er', { serviceNames: ['Saç kesimi'] }, 'Cash', 'Mert Demir', 45000],
  [
    '11:35',
    'Melis Tunç',
    { serviceNames: ['Manikür ve pedikür'] },
    'PackageRedemption',
    'Zeynep Aksoy',
    0,
  ],
  [
    '11:10',
    'Gizem Arslan',
    { isDeposit: true, appointmentStartUtc: utc(today, '14:30') },
    'Transfer',
    null,
    15000,
  ],
  ['10:20', 'Ayşe Yılmaz', { serviceNames: ['Saç kesimi'] }, 'Cash', 'Elif Kaya', 45000],
  ['10:05', 'Can Demir', { serviceNames: ['Sakal ve kesim'] }, 'Card', 'Mert Demir', 55000],
  [
    '09:50',
    'Hande Er',
    { purpose: 'CustomerPackage', packageName: 'Fön paketi', packageSessionCount: 5 },
    'Transfer',
    'Can Yücel',
    50000,
  ],
];

/** Kasa (tasarımdaki bloklar: 14 ödemenin toplamı; tabloda ilk yedisi görünür, tasarım gibi). */
/** branch: istekteki şube (Prism oturumunun şubesi); kapatma gövdesi bu kimlikle gider. */
export function demoCashDay(empty = false, branch = branchId) {
  const zero = { amount: money(0), count: 0 };
  return {
    branchId: branch,
    date: today,
    timeZone: 'Europe/Istanbul',
    status: 'Open',
    total: empty ? zero : { amount: money(1044000), count: 14 },
    cash: empty ? zero : { amount: money(345000), count: 6 },
    card: empty ? zero : { amount: money(589000), count: 5 },
    transfer: {
      ...(empty ? zero : { amount: money(110000), count: 3 }),
      depositAmount: money(empty ? 0 : 60000),
      otherAmount: money(empty ? 0 : 50000),
    },
    cashRefunded: money(0),
    expectedCash: money(empty ? 0 : 345000),
    payments: empty
      ? []
      : kasaRows.map(([time, name, extra, method, staffName, amount], i) => ({
          id: `0193a8f0-0000-7000-8000-0000000c0${String(i).padStart(3, '0')}`,
          branchId: branch,
          method,
          amount: money(amount),
          isDeposit: false,
          status: 'Recorded',
          paidUtc: utc(today, time),
          purpose: 'Appointment',
          customer: {
            brandCustomerId: `0193a8f0-0000-7000-8000-0000000d0${String(i).padStart(3, '0')}`,
            fullName: name,
          },
          staffName,
          recordedAfterDayClose: false,
          ...extra,
        })),
    closing: null,
  };
}

/** Kasa ve KasaBos: takvim demosunun oturumu (sahip) ve saati; kasa yanıtı tasarım verisi. */
export async function cashDemo(page: Page, { empty = false }: { empty?: boolean } = {}) {
  await calendarDemo(page);
  await page.route(/\/api\/v1\/manage\/cash\?/, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(
          json(
            demoCashDay(
              empty,
              new URL(route.request().url()).searchParams.get('branchId') ?? branchId,
            ),
          ),
        ),
  );
}
