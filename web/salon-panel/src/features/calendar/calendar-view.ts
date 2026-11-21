// Takvim yanıtını (CalendarView) ui-web/calendar sunum verisine çevirir; özet blokları aynı veriden hesaplanır.
// Saatler şubenin saat dilimiyle (yanıttaki timeZone) hesaplanır; cihaz saat dilimi kullanılmaz.
import { formatDate, formatDuration, formatTime } from '@omvila/ui-web/format';
import { displayStatus } from '@omvila/ui-web/status';
import type { StatusKey } from '@omvila/ui-web/status';
import type {
  CalendarBlock,
  CalendarStaff,
  CalendarUnavailableRange,
} from '@omvila/ui-web/calendar';
import {
  addDays,
  formatClock,
  localDateTimeToUtc,
  localMinutesOfDay,
  parseClock,
  toBranchDate,
} from '@omvila/ui-web/time';
import { transferReportedNote } from '@/features/deposits/deposit-view';
import type { CalendarAppointment, CalendarDay, CalendarView } from './api';

/** Taşınabilen durumlar: bekleyen ve onaylı (geldi, tamamlandı, gelmedi taşınmaz). */
const movable = new Set<StatusKey>(['onay', 'kapora', 'onaylandi']);
const upcoming = movable;

export function toBlocks(view: CalendarView, canMove: boolean): CalendarBlock[] {
  return view.appointments.flatMap((item) => {
    const status = displayStatus(item.displayStatus);
    if (!status) {
      return [];
    }

    return [
      {
        appointmentId: item.appointmentId,
        itemId: item.itemId,
        staffId: item.staffId,
        startUtc: item.startUtc,
        endUtc: item.endUtc,
        status,
        customerName: item.customerName,
        serviceName: item.serviceName,
        isRecurring: item.isRecurring,
        segments: item.segments,
        ...(status === 'kapora' && item.depositStatus === 'CustomerReported'
          ? { note: transferReportedNote }
          : {}),
        draggable: canMove && movable.has(status),
      },
    ];
  });
}

/** Personel kolonları; alt satır veriden: "5 randevu · bekleyen var". */
export function toStaffColumns(view: CalendarView): CalendarStaff[] {
  return view.staff.map((member) => ({
    id: member.staffId,
    name: member.fullName,
    color: member.calendarColor,
    photoUrl: member.photoUrl ?? null,
    subtitle: `${String(member.appointmentCount)} randevu${member.hasGapAvailability ? ' · bekleyen var' : ''}`,
  }));
}

export function findDay(view: CalendarView | undefined, date: string): CalendarDay | undefined {
  return view?.days.find((day) => day.date === date);
}

/**
 * Izgaranın saatleri: şube saatlerinin en erkeni ve en geçi; dışında kalan randevular varsa onları da kapsar.
 * Hiç saat yoksa randevular; o da yoksa null (çağıran boş gün gösterir).
 */
export function gridHours(
  view: CalendarView,
  dates: readonly string[],
): { openTime: string; closeTime: string } | null {
  const minutes: number[] = [];
  for (const date of dates) {
    const day = findDay(view, date);
    for (const interval of [
      ...(day?.branchIntervals ?? []),
      ...(day?.staffHours.flatMap((s) => s.intervals) ?? []),
    ]) {
      minutes.push(parseClock(interval.start), parseClock(interval.end));
    }
  }

  for (const item of view.appointments) {
    if (dates.includes(toBranchDate(new Date(item.startUtc), view.timeZone))) {
      minutes.push(localMinutesOfDay(item.startUtc, view.timeZone));
      const end = localMinutesOfDay(item.endUtc, view.timeZone);
      minutes.push(end === 0 ? 24 * 60 : end);
    }
  }

  if (minutes.length === 0) {
    return null;
  }

  return {
    openTime: formatClock(Math.min(...minutes)),
    closeTime: formatClock(Math.max(...minutes)),
  };
}

const timeOffLabels: Record<CalendarView['timeOffs'][number]['type'], string> = {
  Leave: 'İzin',
  Break: 'Mola',
  Training: 'Eğitim',
  Sick: 'Hastalık izni',
  Other: 'İzin',
};

/**
 * Gölgeli aralıklar: personelin o gün çalışmadığı saatler (ızgara başından ilk aralığa, aralar, son aralıktan
 * sona; hiç saati yoksa bütün gün) ve izinleri.
 */
