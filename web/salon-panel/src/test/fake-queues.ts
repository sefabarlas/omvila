import type { components } from '@omvila/api-client-ts';
import { maskPhone, parseMobilePhone } from '@omvila/ui-web/format';
import { customersFixture } from './fake-customers';

// Sahte bekleme listesi ve kapıda sıra uçları (manage-waitlist, manage-walk-ins). Şekiller sözleşmeyle aynı; veriler
// salon-paneli/BeklemeListesi ve KapidaSira'dan, kurgusal kişiler. "Şimdi" test saati: 25 Kasım 2026 12:00 İstanbul.
type WaitlistEntry = components['schemas']['WaitlistEntryView'];
type WaitlistCreate = components['schemas']['ManageWaitlistCreate'];
type WalkInTicket = components['schemas']['WalkInTicketView'];
type WalkInCreate = components['schemas']['WalkInCreate'];

export interface QueuesFakeOptions {
  /** Bekleme listesi boş (BeklemeListesiBos). */
  waitlistEmpty?: boolean;
  /** Kapıda sıra boş (KapidaSiraBos). */
  walkInsEmpty?: boolean;
  /** "Saat öner" yanıtı (hata kodu). */
  offerError?: 'slot_taken' | 'insufficient_credits' | 'concurrency_conflict';
  /** "Randevuya çevir" yanıtı (hata kodu). */
  startError?: 'slot_taken' | 'concurrency_conflict';
}

export const queuesNowUtc = '2026-11-25T09:00:00Z';
const branchId = '0193a8f0-0000-7000-8000-0000000000b1';
const elif = '0193a8f0-0000-7000-8000-0000000000c1';
const mert = '0193a8f0-0000-7000-8000-0000000000c2';
const haircut = '0193a8f0-0000-7000-8000-0000000000f1';
const keratin = '0193a8f0-0000-7000-8000-0000000000f2';

export const waitlistIds = {
  nur: '0193a8f0-0000-7000-8000-00000000e001',
  tugba: '0193a8f0-0000-7000-8000-00000000e002',
  kaan: '0193a8f0-0000-7000-8000-00000000e003',
  irem: '0193a8f0-0000-7000-8000-00000000e004',
  created: '0193a8f0-0000-7000-8000-00000000e099',
} as const;

export const walkInIds = {
  okan: '0193a8f0-0000-7000-8000-00000000f008',
  murat: '0193a8f0-0000-7000-8000-00000000f009',
  emre: '0193a8f0-0000-7000-8000-00000000f010',
  created: '0193a8f0-0000-7000-8000-00000000f011',
} as const;

const entry = (
  id: string,
  fullName: string,
  phone: string,
  serviceName: string,
  serviceId: string,
  staff: { id: string; name: string } | null,
  joinedUtc: string,
  dates: string[],
  timeWindows: WaitlistEntry['timeWindows'],
): WaitlistEntry => ({
  id,
  branchId,
  status: 'Waiting',
  customer: {
    brandCustomerId: id,
    fullName,
    phoneMasked: maskPhone(phone),
  },
  serviceIds: [serviceId],
  serviceNames: [serviceName],
  preferredStaffId: staff?.id ?? null,
  preferredStaffName: staff?.name ?? null,
  acceptOtherStaff: staff === null,
  dates,
  timeWindows,
  note: null,
  joinedUtc,
  source: 'Online',
  pendingOffer: null,
  version: 'W/"1"',
});

function waitlistFixture(): WaitlistEntry[] {
  const elifStaff = { id: elif, name: 'Elif Kaya' };
  return [
    {
      ...entry(
        waitlistIds.nur,
        'Nur Ateş',
        '+905301175212',
        'Fön',
        haircut,
        elifStaff,
        '2026-11-22T10:00:00Z',
        ['2026-11-26', '2026-11-27'],
        ['Morning'],
      ),
      // Perşembe 11:00 boşaldı; teklif 11:56'da gitti, 12:11'de düşer (12:00'de 11 dk kaldı).
      pendingOffer: {
        id: '0193a8f0-0000-7000-8000-00000000e101',
        status: 'Pending',
        startUtc: '2026-11-26T08:00:00Z',
        endUtc: '2026-11-26T08:45:00Z',
        staffId: elif,
        staffName: 'Elif Kaya',
        offeredUtc: '2026-11-25T08:56:00Z',
        holdExpiresUtc: '2026-11-25T09:11:00Z',
      },
    },
    entry(
      waitlistIds.tugba,
      'Tuğba Er',
      '+905321234401',
      'Keratin',
      keratin,
      elifStaff,
      '2026-11-19T12:30:00Z',
      [],
      ['Afternoon', 'Evening'],
    ),
    entry(
      waitlistIds.kaan,
      'Kaan Polat',
      '+905321234402',
      'Sakal ve kesim',
      haircut,
      { id: mert, name: 'Mert Demir' },
      '2026-11-23T09:00:00Z',
      ['2026-11-28'],
      [],
    ),
    entry(
      waitlistIds.irem,
      'İrem Sarı',
      '+905321234403',
      'Manikür',
      haircut,
      null,
      '2026-11-24T15:00:00Z',
      [],
      [],
    ),
  ];
}

