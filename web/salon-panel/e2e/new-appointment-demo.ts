import type { Page } from '@playwright/test';
import { calendarDemo } from './calendar-demo';
import { customersDemo } from './customers-demo';

// Yeni randevu ekran görüntüsü için salon-paneli/YeniRandevu verisi: takvim (calendar-demo), müşteriler
// (customers-demo), hizmet çipleri ve Elif'in boş saatleri. Saatler İstanbul yerel saatidir (UTC+3). Kurgusal kişiler.
const corsJson = (body: unknown) => ({
  status: 200,
  contentType: 'application/json',
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});

const staffIds = [
  '0193a8f0-0000-7000-8000-0000000000c1',
  '0193a8f0-0000-7000-8000-0000000000c2',
  '0193a8f0-0000-7000-8000-0000000000c3',
  '0193a8f0-0000-7000-8000-0000000000c4',
];

// [ad, süre dk, fiyat ₺]
const demoServices: [string, number, number][] = [
  ['Saç kesimi', 45, 450],
  ['Fön', 30, 350],
  ['Dip boya', 90, 1250],
  ['Keratin', 60, 1800],
  ['Manikür', 45, 400],
  ['Cilt bakımı', 60, 900],
];

const services = demoServices.map(([name, minutes, lira], i) => ({
  id: `0193a8f0-0000-7000-8000-0000000000f${String(i + 1)}`,
  categoryId: '0193a8f0-0000-7000-8000-0000000000e1',
  name,
  description: null,
  durationMinutes: minutes,
  price: { amountMinor: lira * 100, currency: 'TRY' },
  priceDisplay: 'Fixed',
  bufferBeforeMinutes: 0,
  bufferAfterMinutes: 0,
  deposit: { mode: 'Never' },
  isOnlineBookable: true,
  isActive: true,
  sortOrder: i + 1,
  stages: [],
  staffIds,
  version: '1',
}));

const utc = (date: string, time: string) => new Date(`${date}T${time}:00+03:00`).toISOString();
const free = ['10:30', '11:15', '13:45', '14:30', '16:45', '17:30'];
const taken = ['09:00', '09:45', '15:15'];

function demoSlots(body: {
  date: string;
  items: { serviceId: string; staffId?: string | null }[];
}) {
  const minutes = body.items.reduce(
    (sum, item) => sum + (services.find((s) => s.id === item.serviceId)?.durationMinutes ?? 30),
    0,
  );
  const total = body.items.reduce(
    (sum, item) => sum + (services.find((s) => s.id === item.serviceId)?.price.amountMinor ?? 0),
    0,
  );
  return {
    timeZone: 'Europe/Istanbul',
    date: body.date,
    slots: free.map((time) => {
      const [h = 0, m = 0] = time.split(':').map(Number);
      const end = h * 60 + m + minutes;
      const endLocalTime = `${String(Math.floor(end / 60)).padStart(2, '0')}:${String(end % 60).padStart(2, '0')}`;
      return {
        startUtc: utc(body.date, time),
        localTime: time,
        endLocalTime,
        period: h < 12 ? 'Morning' : h < 17 ? 'Afternoon' : 'Evening',
        items: body.items.map((item) => ({
          serviceId: item.serviceId,
          staffId: item.staffId ?? staffIds[0],
          staffName: 'Elif Kaya',
          startUtc: utc(body.date, time),
          endUtc: utc(body.date, endLocalTime),
        })),
        total: { amountMinor: total, currency: 'TRY' },
      };
    }),
    unavailableTimes: taken,
  };
}

/** Takvim, müşteriler, hizmetler ve boş saatler (YeniRandevu). Oturum calendar-demo'dakidir (son kayıt geçerli). */
export async function newAppointmentDemo(page: Page) {
  await customersDemo(page);
  await calendarDemo(page);
  await page.route(/\/api\/v1\/manage\/services(\?.*)?$/, (route) =>
    route.request().method() === 'GET'
      ? route.fulfill(corsJson({ items: services }))
      : route.fallback(),
  );
  await page.route(/\/api\/v1\/manage\/branches\/[^/]+\/availability\/slots$/, (route) =>
    route.request().method() === 'POST'
      ? route.fulfill(
          corsJson(demoSlots(route.request().postDataJSON() as Parameters<typeof demoSlots>[0])),
        )
      : route.fallback(),
  );
}
