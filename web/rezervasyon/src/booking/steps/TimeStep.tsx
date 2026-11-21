import { BookingAvatar, DayStrip, FullDayNotice, TimeSlotGrid } from '@omvila/ui-web/booking';
import type { TimeSlot } from '@omvila/ui-web/booking';
import { formatDate, formatTime, withTimeSuffix } from '@omvila/ui-web/format';
import { daysBetween, toBranchDate } from '@omvila/ui-web/time';
import { WaitlistForm } from '@/waitlist/WaitlistForm';
import type { WaitlistChoice } from '@/waitlist/WaitlistForm';
import type { SlotSearchResult } from '../booking-api';
import type { Day } from '../use-days';
import styles from './steps.module.css';

interface TimeStepProps {
  days: readonly Day[];
  maxDate: string | undefined;
  nextAvailableUtc: string | null;
  today: string;
  timeZone: string;
  date: string | null;
  time: string | null;
  /** Seçili günün saatleri; yüklenirken undefined. */
  slots: SlotSearchResult | undefined;
  /** Tek personel seçildiyse "{ad} için uygun saatler". */
  staffName: string | null;
  onSelectDate: (date: string) => void;
  onSelectTime: (time: string) => void;
  /** Dolu günden en yakın uygun saate geçiş (kriter 7). */
  onJump: (date: string, time: string) => void;
  /** Dolu günde "Bekleme listesine katıl" (KK3); telefon doğrulaması sonraki adımda. */
  onJoinWaitlist: (choice: WaitlistChoice) => void;
}

/** Gün kısa adıyla (≤ 6 gün sonra) ya da tarihle: "Cuma", "5 Aralık". */
function dayName(from: string, date: string): string {
  const diff = daysBetween(from, date);
  return diff > 0 && diff < 7 ? formatDate(date, 'weekday') : formatDate(date, 'dayMonth');
}

/** Dolu günün ardından ilk uygun saat: şeritteki sonraki günün ilk saati, yoksa nextAvailableUtc. */
function nearest(days: readonly Day[], date: string, nextAvailableUtc: string | null) {
  const later = days.find((day) => day.date > date && day.hasAvailability && day.firstSlotUtc);
  return later?.firstSlotUtc ?? nextAvailableUtc;
}

/** Gün şeridi + saat ızgarası (Saat.png); dolu gün kartı ve bekleme listesi formu (DoluGun.png). */
export function TimeStep({
  days,
  maxDate,
  nextAvailableUtc,
  today,
  timeZone,
  date,
  time,
  slots,
  staffName,
  onSelectDate,
  onSelectTime,
  onJump,
  onJoinWaitlist,
}: TimeStepProps) {
  const selectedDay = days.find((day) => day.date === date);
  const full = selectedDay !== undefined && !selectedDay.hasAvailability && !selectedDay.isClosed;

  const notice = (() => {
    if (!date) return null;
    const nextUtc = nearest(days, date, nextAvailableUtc);
    const nextDate = nextUtc ? toBranchDate(new Date(nextUtc), timeZone) : null;
    const nextTime = nextUtc ? formatTime(nextUtc, timeZone) : null;
    const label = nextDate && nextTime ? `${dayName(date, nextDate)} ${nextTime}` : null;
    return (
      <FullDayNotice
        title={`${formatDate(date, 'weekday')} dolu`}
        description={`${formatDate(date, 'dayMonth')} için boş saat yok.${label ? ` En yakın uygun saat ${label}.` : ''}`}
        {...(nextDate && nextTime
          ? {
              action: {
                label: `${dayName(date, nextDate)} ${withTimeSuffix(nextTime, 'accusative')} seç`,
                onClick: () => {
                  onJump(nextDate, nextTime);
                },
              },
            }
          : {})}
      />
    );
  })();

  const grid: TimeSlot[] = slots
    ? [
        ...slots.slots.map((slot) => ({ time: slot.localTime, available: true })),
        ...slots.unavailableTimes
          .filter((value) => !slots.slots.some((slot) => slot.localTime === value))
          .map((value) => ({ time: value, available: false })),
      ].sort((a, b) => a.time.localeCompare(b.time))
    : [];

  return (
    <div className={styles.stack}>
      <DayStrip
        days={days.map((day) => ({
          date: day.date,
          available: day.hasAvailability,
          closed: day.isClosed ?? false,
        }))}
        value={date}
        onChange={onSelectDate}
        today={today}
        hint={maxDate ? `${String(daysBetween(today, maxDate))} gün ileriye kadar` : undefined}
      />
      {staffName && !full ? (
        <p className={styles.staffHint}>
          <BookingAvatar name={staffName} size="sm" decorative />
          {staffName} için uygun saatler
        </p>
      ) : null}
      {full && date ? (
        <>
          {notice}
          <WaitlistForm date={date} staffName={staffName} onJoin={onJoinWaitlist} />
        </>
      ) : slots === undefined ? (
        <div className={styles.skeleton} aria-hidden="true" />
      ) : (
        <TimeSlotGrid slots={grid} value={time} onChange={onSelectTime} empty={notice} />
      )}
    </div>
  );
}
