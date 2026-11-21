import { Button } from '@omvila/ui-web/panel';
import { formatCount } from '@omvila/ui-web/format';
import type { SmsOverview, SmsSender } from './notification-api';
import styles from './NotificationSettings.module.css';

function senderDetail(sender: SmsSender): string {
  switch (sender.status) {
    case 'Approved':
      return 'operatör onaylı';
    case 'Pending':
      return `${sender.name ?? ''} için operatör onayı bekleniyor`;
    case 'Rejected':
      return `${sender.name ?? ''} onaylanmadı`;
    case 'None':
      return 'Salon adınızla göndermek için başvurun';
  }
}

export interface SmsSummaryProps {
  overview: SmsOverview;
  /** Gönderen adı başvurusu penceresini açar; başvuru beklerken yeni başvuru yapılamaz (409). */
  onRequestSender: () => void;
}

/** AyarlarBildirim üst kartları: kontör, bu ay gönderilen, gönderen adı. Sayılar sunucudan; tahmin yoksa alt satır yok. */
export function SmsSummary({ overview, onRequestSender }: SmsSummaryProps) {
  const { sender, sentThisMonth } = overview;
  return (
    <div className={styles.summary}>
      <section
        className={[styles.summaryCard, styles.summaryKum].join(' ')}
        aria-label="SMS kontörü"
      >
        <span className={styles.summaryLabel}>SMS kontörü</span>
        <span className={styles.summaryValue}>{formatCount(overview.creditBalance)}</span>
        {overview.estimatedDaysLeft !== null ? (
          <span className={styles.summarySub}>
            bu tempoyla ~{formatCount(overview.estimatedDaysLeft)} gün yeter
          </span>
        ) : null}
      </section>
      <section className={styles.summaryCard} aria-label="Bu ay gönderilen">
        <span className={styles.summaryLabel}>Bu ay gönderilen</span>
        <span className={styles.summaryValue}>{formatCount(sentThisMonth.smsCount)}</span>
        <span className={styles.summarySub}>
          SMS · {formatCount(sentThisMonth.notificationCount)} bildirim
        </span>
      </section>
      <section className={styles.summaryCard} aria-label="Gönderen adı">
        <span className={styles.summaryLabel}>Gönderen adı</span>
        <span className={styles.summaryValue}>{sender.effectiveName}</span>
        <span className={styles.summarySub}>{senderDetail(sender)}</span>
        {sender.status === 'Rejected' && sender.rejectionReason ? (
          <span className={styles.summarySub}>{sender.rejectionReason}</span>
        ) : null}
        {sender.status === 'Pending' ? null : (
          <Button
            size="sm"
            variant="ghost"
            className={styles.summaryAction}
            onClick={onRequestSender}
          >
            {sender.status === 'Approved' ? 'Adı değiştir' : 'Başvur'}
          </Button>
        )}
      </section>
    </div>
  );
}
