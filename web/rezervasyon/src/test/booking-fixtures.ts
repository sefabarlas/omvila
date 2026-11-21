import type {
  Catalog,
  DaySearchRequest,
  PublicAppointment,
  SlotSearchRequest,
} from '@/booking/booking-api';
import { addDays } from '@omvila/ui-web/time';

// Rezervasyon akışı testlerinin kurgusal verisi (gerçek kişi yok). Şube Europe/Istanbul; "bugün" 24 Kasım 2026 Salı.

export const nowUtc = '2026-11-24T07:00:00Z';
/** brandFixture() şubesi (Kadıköy). */
export const branchId = '0190a000-0000-7000-8000-000000000002';

export const ids = {
  sac: '0190b000-0000-7000-8000-000000000001',
  fon: '0190b000-0000-7000-8000-000000000002',
  bakim: '0190b000-0000-7000-8000-000000000003',
  dipBoya: '0190b000-0000-7000-8000-000000000004',
  elif: '0190c000-0000-7000-8000-000000000001',
  zeynep: '0190c000-0000-7000-8000-000000000002',
  catSac: '0190d000-0000-7000-8000-000000000001',
  catRenk: '0190d000-0000-7000-8000-000000000002',
};

const tl = (lira: number) => ({ amountMinor: lira * 100, currency: 'TRY' as const });

export function catalogFixture(overrides: Partial<Catalog> = {}): Catalog {
  return {
    branchId,
    timeZone: 'Europe/Istanbul',
    allowStaffSelection: true,
    allowAnyStaffOption: true,
    categories: [
      { id: ids.catSac, name: 'Saç' },
      { id: ids.catRenk, name: 'Renk' },
    ],
    services: [
      {
        id: ids.sac,
        categoryId: ids.catSac,
        name: 'Saç kesimi',
        durationMinutes: 45,
        price: tl(450),
        priceDisplay: 'Fixed',
        hasDeposit: false,
        staffIds: [ids.elif, ids.zeynep],
      },
      {
        id: ids.fon,
        categoryId: ids.catSac,
        name: 'Fön',
        durationMinutes: 30,
        price: tl(250),
        priceDisplay: 'Fixed',
        hasDeposit: false,
        staffIds: [ids.elif, ids.zeynep],
      },
      {
        id: ids.bakim,
        categoryId: ids.catSac,
        name: 'Saç bakımı',
        description: 'Keratin maske',
        durationMinutes: 40,
        price: tl(600),
        priceDisplay: 'Fixed',
        hasDeposit: false,
        staffIds: [ids.zeynep],
      },
      {
        id: ids.dipBoya,
        categoryId: ids.catRenk,
        name: 'Dip boya',
        description: 'Uzunluğa göre değişir',
        durationMinutes: 90,
        price: tl(1100),
        priceDisplay: 'StartingFrom',
        hasDeposit: false,
        staffIds: [ids.elif],
      },
    ],
    staff: [
      {
        id: ids.elif,
        displayName: 'Elif Kaya',
        title: 'Kurucu',
        surcharges: [{ serviceId: ids.sac, amount: tl(50) }],
      },
      { id: ids.zeynep, displayName: 'Zeynep Aksoy', title: null },
    ],
    ...overrides,
  };
}

/** 24 Kas açık, 26 Kas (Perşembe) dolu, 29 Kas (Pazar) kapalı; en ileri tarih 60 gün sonrası. */
export function daysFixture(request: DaySearchRequest) {
  const days = [];
  for (let date = request.from; date <= request.to; date = addDays(date, 1)) {
    const closed = date === '2026-11-29';
    const full = date === '2026-11-26';
    days.push({
      date,
      hasAvailability: !closed && !full,
      isClosed: closed,
      firstSlotUtc: !closed && !full ? `${date}T07:30:00Z` : null,
    });
  }
  return { timeZone: 'Europe/Istanbul', maxDate: addDays('2026-11-24', 60), days };
}

const slotTimes: [string, string][] = [
  ['10:30', '07:30'],
  ['14:30', '11:30'],
  ['14:45', '11:45'],
  ['15:15', '12:15'],
  ['16:15', '13:15'],
];