const ticket = (
  id: string,
  queueNumber: number,
  customerName: string,
  service: { id: string; name: string } | null,
  staff: { id: string; name: string } | null,
  joinedUtc: string,
): WalkInTicket => ({
  id,
  branchId,
  queueDate: '2026-11-25',
  queueNumber,
  status: 'Waiting',
  customerName,
  brandCustomerId: null,
  phoneMasked: null,
  serviceId: service?.id ?? null,
  serviceName: service?.name ?? null,
  preferredStaffId: staff?.id ?? null,
  preferredStaffName: staff?.name ?? null,
  joinedUtc,
  estimatedWaitMinutes: null,
  appointmentId: null,
  version: 'W/"1"',
});

function walkInFixture(): WalkInTicket[] {
  const cut = { id: haircut, name: 'Saç kesimi' };
  const mertStaff = { id: mert, name: 'Mert Demir' };
  return [
    ticket(walkInIds.okan, 8, 'Okan Er', cut, mertStaff, '2026-11-25T08:48:00Z'),
    ticket(walkInIds.murat, 9, 'Murat Çelik', null, null, '2026-11-25T08:52:00Z'),
    ticket(walkInIds.emre, 10, 'Emre Yavuz', cut, mertStaff, '2026-11-25T08:58:00Z'),
  ];
}

const json = (status: number, body: unknown) =>
  new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } });

const bump = (version: string) => `W/"${String(Number(/\d+/.exec(version)?.[0] ?? '0') + 1)}"`;

