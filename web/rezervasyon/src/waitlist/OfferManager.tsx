'use client';

import { BookingAvatar, BookingButton } from '@omvila/ui-web/booking';
import { formatDate, formatDuration, formatMoney, formatTime } from '@omvila/ui-web/format';
import { systemClock, toBranchDate } from '@omvila/ui-web/time';
import type { Clock } from '@omvila/ui-web/time';
import { Hourglass, Timer, UserRoundX, WifiOff } from 'lucide-react';
import { Suspense, lazy, useEffect, useRef, useState } from 'react';
import type { ReactNode } from 'react';
import { LinkButton } from '@/components/LinkButton';
import { LinkMissing } from '@/components/LinkMissing';
import { StateBanner } from '@/components/StateBanner';
import { messageFor, problemCode } from '@/booking/problem';
import { useNow } from '@/lib/deposit';
import { acceptOffer, declineOffer, fetchOffer, leaveWaitlist } from './waitlist-api';
import type { LoadedOffer, PublicAppointment, WaitlistOffer } from './waitlist-api';
import styles from './offer.module.css';

// Kabul sonrası ekranlar rezervasyon akışınınkilerle aynıdır (Başarı, Talep, Havale); ayrı parça.
const OfferResult = lazy(() => import('./OfferResult').then((m) => ({ default: m.OfferResult })));

type State =
  | { kind: 'loading' }
  | { kind: 'missing' }
  | { kind: 'failed' }
  | { kind: 'ready'; loaded: LoadedOffer; loadedAt: number }
  | { kind: 'accepted'; appointment: PublicAppointment; brand: WaitlistOffer['brand'] }
  | { kind: 'left'; brand: WaitlistOffer['brand'] };

/** Teklifin bitmiş hâlleri ve yerel süre dolumu BeklemeDoldu deseniyle gösterilir. */
type Closed = 'Expired' | 'Declined' | 'Withdrawn';

export interface OfferManagerProps {
  code: string;
  /** "Şimdi" (testte sabit saat); geri sayım için yalnız geçen süre kullanılır. */
  clock?: Clock;
}

/** "09:05" (dakika:saniye). */
function countdown(milliseconds: number): string {
  const seconds = Math.max(0, Math.ceil(milliseconds / 1000));
  return `${String(Math.floor(seconds / 60)).padStart(2, '0')}:${String(seconds % 60).padStart(2, '0')}`;
}

/** "Cuma 28 Kasım · 16:00" (şubenin saat diliminde). */
function offerWhen(offer: WaitlistOffer): string {
  const date = toBranchDate(new Date(offer.startUtc), offer.timeZone);
  return `${formatDate(date, 'weekdayDayMonth')} · ${formatTime(offer.startUtc, offer.timeZone)}`;
}

/** "Cuma 16:00". */
function shortWhen(offer: WaitlistOffer): string {
  const date = toBranchDate(new Date(offer.startUtc), offer.timeZone);
  return `${formatDate(date, 'weekday')} ${formatTime(offer.startUtc, offer.timeZone)}`;
}

function BrandBar({ brand }: { brand: WaitlistOffer['brand'] }) {
  return (
    <div className={styles.brand}>
      <BookingAvatar name={brand.name} size="sm" decorative />
      <span>{brand.name}</span>
    </div>
  );
}

/**
 * Bekleme teklifi bağlantısı (omvila.com/b/{kod}; BeklemeTeklif, BeklemeDoldu; KK3). Bağlantı telefonu doğrular, kod
 * istenmez. Geri sayım holdExpiresUtc − generatedUtc ile başlar (cihaz saati değil, açılıştan geçen süre). "Randevuyu
 * al" If-Match ile; randevu online kurallarıyla oluşur (W4): onaylı → Başarı, onay bekleyen → Talep, kapora → Havale.
 */
