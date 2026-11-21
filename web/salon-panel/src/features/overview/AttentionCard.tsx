import { formatTime } from '@omvila/ui-web/format';
import { Button, EmptyState, Skeleton } from '@omvila/ui-web/panel';
import { CircleCheck, Clock } from 'lucide-react';
import type { PendingApprovalList } from '@/features/approvals/api';
import styles from './Overview.module.css';

interface AttentionCardProps {
  /** Onay kuyruğu; izin yoksa çağrılmaz (undefined, yüklenmiyor). */
  queue: PendingApprovalList | undefined;
  loading: boolean;
  isError: boolean;
  onRetry: () => void;
  onOpen: (appointmentId: string) => void;
}

/** "2 online talep onay bekliyor"; kanallar karışıksa yalnız "talep". */
export function approvalTitle(queue: PendingApprovalList): string {
  const online = queue.items.every((item) => item.channel === 'Online');
  return `${String(queue.totalCount)} ${online ? 'online talep' : 'talep'} onay bekliyor`;
}

/**
 * "Dikkat isteyenler" (salon-paneli/GenelBakis). Dilim 3'te yalnız onay kuyruğu satırı; kapora (D4) ve gelmedi
 * satırları kendi dilimlerinde eklenir. "İncele" en acil talebin yan panelini açar.
 */
export function AttentionCard({ queue, loading, isError, onRetry, onOpen }: AttentionCardProps) {
  const first = queue?.items[0];
  return (
    <section className={styles.card} aria-labelledby="dikkat-isteyenler">
      <div className={styles.cardHead}>
        <h2 id="dikkat-isteyenler" className={styles.listTitle}>
          Dikkat isteyenler
        </h2>
      </div>
      {isError ? (
        <div className={styles.cardError} role="alert">
          <p>Onay bekleyen talepler yüklenemedi.</p>
          <Button size="sm" onClick={onRetry}>
            Tekrar dene
          </Button>
        </div>
      ) : loading ? (
        <Skeleton variant="block" height="72px" />
      ) : queue && first ? (
        <ul className={styles.attention}>
          <li className={styles.attentionRow}>
            <span className={styles.attentionIcon} aria-hidden="true">
              <Clock size={18} />
            </span>
            <span className={styles.attentionText}>
              <strong>{approvalTitle(queue)}</strong>
              <small>
                {queue.items
                  .map((item) => `${item.customerName} ${formatTime(item.startUtc, item.timeZone)}`)
                  .join(', ')}
              </small>
            </span>
            <Button
              size="sm"
              variant="primary"
              aria-label={`İncele: ${first.customerName}`}
              onClick={() => {
                onOpen(first.appointmentId);
              }}
            >
              İncele
            </Button>
          </li>
        </ul>
      ) : (
        <EmptyState
          icon={<CircleCheck size={24} />}
          title="Dikkat isteyen bir şey yok"
          description="Onay bekleyen talepler burada görünür."
        />
      )}
    </section>
  );
}
