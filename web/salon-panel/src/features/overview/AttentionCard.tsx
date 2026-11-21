import { ApiProblem, idempotencyKey } from '@omvila/api-client-ts';
import { errorMessage } from '@omvila/ui-web/errors';
import { formatDuration, formatTime } from '@omvila/ui-web/format';
import { Button, EmptyState, Skeleton, useToast } from '@omvila/ui-web/panel';
import { Banknote, CircleCheck, Clock } from 'lucide-react';
import { useState } from 'react';
import { splitQueue } from '@/features/approvals/api';
import type { PendingApprovalItem, PendingApprovalList } from '@/features/approvals/api';
import { remainingMinutes } from '@/features/approvals/remaining';
import { useMarkDepositReceivedById, useRemindDeposit } from '@/features/deposits/api';
import { transferReportedTitle } from '@/features/deposits/deposit-view';
import styles from './Overview.module.css';

interface AttentionCardProps {
  /** Onay kuyruğu; izin yoksa çağrılmaz (undefined, yüklenmiyor). */
  queue: PendingApprovalList | undefined;
  loading: boolean;
  isError: boolean;
  /** "Hatırlat" ve "Kapora geldi" (appointment.deposit.mark); yoksa satır "İncele" gösterir. */
  canMarkDeposit: boolean;
  onRetry: () => void;
  onOpen: (appointmentId: string) => void;
}

/** "2 online talep onay bekliyor"; kanallar karışıksa yalnız "talep". */
export function approvalTitle(items: readonly PendingApprovalItem[]): string {
  const online = items.every((item) => item.channel === 'Online');
  return `${String(items.length)} ${online ? 'online talep' : 'talep'} onay bekliyor`;
}

/** "14:30 · havale işaretlenmedi · 1 sa 20 dk kaldı"; müşteri bildirdiyse "Havale bildirildi · kontrol edin". */
export function depositLine(item: PendingApprovalItem, now: string): string {
  const minutes = remainingMinutes(item.holdExpiresUtc, now);
  return [
    formatTime(item.startUtc, item.timeZone),
    item.depositStatus === 'CustomerReported' ? transferReportedTitle : 'havale işaretlenmedi',
    minutes > 0 ? `${formatDuration(minutes)} kaldı` : 'süre doldu',
  ].join(' · ');
}

/**
 * "Dikkat isteyenler" (salon-paneli/GenelBakis): onay bekleyen talepler tek satır ("İncele" en acil talebin yan
 * panelini açar), kapora bekleyen her talep ayrı satır: "Hatırlat" ve "Kapora geldi". Kuyruk satırında hatırlatma anı
 * yok; gönderilen hatırlatma bu oturumda "Hatırlatıldı" olur, ikinci deneme sunucudan 409
 * deposit_reminder_already_sent alır (acik-sorular.md T-84). Gelmedi satırı kendi diliminde eklenir.
 */
export function AttentionCard({
  queue,
  loading,
  isError,
  canMarkDeposit,
  onRetry,
  onOpen,
}: AttentionCardProps) {
  const toast = useToast();
  const remind = useRemindDeposit();
  const markReceived = useMarkDepositReceivedById();
  const [reminded, setReminded] = useState<ReadonlySet<string>>(new Set());
  const { approvals, deposits } = splitQueue(queue?.items ?? []);
  const first = approvals[0];
  const busyId = remind.isPending
    ? remind.variables
    : markReceived.isPending
      ? markReceived.variables.appointmentId
      : undefined;

  const showError = (error: unknown) => {
    toast.show({
      tone: 'hata',
      title: errorMessage(error instanceof ApiProblem ? error.code : undefined),
    });
  };

  const sendReminder = (item: PendingApprovalItem) => {
    remind.mutate(item.appointmentId, {
      onSuccess: () => {
        setReminded((set) => new Set(set).add(item.appointmentId));
        toast.show({
          tone: 'basari',
          title: 'Hatırlatma gönderildi',
          message: 'Kapora SMS’i müşteriye yeniden gider.',
        });
      },
      onError: (error) => {
        if (error instanceof ApiProblem && error.code === 'deposit_reminder_already_sent') {
          setReminded((set) => new Set(set).add(item.appointmentId));
        }

        showError(error);
      },
    });
  };

  const confirmReceived = (item: PendingApprovalItem) => {
    markReceived.mutate(
      { appointmentId: item.appointmentId, key: idempotencyKey()['Idempotency-Key'] },
      {
        onSuccess: () => {
          toast.show({
            tone: 'basari',
            title: 'Kapora geldi olarak işaretlendi',
            message: `${item.customerName} randevusu onaylandı, müşteriye SMS gider.`,
          });
        },
        onError: showError,
      },
    );
  };

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
      ) : queue && (first || deposits.length > 0) ? (
        <ul className={styles.attention}>
          {first ? (
            <li className={styles.attentionRow}>
              <span className={styles.attentionIcon} aria-hidden="true">
                <Clock size={18} />
              </span>
              <span className={styles.attentionText}>
                <strong>{approvalTitle(approvals)}</strong>
                <small>
                  {approvals
                    .map(
                      (item) => `${item.customerName} ${formatTime(item.startUtc, item.timeZone)}`,
                    )
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
          ) : null}
          {deposits.map((item) => {
            const sent = reminded.has(item.appointmentId);
            const busy = busyId !== undefined;
            return (
              <li key={item.appointmentId} className={styles.attentionRow}>
                <span
                  className={[styles.attentionIcon, styles.kapora].filter(Boolean).join(' ')}
                  aria-hidden="true"
                >
                  <Banknote size={18} />
                </span>
                <span className={styles.attentionText}>
                  <strong>{`${item.customerName} kapora bekliyor`}</strong>
                  <small>{depositLine(item, queue.generatedUtc)}</small>
                </span>
                <span className={styles.attentionActions}>
                  {canMarkDeposit ? (
                    <>
                      <Button
                        size="sm"
                        aria-label={`${sent ? 'Hatırlatıldı' : 'Hatırlat'}: ${item.customerName}`}
                        disabled={sent || busy}
                        loading={remind.isPending && busyId === item.appointmentId}
                        loadingLabel="Gönderiliyor…"
                        onClick={() => {
                          sendReminder(item);
                        }}
                      >
                        {sent ? 'Hatırlatıldı' : 'Hatırlat'}
                      </Button>
                      <Button
                        size="sm"
                        variant="primary"
                        aria-label={`Kapora geldi: ${item.customerName}`}
                        disabled={busy}
                        loading={markReceived.isPending && busyId === item.appointmentId}
                        loadingLabel="İşaretleniyor…"
                        onClick={() => {
                          confirmReceived(item);
                        }}
                      >
                        Kapora geldi
                      </Button>
                    </>
                  ) : (
                    <Button
                      size="sm"
                      aria-label={`İncele: ${item.customerName}`}
                      onClick={() => {
                        onOpen(item.appointmentId);
                      }}
                    >
                      İncele
                    </Button>
                  )}
                </span>
              </li>
            );
          })}
        </ul>
      ) : (
        <EmptyState
          icon={<CircleCheck size={24} />}
          title="Dikkat isteyen bir şey yok"
          description="Onay ve kapora bekleyen talepler burada görünür."
        />
      )}
    </section>
  );
}