const minutes = (time: string) => {
  const [h, m] = time.split(':').map(Number) as [number, number];
  return h * 60 + m;
};
const clock = (total: number) =>
  `${String(Math.floor(total / 60)).padStart(2, '0')}:${String(total % 60).padStart(2, '0')}`;

/** Seçilen hizmetler sırayla; "fark etmez" kalemine Zeynep Aksoy atanır (kriter 6). */
export function slotsFixture(request: SlotSearchRequest, catalog = catalogFixture()) {
  const staffName = (id: string) =>
    catalog.staff.find((member) => member.id === id)?.displayName ?? '';
  return {
    timeZone: 'Europe/Istanbul',
    date: request.date,
    unavailableTimes: ['13:00', '15:00'],
    slots: slotTimes.map(([localTime, utc]) => {
      let cursor = minutes(localTime);
      let total = 0;
      const items = request.items.map((item) => {
        const service = catalog.services.find((candidate) => candidate.id === item.serviceId);
        const staffId = item.staffId ?? ids.zeynep;
        const surcharge =
          catalog.staff
            .find((member) => member.id === staffId)
            ?.surcharges?.find((s) => s.serviceId === item.serviceId)?.amount.amountMinor ?? 0;
        const amount = (service?.price?.amountMinor ?? 0) + surcharge;
        total += amount;
        const start = cursor;
        cursor += service?.durationMinutes ?? 0;
        return {
          serviceId: item.serviceId,
          staffId,
          staffName: staffName(staffId),
          startUtc: `${request.date}T${clock(start - 180)}:00Z`,
          endUtc: `${request.date}T${clock(cursor - 180)}:00Z`,
          price: { amountMinor: amount, currency: 'TRY' as const },
        };
      });
      return {
        startUtc: `${request.date}T${utc}:00Z`,
        localTime,
        endLocalTime: clock(cursor),
        period: localTime < '12:00' ? 'Morning' : localTime < '17:00' ? 'Afternoon' : 'Evening',
        items,
        total: { amountMinor: total, currency: 'TRY' as const },
      };
    }),
  };
}

export function appointmentFixture(overrides: Partial<PublicAppointment> = {}): PublicAppointment {
  return {
    id: '0190e000-0000-7000-8000-000000000001',
    accessCode: '7Kx2Pq9a',
    manageUrl: 'https://omvila.com/r/7Kx2Pq9a',
    status: 'Confirmed',
    displayStatus: 'Confirmed',
    brand: { name: 'Kuaför Elif', slug: 'kuafor-elif', logoUrl: null },
    branch: {
      id: branchId,
      name: 'Kadıköy',
      addressLine: 'Moda Cad. 42',
      district: 'Kadıköy',
      phone: '+905321234567',
    },
    startUtc: '2026-11-24T11:30:00Z',
    endUtc: '2026-11-24T12:45:00Z',
    timeZone: 'Europe/Istanbul',
    items: [
      {
        serviceName: 'Saç kesimi',
        staffName: 'Elif Kaya',
        startUtc: '2026-11-24T11:30:00Z',
        endUtc: '2026-11-24T12:15:00Z',
        price: tl(500),
        paidFromPackage: false,
      },
      {
        serviceName: 'Fön',
        staffName: 'Elif Kaya',
        startUtc: '2026-11-24T12:15:00Z',
        endUtc: '2026-11-24T12:45:00Z',
        price: tl(250),
        paidFromPackage: false,
      },
    ],
    total: tl(750),
    amountDue: tl(750),
    canCancel: true,
    canReschedule: true,
    version: '1',
    ...overrides,
  };
}

export const challengeFixture = {
  otpId: '0190f000-0000-7000-8000-000000000001',
  expiresInSeconds: 180,
  resendAfterSeconds: 60,
  phoneMasked: '0532 *** ** 67',
};

export const tokenFixture = {
  bookingToken: 'rezervasyon-jetonu',
  expiresInSeconds: 1800,
  phoneMasked: '0532 *** ** 67',
};
