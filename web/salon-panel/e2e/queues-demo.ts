import type { Page } from '@playwright/test';
import { demoNow, ownerMembership, ownerPermissions } from './calendar-demo';
import { sessionAs } from './support';

// Bekleme listesi ve kapıda sıra ekran görüntüleri için salon-paneli/BeklemeListesi ve KapidaSira verisi (Prism
// örnekleri tek satır döner). "Şimdi" takvim demosuyla aynı: 25 Kasım 2026 12:10 İstanbul (09:10Z). Kurgusal kişiler.
const corsJson = (body: unknown, status = 200) => ({
  status,
  contentType: 'application/json',
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});

const nowUtc = new Date(demoNow).toISOString();
const branchId = '0193a8f0-0000-7000-8000-0000000000b1';
const elif = { id: '0193a8f0-0000-7000-8000-0000000000c1', name: 'Elif Kaya' };
const mert = { id: '0193a8f0-0000-7000-8000-0000000000c2', name: 'Mert Demir' };

// [ad, hizmet, personel, katılma (yerel), günler, saat dilimleri]
type DemoEntry = [string, string, typeof elif | null, string, string[], string[]];
const demoEntries: DemoEntry[] = [
  ['Nur Ateş', 'Fön', elif, '2026-11-22T10:00', ['2026-11-26', '2026-11-27'], ['Morning']],
  ['Tuğba Er', 'Keratin', elif, '2026-11-19T15:30', [], ['Afternoon', 'Evening']],
  ['Kaan Polat', 'Sakal ve kesim', mert, '2026-11-23T12:00', ['2026-11-28'], []],
  ['İrem Sarı', 'Manikür', null, '2026-11-24T18:00', [], []],
];

const waitlistItems = demoEntries.map(([fullName, service, staff, joined, dates, windows], i) => ({
  id: `0193a8f0-0000-7000-8000-00000000e00${String(i)}`,
  branchId,
  status: 'Waiting',
  customer: {
    brandCustomerId: `0193a8f0-0000-7000-8000-00000000d10${String(i)}`,
    fullName,
    phoneMasked: '0532 *** ** 12',
  },
  serviceIds: ['0193a8f0-0000-7000-8000-0000000000f1'],
  serviceNames: [service],
  preferredStaffId: staff?.id ?? null,
  preferredStaffName: staff?.name ?? null,
  acceptOtherStaff: staff === null,
  dates,
  timeWindows: windows,
  note: null,
  joinedUtc: new Date(`${joined}:00+03:00`).toISOString(),
  source: 'Online',
  // Perşembe 11:00 boşaldı; teklif 12:06'da gitti, 12:21'de düşer (12:10'da 11 dk kaldı).
  pendingOffer:
    i === 0
      ? {
          id: '0193a8f0-0000-7000-8000-00000000e101',
          status: 'Pending',
          startUtc: '2026-11-26T08:00:00Z',
          endUtc: '2026-11-26T08:45:00Z',
          staffId: elif.id,
          staffName: elif.name,
          offeredUtc: '2026-11-25T09:06:00Z',
          holdExpiresUtc: '2026-11-25T09:21:00Z',
        }
      : null,
  version: 'W/"1"',
}));

// [no, ad, hizmet, personel, katılma (yerel saat)]
type DemoTicket = [number, string, string | null, typeof mert | null, string];
const demoTickets: DemoTicket[] = [
  [8, 'Okan Er', 'Saç kesimi', mert, '11:58'],
  [9, 'Murat Çelik', 'Sakal', null, '12:02'],
  [10, 'Emre Yavuz', 'Saç kesimi', mert, '12:08'],
];

const tickets = demoTickets.map(([queueNumber, customerName, service, staff, joined], i) => ({
  id: `0193a8f0-0000-7000-8000-00000000f00${String(i)}`,
  branchId,
  queueDate: '2026-11-25',
  queueNumber,
  status: 'Waiting',
  customerName,
  brandCustomerId: null,
  phoneMasked: null,
  serviceId: service ? '0193a8f0-0000-7000-8000-0000000000f1' : null,
  serviceName: service,
  preferredStaffId: staff?.id ?? null,
  preferredStaffName: staff?.name ?? null,
  joinedUtc: new Date(`2026-11-25T${joined}:00+03:00`).toISOString(),
  estimatedWaitMinutes: null,
  appointmentId: null,
  version: 'W/"1"',
}));

/** Bekleme listesi ve kapıda sıra uçları; tam yetkili sahip oturumu. `empty`: ikisi de boş (…Bos ekranları). */
export async function queuesDemo(page: Page, { empty = false } = {}) {
  await page.clock.setFixedTime(demoNow);
  await sessionAs(page, {
    permissions: ownerPermissions,
    membership: ownerMembership,
  });
  await page.route(/\/api\/v1\/manage\/waitlist(\?.*)?$/, async (route) => {
    if (route.request().method() !== 'GET') {
      await route.fallback();
      return;
    }

    await route.fulfill(
      corsJson({
        branchId,
        timeZone: 'Europe/Istanbul',
        generatedUtc: nowUtc,
        items: empty ? [] : waitlistItems,
        overview: {
          waitingCount: empty ? 0 : waitlistItems.length,
          oldestJoinedUtc: empty ? null : waitlistItems[1]?.joinedUtc,
          convertedThisMonth: empty ? 0 : 11,
          offerHoldMinutes: 15,
        },
      }),
    );
  });
  await page.route(/\/api\/v1\/manage\/walk-in-tickets(\?.*)?$/, async (route) => {
    if (route.request().method() !== 'GET') {
      await route.fallback();
      return;
    }

    await route.fulfill(
      corsJson({
        branchId,
        date: '2026-11-25',
        timeZone: 'Europe/Istanbul',
        generatedUtc: nowUtc,
        items: empty ? [] : tickets,
        waitingCount: empty ? 0 : tickets.length,
        oldestJoinedUtc: empty ? null : tickets[0]?.joinedUtc,
        estimatedWaitMinutesForNew: empty ? 0 : 35,
        servedCount: empty ? 0 : 7,
        nextQueueNumber: empty ? 1 : 11,
      }),
    );
  });
}
