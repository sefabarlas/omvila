import type { Page } from '@playwright/test';
import { sessionAs } from './support';

// Takvim ekran görüntüleri için salon-paneli/Main verisi (Prism'in örnek takvimi 2019 tarihli tek bir gün döner).
// Saatler İstanbul yerel saatidir (UTC+3, yaz saati yok); pazar günleri şube kapalıdır. Kurgusal kişiler.
const corsJson = (body: unknown) => ({
  status: 200,
  contentType: 'application/json',
  headers: {
    'Access-Control-Allow-Origin': 'http://localhost:5180',
    'Access-Control-Allow-Credentials': 'true',
  },
  body: JSON.stringify(body),
});

export const demoStaff = [
  {
    staffId: '0193a8f0-0000-7000-8000-0000000000c1',
    fullName: 'Elif Kaya',
    calendarColor: '#14857A',
  },
  {
    staffId: '0193a8f0-0000-7000-8000-0000000000c2',
    fullName: 'Mert Demir',
    calendarColor: '#C2410C',
  },
  {
    staffId: '0193a8f0-0000-7000-8000-0000000000c4',
    fullName: 'Zeynep Aksoy',
    calendarColor: '#7C3AED',
  },
  {
    staffId: '0193a8f0-0000-7000-8000-0000000000c3',
    fullName: 'Can Yücel',
    calendarColor: '#2563EB',
  },
];

// [personel sırası, başlangıç, bitiş, müşteri, hizmet, gösterim durumu, tekrarlayan, bekleme dilimi]
type DemoRow = [number, string, string, string, string, string, boolean?, [string, string]?];
const demoRows: DemoRow[] = [
  [0, '09:30', '10:15', 'Ayşe Yılmaz', 'Saç kesimi', 'Completed'],
  [0, '10:30', '12:30', 'Selin Öztürk', 'Boya ve fön', 'CheckedIn', false, ['10:45', '11:30']],
  [0, '13:00', '13:45', 'Derya Şahin', 'Saç kesimi', 'Confirmed', true],
  [0, '15:30', '16:30', 'Buse Kaya', 'Keratin', 'Confirmed'],
  [0, '17:00', '17:45', 'Nur Ateş', 'Fön', 'Confirmed'],
  [1, '09:00', '10:00', 'Can Demir', 'Sakal ve kesim', 'Completed'],
  [1, '11:00', '11:30', 'Okan Er', 'Saç kesimi', 'CheckedIn'],
  [1, '14:30', '15:00', 'Gizem Arslan', 'Fön', 'DepositPending'],
  [1, '16:00', '16:45', 'Kerem Aydın', 'Saç kesimi', 'Confirmed'],
  [2, '10:00', '11:30', 'Melis Tunç', 'Manikür ve pedikür', 'Completed'],
  [2, '12:30', '13:30', 'Ece Koç', 'Kaş ve kirpik', 'NoShow'],
  [2, '16:00', '17:00', 'Pınar Yalçın', 'Cilt bakımı', 'PendingApproval'],
  [3, '09:30', '11:00', 'Hande Er', 'Dip boya', 'Completed'],
  [3, '12:00', '12:45', 'Deniz Acar', 'Saç kesimi', 'PendingApproval'],
  [3, '14:00', '15:30', 'Aslı Kurt', 'Balyaj', 'Confirmed'],
];

const utc = (date: string, time: string) => new Date(`${date}T${time}:00+03:00`).toISOString();

/** Ekran görüntülerinde sayfanın saati sabittir: Çarşamba 25 Kasım 2026, 12:10 İstanbul (Main'deki şu anki saat). */
export const demoNow = '2026-11-25T12:10:00+03:00';
const demoToday = '2026-11-25';
const istanbulToday = () => demoToday;