/** Bekleme listesi ve kapıda sıra uçlarını karşılar; ilgisiz istekte null. */
export function createQueuesFake(options: QueuesFakeOptions = {}) {
  const waitlist = options.waitlistEmpty ? [] : waitlistFixture();
  const tickets = options.walkInsEmpty ? [] : walkInFixture();
  let served = options.walkInsEmpty ? 0 : 7;
  let nextNumber = options.walkInsEmpty ? 1 : 11;

  return async (request: Request): Promise<Response | null> => {
    const url = new URL(request.url);
    const path = url.pathname;
    const ifMatch = request.headers.get('If-Match');

    if (path === '/api/v1/manage/waitlist') {
      if (request.method === 'GET') {
        const oldest = [...waitlist].sort((a, b) => a.joinedUtc.localeCompare(b.joinedUtc))[0];
        return json(200, {
          branchId,
          timeZone: 'Europe/Istanbul',
          generatedUtc: queuesNowUtc,
          items: waitlist,
          overview: {
            waitingCount: waitlist.length,
            oldestJoinedUtc: oldest?.joinedUtc ?? null,
            convertedThisMonth: options.waitlistEmpty ? 0 : 11,
            offerHoldMinutes: 15,
          },
        });
      }

      const body = (await request.clone().json()) as WaitlistCreate;
      let customer: WaitlistEntry['customer'];
      if (body.newCustomer) {
        const phone = parseMobilePhone(body.newCustomer.phone);
        const existing = customersFixture.find((c) => c.phone === phone);
        if (existing) {
          return json(409, {
            code: 'phone_already_exists',
            meta: { brandCustomerId: existing.id },
          });
        }

        customer = {
          brandCustomerId: '0193a8f0-0000-7000-8000-00000000c098',
          fullName: body.newCustomer.fullName,
          ...(phone ? { phoneMasked: maskPhone(phone) } : {}),
        };
      } else {
        const found = customersFixture.find((c) => c.id === body.brandCustomerId);
        customer = {
          brandCustomerId: body.brandCustomerId ?? '',
          fullName: found?.fullName ?? 'Müşteri',
          ...(found?.phone ? { phoneMasked: maskPhone(found.phone) } : {}),
        };
      }

      const created: WaitlistEntry = {
        id: waitlistIds.created,
        branchId: body.branchId,
        status: 'Waiting',
        customer,
        serviceIds: body.preferences.serviceIds,
        serviceNames: body.preferences.serviceIds.map((id) =>
          id === keratin ? 'Keratin' : 'Saç kesimi',
        ),
        preferredStaffId: body.preferences.preferredStaffId ?? null,
        preferredStaffName: null,
        acceptOtherStaff: body.preferences.acceptOtherStaff,
        dates: body.preferences.dates,
        timeWindows: body.preferences.timeWindows,
        note: body.note ?? null,
        joinedUtc: queuesNowUtc,
        source: 'Panel',
        pendingOffer: null,
        version: 'W/"1"',
      };
      waitlist.push(created);
      return json(201, created);
    }

    const offer = /^\/api\/v1\/manage\/waitlist\/([^/]+)\/offer(\/withdraw)?$/.exec(path);
    if (offer && request.method === 'POST') {
      const found = waitlist.find((e) => e.id === offer[1]);
      if (!found) {
        return json(404, { code: 'not_found' });
      }

      if (ifMatch !== found.version) {
        return json(412, { code: 'concurrency_conflict' });
      }

      if (offer[2]) {
        if (!found.pendingOffer) {
          return json(409, { code: 'invalid_state_transition' });
        }

        found.pendingOffer = null;
        found.version = bump(found.version);
        return json(200, found);
      }

      if (options.offerError === 'concurrency_conflict') {
        return json(412, { code: 'concurrency_conflict' });
      }

      if (options.offerError) {
        return json(409, { code: options.offerError });
      }

      const body = (await request.clone().json()) as { startUtc: string; staffId: string };
      found.pendingOffer = {
        id: '0193a8f0-0000-7000-8000-00000000e199',
        status: 'Pending',
        startUtc: body.startUtc,
        endUtc: new Date(Date.parse(body.startUtc) + 45 * 60_000).toISOString(),
        staffId: body.staffId,
        staffName: null,
        offeredUtc: queuesNowUtc,
        holdExpiresUtc: '2026-11-25T09:15:00Z',
      };
      found.version = bump(found.version);
      return json(200, found);
    }

    if (path === '/api/v1/manage/walk-in-tickets') {
      if (request.method === 'GET') {
        return json(200, {
          branchId,
          date: '2026-11-25',
          timeZone: 'Europe/Istanbul',
          generatedUtc: queuesNowUtc,
          items: tickets,
          waitingCount: tickets.length,
          oldestJoinedUtc: tickets[0]?.joinedUtc ?? null,
          estimatedWaitMinutesForNew: tickets.length === 0 ? 0 : 35,
          servedCount: served,
          nextQueueNumber: nextNumber,
        });
      }

      const body = (await request.clone().json()) as WalkInCreate;
      const phone = body.phone ? parseMobilePhone(body.phone) : null;
      const created: WalkInTicket = {
        ...ticket(
          walkInIds.created,
          nextNumber,
          body.fullName,
          body.serviceId ? { id: body.serviceId, name: 'Saç kesimi' } : null,
          body.preferredStaffId ? { id: body.preferredStaffId, name: 'Mert Demir' } : null,
          queuesNowUtc,
        ),
        phoneMasked: phone ? maskPhone(phone) : null,
      };
      nextNumber += 1;
      tickets.push(created);
      return json(201, created);
    }

    const action = /^\/api\/v1\/manage\/walk-in-tickets\/([^/]+)\/(start|leave)$/.exec(path);
    if (action && request.method === 'POST') {
      const index = tickets.findIndex((t) => t.id === action[1]);
      const found = tickets[index];
      if (!found) {
        return json(404, { code: 'not_found' });
      }

      if (
        ifMatch !== found.version ||
        (action[2] === 'start' && options.startError === 'concurrency_conflict')
      ) {
        return json(412, { code: 'concurrency_conflict' });
      }

      if (action[2] === 'start' && options.startError) {
        return json(409, { code: options.startError });
      }

      tickets.splice(index, 1);
      if (action[2] === 'leave') {
        return json(200, { ...found, status: 'Left', version: bump(found.version) });
      }

      served += 1;
      return json(200, {
        ticket: { ...found, status: 'Served', version: bump(found.version) },
        // Yanıtın randevusu sayfada kullanılmıyor; takvim tazelenir.
        appointment: { id: '0193a8f0-0000-7000-8000-00000000a0fe', channel: 'WalkIn' },
      });
    }

    return null;
  };
}
