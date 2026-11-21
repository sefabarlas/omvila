import { BookingButton } from '@omvila/ui-web/booking';
import { formatDate } from '@omvila/ui-web/format';
import { BellRing, WifiOff } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { StateBanner } from '@/components/StateBanner';
import { messageFor, problemCode } from '@/booking/problem';
import { privacyNoticeVersion } from '@/lib/legal';
import { joinWaitlist } from './waitlist-api';
import type { WaitlistEntry, WaitlistJoin as JoinBody } from './waitlist-api';
import { windowsLabel } from './windows';
import styles from './waitlist.module.css';
import shared from '@/booking/steps/steps.module.css';

interface WaitlistJoinProps {
  branchId: string;
  bookingToken: string;
  preferences: JoinBody['preferences'];
  fullName: string;
  onJoined: (entry: WaitlistEntry) => void;
  /** Jetonun süresi doldu: telefon adımına dönülür. */
  onTokenExpired: () => void;
  onBookingDisabled: () => void;
}

/**
 * Telefon doğrulandıktan sonra publicJoinWaitlist (KK3). Kendiliğinden bir kez çağrılır; aynı müşterinin aynı
 * hizmetler için bekleyen kaydı varsa sunucu tercihleri günceller (200), ikinci kayıt açılmaz.
 */
export function WaitlistJoin({
  branchId,
  bookingToken,
  preferences,
  fullName,
  onJoined,
  onTokenExpired,
  onBookingDisabled,
}: WaitlistJoinProps) {
  const [failure, setFailure] = useState<unknown>(null);
  const [attempt, setAttempt] = useState(0);
  const sent = useRef(-1);
  // Yanıt gelene kadar üst bileşen yeniden çizilebilir; istek deneme başına bir kez gider.
  useEffect(() => {
    if (sent.current === attempt) return;
    sent.current = attempt;
    joinWaitlist(branchId, bookingToken, {
      preferences,
      fullName: fullName.trim(),
      privacyNoticeAcknowledged: true,
      privacyNoticeVersion,
    }).then(onJoined, (error: unknown) => {
      const code = problemCode(error);
      if (code === 'unauthorized' || code === 'token_expired') onTokenExpired();
      else if (code === 'booking_disabled') onBookingDisabled();
      else setFailure(error);
    });
  }, [
    attempt,
    branchId,
    bookingToken,
    preferences,
    fullName,
    onJoined,
    onTokenExpired,
    onBookingDisabled,
  ]);

  if (failure) {
    return (
      <StateBanner
        tone="hata"
        icon={<WifiOff size={20} />}
        title="Bekleme listesine eklenemediniz."
        description={messageFor(failure)}
      >
        {problemCode(failure) === 'customer_blocked' ? null : (
          <BookingButton
            variant="secondary"
            size="sm"
            onClick={() => {
              setFailure(null);
              setAttempt((value) => value + 1);
            }}
          >
            Tekrar dene
          </BookingButton>
        )}
      </StateBanner>
    );
  }
  return (
    <div
      className={shared.skeleton}
      role="status"
      aria-busy="true"
      aria-label="Listeye ekleniyor"
    />
  );
}

interface WaitlistJoinedProps {
  entry: WaitlistEntry;
  brandSlug: string;
}

/** Katılım sonucu (tasarımda ayrı ekran yok; Talep deseni, T-100): istenen gün, saatler ve personel. */
export function WaitlistJoined({ entry, brandSlug }: WaitlistJoinedProps) {
  const days = entry.dates.map((date) => formatDate(date, 'panel')).join(', ');
  return (
    <div className={shared.stack}>
      <span className={styles.doneIcon} aria-hidden="true">
        <BellRing size={28} />
      </span>
      <div className={shared.center} role="status">
        <h2 className={shared.titleMedium}>Bekleme listesine katıldınız</h2>
        <p className={shared.note}>
          Yer açılırsa size SMS göndereceğiz. Bağlantıdaki süre içinde onaylarsanız randevu sizin
          olur.
        </p>
      </div>
      <dl className={styles.facts} aria-label="Bekleme tercihiniz">
        <div>
          <dt>Hizmet</dt>
          <dd>{entry.serviceNames.join(', ')}</dd>
        </div>
        <div>
          <dt>Gün</dt>
          <dd>{days || 'Herhangi bir gün'}</dd>
        </div>
        <div>
          <dt>Saat</dt>
          <dd>{windowsLabel(entry.timeWindows)}</dd>
        </div>
        <div>
          <dt>Kişi</dt>
          <dd>
            {entry.preferredStaffName
              ? entry.acceptOtherStaff
                ? `${entry.preferredStaffName} veya başka biri`
                : entry.preferredStaffName
              : 'Fark etmez'}
          </dd>
        </div>
      </dl>
      <a
        className={[shared.linkButton, shared.center].join(' ')}
        href={`/${encodeURIComponent(brandSlug)}`}
      >
        Salon sayfasına dön
      </a>
    </div>
  );
}