export function unavailableRanges(
  view: CalendarView,
  dates: readonly string[],
  hours: { openTime: string; closeTime: string },
): CalendarUnavailableRange[] {
  const tz = view.timeZone;
  const ranges: CalendarUnavailableRange[] = [];
  const open = parseClock(hours.openTime);
  const close = parseClock(hours.closeTime);
  for (const date of dates) {
    const day = findDay(view, date);
    if (!day || day.isClosed) {
      continue;
    }

    for (const member of view.staff) {
      const intervals = [
        ...(day.staffHours.find((s) => s.staffId === member.staffId)?.intervals ?? []),
      ].sort((a, b) => a.start.localeCompare(b.start));
      let cursor = open;
      const gaps: { start: number; end: number; label?: string }[] = [];
      for (const interval of intervals) {
        const start = parseClock(interval.start);
        if (start > cursor) {
          gaps.push({ start: cursor, end: start, ...(cursor > open ? { label: 'Mola' } : {}) });
        }

        cursor = Math.max(cursor, parseClock(interval.end));
      }

      if (cursor < close) {
        gaps.push({
          start: cursor,
          end: close,
          ...(intervals.length === 0 ? { label: 'Çalışmıyor' } : {}),
        });
      }

      for (const gap of gaps) {
        ranges.push({
          staffId: member.staffId,
          startUtc: localDateTimeToUtc(date, formatClock(gap.start), tz),
          endUtc: localDateTimeToUtc(date, formatClock(gap.end), tz),
          ...(gap.label ? { label: gap.label } : {}),
        });
      }
    }
  }

  for (const off of view.timeOffs) {
    ranges.push({
      staffId: off.staffId,
      startUtc: off.startUtc,
      endUtc: off.endUtc,
      label: timeOffLabels[off.type],
    });
  }

  return ranges;
}

/** Haftanın pazartesisi (yerel takvim günü). */
export function weekStart(date: string): string {
  const [y, m, d] = date.split('-').map(Number) as [number, number, number];
  const weekday = new Date(Date.UTC(y, m - 1, d)).getUTCDay();
  return addDays(date, -((weekday + 6) % 7));
}

export function weekDates(date: string): string[] {
  const start = weekStart(date);
  return Array.from({ length: 7 }, (_, i) => addDays(start, i));
}

/** Hafta kolonları: kapalı ve randevusuz günler gösterilmez (HaftaTakvim: Pzt–Cmt). */
export function visibleWeekDays(
  view: CalendarView | undefined,
  dates: readonly string[],
): string[] {
  if (!view) {
    return [...dates];
  }

  const busy = new Set(
    view.appointments.map((item) => toBranchDate(new Date(item.startUtc), view.timeZone)),
  );
  const shown = dates.filter((date) => !findDay(view, date)?.isClosed || busy.has(date));
  return shown.length > 0 ? shown : [...dates];
}

/** "24–29 Kasım", ay değişirse "30 Kasım–5 Aralık". */
export function weekLabel(days: readonly string[]): string {
  const first = days[0];
  const last = days.at(-1);
  if (!first || !last) {
    return '';
  }

  if (first.slice(0, 7) === last.slice(0, 7)) {
    return `${String(Number(first.slice(8)))}–${formatDate(last, 'dayMonth')}`;
  }

  return `${formatDate(first, 'dayMonth')}–${formatDate(last, 'dayMonth')}`;
}

export interface CalendarSummary {
  next: { inMinutes: number; time: string; line: string } | null;
  pending: { count: number; expiresInMinutes: number | null };
  today: { appointments: number; completed: number };
}

/** Özet blokları (Main): sıradaki, onay bekleyen, bugün; veri yoksa çağıran 0 veya "—" gösterir. */
export function summarize(view: CalendarView | undefined, now: Date): CalendarSummary {
  const items = view?.appointments ?? [];
  const tz = view?.timeZone ?? 'UTC';
  const staffName = new Map(view?.staff.map((s) => [s.staffId, s.fullName.split(' ')[0] ?? '']));
  const byAppointment = new Map<string, CalendarAppointment>();
  for (const item of items) {
    const known = byAppointment.get(item.appointmentId);
    if (!known || item.startUtc < known.startUtc) {
      byAppointment.set(item.appointmentId, item);
    }
  }

  const appointments = [...byAppointment.values()];
  const nowMs = now.getTime();
  const next = appointments
    .filter((a) => {
      const status = displayStatus(a.displayStatus);
      return status !== null && upcoming.has(status) && Date.parse(a.startUtc) > nowMs;
    })
    .sort((a, b) => a.startUtc.localeCompare(b.startUtc))[0];
  // "Onay bekliyor" gösterim durumuyla sayılır: kapora bekleyen (DepositPending) da PendingApproval'dır ama salonun
  // onayını değil havaleyi bekler; bentoda sayılmaz (Main, RandevuPanel; acik-sorular.md T-72 (3)).
  const pending = appointments.filter((a) => displayStatus(a.displayStatus) === 'onay');
  const expiries = pending
    .map((a) => (a.holdExpiresUtc ? Date.parse(a.holdExpiresUtc) - nowMs : null))
    .filter((ms): ms is number => ms !== null && ms > 0);

  return {
    next: next
      ? {
          inMinutes: Math.ceil((Date.parse(next.startUtc) - nowMs) / 60_000),
          time: formatTime(next.startUtc, tz),
          line: [
            next.customerName,
            next.serviceName,
            next.staffId ? staffName.get(next.staffId) : null,
          ]
            .filter(Boolean)
            .join(' · '),
        }
      : null,
    pending: {
      count: pending.length,
      expiresInMinutes: expiries.length > 0 ? Math.ceil(Math.min(...expiries) / 60_000) : null,
    },
    today: {
      appointments: appointments.length,
      completed: appointments.filter((a) => a.status === 'Completed').length,
    },
  };
}

export function inLabel(minutes: number): string {
  return `${formatDuration(minutes)} sonra`;
}