export function OfferManager({ code, clock = systemClock }: OfferManagerProps) {
  const [state, setState] = useState<State>({ kind: 'loading' });
  const [attempt, setAttempt] = useState(0);
  const [busy, setBusy] = useState<'accept' | 'decline' | 'leave' | null>(null);
  const [failure, setFailure] = useState<unknown>(null);
  const [notice, setNotice] = useState<string | null>(null);
  // Sunucu süre doldu dedi (409 hold_expired); yerel saat henüz dolmamış olabilir.
  const [serverExpired, setServerExpired] = useState(false);
  const now = useNow(clock, 1000).getTime();
  const mainRef = useRef<HTMLElement>(null);

  useEffect(() => {
    let live = true;
    fetchOffer(code).then(
      (loaded) => {
        if (!live) return;
        setState(
          loaded ? { kind: 'ready', loaded, loadedAt: clock.now().getTime() } : { kind: 'missing' },
        );
      },
      () => {
        if (live) setState({ kind: 'failed' });
      },
    );
    return () => {
      live = false;
    };
  }, [code, clock, attempt]);

  const reload = (message?: string) => {
    setNotice(message ?? null);
    setAttempt((value) => value + 1);
  };

  useEffect(() => {
    if (state.kind === 'missing') document.title = 'Bu teklif bulunamadı';
    if (state.kind === 'accepted' || state.kind === 'left') mainRef.current?.focus();
  }, [state.kind]);

  if (state.kind === 'missing') {
    return <LinkMissing title="Bu teklif bulunamadı" />;
  }
  if (state.kind === 'failed') {
    return (
      <main className={styles.page}>
        <h1 className={styles.srOnly}>Bekleme teklifi</h1>
        <StateBanner
          tone="hata"
          icon={<WifiOff size={20} />}
          title="Teklif yüklenemedi."
          description="Bağlantınızı kontrol edip tekrar deneyin."
        >
          <BookingButton
            variant="secondary"
            size="sm"
            onClick={() => {
              setState({ kind: 'loading' });
              reload();
            }}
          >
            Tekrar dene
          </BookingButton>
        </StateBanner>
      </main>
    );
  }
  if (state.kind === 'loading') {
    return (
      <main className={styles.page} aria-busy="true">
        <h1 className={styles.srOnly}>Bekleme teklifi yükleniyor</h1>
        <span className={styles.skeleton} />
        <span className={styles.skeleton} />
      </main>
    );
  }
  if (state.kind === 'accepted') {
    return (
      <main ref={mainRef} className={styles.page} tabIndex={-1}>
        <BrandBar brand={state.brand} />
        <h1 className={styles.srOnly}>Randevunuz</h1>
        <Suspense fallback={<span className={styles.skeleton} />}>
          <OfferResult
            appointment={state.appointment}
            clock={clock}
            onChange={(appointment) => {
              setState({ ...state, appointment });
            }}
          />
        </Suspense>
      </main>
    );
  }
  if (state.kind === 'left') {
    return (
      <main ref={mainRef} className={styles.page} tabIndex={-1}>
        <BrandBar brand={state.brand} />
        <span className={styles.icon} aria-hidden="true">
          <UserRoundX size={24} />
        </span>
        <h1 className={styles.title}>Bekleme listesinden çıktınız</h1>
        <p className={styles.lead}>Bu salon için size bekleme listesi mesajı gelmeyecek.</p>
        <div className={styles.actions}>
          <LinkButton href={`/${encodeURIComponent(state.brand.slug)}`} block>
            Başka saat seç
          </LinkButton>
        </div>
      </main>
    );
  }

  const { offer, etag } = state.loaded;
  const initial = new Date(offer.holdExpiresUtc).getTime() - new Date(offer.generatedUtc).getTime();
  const remaining = initial - (now - state.loadedAt);
  const closed: Closed | null =
    offer.status === 'Pending'
      ? remaining <= 0 || serverExpired
        ? 'Expired'
        : null
      : offer.status === 'Accepted'
        ? null
        : offer.status;

  const act = async (kind: 'accept' | 'decline' | 'leave') => {
    setBusy(kind);
    setFailure(null);
    setNotice(null);
    try {
      if (kind === 'accept') {
        const appointment = await acceptOffer(code, etag);
        setState({ kind: 'accepted', appointment, brand: offer.brand });
      } else if (kind === 'decline') {
        const loaded = await declineOffer(code, etag);
        setState({ kind: 'ready', loaded, loadedAt: clock.now().getTime() });
      } else {
        await leaveWaitlist(offer.waitlistAccessCode);
        setState({ kind: 'left', brand: offer.brand });
      }
    } catch (error) {
      const errorCode = problemCode(error);
      if (errorCode === 'hold_expired') setServerExpired(true);
      else if (
        errorCode === 'invalid_state_transition' ||
        errorCode === 'concurrency_conflict' ||
        errorCode === 'not_found'
      ) {
        reload('Teklif bu arada değişti; güncel hâli gösteriliyor.');
      } else setFailure(error);
    } finally {
      setBusy(null);
    }
  };

  const errorLine = failure ? (
    <p className={styles.error} role="alert">
      {messageFor(failure)}
    </p>
  ) : null;
  const noticeLine = notice ? (
    <p className={styles.notice} role="status">
      {notice}
    </p>
  ) : null;

  if (offer.status === 'Accepted') {
    return (
      <main className={styles.page}>
        <BrandBar brand={offer.brand} />
        <h1 className={styles.title}>Bu saati aldınız</h1>
        <p className={styles.lead}>{offerWhen(offer)} randevunuz bu teklifle oluşturuldu.</p>
        {offer.appointmentAccessCode ? (
          <div className={styles.actions}>
            <LinkButton href={`/r/${encodeURIComponent(offer.appointmentAccessCode)}`} block>
              Randevunuzu görün
            </LinkButton>
          </div>
        ) : null}
      </main>
    );
  }

  if (closed) {
    const titles: Record<Closed, string> = {
      Expired: 'Bu saatin süresi doldu',
      Declined: 'Bu saati geri çevirdiniz',
      Withdrawn: 'Bu teklif geri alındı',
    };
    const reasons: Record<Closed, string> = {
      Expired: `${shortWhen(offer)} için ayrılan süre geçti, saat sıradaki kişiye önerildi.`,
      Declined: `${shortWhen(offer)} sıradaki kişiye önerildi.`,
      Withdrawn: `Salon ${shortWhen(offer)} saatini başka bir düzenleme için geri aldı.`,
    };
    return (
      <main className={styles.page}>
        <BrandBar brand={offer.brand} />
        <span className={styles.icon} aria-hidden="true">
          <Hourglass size={24} />
        </span>
        <h1 className={styles.title}>{titles[closed]}</h1>
        <p className={styles.lead}>
          {reasons[closed]} Bekleme listesinde kalmaya devam ediyorsunuz; yeni bir saat boşalınca
          tekrar SMS göndereceğiz.
        </p>
        {noticeLine}
        {errorLine}
        <div className={styles.actions}>
          <LinkButton href={`/${encodeURIComponent(offer.brand.slug)}`} block>
            Başka saat seç
          </LinkButton>
          <BookingButton
            variant="danger"
            block
            loading={busy === 'leave'}
            loadingLabel="Çıkılıyor…"
            onClick={() => {
              void act('leave');
            }}
          >
            Listeden çık
          </BookingButton>
        </div>
      </main>
    );
  }

  const staff = [...new Set(offer.items.map((item) => item.staffName).filter(Boolean))];
  const minutes = offer.items.reduce((sum, item) => sum + item.durationMinutes, 0);
  const rows: [string, ReactNode][] = [
    ['Tarih', offerWhen(offer)],
    [
      'Hizmet',
      `${offer.items.map((item) => item.serviceName).join(', ')} · ${formatDuration(minutes)}`,
    ],
    ...(staff.length > 0 ? [['Kişi', staff.join(', ')] as [string, ReactNode]] : []),
    ...(offer.depositAmount
      ? [['Kapora (havale ile)', formatMoney(offer.depositAmount)] as [string, ReactNode]]
      : []),
    ['Salonda ödenecek', formatMoney(offer.amountDue)],
  ];
  const ratio = initial > 0 ? Math.min(1, Math.max(0, remaining / initial)) : 0;

  return (
    <main className={styles.page}>
      <BrandBar brand={offer.brand} />
      <section className={styles.timer} aria-label="Kalan süre">
        <p className={styles.timerHead}>
          <span>Bekleme listesi</span>
          <span className={styles.held}>
            <Timer size={16} aria-hidden="true" />
            Sizin için tutuluyor
          </span>
        </p>
        <p className={styles.clock} role="timer">
          {countdown(remaining)}
        </p>
        <p className={styles.timerNote}>dakika içinde onaylarsanız saat sizin</p>
        <span className={styles.bar} aria-hidden="true">
          <span className={styles.barFill} style={{ inlineSize: `${String(ratio * 100)}%` }} />
        </span>
      </section>
      <h1 className={styles.title}>
        {offer.customerFirstName
          ? `${offer.customerFirstName}, beklediğiniz saat boşaldı`
          : 'Beklediğiniz saat boşaldı'}
      </h1>
      <dl className={styles.facts}>
        {rows.map(([label, value]) => (
          <div key={label}>
            <dt>{label}</dt>
            <dd>{value}</dd>
          </div>
        ))}
      </dl>
      <p className={styles.lead}>
        Telefonunuz bu bağlantıyla doğrulandı, kod girmeniz gerekmez.
        {offer.requiresApproval
          ? offer.depositAmount
            ? ' Randevu kapora havalesinden sonra kesinleşir.'
            : ' Randevu salon onayından sonra kesinleşir.'
          : ''}{' '}
        Onaylamazsanız saat sıradaki kişiye önerilir.
      </p>
      {noticeLine}
      {errorLine}
      <div className={styles.actions}>
        <BookingButton
          block
          loading={busy === 'accept'}
          loadingLabel="Randevu alınıyor…"
          disabled={busy !== null}
          onClick={() => {
            void act('accept');
          }}
        >
          Randevuyu al
        </BookingButton>
        <BookingButton
          variant="ghost"
          block
          loading={busy === 'decline'}
          loadingLabel="Gönderiliyor…"
          disabled={busy !== null}
          onClick={() => {
            void act('decline');
          }}
        >
          Bu saat olmaz
        </BookingButton>
      </div>
    </main>
  );
}