/** Tasarımdaki takvim (manageGetCalendar yanıtı); adresin from, to ve staffIds parametrelerine göre. */
export function demoCalendar(url: URL) {
  const from = url.searchParams.get('from') ?? istanbulToday();
  const to = url.searchParams.get('to') ?? from;
  const only = url.searchParams.get('staffIds')?.split(',') ?? [];
  const staff = demoStaff.filter((s) => only.length === 0 || only.includes(s.staffId));
  const dates: string[] = [];
  for (
    let ms = Date.parse(`${from}T00:00:00Z`);
    ms <= Date.parse(`${to}T00:00:00Z`);
    ms += 86_400_000
  ) {
    dates.push(new Date(ms).toISOString().slice(0, 10));
  }

  const open = dates.filter((date) => new Date(`${date}T00:00:00Z`).getUTCDay() !== 0);
  const appointments = open.flatMap((date, dayIndex) =>
    demoRows
      // Hafta görünümünde (tek personel) her güne bir personelin günü düşer (çakışmasız).
      .filter(([staffIndex]) => only.length === 0 || staffIndex === dayIndex % demoStaff.length)
      .map(([staffIndex, start, end, customer, service, display, recurring, gap], i) => {
        const id = `0193a8f0-0000-7000-8000-${String(dayIndex).padStart(4, '0')}${String(i).padStart(8, '0')}`;
        return {
          appointmentId: id,
          itemId: id,
          staffId: only[0] ?? demoStaff[staffIndex]?.staffId ?? '',
          startUtc: utc(date, start),
          endUtc: utc(date, end),
          status: display === 'DepositPending' ? 'PendingApproval' : display,
          displayStatus: display,
          // Kapora bekleyen (GenelBakis, Main: Gizem Arslan 150 ₺; havale süresi 13:45'te).
          ...(display === 'DepositPending'
            ? { depositStatus: 'Awaiting', depositAmount: { amountMinor: 15000, currency: 'TRY' } }
            : {}),
          customerName: customer,
          serviceName: service,
          isRecurring: recurring ?? false,
          // Onay bekleyenlerin ilki 13:45'te düşer: takvim bentosu Main ve Genel bakışla aynı "ilki 1 sa 35 dk".
          holdExpiresUtc:
            display === 'PendingApproval' || display === 'DepositPending'
              ? utc(date, '13:45')
              : null,
          segments: gap
            ? [
                { startUtc: utc(date, start), endUtc: utc(date, gap[0]), staffBusy: true },
                { startUtc: utc(date, gap[0]), endUtc: utc(date, gap[1]), staffBusy: false },
                { startUtc: utc(date, gap[1]), endUtc: utc(date, end), staffBusy: true },
              ]
            : [],
        };
      }),
  );
  return {
    branchId: '0193a8f0-0000-7000-8000-0000000000b1',
    timeZone: 'Europe/Istanbul',
    from,
    to,
    staff: staff.map((s) => ({
      ...s,
      photoUrl: null,
      appointmentCount: appointments.filter((a) => a.staffId === s.staffId).length,
      hasGapAvailability: s.fullName !== 'Elif Kaya',
    })),
    days: dates.map((date) => {
      const closed = !open.includes(date);
      const intervals = closed ? [] : [{ start: '09:00', end: '19:00' }];
      return {
        date,
        isClosed: closed,
        branchIntervals: intervals,
        staffHours: staff.map((s) => ({ staffId: s.staffId, intervals })),
      };
    }),
    appointments,
    timeOffs: [],
  };
}

/** Derya Şahin'in randevu ayrıntısı (manageGetAppointment yanıtı); kimlik ve diğer alanlar örnekten. */
export function demoAppointment(example: Record<string, unknown>) {
  const today = istanbulToday();
  return {
    ...example,
    reference: 'A-20476',
    status: 'Confirmed',
    displayStatus: 'Confirmed',
    channel: 'Phone',
    startUtc: utc(today, '13:00'),
    endUtc: utc(today, '13:45'),
    timeZone: 'Europe/Istanbul',
    customer: {
      brandCustomerId: '0193a8f0-0000-7000-8000-0000000000d7',
      fullName: 'Derya Şahin',
      phone: '+905327602118',
      visitCount: 6,
      isNew: false,
    },
    items: [
      {
        id: '0193a8f0-0000-7000-8000-0000000000e3',
        serviceId: '0193a8f0-0000-7000-8000-0000000000f1',
        serviceName: 'Saç kesimi',
        staffId: demoStaff[0]?.staffId,
        staffName: 'Elif Kaya',
        startUtc: utc(today, '13:00'),
        endUtc: utc(today, '13:45'),
        price: { amountMinor: 45000, currency: 'TRY' },
        status: 'Active',
        segments: [],
      },
    ],
    deposit: null,
    total: { amountMinor: 45000, currency: 'TRY' },
    amountDue: { amountMinor: 45000, currency: 'TRY' },
    isRecurring: true,
    noShowAtUtc: utc(today, '13:15'),
    cancellation: null,
    version: '3',
  };
}

