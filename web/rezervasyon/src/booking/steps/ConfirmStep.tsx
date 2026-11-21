import { BookingButton } from '@omvila/ui-web/booking';
import { formatTime } from '@omvila/ui-web/format';
import { CalendarX } from 'lucide-react';
import { useState } from 'react';
import type { ReactNode } from 'react';
import { StateBanner } from '@/components/StateBanner';
import { privacyNoticeVersion } from '@/lib/legal';
import { createAppointment } from '../booking-api';
import type { ItemRequest, PublicAppointment, Slot } from '../booking-api';
import { alternatives, messageFor, problemCode } from '../problem';
import styles from './steps.module.css';

interface ConfirmStepProps {
  branchId: string;
  timeZone: string;
  bookingToken: string;
  /** "Ayşe": yalnız bu markada kayıtlı müşteride (T-51 (9)). */
  knownFirstName: string | null;
  slot: Slot | null;
  items: ItemRequest[];
  sameStaffForAll: boolean;
  fullName: string;
  note: string;
  marketingConsent: boolean;
  /** Özet kartı. */
  summary: ReactNode;
  onCreated: (appointment: PublicAppointment) => void;
  /** Saat doldu: alternatif saate geçiş (aynı gün). */
  onPickTime: (localTime: string) => void;
  onChangeTime: () => void;
  /** Jetonun süresi doldu: telefon adımına dönülür. */
  onTokenExpired: () => void;
  onBookingDisabled: () => void;
}

/** "14:45 ve 15:15", "14:45, 15:15 ve 15:45". */
function joinTimes(times: string[]): string {
  if (times.length <= 1) return times.join('');
  return `${times.slice(0, -1).join(', ')} ve ${times.at(-1) ?? ''}`;
}

/**
 * Son kontrol (Onay.png): özet, "Randevuyu oluştur". Paket ve kapora kutuları Dilim 4–5'te. Saat dolduysa
 * Durumlar.png "saat az önce doldu" deseni ve alternatif saat çipleri.
 */
export function ConfirmStep({
  branchId,
  timeZone,
  bookingToken,
  knownFirstName,
  slot,
  items,
  sameStaffForAll,
  fullName,
  note,
  marketingConsent,
  summary,
  onCreated,
  onPickTime,
  onChangeTime,
  onTokenExpired,
  onBookingDisabled,
}: ConfirmStepProps) {
  const [failure, setFailure] = useState<{ error: unknown; time: string } | null>(null);
  const [busy, setBusy] = useState(false);

  const submit = async () => {
    if (!slot) return;
    setBusy(true);
    setFailure(null);
    try {
      onCreated(
        await createAppointment(branchId, bookingToken, {
          items,
          sameStaffForAll,
          startUtc: slot.startUtc,
          fullName: fullName.trim(),
          customerNote: note.trim() || null,
          privacyNoticeAcknowledged: true,
          privacyNoticeVersion,
          marketingConsent,
        }),
      );
    } catch (error) {
      const code = problemCode(error);
      if (code === 'unauthorized' || code === 'token_expired') onTokenExpired();
      else if (code === 'booking_disabled') onBookingDisabled();
      else setFailure({ error, time: slot.localTime });
    } finally {
      setBusy(false);
    }
  };

  const taken = failure && problemCode(failure.error) === 'slot_taken';
  const alternativeTimes = taken
    ? alternatives(failure.error).map((utc) => formatTime(utc, timeZone))
    : [];

  return (
    <div className={styles.stack}>
      <p className={[styles.text, styles.welcome].join(' ')}>
        {knownFirstName ? (
          <>
            Tekrar hoş geldiniz, <strong>{knownFirstName}</strong>.{' '}
          </>
        ) : null}
        Randevunuzu onaylamadan önce son bir kez bakın.
      </p>
      {summary}
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
          {alternativeTimes.map((time) => (
            <BookingButton
              key={time}
              variant="secondary"
              size="sm"
              onClick={() => {
                setFailure(null);
                onPickTime(time);
              }}
            >
              {time}
            </BookingButton>
          ))}
          {alternativeTimes.length === 0 ? (
            <BookingButton variant="secondary" size="sm" onClick={onChangeTime}>
              Saati değiştir
            </BookingButton>
          ) : null}
        </StateBanner>
      ) : failure ? (
        <p className={styles.note} role="alert">
          {messageFor(failure.error)}
        </p>
      ) : null}
      <BookingButton
        block
        disabled={!slot || Boolean(taken)}
        loading={busy}
        loadingLabel="Oluşturuluyor…"
        onClick={() => {
          void submit();
        }}
      >
        Randevuyu oluştur
      </BookingButton>
      <p className={[styles.note, styles.center].join(' ')}>
        Ödeme sayfadan alınmaz; ücreti salonda ödersiniz.
      </p>
    </div>
  );
}
