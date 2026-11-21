import type { components } from '@omvila/api-client-ts';
import { formatMoney, formatTime } from '@omvila/ui-web/format';
import { Banner } from '@omvila/ui-web/panel';
import { replyWithinLabel } from './remaining';
import styles from './Approvals.module.css';

type AppointmentDetail = components['schemas']['AppointmentDetail'];
type HistoryKind = components['schemas']['AppointmentHistoryEntry']['kind'];

/** Geçmiş satırı; sunucu detail verirse o yazılır. Declined salon içi metindir, müşteri "iptal" görür. */
const historyLabels: Record<HistoryKind, string> = {
  Created: 'Randevu oluşturuldu',
  Approved: 'Onaylandı',
  Declined: 'Reddet ile iptal edildi',
  HoldExpired: 'Yanıt süresi doldu, otomatik iptal edildi',
  Rescheduled: 'Saati değiştirildi',
  Cancelled: 'İptal edildi',
  CheckedIn: 'Geldi olarak işaretlendi',
  NoShow: 'Gelmedi olarak işaretlendi',
  NoShowUndone: 'Gelmedi geri alındı',
  Completed: 'Tamamlandı',
  DepositReported: 'Müşteri kaporayı gönderdiğini bildirdi',
  DepositReceived: 'Kapora geldi',
  NotificationSent: 'Bildirim gönderildi',
  DepositWaived: 'Kaporasız onaylandı',
  DepositReminded: 'Kapora hatırlatması gönderildi',
  DepositRefunded: 'Salon kaporayı iade etti',
};

interface HoldNoticeProps {
  /** Kalan dakika (0 = süre doldu). */
  minutesLeft: number;
  canApprove: boolean;
  /** Sunucu 409 hold_expired döndü. */
  expired: boolean;
  /** Kapora bekleyen randevu: süre havale süresidir, metin kaporayı anar. */
  deposit?: boolean;
}

/** Kalan süre şeridi (salon-paneli/RandevuPanel): "1 sa 35 dk içinde yanıtlayın." */
export function HoldNotice({ minutesLeft, canApprove, expired, deposit = false }: HoldNoticeProps) {
  if (!canApprove) {
    return (
      <Banner>
        {deposit
          ? 'Kaporayı yalnızca yönetici ve işletme sahibi işaretler.'
          : 'Onay bekleyen talepleri yalnızca yönetici ve işletme sahibi onaylar.'}
      </Banner>
    );
  }

  if (expired || minutesLeft === 0) {
    return (
      <Banner tone="uyari" title={deposit ? 'Havale süresi doldu' : 'Yanıt süresi doldu'}>
        Talep birazdan otomatik iptal edilir ve müşteriye bildirim gider.
      </Banner>
    );
  }

  return (
    <Banner title={replyWithinLabel(minutesLeft)}>
      Süre dolarsa talep otomatik iptal edilir, saat boşa çıkar ve müşteriye bildirim gider.
    </Banner>
  );
}

/** Randevu geçmişi (RandevuPanel "Geçmiş"); bilinmeyen tür genel satır olarak yazılır. */
export function AppointmentHistory({ detail }: { detail: AppointmentDetail }) {
  if (detail.history.length === 0) {
    return null;
  }

  const tz = detail.timeZone;
  return (
    <section className={styles.section} aria-labelledby="randevu-gecmisi">
      <h3 id="randevu-gecmisi" className={styles.sectionTitle}>
        Geçmiş
      </h3>
      <ul className={styles.history}>
        {detail.history.map((entry) => (
          <li key={`${entry.kind}-${entry.occurredUtc}`} className={styles.historyItem}>
            <span className={styles.historyDot} aria-hidden="true" />
            <span className={styles.historyText}>
              {entry.detail ??
                (historyLabels[entry.kind] as string | undefined) ??
                'Randevu güncellendi'}
            </span>
            <time dateTime={entry.occurredUtc} className={styles.historyTime}>
              {formatTime(entry.occurredUtc, tz)}
            </time>
          </li>
        ))}
      </ul>
    </section>
  );
}

/**
 * Onay bekleyen randevunun ücret satırları, müşteri notu ve geçmişi (salon-paneli/RandevuPanel). Kapora satırı
 * kapora istenmeyen talepte "Gerekmiyor" (tasarımdaki gibi), kapora istenende tutar; ayrıntılar kapora bölümündedir.
 */
export function PendingApprovalDetails({ detail }: { detail: AppointmentDetail }) {
  return (
    <>
      <dl className={styles.rows}>
        {detail.items.map((item) => (
          <div key={item.id} className={styles.row}>
            <dt>{[item.serviceName, item.staffName?.split(' ')[0]].filter(Boolean).join(' · ')}</dt>
            <dd>{formatMoney(item.price)}</dd>
          </div>
        ))}
        <div className={styles.row}>
          <dt>Kapora</dt>
          <dd>{detail.deposit ? formatMoney(detail.deposit.required) : 'Gerekmiyor'}</dd>
        </div>
        <div className={styles.row}>
          <dt>Salonda ödenecek</dt>
          <dd>{formatMoney(detail.amountDue)}</dd>
        </div>
      </dl>
      {detail.customerNote ? (
        <section className={styles.section} aria-labelledby="musteri-notu">
          <h3 id="musteri-notu" className={styles.sectionTitle}>
            Müşteri notu
          </h3>
          <p className={styles.noteBox}>{detail.customerNote}</p>
        </section>
      ) : null}
      <AppointmentHistory detail={detail} />
    </>
  );
}
