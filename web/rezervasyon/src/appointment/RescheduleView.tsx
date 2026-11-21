import {
  BookingAvatar,
  BookingButton,
  DayStrip,
  StickyFooter,
  TimeSlotGrid,
} from '@omvila/ui-web/booking';
import type { TimeSlot } from '@omvila/ui-web/booking';
import { formatDate, formatTime, withTimeSuffix } from '@omvila/ui-web/format';
import { addDays, branchToday, toBranchDate } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import { ArrowLeft, CalendarClock, CalendarX } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { StateBanner } from '@/components/StateBanner';
import { alternatives, messageFor, problemCode } from '@/booking/problem';
import { useRemote } from '@/booking/use-remote';
import { carriesDeposit } from '@/lib/deposit';
import type { Outcome } from './AppointmentManager';
import { rescheduleAppointment, searchRescheduleSlots } from './appointment-api';
import type { PublicAppointment } from './appointment-api';
import { detailsLine, rescheduleRule, whenLabel } from './details';
import styles from './appointment.module.css';

/**
 * Gün şeridinin uzunluğu: yalnız görünüm (iş kuralı değil). Saat değişikliği için gün uygunluğu ucu yok
 * (yalnız saat araması); en ileri tarihi sunucu saat aramasında uygular (T-59 (5)).
 */
const stripDays = 14;

/** Sunucunun ele aldığı hatalar: ekran kapanır, randevu yeniden okunur (Randevum şeridi). */
const terminal = new Set([
  'cutoff_passed',
  'concurrency_conflict',
  'invalid_state_transition',
  'not_found',
]);

const loadSlots = (request: { code: string; date: string }) =>
  searchRescheduleSlots(request.code, request.date);

/** "14:45 ve 15:15", "14:45, 15:15 ve 15:45". */
function joinTimes(times: string[]): string {
  if (times.length <= 1) return times.join('');
  return `${times.slice(0, -1).join(', ')} ve ${times.at(-1) ?? ''}`;
}

interface RescheduleViewProps {
  code: string;
  appointment: PublicAppointment;
  etag: string;
  clock: Clock;
  onBack: () => void;
  outcome: Outcome;
}

/**
 * Saat değiştir (Ertele.png): aynı hizmetler ve personelle yeni gün ve saat. Kapora varsa yeni saate aktarılır (A14).
 * `409 slot_taken` → alternatif saat çipleri; `cutoff_passed` ve `412` Randevum'a döner (T-52 (6)).
 */
