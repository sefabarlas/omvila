import type { components } from '@omvila/api-client-ts';
import { addDays, localDateTimeToUtc } from '@omvila/ui-web/time';
import type { ManageSession } from '@/app/api';

// Sahte takvim ve randevu uçları (manageGetCalendar, manageGetAppointment, durum eylemleri, sürükle-bırak, "Yalnızca
// bu gün için aç"). Şekiller sözleşmeyle aynı; veriler salon-paneli/Main'den, kurgusal kişiler. Bugün Çarşamba 25 Kasım
// 2026, saat 12:00 (İstanbul); Pazar şube kapalı.
type CalendarView = components['schemas']['CalendarView'];
type CalendarAppointment = components['schemas']['CalendarAppointment'];
type AppointmentDetail = components['schemas']['AppointmentDetail'];
type AppointmentStatus = components['schemas']['AppointmentStatus'];

const tz = 'Europe/Istanbul';
export const calendarDate = '2026-11-25';
export const closedSunday = '2026-11-29';
export const staffIds = {
  elif: '0193a8f0-0000-7000-8000-0000000000c1',
  mert: '0193a8f0-0000-7000-8000-0000000000c2',
  can: '0193a8f0-0000-7000-8000-0000000000c3',
  zeynep: '0193a8f0-0000-7000-8000-0000000000c4',
} as const;

const staff = [
  { staffId: staffIds.elif, fullName: 'Elif Kaya', calendarColor: '#14857A' },
  { staffId: staffIds.mert, fullName: 'Mert Demir', calendarColor: '#C2410C' },
  { staffId: staffIds.zeynep, fullName: 'Zeynep Aksoy', calendarColor: '#7C3AED' },
  { staffId: staffIds.can, fullName: 'Can Yücel', calendarColor: '#2563EB' },
];

interface Row {
  id: string;
  staffId: string;
  date?: string;
  start: string;
  end: string;
  customer: string;
  service: string;
  status: AppointmentStatus;
  channel?: AppointmentDetail['channel'];
  seriesId?: string;
  gap?: [string, string];
  holdExpires?: string;
}

const series = '0193a8f0-0000-7000-8000-00000000a5e1';
export const appointmentIds = {
  ayse: '0193a8f0-0000-7000-8000-00000000a001',
  selin: '0193a8f0-0000-7000-8000-00000000a002',
  derya: '0193a8f0-0000-7000-8000-00000000a003',
  ece: '0193a8f0-0000-7000-8000-00000000a004',
  pinar: '0193a8f0-0000-7000-8000-00000000a005',
  okan: '0193a8f0-0000-7000-8000-00000000a006',
  asli: '0193a8f0-0000-7000-8000-00000000a007',
  deryaNext: '0193a8f0-0000-7000-8000-00000000a008',
} as const;

const rows: Row[] = [
  {
    id: appointmentIds.ayse,
    staffId: staffIds.elif,
    start: '09:30',
    end: '10:15',
    customer: 'Ayşe Yılmaz',
    service: 'Saç kesimi',
    status: 'Completed',
  },
  // Aşamalı Boya (45 + 40 bekleme + 35), kabul kriteri 3.
  {
    id: appointmentIds.selin,
    staffId: staffIds.elif,
    start: '10:30',
    end: '12:30',
    customer: 'Selin Öztürk',
    service: 'Boya',
    status: 'CheckedIn',
    gap: ['11:15', '11:55'],
  },
  {
    id: appointmentIds.derya,
    staffId: staffIds.elif,
    start: '13:00',
    end: '13:45',
    customer: 'Derya Şahin',
    service: 'Saç kesimi',
    status: 'Confirmed',
    channel: 'Phone',
    seriesId: series,
  },
  {
    id: appointmentIds.deryaNext,
    staffId: staffIds.elif,
    date: '2026-12-23',
    start: '13:00',
    end: '13:45',
    customer: 'Derya Şahin',
    service: 'Saç kesimi',
    status: 'Confirmed',
    channel: 'Phone',
    seriesId: series,
  },
  {
    id: appointmentIds.ece,
    staffId: staffIds.zeynep,
    start: '10:30',
    end: '11:00',
    customer: 'Ece Koç',
    service: 'Kaş ve kirpik',
    status: 'NoShow',
  },
  {
    id: appointmentIds.pinar,
    staffId: staffIds.zeynep,
    start: '16:00',
    end: '17:00',
    customer: 'Pınar Yalçın',
    service: 'Cilt bakımı',
    status: 'PendingApproval',
    holdExpires: '13:35',
  },
  {
    id: appointmentIds.okan,
    staffId: staffIds.mert,
    start: '11:00',
    end: '11:30',
    customer: 'Okan Er',
    service: 'Saç kesimi',
    status: 'CheckedIn',
  },
  {
    id: appointmentIds.asli,
    staffId: staffIds.can,
    start: '14:00',
    end: '15:30',
    customer: 'Aslı Kurt',
    service: 'Balyaj',
    status: 'Confirmed',
  },
];

