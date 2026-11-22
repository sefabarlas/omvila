import { formatCount } from '@omvila/ui-web/format';
import { Avatar, Badge, Banner, Button, EmptyState, Tabs, useToast } from '@omvila/ui-web/panel';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from '@tanstack/react-router';
import { Check, History } from 'lucide-react';
import { useState } from 'react';
import { useApi } from '@/app/api-context';
import { useNow } from '@/app/clock';
import { useSession } from '@/app/session';
import { Page } from '@/shared/Page';
import { QueryState } from '@/shared/QueryState';
import { approvalQueueQuery, useApproveSender, useMatchNotice } from './api';
import type { ApprovalItem, ApprovalQueue, QueueNotice, QueueSender } from './api';
import styles from './Approvals.module.css';
import { decisionError } from './decision-error';
import {
  checkReason,
  expectedAmountText,
  noticeHeadline,
  waitingDays,
  waitingText,
} from './labels';
import { RejectSenderModal } from './RejectSenderModal';

export type ApprovalTab = 'havale' | 'sms';

/**
 * platform-paneli/POnay ve POnayBos: bekleyen havale bildirimleri ve SMS gönderen adı başvuruları (en eski üstte).
 * "Kod tutuyor" adayı olan bildirim listeden eşleştirilir; diğerleri "İncele" ile POnayDetay'da karara bağlanır.
 */
export function ApprovalsPage({ tab }: { tab: ApprovalTab | undefined }) {
  const query = useQuery(approvalQueueQuery(useApi()));
  const now = useNow();
  const queue = query.data;
  const total = queue ? queue.paymentNoticeCount + queue.smsSenderCount : 0;
  const context = !queue
    ? undefined
    : total === 0
      ? 'Bekleyen yok'
      : `${formatCount(total)} bekleyen${
          queue.oldestSubmittedUtc
            ? ` · en eskisi ${
                waitingDays(queue.oldestSubmittedUtc, now) === 0
                  ? 'bugün'
                  : `${formatCount(waitingDays(queue.oldestSubmittedUtc, now))} gün`
              }`
            : ''
        }`;

  return (
    <Page title="Onay kuyrukları" {...(context ? { context } : {})}>
      <QueryState query={query}>{(data) => <Queue queue={data} tab={tab} />}</QueryState>
    </Page>
  );
}

function Queue({ queue, tab }: { queue: ApprovalQueue; tab: ApprovalTab | undefined }) {
  const navigate = useNavigate();
  const { can } = useSession();
  const canDecideSenders = can('platform.sms-senders.decide');
  if (queue.paymentNoticeCount + queue.smsSenderCount === 0) {
    return (
      <div className={styles.emptyCard}>
        <EmptyState
          icon={<Check size={22} />}
          title="Bekleyen onay yok"
          description="Havale ile ödenen abonelik ve kontör bildirimleri ile SMS gönderen adı talepleri burada sıraya girer. En eskisi en üstte durur."
          actions={
            <Button
              icon={<History size={16} aria-hidden="true" />}
              onClick={() => void navigate({ to: '/denetim' })}
            >
              Denetim kaydı
            </Button>
          }
        />
      </div>
    );
  }

  // Varsayılan sekme: havale bildirimi varsa o, yoksa SMS başlığı.
  const active: ApprovalTab =
    tab ?? (queue.paymentNoticeCount === 0 && canDecideSenders ? 'sms' : 'havale');
  const items = queue.items.filter((item) =>
    active === 'sms' ? item.type === 'SmsSender' : item.type === 'PaymentNotice',
  );
  const tabs = [
    { value: 'havale', label: 'Havale bildirimi', count: queue.paymentNoticeCount },
    ...(canDecideSenders
      ? [{ value: 'sms', label: 'SMS başlığı', count: queue.smsSenderCount }]
      : []),
  ];

  return (
    <>
      <Tabs
        label="Onay kuyrukları"
        idPrefix="onay"
        items={tabs}
        value={active}
        onChange={(next) =>
          void navigate({ to: '/onay', search: next === 'havale' ? {} : { sekme: 'sms' } })
        }
      />
      <div role="tabpanel" id={`onay-panel-${active}`} aria-labelledby={`onay-tab-${active}`}>
        {items.length === 0 ? (
          <div className={styles.emptyCard}>
            <EmptyState
              icon={<Check size={22} />}
              title={
                active === 'sms' ? 'Bekleyen SMS başlığı yok' : 'Bekleyen havale bildirimi yok'
              }
              description="Yeni talep geldiğinde burada sıraya girer."
            />
          </div>
        ) : (
          <ul className={styles.list} aria-label="Bekleyen kayıtlar">
            {items.map((item) =>
              item.paymentNotice ? (
                <NoticeItem key={item.id} item={item} notice={item.paymentNotice} />
              ) : item.smsSender ? (
                <SenderItem key={item.id} item={item} sender={item.smsSender} />
              ) : null,
            )}
          </ul>
        )}
      </div>
      <p className={styles.note}>
        Eşleşen havale aboneliği ya da kontörü hemen açar ve markaya bildirim gider. Müşteri
        kaporaları buraya düşmez; onları salon kendisi işaretler.
      </p>
    </>
  );
}