export function RescheduleView({
  code,
  appointment,
  etag,
  clock,
  onBack,
  outcome,
}: RescheduleViewProps) {
  const { timeZone } = appointment;
  const today = branchToday(clock, timeZone);
  const current = toBranchDate(new Date(appointment.startUtc), timeZone);
  const [date, setDate] = useState(() => (current >= today ? current : today));
  const [time, setTime] = useState<string | null>(null);
  const [failure, setFailure] = useState<{ error: unknown; time: string } | null>(null);
  const [busy, setBusy] = useState(false);

  const slotsQuery = useRemote({ code, date }, loadSlots);
  const { failed } = outcome;
  const slotsCode = problemCode(slotsQuery.error);
  const reported = useRef(false);
  useEffect(() => {
    // Süre bu arada geçtiyse (409 cutoff_passed) veya randevu etkin değilse Randevum'a dönülür; bir kez.
    if (reported.current || !slotsCode || !terminal.has(slotsCode)) return;
    reported.current = true;
    failed(slotsQuery.error);
  }, [slotsCode, slotsQuery.error, failed]);

  const days = Array.from({ length: stripDays }, (_, index) => addDays(today, index));
  if (!days.includes(date)) days.push(date);

  // Randevunun şu anki saati seçilemez (aynı saate değişiklik yok).
  const slots = (slotsQuery.data?.slots ?? []).filter(
    (slot) => slot.startUtc !== appointment.startUtc,
  );
  const grid: TimeSlot[] = slotsQuery.data
    ? [
        ...slots.map((slot) => ({ time: slot.localTime, available: true })),
        ...[
          ...slotsQuery.data.unavailableTimes,
          ...(date === current ? [formatTime(appointment.startUtc, timeZone)] : []),
        ]
          .filter((value, index, all) => all.indexOf(value) === index)
          .filter((value) => !slots.some((slot) => slot.localTime === value))
          .map((value) => ({ time: value, available: false })),
      ].sort((a, b) => a.time.localeCompare(b.time))
    : [];
  const slot = slots.find((item) => item.localTime === time) ?? null;

  const submit = async () => {
    if (!slot) return;
    setBusy(true);
    setFailure(null);
    try {
      outcome.done(await rescheduleAppointment(code, etag, slot.startUtc));
    } catch (error) {
      const errorCode = problemCode(error);
      if (errorCode && terminal.has(errorCode)) {
        outcome.failed(error);
        return;
      }
      setFailure({ error, time: slot.localTime });
      if (errorCode === 'slot_taken') {
        setTime(null);
        slotsQuery.reload();
      }
    } finally {
      setBusy(false);
    }
  };

  const taken = failure !== null && problemCode(failure.error) === 'slot_taken';
  const alternativeTimes = taken
    ? alternatives(failure.error).map((utc) => formatTime(utc, timeZone))
    : [];
  const rule = rescheduleRule(appointment);

  return (
    <main className={styles.page}>
      <div className={styles.reschedule}>
        <div className={styles.topbar}>
          <button type="button" className={styles.back} aria-label="Geri" onClick={onBack}>
            <ArrowLeft size={24} aria-hidden="true" />
          </button>
          <span className={styles.brand}>
            <BookingAvatar
              name={appointment.brand.name}
              src={appointment.brand.logoUrl ?? null}
              size="sm"
              decorative
              className={styles.brandLogo ?? ''}
            />
            {appointment.brand.name}
          </span>
        </div>
        <div className={styles.titleGroup}>
          <h1 className={styles.title}>Yeni saat seçin</h1>
          <p className={styles.muted}>{detailsLine(appointment)}</p>
        </div>
        <p className={styles.currentPill}>
          <CalendarClock size={18} aria-hidden="true" />
          <span>
            Şu anki: <s className={styles.currentWhen}>{whenLabel(appointment)}</s>
          </span>
        </p>
        <DayStrip
          days={days.map((value) => ({ date: value, available: true }))}
          value={date}
          today={today}
          showMonth={false}
          onChange={(value) => {
            setDate(value);
            setTime(null);
            setFailure(null);
          }}
        />
        <h2 className={styles.dayTitle}>{formatDate(date, 'weekdayDayMonth')}</h2>
        {taken ? (
          <StateBanner
            tone="hata"
            icon={<CalendarX size={20} />}
            title={`${failure.time} artık boş değil.`}
            description={
              alternativeTimes.length > 0
                ? `Siz seçerken başka biri aldı. En yakın boş saatler ${joinTimes(alternativeTimes)}.`
                : 'Siz seçerken başka biri aldı. Başka bir saat seçin.'
            }
          >
            {alternativeTimes.map((value) => (
              <BookingButton
                key={value}
                variant="secondary"
                size="sm"
                onClick={() => {
                  setFailure(null);
                  setTime(value);
                }}
              >
                {value}
              </BookingButton>
            ))}
          </StateBanner>
        ) : failure ? (
          <p className={styles.error} role="alert">
            {messageFor(failure.error)}
          </p>
        ) : null}
        {slotsQuery.error && !(slotsCode && terminal.has(slotsCode)) ? (
          <StateBanner
            tone="hata"
            icon={<CalendarX size={20} />}
            title="Saatler yüklenemedi."
            description="Bağlantınızı kontrol edip tekrar deneyin."
          >
            <BookingButton variant="secondary" size="sm" onClick={slotsQuery.reload}>
              Tekrar dene
            </BookingButton>
          </StateBanner>
        ) : slotsQuery.data === undefined ? (
          <div className={styles.skeleton} aria-hidden="true" />
        ) : (
          <TimeSlotGrid
            slots={grid}
            value={time}
            grouped={false}
            onChange={(value) => {
              setTime(value);
              setFailure(null);
            }}
            empty={
              <p className={styles.text} role="status">
                {formatDate(date, 'dayMonth')} için boş saat yok. Başka bir gün seçin.
              </p>
            }
          />
        )}
        {rule ? <p className={styles.muted}>{rule}</p> : null}
        {carriesDeposit(appointment) ? (
          <p className={styles.muted}>Kapora yeni saate aktarılır.</p>
        ) : null}
      </div>
      <StickyFooter
        summary={slot ? `${formatDate(date, 'compact')} · ${slot.localTime}` : undefined}
        detail={
          slot?.endLocalTime ? `${withTimeSuffix(slot.endLocalTime, 'locative')} biter` : undefined
        }
        action={
          <BookingButton
            disabled={!slot}
            loading={busy}
            loadingLabel="Değiştiriliyor…"
            onClick={() => {
              void submit();
            }}
          >
            Saati değiştir
          </BookingButton>
        }
      />
    </main>
  );
}
