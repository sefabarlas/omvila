import {
  BookingAvatar,
  BookingButton,
  CopyField,
  CopyList,
  StickyFooter,
  appointmentWhen,
} from '@omvila/ui-web/booking';
import {
  formatDuration,
  formatIban,
  formatMoney,
  formatTime,
  moneyInputValue,
  normalizeIban,
  withTimeSuffix,
} from '@omvila/ui-web/format';
import type { Clock } from '@omvila/ui-web/time';
import { Hourglass, Info, Timer } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import type { ReactNode } from 'react';
import { messageFor, problemCode } from '@/booking/problem';
import { reportDepositTransfer } from '@/appointment/appointment-api';
import type { Loaded, PublicAppointment } from '@/appointment/appointment-api';
import { awaitingTransfer, minutesLeft, useNow } from '@/lib/deposit';
import styles from './DepositTransfer.module.css';

/** Sunucunun "artık beklenmiyor" yanıtları: sayfa durumu yeniden okunur. */
const stale = new Set(['hold_expired', 'invalid_state_transition', 'not_found']);

async function copy(value: string): Promise<boolean> {
  try {
    await navigator.clipboard.writeText(value);
    return true;
  } catch {
    return false;
  }
}

interface DepositTransferProps {
  appointment: PublicAppointment;
  clock: Clock;
  /** Akışta h2 (salon başlığı h1), SMS bağlantısında h1. */
  headingLevel: 1 | 2;
  /** Bildirim sonrası güncel randevu (deposit.status CustomerReported) ve ETag. */
  onReported: (loaded: Loaded) => void;
  /** Süre bu sayfa açıkken doldu veya sunucu kaporayı artık beklemiyor: durum yeniden okunur. */
  onStale: () => void;
  /** Bilgi kutusunun altına (SMS bağlantısında "Plan değişti mi?"). */
  children?: ReactNode;
}

/**
 * Havale (Havale.png, KK3–KK4): kapora tutarı ve geri sayım, kopyalanabilir alıcı / IBAN / açıklama kodu / tutar,
 * "Havaleyi yaptım, salona bildir". Bildirimden sonra "Salon onayı bekleniyor" (S-03); "onaylandı" yazılmaz, onay
 * salonun "Kapora geldi" işaretidir. IBAN boşluklu gösterilir, boşluksuz kopyalanır. OMVILA para almaz.
 */
export function DepositTransfer({
  appointment,
  clock,
  headingLevel,
  onReported,
  onStale,
  children,
}: DepositTransferProps) {
  const deposit = awaitingTransfer(appointment);
  const now = useNow(clock);
  const [busy, setBusy] = useState(false);
  const [failure, setFailure] = useState<unknown>(null);
  const left = deposit ? minutesLeft(deposit.instructions.dueUtc, now) : 0;

  // Süre sayfa açıkken dolarsa bir kez yeniden okunur (açılışta zaten dolmuşsa döngüye girmez).
  const wasRunning = useRef(left > 0);
  useEffect(() => {
    if (left > 0) {
      wasRunning.current = true;
    } else if (wasRunning.current) {
      wasRunning.current = false;
      onStale();
    }
  }, [left, onStale]);

  if (!deposit) return null;
  const { instructions } = deposit;
  const reported = deposit.status === 'CustomerReported';
  const expired = left === 0;
  const Heading = headingLevel === 1 ? 'h1' : 'h2';
  const dueTime = withTimeSuffix(formatTime(instructions.dueUtc, appointment.timeZone), 'dative');

  const report = async () => {
    setBusy(true);
    setFailure(null);
    try {
      onReported(await reportDepositTransfer(appointment.accessCode));
    } catch (error) {
      const code = problemCode(error);
      if (code && stale.has(code)) onStale();
      else setFailure(error);
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className={styles.screen}>
      <p className={styles.brand}>
        <BookingAvatar
          name={appointment.brand.name}
          src={appointment.brand.logoUrl ?? null}
          size="sm"
          decorative
        />
        {appointment.brand.name}
      </p>
      <section className={styles.hero} aria-labelledby="kapora-baslik">
        <div className={styles.heroTop}>
          <span id="kapora-baslik" className={styles.eyebrow}>
            Kapora bekleniyor
          </span>
          <span className={styles.timer}>
            <Timer size={16} aria-hidden="true" />
            {expired ? 'Süre doldu' : `${formatDuration(left)} kaldı`}
          </span>
        </div>
        <p className={styles.amount}>{formatMoney(instructions.amount)}</p>
        <p className={styles.when}>
          {appointmentWhen(appointment.startUtc, appointment.timeZone)} · {appointment.brand.name}
        </p>
      </section>

      {reported ? (
        <section className={styles.reported} role="status" aria-labelledby="salon-onayi">
          <span className={styles.reportedIcon} aria-hidden="true">
            <Hourglass size={20} />
          </span>
          <div className={styles.reportedBody}>
            <h2 id="salon-onayi" className={styles.reportedTitle}>
              Salon onayı bekleniyor
            </h2>
            <p>
              Havalenizi salona bildirdik. Salon kaporayı hesabında görünce randevunuzu onaylar ve
              size SMS gelir.
            </p>
          </div>
        </section>
      ) : null}

      <div className={styles.intro}>
        <Heading className={styles.title}>Kaporayı havale ile gönderin</Heading>
        <p className={styles.lead}>
          Kendi bankanızdan aşağıdaki hesaba gönderin. Açıklamaya kodu yazmayı unutmayın; salon
          havalenizi bu kodla eşleştirir.
        </p>
      </div>

      <CopyList>
        <CopyField label="Alıcı" value={instructions.accountHolderName} onCopy={copy} />
        <CopyField
          label="IBAN"
          value={normalizeIban(instructions.iban)}
          displayValue={formatIban(instructions.iban)}
          onCopy={copy}
        />
        <CopyField label="Açıklama kodu" value={instructions.transferCode} onCopy={copy} />
        <CopyField
          label="Tutar"
          value={moneyInputValue(instructions.amount.amountMinor)}
          displayValue={formatMoney(instructions.amount)}
          onCopy={copy}
        />
      </CopyList>

      <div className={styles.info}>
        <Info size={20} aria-hidden="true" className={styles.infoIcon} />
        <p>
          Salon havaleyi görünce onaylar, size SMS gelir. Kapora {dueTime} kadar gelmezse randevu
          otomatik iptal olur ve saat başkasına açılır.
        </p>
      </div>

      {failure ? (
        <p className={styles.error} role="alert">
          {messageFor(failure)}
        </p>
      ) : null}

      {children}

      {reported || expired ? null : (
        <StickyFooter
          className={styles.footer ?? ''}
          action={
            <BookingButton
              block
              loading={busy}
              loadingLabel="Bildiriliyor…"
              onClick={() => {
                void report();
              }}
            >
              Havaleyi yaptım, salona bildir
            </BookingButton>
          }
          note="Bu bilgiler SMS ile de gönderildi."
        />
      )}
    </div>
  );
}