function NoticeItem({ item, notice }: { item: ApprovalItem; notice: QueueNotice }) {
  const navigate = useNavigate();
  const toast = useToast();
  const now = useNow();
  const match = useMatchNotice();
  const [failure, setFailure] = useState<ReturnType<typeof decisionError> | null>(null);
  const candidate = notice.candidate;
  const codeMatches = candidate?.verdict === 'CodeMatches';
  const details = [
    `Açıklama ${notice.transferCode}`,
    codeMatches ? 'tutar ve açıklama tutuyor' : expectedAmountText(notice.expectedAmount),
    notice.senderName ? `gönderen “${notice.senderName}”` : null,
    notice.hasReceipt ? 'dekont yüklendi' : null,
    waitingText(item.submittedUtc, now),
  ].filter(Boolean);
  const open = () => void navigate({ to: '/onay/havale/$noticeId', params: { noticeId: item.id } });
  const searchBank = () =>
    void navigate({ to: '/onay/banka-hareketleri', search: { q: notice.transferCode } });

  // mutateAsync: başarıda satır kuyruktan düşer; çağrı başına geri çağrı kaybolmasın.
  const matchCandidate = async () => {
    if (!candidate) {
      return;
    }

    setFailure(null);
    try {
      await match.mutateAsync({
        noticeId: item.id,
        version: notice.version,
        bankTransactionId: candidate.bankTransactionId,
      });
      toast.show({
        tone: 'basari',
        title: 'Havale eşleştirildi',
        message: `${item.brandName} markasına bildirim gitti.`,
      });
    } catch (problem) {
      setFailure(decisionError(problem));
    }
  };

  return (
    <li className={styles.item}>
      <div className={styles.row}>
        <Avatar name={item.brandName} size="sm" />
        <div className={styles.body}>
          <p className={styles.title}>
            {item.brandName} <span>· {noticeHeadline(notice)}</span>
          </p>
          <span>{details.join(' · ')}</span>
        </div>
        <div className={styles.actions}>
          {codeMatches ? (
            <>
              <Button size="sm" onClick={open}>
                İncele
              </Button>
              <Button
                size="sm"
                variant="primary"
                icon={<Check size={14} aria-hidden="true" />}
                loading={match.isPending}
                loadingLabel="Eşleştiriliyor…"
                onClick={() => void matchCandidate()}
                aria-label={`${item.brandName} havalesini eşleştir`}
              >
                Eşleştir
              </Button>
            </>
          ) : (
            <>
              <Button size="sm" onClick={searchBank}>
                Banka hareketlerinde ara
              </Button>
              <Button size="sm" variant="primary" onClick={open}>
                İncele
              </Button>
            </>
          )}
        </div>
      </div>
      {codeMatches ? (
        <div className={styles.badgeRow}>
          <Badge tone="basari">Bankada eşleşen hareket var</Badge>
        </div>
      ) : candidate ? (
        <Banner tone="uyari" title="Kontrol edin:">
          {checkReason(candidate)}
        </Banner>
      ) : (
        <p className={styles.note}>Aday aralığında eşleşmemiş banka hareketi yok.</p>
      )}
      {failure ? (
        <Banner tone="hata" title={failure.title}>
          {failure.text}
        </Banner>
      ) : null}
    </li>
  );
}

function SenderItem({ item, sender }: { item: ApprovalItem; sender: QueueSender }) {
  const toast = useToast();
  const now = useNow();
  const approve = useApproveSender();
  const [rejecting, setRejecting] = useState(false);
  const [failure, setFailure] = useState<ReturnType<typeof decisionError> | null>(null);
  const approveSender = async () => {
    setFailure(null);
    try {
      await approve.mutateAsync({ senderId: item.id, version: sender.version });
      toast.show({
        tone: 'basari',
        title: 'SMS gönderen adı onaylandı',
        message: `${item.brandName} SMS'leri ${sender.requestedName} adıyla gidecek.`,
      });
    } catch (problem) {
      setFailure(decisionError(problem));
    }
  };

  return (
    <li className={styles.item}>
      <div className={styles.row}>
        <Avatar name={item.brandName} size="sm" />
        <div className={styles.body}>
          <p className={styles.title}>
            {item.brandName} <span>· SMS gönderen adı</span>
          </p>
          <span>
            {`${sender.requestedName} · şu an ${sender.currentEffectiveName} adıyla gidiyor · ${waitingText(item.submittedUtc, now)}`}
          </span>
        </div>
        <div className={styles.actions}>
          <Button
            size="sm"
            onClick={() => {
              setRejecting(true);
            }}
            aria-label={`${sender.requestedName} adını reddet`}
          >
            Reddet
          </Button>
          <Button
            size="sm"
            variant="primary"
            icon={<Check size={14} aria-hidden="true" />}
            loading={approve.isPending}
            loadingLabel="Onaylanıyor…"
            aria-label={`${sender.requestedName} adını onayla`}
            onClick={() => void approveSender()}
          >
            Onayla
          </Button>
        </div>
      </div>
      {failure ? (
        <Banner tone="hata" title={failure.title}>
          {failure.text}
        </Banner>
      ) : null}
      {rejecting ? (
        <RejectSenderModal
          brandName={item.brandName}
          senderId={item.id}
          sender={sender}
          onClose={() => {
            setRejecting(false);
          }}
        />
      ) : null}
    </li>
  );
}