/** Tam yetkili sahip oturumunun izinleri. */
export const ownerPermissions = [
  'settings.manage',
  'service.manage',
  'staff.manage',
  'calendar.view',
  'calendar.view.all-staff',
  'appointment.create',
  'appointment.update',
  'appointment.cancel',
  'appointment.approve',
  'appointment.checkin',
  'appointment.deposit.mark',
  'appointment.complete',
  'payment.collect',
  'cash.close',
  'customer.view',
  'customer.phone.view',
  'customer.edit',
  'report.view',
  'waitlist.manage',
  'walkin.manage',
];

export const ownerMembership = { role: 'Owner', branchIds: [], staffId: demoStaff[0]?.staffId };

/** Takvim, randevu ayrıntısı ve tam yetkili sahip oturumu (Main, HaftaTakvim, TakvimKapali, RandevuOnayli). */
export async function calendarDemo(page: Page) {
  await page.clock.setFixedTime(demoNow);
  await sessionAs(page, { permissions: ownerPermissions, membership: ownerMembership });
  await page.route(/\/api\/v1\/manage\/calendar\?/, (route) =>
    route.request().method() === 'OPTIONS'
      ? route.fallback()
      : route.fulfill(corsJson(demoCalendar(new URL(route.request().url())))),
  );
  await page.route(/\/api\/v1\/manage\/appointments\/[^/]+$/, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const response = await route.fetch();
    await route.fulfill(
      corsJson(demoAppointment((await response.json()) as Record<string, unknown>)),
    );
  });
}

/**
 * GenelBakis'teki kapora bekleyen talep (kuyruk satırı; demoCalendar'ın ilk gününde 8. satır): Gizem Arslan 14:30,
 * 150 ₺, Prism kuyruğunun generatedUtc'sinden (12:25) 1 sa 20 dk sonra düşer.
 */
export const demoDepositQueueItem = {
  appointmentId: '0193a8f0-0000-7000-8000-000000000007',
  reference: 'A-20512',
  branchId: '0193a8f0-0000-7000-8000-0000000000b1',
  branchName: 'Kadıköy',
  timeZone: 'Europe/Istanbul',
  customerName: 'Gizem Arslan',
  isNewCustomer: false,
  serviceSummary: 'Fön',
  staffName: 'Mert Demir',
  startUtc: '2026-11-25T11:30:00Z',
  endUtc: '2026-11-25T12:00:00Z',
  holdExpiresUtc: '2026-11-25T10:45:00Z',
  createdUtc: '2026-11-25T08:40:00Z',
  channel: 'Online',
  depositStatus: 'Awaiting',
  depositAmount: { amountMinor: 15000, currency: 'TRY' },
};

/**
 * Genel bakış (GenelBakis): takvim demosu + kurulumu bitmiş salon. Prism'in oturum örneği Onboarding döner; genel
 * bakış o hâlde kurulum adımlarını (KayitKurulum) gösterir. Onay kuyruğu Prism örneğinden (Pınar Yalçın, Deniz Acar)
 * ve tasarımdaki kapora bekleyen talep (Gizem Arslan) eklenir.
 */
export async function overviewDemo(page: Page) {
  await calendarDemo(page);
  await sessionAs(page, (session) => ({
    permissions: ownerPermissions,
    membership: ownerMembership,
    brand: { ...(session.brand as Record<string, unknown>), status: 'Active' },
  }));
  await page.route(/\/api\/v1\/manage\/approval-queue/, async (route) => {
    if (route.request().method() === 'OPTIONS') {
      await route.fallback();
      return;
    }

    const response = await route.fetch();
    const queue = (await response.json()) as { totalCount: number; items: unknown[] };
    await route.fulfill(
      corsJson({
        ...queue,
        totalCount: queue.totalCount + 1,
        // Kuyruk holdExpiresUtc'ye göre artan sıralı; kapora talebi en acili.
        items: [demoDepositQueueItem, ...queue.items],
      }),
    );
  });
}

/** Örnek haftanın kapalı pazarı. */
export const demoSunday = '2026-11-29';