interface Stored {
  row: Row;
  date: string;
  staffId: string;
  startUtc: string;
  endUtc: string;
  status: AppointmentStatus;
  version: number;
}

export interface CalendarFakeOptions {
  /** Sürükle-bırak yanıtı: dolu saat, kayıt çakışması veya çalışma saati dışı (onaylanınca geçer). */
  rescheduleError?: 'slot_taken' | 'concurrency_conflict' | 'outside_working_hours';
  /** Durum eylemlerinde (Geldi, geri al, iptal) 412. */
  actionConflict?: boolean;
}

const json = (status: number, body: unknown) =>
  new Response(JSON.stringify(body), { status, headers: { 'Content-Type': 'application/json' } });

const shift = (iso: string, ms: number) => new Date(Date.parse(iso) + ms).toISOString();

/** Takvim uçlarını karşılar; ilgisiz istekte null döner (fakeFetch devam eder). */
export function createCalendarFake(session: ManageSession, options: CalendarFakeOptions = {}) {
  const store: Stored[] = rows.map((row) => {
    const date = row.date ?? calendarDate;
    return {
      row,
      date,
      staffId: row.staffId,
      startUtc: localDateTimeToUtc(date, row.start, tz),
      endUtc: localDateTimeToUtc(date, row.end, tz),
      status: row.status,
      version: 1,
    };
  });
  const opened = new Map<string, { start: string; end: string }[]>();

  const toItem = (s: Stored): CalendarAppointment => {
    const offset = Date.parse(s.startUtc) - Date.parse(localDateTimeToUtc(s.date, s.row.start, tz));
    const at = (time: string) => shift(localDateTimeToUtc(s.date, time, tz), offset);
    return {
      appointmentId: s.row.id,
      itemId: `${s.row.id.slice(0, -4)}b${s.row.id.slice(-3)}`,
      staffId: s.staffId,
      startUtc: s.startUtc,
      endUtc: s.endUtc,
      status: s.status,
      displayStatus: s.status === 'Draft' ? 'PendingApproval' : s.status,
      customerName: s.row.customer,
      serviceName: s.row.service,
      isNewCustomer: false,
      isRecurring: s.row.seriesId !== undefined,
      holdExpiresUtc: s.row.holdExpires ? localDateTimeToUtc(s.date, s.row.holdExpires, tz) : null,
      segments: s.row.gap
        ? [
            { startUtc: s.startUtc, endUtc: at(s.row.gap[0]), staffBusy: true },
            { startUtc: at(s.row.gap[0]), endUtc: at(s.row.gap[1]), staffBusy: false },
            { startUtc: at(s.row.gap[1]), endUtc: s.endUtc, staffBusy: true },
          ]
        : [{ startUtc: s.startUtc, endUtc: s.endUtc, staffBusy: true }],
    };
  };

  const toDetail = (s: Stored): AppointmentDetail => {
    const item = toItem(s);
    return {
      id: s.row.id,
      branchId: session.branches[0]?.id ?? '',
      status: s.status,
      displayStatus: item.displayStatus,
      channel: s.row.channel ?? 'Online',
      startUtc: s.startUtc,
      endUtc: s.endUtc,
      timeZone: tz,
      customer: {
        brandCustomerId: `${s.row.id.slice(0, -4)}c${s.row.id.slice(-3)}`,
        fullName: s.row.customer,
        phone: '+905327602118',
        visitCount: 6,
        isNew: false,
      },
      items: [
        {
          id: item.itemId,
          serviceId: '0193a8f0-0000-7000-8000-0000000000f1',
          serviceName: s.row.service,
          staffId: s.staffId,
          staffName: staff.find((m) => m.staffId === s.staffId)?.fullName ?? null,
          startUtc: s.startUtc,
          endUtc: s.endUtc,
          price: { amountMinor: 45000, currency: 'TRY' },
          status: 'Active',
          segments: item.segments,
        },
      ],
      total: { amountMinor: 45000, currency: 'TRY' },
      amountDue: { amountMinor: 45000, currency: 'TRY' },
      isRecurring: item.isRecurring,
      seriesId: s.row.seriesId ?? null,
      noShowAtUtc: s.status === 'Confirmed' ? shift(s.startUtc, 15 * 60_000) : null,
      cancellation: null,
      history: [],
      version: String(s.version),
    };
  };

  const calendar = (url: URL): CalendarView => {
    const from = url.searchParams.get('from') ?? calendarDate;
    const to = url.searchParams.get('to') ?? from;
    const requested = url.searchParams.get('staffIds')?.split(',') ?? [];
    const own = session.membership.staffId;
    const visible = staff.filter(
      (m) =>
        (session.permissions.includes('calendar.view.all-staff') || m.staffId === own) &&
        (requested.length === 0 || requested.includes(m.staffId)),
    );
    const dates: string[] = [];
    for (let d = from; d <= to; d = addDays(d, 1)) {
      dates.push(d);
    }

    const items = store.filter(
      (s) =>
        s.status !== 'Cancelled' &&
        dates.includes(s.date) &&
        visible.some((m) => m.staffId === s.staffId),
    );
    return {
      branchId: session.branches[0]?.id ?? '',
      timeZone: tz,
      from,
      to,
      staff: visible.map((m) => ({
        ...m,
        photoUrl: null,
        appointmentCount: items.filter((s) => s.staffId === m.staffId).length,
        hasGapAvailability: items.some((s) => s.staffId === m.staffId && s.row.gap),
      })),
      days: dates.map((date) => {
        const closed = date === closedSunday && !opened.has(date);
        const intervals = opened.get(date) ?? (closed ? [] : [{ start: '09:00', end: '19:00' }]);
        return {
          date,
          isClosed: closed,
          branchIntervals: intervals,
          staffHours: visible.map((m) => ({
            staffId: m.staffId,
            intervals:
              m.staffId === staffIds.zeynep && !closed && !opened.has(date)
                ? [{ start: '10:00', end: '19:00' }]
                : intervals,
          })),
        };
      }),
      appointments: items.map(toItem),
      timeOffs: visible.some((m) => m.staffId === staffIds.mert)
        ? [
            {
              id: '0193a8f0-0000-7000-8000-0000000000aa',
              staffId: staffIds.mert,
              type: 'Break',
              startUtc: localDateTimeToUtc(calendarDate, '17:00', tz),
              endUtc: localDateTimeToUtc(calendarDate, '19:00', tz),
              isAllDay: false,
            },
          ]
        : [],
    };
  };

  return async (request: Request): Promise<Response | null> => {
    const url = new URL(request.url);
    const path = url.pathname;
    if (path === '/api/v1/manage/calendar' && request.method === 'GET') {
      return json(200, calendar(url));
    }

    if (/^\/api\/v1\/manage\/branches\/[^/]+\/overrides$/.test(path) && request.method === 'POST') {
      const body = (await request.clone().json()) as {
        fromDate: string;
        kind: string;
        intervals?: { start: string; end: string }[];
      };
      if (body.kind !== 'ExtraOpening') {
        return null;
      }

      opened.set(body.fromDate, body.intervals ?? []);
      return json(201, {
        id: '0193a8f0-0000-7000-8000-0000000000da',
        date: body.fromDate,
        kind: 'ExtraOpening',
        isClosed: false,
        intervals: body.intervals ?? [],
        note: null,
      });
    }

    const match = /^\/api\/v1\/manage\/appointments\/([^/]+)(\/[a-z-]+)?$/.exec(path);
    const stored = match ? store.find((s) => s.row.id === match[1]) : undefined;
    if (!match) {
      return null;
    }

    if (!stored) {
      return json(404, { code: 'not_found' });
    }

    const action = match[2] ?? '';
    if (action === '' && request.method === 'GET') {
      return json(200, toDetail(stored));
    }

    if (request.headers.get('If-Match') !== String(stored.version)) {
      return json(412, { code: 'concurrency_conflict' });
    }

    const save = (status: AppointmentStatus) => {
      stored.status = status;
      stored.version += 1;
      return json(200, toDetail(stored));
    };

    switch (action) {
      case '/check-in':
        if (options.actionConflict) {
          return json(412, { code: 'concurrency_conflict' });
        }

        return stored.status === 'Confirmed'
          ? save('CheckedIn')
          : json(409, { code: 'invalid_state_transition' });
      case '/undo-no-show':
        return stored.status === 'NoShow'
          ? save('CheckedIn')
          : json(409, { code: 'invalid_state_transition' });
      case '/cancel': {
        if (options.actionConflict) {
          return json(412, { code: 'concurrency_conflict' });
        }

        const body = (await request.clone().json()) as { scope?: string };
        if (body.scope === 'ThisAndFollowing' && stored.row.seriesId) {
          for (const other of store) {
            if (other.row.seriesId === stored.row.seriesId && other.startUtc > stored.startUtc) {
              other.status = 'Cancelled';
            }
          }
        }

        return save('Cancelled');
      }
      case '/reschedule': {
        const body = (await request.clone().json()) as {
          startUtc: string;
          items?: { itemId: string; staffId: string }[];
          allowOutsideWorkingHours?: boolean;
        };
        if (options.rescheduleError === 'slot_taken') {
          return json(409, { code: 'slot_taken', meta: { alternatives: [] } });
        }

        if (options.rescheduleError === 'concurrency_conflict') {
          return json(412, { code: 'concurrency_conflict' });
        }

        if (options.rescheduleError === 'outside_working_hours' && !body.allowOutsideWorkingHours) {
          return json(409, { code: 'outside_working_hours' });
        }

        const duration = Date.parse(stored.endUtc) - Date.parse(stored.startUtc);
        stored.startUtc = new Date(body.startUtc).toISOString();
        stored.endUtc = shift(stored.startUtc, duration);
        stored.staffId = body.items?.[0]?.staffId ?? stored.staffId;
        stored.version += 1;
        return json(200, toDetail(stored));
      }
      default:
        return null;
    }
  };
}
